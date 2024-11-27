using FluentValidation;
using WriteOffService.Application.DTOs.Request.WriteOffRequest;
using WriteOffService.Domain.Enums;

namespace WriteOffService.Presentation.Validators;

public class UpdateWriteOffRequestDtoValidator : AbstractValidator<UpdateWriteOffRequestDto>
{
    public UpdateWriteOffRequestDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value");

        When(x => x.Status == RequestStatus.Rejected || x.Status == RequestStatus.Created, () =>
        {
            RuleFor(x => x.ApprovedByUserId)
                .NotEmpty().WithMessage("ApprovedByUserId is required when request is rejected or created");
        });
    }
}