using System.Text.Json;
using LogisticsPlatform.CatalogService.Domain;
using LogisticsPlatform.CatalogService.Infrastructure;
using StackExchange.Redis;

namespace LogisticsPlatform.CatalogService.Services;

public class ProductService
{
    private readonly CatalogDbContext _context;
    private readonly IConnectionMultiplexer _redis;

    public ProductService(CatalogDbContext context, IConnectionMultiplexer redis)
    {
        _context = context;
        _redis = redis;
    }

    // QUERY: Lectura optimizada con Cache-Aside
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var db = _redis.GetDatabase();
        string key = $"product:{id}";

        // 1. Intentar obtener de Redis (Cache Hit)
        var cachedProduct = await db.StringGetAsync(key);
        if (!cachedProduct.IsNullOrEmpty)
        {
            // Deserializamos y retornamos rápido
            return JsonSerializer.Deserialize<Product>(cachedProduct.ToString());
        }

        // 2. Si no está en caché, buscar en SQL (Cache Miss)
        var product = await _context.Products.FindAsync(id);

        // 3. Si existe en BD, guardarlo en Redis para la próxima (TTL 10 minutos)
        if (product != null)
        {
            var serialized = JsonSerializer.Serialize(product);
            await db.StringSetAsync(key, serialized, TimeSpan.FromMinutes(10));
        }

        return product;
    }

    // COMMAND: Escritura (Solo BD por simplicidad, invalidamos caché si es necesario)
    public async Task CreateProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        // Nota: En un escenario real, aquí deberíamos invalidar/actualizar el caché.
    }
}