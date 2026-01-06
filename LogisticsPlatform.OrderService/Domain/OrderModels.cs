namespace LogisticsPlatform.OrderService.Domain;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string CustomerId { get; set; } = string.Empty; // Simulado
    
    // Relación 1 a Muchos
    public List<OrderItem> Items { get; set; } = new();
    
    public decimal TotalAmount => Items.Sum(i => i.UnitPrice * i.Quantity);
}

public class OrderItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    // Clave foránea (Foreign Key) para EF Core
    public int OrderId { get; set; }
    // Propiedad de navegación (opcional, para evitar ciclos en JSON recomiendo ignorarla o no ponerla si no es estricta)
    // public Order Order { get; set; } 
}