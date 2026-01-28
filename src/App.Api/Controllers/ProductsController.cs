using App.Core.Dtos;
using App.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpPost]
    public ActionResult<ProductResponse> Create([FromBody] CreateProductRequest request)
    {
        var created = _service.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<ProductResponse>> GetAll()
    {
        var products = _service.GetAll();
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<ProductResponse> GetById([FromRoute] Guid id)
    {
        var product = _service.GetById(id);
        return Ok(product);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<ProductResponse> Update([FromRoute] Guid id, [FromBody] UpdateProductRequest request)
    {
        var updated = _service.Update(id, request);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        _service.Delete(id);
        return NoContent();
    }
}
