namespace PharmacyApp.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public Guid DrugId { get; set; }
        public Drug? Drug { get; set; }

        public Guid Quantity { get; set; }
        public decimal Price { get; set; }
    }
}