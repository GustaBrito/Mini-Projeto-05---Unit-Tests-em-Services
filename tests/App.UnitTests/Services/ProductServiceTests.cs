using App.Core.Dtos;
using App.Core.Entities;
using App.Core.Persistence;
using App.Core.Services;
using Xunit;

namespace App.UnitTests.Services;

public sealed class ProductServiceTests
{
    private sealed class FakeRepository : IProductRepository
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

        public void Seed(Product product) => _products.Add(product);
    }

    [Fact]
    public void GetAll_ReturnsAllProducts()
    {
        var repository = new FakeRepository();
        repository.Seed(new Product { Name = "Keyboard", Price = 120 });
        repository.Seed(new Product { Name = "Mouse", Price = 60 });
        var service = new ProductService(repository);

        var result = service.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetById_WithMissingProduct_Throws()
    {
        var repository = new FakeRepository();
        var service = new ProductService(repository);

        Assert.Throws<KeyNotFoundException>(() => service.GetById(Guid.NewGuid()));
    }

    [Fact]
    public void GetById_WithExistingProduct_ReturnsProduct()
    {
        var repository = new FakeRepository();
        var existing = new Product { Name = "Monitor", Price = 800 };
        repository.Seed(existing);
        var service = new ProductService(repository);

        var result = service.GetById(existing.Id);

        Assert.Equal(existing.Id, result.Id);
        Assert.Equal("Monitor", result.Name);
        Assert.Equal(800, result.Price);
    }

    [Fact]
    public void Create_WithValidInput_ReturnsProduct()
    {
        var repository = new FakeRepository();
        var service = new ProductService(repository);
        var request = new CreateProductRequest("  Keyboard  ", 120);

        var result = service.Create(request);

        Assert.Equal("Keyboard", result.Name);
        Assert.Equal(120, result.Price);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public void Create_WithInvalidName_Throws()
    {
        var repository = new FakeRepository();
        var service = new ProductService(repository);
        var request = new CreateProductRequest("   ", 50);

        var exception = Assert.Throws<ArgumentException>(() => service.Create(request));

        Assert.Equal("Name", exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidPrice_Throws()
    {
        var repository = new FakeRepository();
        var service = new ProductService(repository);
        var request = new CreateProductRequest("Mouse", 0);

        var exception = Assert.Throws<ArgumentException>(() => service.Create(request));

        Assert.Equal("Price", exception.ParamName);
    }

    [Fact]
    public void Create_WithDuplicateName_Throws()
    {
        var repository = new FakeRepository();
        repository.Seed(new Product { Name = "Keyboard", Price = 100 });
        var service = new ProductService(repository);
        var request = new CreateProductRequest("  keyboard  ", 120);

        Assert.Throws<InvalidOperationException>(() => service.Create(request));
    }

    [Fact]
    public void Update_WithMissingProduct_Throws()
    {
        var repository = new FakeRepository();
        var service = new ProductService(repository);
        var request = new UpdateProductRequest("Monitor", 900);

        Assert.Throws<KeyNotFoundException>(() => service.Update(Guid.NewGuid(), request));
    }

    [Fact]
    public void Update_WithInvalidName_Throws()
    {
        var repository = new FakeRepository();
        var existing = new Product { Name = "Monitor", Price = 800 };
        repository.Seed(existing);
        var service = new ProductService(repository);
        var request = new UpdateProductRequest(" ", 950);

        var exception = Assert.Throws<ArgumentException>(() => service.Update(existing.Id, request));

        Assert.Equal("Name", exception.ParamName);
    }

    [Fact]
    public void Update_WithDuplicateName_Throws()
    {
        var repository = new FakeRepository();
        var existing = new Product { Name = "Monitor", Price = 800 };
        var other = new Product { Name = "Keyboard", Price = 120 };
        repository.Seed(existing);
        repository.Seed(other);
        var service = new ProductService(repository);
        var request = new UpdateProductRequest("  keyboard  ", 950);

        Assert.Throws<InvalidOperationException>(() => service.Update(existing.Id, request));
    }

    [Fact]
    public void Update_WithValidInput_ReturnsUpdatedProduct()
    {
        var repository = new FakeRepository();
        var existing = new Product { Name = "Monitor", Price = 800 };
        repository.Seed(existing);
        var service = new ProductService(repository);
        var request = new UpdateProductRequest("Monitor 2", 950);

        var result = service.Update(existing.Id, request);

        Assert.Equal(existing.Id, result.Id);
        Assert.Equal("Monitor 2", result.Name);
        Assert.Equal(950, result.Price);
    }

    [Fact]
    public void Delete_WithMissingProduct_Throws()
    {
        var repository = new FakeRepository();
        var service = new ProductService(repository);

        Assert.Throws<KeyNotFoundException>(() => service.Delete(Guid.NewGuid()));
    }

    [Fact]
    public void Delete_WithExistingProduct_RemovesProduct()
    {
        var repository = new FakeRepository();
        var existing = new Product { Name = "Monitor", Price = 800 };
        repository.Seed(existing);
        var service = new ProductService(repository);

        service.Delete(existing.Id);

        Assert.Empty(repository.GetAll());
    }
}
