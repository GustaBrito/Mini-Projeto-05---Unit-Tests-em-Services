using App.Core.Dtos;

namespace App.Core.Services;

public interface IProductService
{
    IReadOnlyList<ProductResponse> GetAll();
    ProductResponse GetById(Guid id);
    ProductResponse Create(CreateProductRequest request);
    ProductResponse Update(Guid id, UpdateProductRequest request);
    void Delete(Guid id);
}
