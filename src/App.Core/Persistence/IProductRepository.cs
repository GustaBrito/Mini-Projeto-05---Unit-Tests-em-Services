using App.Core.Entities;

namespace App.Core.Persistence;

public interface IProductRepository
{
    IReadOnlyList<Product> GetAll();
    Product? GetById(Guid id);
    Product? GetByName(string name);
    void Add(Product product);
    void Update(Product product);
    void Remove(Product product);
}
