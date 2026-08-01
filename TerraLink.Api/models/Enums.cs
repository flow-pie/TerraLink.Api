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

    public enum RepaymentFrequency
    {
        WEEKLY,
        BIWEEKLY,
        MONTHLY
    }

    public enum LoanProductStatus
    {
        ACTIVE,
        INACTIVE
    }

    public enum LoanApplicationStatus
    {
        SUBMITTED,
        UNDER_REVIEW,
        APPROVED,
        REJECTED,
        INFO_REQUESTED
    }

    public enum LoanDecision
    {
        APPROVED,
        REJECTED,
        INFO_REQUESTED
    }

    public enum LoanStatus
    {
        ACTIVE,
        COMPLETED,
        DEFAULTED,
        WRITTEN_OFF
    }
    public enum InstallmentStatus
    {
        PENDING,
        PAID,
        OVERDUE
    }

        public enum PaymentMethod
    {
        MPESA,
        CASH,
        BANK_TRANSFER
    }

    public enum PaymentStatus
    {
        PENDING,
        SUCCESS,
        FAILED
    }

}