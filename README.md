# Mini Project 05 - Unit Tests in Services
![tests](https://github.com/GustaBrito/Mini-Projeto-05---Unit-Tests-em-Services/actions/workflows/tests.yml/badge.svg)

## Unit Testing Business Logic in ASP.NET Core

Focused, production‑style unit tests for service‑layer business rules, with a clean architecture boundary and a thin API.

### What this project demonstrates
- Business logic extracted into services
- Dependency inversion for testability
- Unit tests focused on behavior
- Domain separated in App.Core (no ASP.NET dependency)
- Swagger endpoints to exercise the API
- Centralized exception handling for clean controllers
- A minimal, readable architecture for portfolio review

### Project structure
```
src/
  App.Api/
  App.Core/
tests/
  App.UnitTests/
```

### Exposed routes
- GET /api/products
- GET /api/products/{id}
- POST /api/products
- PUT /api/products/{id}
- DELETE /api/products/{id}

## Getting started

### Run the API
```powershell
dotnet run --project src/App.Api/App.Api.csproj
```

### Open Swagger
With the API running, open the Swagger UI shown in the console output (typically `https://localhost:xxxx/swagger`).

### Example requests (via Swagger or curl)
Create:
```powershell
curl -X POST https://localhost:5001/api/products `
  -H "Content-Type: application/json" `
  -d '{ "name": "Keyboard", "price": 120 }'
```

List:
```powershell
curl https://localhost:5001/api/products
```

Update:
```powershell
curl -X PUT https://localhost:5001/api/products/{id} `
  -H "Content-Type: application/json" `
  -d '{ "name": "Keyboard Pro", "price": 180 }'
```

Delete:
```powershell
curl -X DELETE https://localhost:5001/api/products/{id}
```

## Testing (xUnit)

### Run all tests
```powershell
dotnet test
```

### Run tests with clear PASS/FAIL output
```powershell
scripts/run-tests.ps1
```

### Where the tests live
- `tests/App.UnitTests/Services/ProductServiceTests.cs`

### What the tests validate
- Create with valid input → success
- Create with invalid name/price → exception
- Create with duplicate name → exception
- Update missing product → exception
- Update valid product → success
- Update to duplicate name → exception
- Get all / Get by id → success
- Delete missing / existing product → exception or success

### xUnit structure (Arrange / Act / Assert)
Each test follows the pattern:
1) Arrange: setup input and fake repository  
2) Act: call the service method  
3) Assert: verify output or exception

Example test pattern:
```csharp
[Fact]
public void Create_WithInvalidPrice_Throws()
{
    // Arrange
    var repository = new FakeRepository();
    var service = new ProductService(repository);
    var request = new CreateProductRequest("Mouse", 0);

    // Act
    var exception = Assert.Throws<ArgumentException>(() => service.Create(request));

    // Assert
    Assert.Equal("Price", exception.ParamName);
}
```

### Why this matters
- Easier refactoring
- Safer code evolution
- Faster feedback loop

### Example scenarios tested
- Valid creation
- Invalid input handling
- Not found scenarios
- Duplicate name handling

### Next
- Integration tests with real database
- API-level tests

## Error responses
The API returns consistent ProblemDetails messages:
- 400: `Product name is required.`
- 400: `Price must be greater than zero.`
- 409: `A product with the same name already exists.`
- 404: `Product not found.`

## License
MIT - see `LICENSE`.

