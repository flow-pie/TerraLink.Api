using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Common;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs;
using TerraLink.Api.DTOs.Clients;
using TerraLink.Api.Models;

namespace TerraLink.Api.Services.Clients;

public class ClientService(
    TerraLinkDbContext dbContext,
    IFileStorageService fileStorageService
) : IClientService
{
    public async Task<ClientRegistrationResult>
        RegisterAsync(
            RegisterClientRequest request,
            long? authenticatedUserId,
            CancellationToken cancellationToken
        )
    {
        var nationalId =
            request.NationalId.Trim();

        var phone = request.Phone.Trim();

        var email =
            request.Email?.Trim()
                .ToLowerInvariant();

        var existingNationalId =
            await dbContext.Clients.AnyAsync(
                client =>
                    client.NationalId == nationalId,
                cancellationToken
            );

        if (existingNationalId)
            return new ClientRegistrationResult(
                ClientRegistrationStatus
                    .DuplicateNationalId
            );


        var existingPhone =
            await dbContext.Clients.AnyAsync(
                client => client.Phone == phone,
                cancellationToken
            );

        if (existingPhone)
            return new ClientRegistrationResult(
                ClientRegistrationStatus
                    .DuplicatePhone
            );

        User? officer = null;

        if (authenticatedUserId is not null)
        {
            officer = await dbContext.Users
                .Include(user => user.Role)
                .SingleOrDefaultAsync(
                    user =>
                        user.Id ==
                        authenticatedUserId.Value,
                    cancellationToken
                );

            if (officer is null ||
                officer.Status !=
                    UserStatus.ACTIVE ||
                officer.Role.Name !=
                    "Loan Officer")
                return new ClientRegistrationResult(
                    ClientRegistrationStatus
                        .InvalidOfficer
                );
        }

        var isOfficerRegistration =
            officer is not null;

        if (!isOfficerRegistration)
        {
            if (string.IsNullOrWhiteSpace(
                    request.Password))
                throw new ArgumentException(
                    "Password is required for " +
                    "self-registration."
                );

            if (string.IsNullOrWhiteSpace(
                    email))
                throw new ArgumentException(
                    "Email is required for " +
                    "self-registration."
                );

            var emailExists =
                await dbContext.Users.AnyAsync(
                    client =>
                        client.Email == email,
                    cancellationToken
                );

            if (emailExists)
                return new ClientRegistrationResult(
                    ClientRegistrationStatus
                        .DuplicateEmail
                );
        }

        string? nationalIdFront = null;
        string? nationalIdBack = null;
        string? passport = null;

        await using var transaction =
            await dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken
                );

        try
        {
            nationalIdFront =
                await fileStorageService.SaveAsync(
                    request.NationalIdFront!,
                    StorageFolder.Kyc,
                    cancellationToken
                );

            nationalIdBack =
                await fileStorageService.SaveAsync(
                    request.NationalIdBack!,
                    StorageFolder.Kyc,
                    cancellationToken
                );

            passport =
                await fileStorageService.SaveAsync(
                    request.PassportPhoto!,
                    StorageFolder.Kyc,
                    cancellationToken);


            User? clientUser = null;

            var clientNo = GenerateClientNumber();

            var clientRole =
                    await dbContext.Roles
                        .SingleOrDefaultAsync(
                            role =>
                                role.Name == "Client",
                            cancellationToken
                        );

            if (clientRole is null)
                throw new InvalidOperationException(
                    "Client role is missing. " +
                    "Seed the roles table."
                );

            clientUser = new User
            {
                EmployeeNo = clientNo,
                Username = phone,
                Email = email,
                RoleId = clientRole.Id,
                Status = isOfficerRegistration
                    ? UserStatus.INACTIVE
                    : UserStatus.ACTIVE,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    request.Password ?? "Terralink@2026")
            };

            dbContext.Users.Add(clientUser);

            await dbContext.SaveChangesAsync(cancellationToken);

            var now = DateTime.UtcNow;

            var client =
                new Client
                {
                    ClientNo =
                        clientNo,

                    UserId =
                        clientUser?.Id,

                    FullName =
                        request.FullName.Trim(),

                    NationalId =
                        nationalId,

                    Phone = phone,

                    DateOfBirth =
                        request.DateOfBirth,

                    Gender =
                        request.Gender,

                    Address =
                        request.Address.Trim(),

                    RegistrationChannel =
                        isOfficerRegistration
                            ? RegistrationChannel
                                .OFFICER
                            : RegistrationChannel
                                .SELF,

                    RegisteredBy =
                        officer?.Id,

                    VerificationStatus =
                        isOfficerRegistration
                            ? VerificationStatus
                                .VERIFIED
                            : VerificationStatus
                                .PENDING,

                    VerifiedBy =
                        isOfficerRegistration
                            ? officer!.Id
                            : null,

                    VerifiedAt =
                        isOfficerRegistration
                            ? now
                            : null,

                    Status =
                        ClientStatus.ACTIVE
                };

            dbContext.Clients.Add(
                client
            );

            await dbContext.SaveChangesAsync(
                cancellationToken
            );

            var documents = new List<KycDocument>
            {
                new()
                {
                    ClientId =      client.Id,
                    DocType = KycDocType.ID_FRONT,
                    FileUrl = nationalIdFront,
                    Verified = isOfficerRegistration,
                    VerifiedBy = officer?.Id,
                    VerifiedAt = isOfficerRegistration
                        ? now
                        : null
                },

                new()
                {
                    ClientId =      client.Id,
                    DocType = KycDocType.ID_BACK,
                    FileUrl = nationalIdBack,
                    Verified = isOfficerRegistration,
                    VerifiedBy = officer?.Id,
                    VerifiedAt = isOfficerRegistration
                        ? now
                        : null
                },

                new()
                {
                    ClientId =      client.Id,
                    DocType = KycDocType.PASSPORT_PHOTO,
                    FileUrl = passport,
                    Verified = isOfficerRegistration,
                    VerifiedBy = officer?.Id,
                    VerifiedAt = isOfficerRegistration
                        ? now
                        : null
                }
            };

            dbContext.KycDocuments.AddRange(documents);

            await dbContext.SaveChangesAsync(
                cancellationToken
            );

            await transaction.CommitAsync(
                cancellationToken
            );

            return new ClientRegistrationResult(
                ClientRegistrationStatus.Success,
                new RegisterClientResponse(
                    ClientId: client.Id,
                    UserId: clientUser?.Id,
                    ClientNo:
                        client.ClientNo!,
                    RegistrationChannel:
                        client.RegistrationChannel
                            .ToString(),
                    VerificationStatus:
                        client.VerificationStatus
                            .ToString(),
                    Message:
                        isOfficerRegistration
                            ? "Client registered and " +
                              "automatically verified."
                            : "Registration submitted. " +
                              "Your account is pending " +
                              "verification."
                )
            );
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);

            if (nationalIdFront is not null)
                await fileStorageService.DeleteAsync(
                    nationalIdFront,
                    cancellationToken);

            if (nationalIdBack is not null)
                await fileStorageService.DeleteAsync(
                    nationalIdBack,
                    cancellationToken);

            if (passport is not null)
                await fileStorageService.DeleteAsync(
                    passport,
                    cancellationToken);

            throw;
        }
    }

    private static string
        GenerateClientNumber()
    {
        return
            $"TC-{Guid.NewGuid()
                .ToString("N")[..4]
                .ToUpperInvariant()}";
    }

    public async Task<PagedResponse<ClientsListItemResponse>> GetAllClientsAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        //query
        var query = dbContext.Clients;

        //total count 
        var totalCount = await query.CountAsync(cancellationToken);

        //clients
        var clients = await query
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(client => new ClientsListItemResponse(
                client.Id,
                client.User!.Username,
                client.User.Email,
                client.User.EmployeeNo,
                client.User.Role.Name,
                client.User.LastLogin,
                client.NationalId,
                client.Phone,
                client.FullName,
                client.DateOfBirth,
                client.Gender,
                client.VerifiedAt,
                client.Address,
                client.Status,
                client.VerificationStatus
            )).ToListAsync(cancellationToken);

        //build the response
        return new PagedResponse<ClientsListItemResponse>(
            Items: clients,
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount
        );
    }

    public async Task<ClientProfileResponse?> GetClientByIdAsync(
         long clientId,
         CancellationToken cancellationToken)
    {
        var client = await dbContext.Clients
            .Include(c => c.User)
            .ThenInclude(u => u!.Role)
            .SingleOrDefaultAsync(
                c => c.Id == clientId,
                cancellationToken
            );

        if (client is null)
            return null;

        return new ClientProfileResponse(
            client.Id,
            client.User!.Username,
            client.User.Email,
            client.User.EmployeeNo,
            client.User.Role.Name,
            client.User.LastLogin,
            client.NationalId,
            client.Phone,
            client.FullName,
            client.DateOfBirth,
            client.Gender,
            client.VerifiedAt,
            client.Address,
            client.Status,
            client.VerificationStatus
        );
    }

    public async Task<ClientProfileResponse?> UpdateClientAsync(
        long clientId,
        UpdateClientRequest request,
        CancellationToken cancellationToken)
    {
        //find the client
        var client = await dbContext.Clients
        .Include(c => c.User)
        .ThenInclude(u => u!.Role)
        .SingleOrDefaultAsync(c => c.Id == clientId,
        cancellationToken);

        if (client is null)
            return null;

        if (request.Phone is not null)

        {
            var phoneExists = await dbContext.Clients
                .AnyAsync(
                    c => c.Id != clientId && c.Phone == request.Phone,
                    cancellationToken
                );

            if (phoneExists)
            {
                throw new ConflictException("The phone number already exists in the database");
            }
        }

        if (request.NationalId is not null)
        {
            var nationalIdExists = await dbContext.Clients
                .AnyAsync(
                    c => c.NationalId == request.NationalId &&
                         c.Id != clientId,
                    cancellationToken);

            if (nationalIdExists)
                throw new ConflictException(
                    "National ID is already in use.");
        }

        if (request.Phone is not null)
            client.Phone = request.Phone;

        if (request.NationalId is not null)
            client.NationalId = request.NationalId;


        if (request.FullName is not null)
            client.FullName = request.FullName;

        if (request.Address is not null)
            client.Address = request.Address;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ClientProfileResponse(
           client.Id,
           client.User!.Username,
           client.User.Email,
           client.User.EmployeeNo,
           client.User.Role.Name,
           client.User.LastLogin,
           client.NationalId,
           client.Phone,
           client.FullName,
           client.DateOfBirth,
           client.Gender,
           client.VerifiedAt,
           client.Address,
           client.Status,
           client.VerificationStatus
       );


    }

    public async Task<ClientProfileResponse?> VerifyClientAsync(
        long clientId,
        long officerId,
        CancellationToken cancellationToken)
    {
        var client = await dbContext.Clients
        .Include(c => c.User)
            .ThenInclude(u => u!.Role)
        .SingleOrDefaultAsync(
            c => c.Id == clientId,
            cancellationToken);

        if (client is null)
            return null;

        if (client.VerificationStatus != VerificationStatus.PENDING)
        {
            throw new ConflictException(
                "Client verification status is not currently pending.");
        }

        client.VerificationStatus = VerificationStatus.VERIFIED;

        client.VerifiedBy = officerId;

        client.VerifiedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ClientProfileResponse(
           client.Id,
           client.User!.Username,
           client.User.Email,
           client.User.EmployeeNo,
           client.User.Role.Name,
           client.User.LastLogin,
           client.NationalId,
           client.Phone,
           client.FullName,
           client.DateOfBirth,
           client.Gender,
           client.VerifiedAt,
           client.Address,
           client.Status,
           client.VerificationStatus
       );
    }

    public async Task<RejectClientResponse?> RejectClientAsync(
     long clientId,
     long officerId,
     string reason,
     CancellationToken cancellationToken)
    {
        var client = await dbContext.Clients
            .SingleOrDefaultAsync(
                c => c.Id == clientId,
                cancellationToken);

        if (client is null)
            return null;

        if (client.VerificationStatus != VerificationStatus.PENDING)
        {
            throw new ConflictException(
                "Client verification status is not currently pending.");
        }

        client.VerificationStatus = VerificationStatus.REJECTED;
        client.RejectionReason = reason.Trim();
        client.RejectedBy = officerId;
        client.RejectedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new RejectClientResponse(
            client.Id,
            client.VerificationStatus,
            client.RejectionReason
        );
    }
}