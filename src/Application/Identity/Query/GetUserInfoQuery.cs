using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using FluentValidation;
using MediatR;

namespace BoardGameTracker.Application.Identity.Query;

public record class GetUserInfoQuery(string UserId) : IRequest<GetUserInfoResponse>;

public class GetUserInfoQueryValidator : AbstractValidator<GetUserInfoQuery>
{
    public GetUserInfoQueryValidator()
    {
        RuleFor(x => x.UserId).NotNull().WithMessage("UserId is required");
    }
}

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, GetUserInfoResponse>
{
    private readonly IValidator<GetUserInfoQuery> validator;
    private readonly IIdentityService identity_service;

    public GetUserInfoQueryHandler(IValidator<GetUserInfoQuery> validator, IIdentityService identity_service)
    {
        this.validator = validator;
        this.identity_service = identity_service;
    }

    public async Task<GetUserInfoResponse> Handle(GetUserInfoQuery query, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(query, cancellationToken);
        if (!validation_result.IsValid)
            return GetUserInfoResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        var user = await identity_service.FindUserByIdAsync(query.UserId);
        if (user == null)
            return GetUserInfoResponse.Failure("Unknown user");

        return GetUserInfoResponse.Success(user.AccountCreated, user.LastActive);
    }
}