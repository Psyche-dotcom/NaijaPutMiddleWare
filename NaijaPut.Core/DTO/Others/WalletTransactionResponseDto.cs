namespace NaijaPut.Core.DTO.Others
{
    public class WalletTransactionResponseDto
    {
        public string Narration { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
    }
}
