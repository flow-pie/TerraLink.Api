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
        PENDING_DISBURSEMENT,
        ACTIVE,
        IN_ARREARS,
        COMPLETED,
        CLOSED,
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
    public enum DisbursementStatus
    {
        PENDING,
        SENT,
        COMPLETED,
        FAILED
    }
    public enum AssetType
    {
        LIVESTOCK,
        MOTORBIKE,
        WATER_PUMP,
        OTHER
    }

    public enum NotificationType
    {
        CLIENT_VERIFICATION_PENDING,
        LOAN_APPLICATION_SUBMITTED,
        LOAN_APPROVED,
        LOAN_REJECTED,
        PAYMENT_DUE,
        PAYMENT_RECEIVED,
        PAYMENT_FAILED,
        DISBURSEMENT_COMPLETED
    }
    public enum ReportFrequency
    {
        DAILY,
        WEEKLY,
        MONTHLY
    }

    public enum StorageFolder
    {
        Kyc,
        ProfilePhotos,
        Reports
    }

}