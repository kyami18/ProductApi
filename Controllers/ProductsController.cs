using Microsoft.AspNetCore.Mvc;
using ProductApi.Configurations;
using ProductApi.DTOs;
using ProductApi.Models;
using ProductApi.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;


namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService productService;
    private readonly ProductSettings productSettings;

    public ProductsController(
     IProductService productService,
     IOptions<ProductSettings> productSettings)
    {
        this.productService = productService;
        this.productSettings = productSettings.Value;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
    string? name,
    int page = 1,
    int? pageSize = null,
    string? sortBy = null,
    string? sortOrder = null)
    {
        int currentPageSize = pageSize ?? productSettings.DefaultPageSize;

        sortBy ??= productSettings.DefaultSortBy;
        sortOrder ??= productSettings.DefaultSortOrder;
        if (currentPageSize < 1)
        {
            currentPageSize = productSettings.DefaultPageSize;
        }

        if (currentPageSize > productSettings.MaxPageSize)
        {
            currentPageSize = productSettings.MaxPageSize;
        }
        var products = await productService.GetAll(
        name,
        page,
        currentPageSize,
        sortBy,
        sortOrder);
        var totalItems = await productService.Count(name);
        var totalPages = (int)Math.Ceiling(
            (double)totalItems / currentPageSize
        );
        if (page > totalPages && totalPages > 0)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = $"Trang {page} không tồn tại. Tổng số trang là {totalPages}.",
                Data = null
            });
        }
        var pagination = new PaginationResponse<ProductResponse>
        {
            Data = products.Select(product => new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            }).ToList(),

            Page = page,
            PageSize = currentPageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };

        var apiResponse = new ApiResponse<PaginationResponse<ProductResponse>>
        {
            Success = true,
            Message = "Lấy danh sách sản phẩm thành công",
            Data = pagination
        };



        return Ok(apiResponse); 
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(int id)
    {

        var product = await productService.GetById(id);

        if (product is null)
        {
            return NotFound(new ApiResponse<ProductResponse>
            {
                Success = false,
                Message = "Không tìm thấy sản phẩm",
                Data = null
            });
        }

        var response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };

        var apiResponse = new ApiResponse<ProductResponse>
        {
            Success = true,
            Message = "Lấy sản phẩm thành công",
            Data = response
        };

        return Ok(apiResponse);
    }

    [Authorize(Roles = "Admin")]

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductRequest request)
    {
        var product = await productService.Create(request);

        var response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };

        var apiResponse = new ApiResponse<ProductResponse>
        {
            Success = true,
            Message = "Tạo sản phẩm thành công",
            Data = response
        };

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = product.Id },
            apiResponse
        );
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(
        int id,
        [FromBody] UpdateProductRequest request)
    {
        var product = await productService.Update(id, request);

        if (product is null)
        {
            return NotFound(new ApiResponse<ProductResponse>
            {
                Success = false,
                Message = "Không tìm thấy sản phẩm",
                Data = null
            });
        }

        var response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };

        var apiResponse = new ApiResponse<ProductResponse>
        {
            Success = true,
            Message = "Cập nhật sản phẩm thành công",
            Data = response
        };

        return Ok(apiResponse);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await productService.Delete(id);

        if (!deleted)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Không tìm thấy sản phẩm",
                Data = null
            });
        }

        return NoContent();
    }



}