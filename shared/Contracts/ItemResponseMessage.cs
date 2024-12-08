public class ItemResponseMessage
{
    public string Name { get; set; }
        public string UniqueCode { get; set; }
        public long Quantity { get; set; }
        public decimal EstimatedValue { get; set; }
        public DateTime ExpirationDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string Supplier{get;set;}
}