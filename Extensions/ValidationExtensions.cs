using FluentValidation.Results;
using ProductApi.DTOs;

namespace ProductApi.Extensions;

public static class ValidationExtensions
{
    public static ApiResponse<object> ToApiResponse(
        this ValidationResult validationResult)
    {
        return new ApiResponse<object>
        {
            Success = false,
            Message = "Dữ liệu không hợp lệ",
            Data = validationResult.Errors
                .Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                })
                .ToList()
        };
    }
}