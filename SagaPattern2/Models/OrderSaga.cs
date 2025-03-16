namespace SagaPattern2.Models
{
    // Models/OrderSaga.cs
    public class OrderSaga
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string State { get; set; } // State of the order (e.g., "PaymentPending", "InventoryReserved", etc.)
        public DateTime CreatedDate { get; set; }
    }

}
