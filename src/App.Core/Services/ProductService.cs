using App.Core.Dtos;
using App.Core.Entities;
using App.Core.Persistence;

namespace App.Core.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<ProductResponse> GetAll()
    {
        return _repository.GetAll()
            .Select(p => new ProductResponse(p.Id, p.Name, p.Price))
            .ToList();
    }

    public ProductResponse GetById(Guid id)
    {
        var product = GetExistingProduct(id);
        return new ProductResponse(product.Id, product.Name, product.Price);
    }

    public ProductResponse Create(CreateProductRequest request)
    {
        var normalizedName = NormalizeName(request.Name, nameof(request.Name));
        EnsureValidPrice(request.Price, nameof(request.Price));
        EnsureNameAvailable(normalizedName);

        var product = new Product
        {
            Name = normalizedName,
            Price = request.Price
        };

        _repository.Add(product);

        return new ProductResponse(product.Id, product.Name, product.Price);
    }

    public ProductResponse Update(Guid id, UpdateProductRequest request)
    {
        var product = GetExistingProduct(id);
        var normalizedName = NormalizeName(request.Name, nameof(request.Name));
        EnsureValidPrice(request.Price, nameof(request.Price));
        EnsureNameAvailable(normalizedName, id);

        product.Name = normalizedName;
        product.Price = request.Price;

        _repository.Update(product);

        return new ProductResponse(product.Id, product.Name, product.Price);
    }

    public void Delete(Guid id)
    {
        var product = GetExistingProduct(id);
        _repository.Remove(product);
    }

    private static string NormalizeName(string name, string paramName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.", paramName);
        }

        return name.Trim();
    }

    private static void EnsureValidPrice(decimal price, string paramName)
    {
        if (price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.", paramName);
        }
    }

    private void EnsureNameAvailable(string normalizedName, Guid? currentId = null)
    {
        var existing = _repository.GetByName(normalizedName);
        if (existing is null)
        {
            return;
        }

        if (currentId is null || existing.Id != currentId)
        {
            throw new InvalidOperationException("A product with the same name already exists.");
        }
    }

    private Product GetExistingProduct(Guid id)
    {
        var product = _repository.GetById(id);
        if (product is null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        return product;
    }
}
