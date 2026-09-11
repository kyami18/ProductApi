using FluentValidation;
using ProductApi.DTOs;

namespace ProductApi.Validators;

public class UpdateProductRequestValidator
    : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên sản phẩm không được để trống")
            .MaximumLength(100)
            .WithMessage("Tên sản phẩm không được vượt quá 100 ký tự");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Giá sản phẩm phải lớn hơn 0");
    }
}