using Microsoft.AspNetCore.Mvc;
using ProductApi.Configurations;
using ProductApi.DTOs;
using ProductApi.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using ProductApi.Extensions.Mappings;
using ProductApi.Extensions;

namespace ProductApi.Controllers;

/// <summary>
/// Quản lý các API liên quan đến sản phẩm.
/// </summary>
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

    /// <summary>
    /// Lấy danh sách sản phẩm có hỗ trợ tìm kiếm, phân trang và sắp xếp.
    /// </summary>
    /// <param name="name">Tên sản phẩm cần tìm kiếm.</param>
    /// <param name="page">Số trang, bắt đầu từ 1.</param>
    /// <param name="pageSize">Số sản phẩm trên mỗi trang.</param>
    /// <param name="sortBy">Trường dùng để sắp xếp: id, name hoặc price.</param>
    /// <param name="sortOrder">Thứ tự sắp xếp: asc hoặc desc.</param>
    /// <returns>Danh sách sản phẩm kèm thông tin phân trang.</returns>
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
            Data = products
            .Select(product => product.ToResponse())
            .ToList(),

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
    /// <summary>
    /// Lấy thông tin một sản phẩm theo ID.
    /// </summary>
    /// <param name="id">ID của sản phẩm.</param>
    /// <returns>Thông tin sản phẩm nếu tìm thấy.</returns>
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

        var response = product.ToResponse();
        var apiResponse = new ApiResponse<ProductResponse>
        {
            Success = true,
            Message = "Lấy sản phẩm thành công",
            Data = response
        };

        return Ok(apiResponse);
    }
    /// <summary>
    /// Tạo một sản phẩm mới.
    /// </summary>
    /// <param name="request">Thông tin sản phẩm cần tạo.</param>
    /// <returns>Sản phẩm vừa được tạo.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductRequest request)

    {

        var product = await productService.Create(request);

        var response = product.ToResponse();

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
    /// <summary>
    /// Cập nhật thông tin một sản phẩm.
    /// </summary>
    /// <param name="id">ID của sản phẩm cần cập nhật.</param>
    /// <param name="request">Thông tin mới của sản phẩm.</param>
    /// <returns>Sản phẩm sau khi cập nhật.</returns>
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

        var response = product.ToResponse();

        var apiResponse = new ApiResponse<ProductResponse>
        {
            Success = true,
            Message = "Cập nhật sản phẩm thành công",
            Data = response
        };

        return Ok(apiResponse);
    }
    /// <summary>
    /// Xóa một sản phẩm.
    /// </summary>
    /// <param name="id">ID của sản phẩm cần xóa.</param>
    /// <returns>Không có nội dung nếu xóa thành công.</returns>
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