namespace UrGuide.Model.Payments
{
    public class TransactionHistoryResponse
    {
        public List<TransactionItem> Transactions { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class TransactionItem
    {
        public string TransactionId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
