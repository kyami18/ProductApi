namespace ProductApi.Configurations;

public class ProductSettings
{
    public string DefaultSortBy { get; set; } = "id";
    public string DefaultSortOrder { get; set; } = "asc";
    public int DefaultPageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 100;
}