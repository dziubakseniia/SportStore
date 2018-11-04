namespace SportsStore.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string Info { get; set; }
        public string UserId { get; set; }
    }
}
