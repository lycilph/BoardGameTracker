using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using FluentValidation;
using MediatR;

namespace BoardGameTracker.Application.Identity.Commands;

public record class UpdatePasswordCommand(UpdatePasswordRequest Request) : IRequest<UpdatePasswordResponse>;

public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator(IValidator<UpdatePasswordRequest> request_validator)
    {
        RuleFor(x => x.Request).NotNull();
        RuleFor(x => x.Request).SetValidator(request_validator);
    }
}

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, UpdatePasswordResponse>
{
    private readonly IValidator<UpdatePasswordCommand> validator;
    private readonly IIdentityService identity_service;

    public UpdatePasswordCommandHandler(IValidator<UpdatePasswordCommand> validator, IIdentityService identity_service)
    {
        this.validator = validator;
        this.identity_service = identity_service;
    }

    public async Task<UpdatePasswordResponse> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(command, cancellationToken);
        if (!validation_result.IsValid)
            return UpdatePasswordResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        var request = command.Request;

        var user = await identity_service.FindUserByIdAsync(request.UserId);
        if (user == null)
            return UpdatePasswordResponse.Failure("Unknown user");

        var response = await identity_service.ChangePasswordAsync(user, request.Password, request.NewPassword);
        if (!response.Succeeded)
            return UpdatePasswordResponse.Failure(response.Errors.Select(e => e.Description));

        return UpdatePasswordResponse.Success();
    }
}