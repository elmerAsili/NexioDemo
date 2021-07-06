namespace Nexio.Models
{
    public enum TransactionStatus
    {
        authOnlyPending = 3,
        authorizedPending = 9,
        pending = 10,
        authOnly = 11,
        declined = 30,
        fraudReject = 32,
        settled = 20,
        voidPending = 39,
        voided = 40,
        error = 50
    }
}
