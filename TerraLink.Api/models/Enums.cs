namespace TerraLink.Api.Models
{
    public enum UserStatus
    {
        ACTIVE,
        INACTIVE,
        SUSPENDED
    }


    public enum Gender
    {
        MALE,
        FEMALE,
        OTHER
    }


    public enum RegistrationChannel
    {
        OFFICER,
        SELF
    }

    public enum VerificationStatus
    {
        PENDING,
        VERIFIED,
        REJECTED
    }


    public enum ClientStatus
    {
        ACTIVE,
        INACTIVE,
        SUSPENDED
    }

    public enum BranchStatus
    {
        ACTIVE,
        INACTIVE
    }

    public enum MeetingFrequency
    {
        WEEKLY,
        BIWEEKLY,
        MONTHLY
    }

    public enum GroupHealthStatus
    {
        HEALTHY,
        WATCHLIST
    }

    public enum GroupStatus
    {
        ACTIVE,
        INACTIVE
    }

    public enum KycDocType
    {
        ID_FRONT,
        ID_BACK,
        PASSPORT_PHOTO
    }
}