using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
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

        var phone =
            request.Phone.Trim();

        var email =
            request.Email?.Trim()
                .ToLowerInvariant();

        var existingNationalId =
            await dbContext.Clients.AnyAsync(
                user =>
                    user.NationalId == nationalId,
                cancellationToken
            );

        if (existingNationalId)
            return new ClientRegistrationResult(
                ClientRegistrationStatus
                    .DuplicateNationalId
            );


        var existingPhone =
            await dbContext.Clients.AnyAsync(
                user =>
                    user.Phone == phone,
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
                    user =>
                        user.Email == email,
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

            var    user =
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

                    Phone =
                        phone,

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
                user
            );

            await dbContext.SaveChangesAsync(
                cancellationToken
            );

            var documents = new List<KycDocument>
            {
                new()
                {
                    ClientId =     user.Id,
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
                    ClientId =     user.Id,
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
                    ClientId =     user.Id,
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
                    ClientId:      user.Id,
                    UserId: clientUser?.Id,
                    ClientNo:
                        user.ClientNo!,
                    RegistrationChannel:
                        user.RegistrationChannel
                            .ToString(),
                    VerificationStatus:
                        user.VerificationStatus
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
        var query = dbContext.Users.Where(u => u.Role.Name == "client");

        //total count 
        var totalCount = await query.CountAsync(cancellationToken);

        //clients
        var clients = await query
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(user => user.Client)
            .Select( user => new ClientsListItemResponse(
                user.Id,
                user.Username,
                user.Email,
                user.EmployeeNo,
                user.Role.Name,
                user.LastLogin,
                user.Client!.NationalId,
                user.Client.Phone,
                user.Client.FullName,
                user.Client.DateOfBirth,
                user.Client.Gender,
                user.Client.VerifiedAt,
                user.Client.Address,
                user.Client.Status,
                user.Client.VerificationStatus
            )).ToListAsync(cancellationToken);

        //build the response
        return new PagedResponse<ClientsListItemResponse>(
            Items: clients,
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount
        );
    }
}