namespace SportsStore.Domain.Entities
{
    /// <summary>
    /// Manages CartLines.
    /// </summary>
    public class CartLine
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}