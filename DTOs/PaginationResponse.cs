namespace ProductApi.DTOs;

public class PaginationResponse<T>
{
    public List<T> Data { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }
}