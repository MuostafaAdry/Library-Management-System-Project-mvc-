namespace LibraryManagementSystem.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; }
        public double OrderTotal { get; set; }
        public OrderStatus Status { get; set; }
        public OrderType Type { get; set; }

        public bool OrderShipedStatus { get; set; }
        public bool PaymentStatus { get; set; }
        public string? Carrier { get; set; }
        public string? TrackingNumber { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentStripeId { get; set; }
    }

    public enum OrderStatus
    {
        Canceled = 0,
        Pending = 1,    // الطلب قيد الانتظار
        Processing = 2, // الطلب قيد المعالجة
        Shipped = 3,    // الطلب تم شحنه
        Delivered = 4   // الطلب تم توصيله

    }

    public enum OrderType
    {
        Buying = 0,
        Borrowing = 1,    

    }
}
