using App.Core.Entities;
using App.Core.Persistence;

namespace App.Api.Persistence;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();

    public IReadOnlyList<Product> GetAll() => _products.ToList();

    public Product? GetById(Guid id) => _products.FirstOrDefault(p => p.Id == id);

    public Product? GetByName(string name) => _products.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

    public void Add(Product product) => _products.Add(product);

    public void Update(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index >= 0)
        {
            _products[index] = product;
        }
    }

    public void Remove(Product product) => _products.Remove(product);
}
