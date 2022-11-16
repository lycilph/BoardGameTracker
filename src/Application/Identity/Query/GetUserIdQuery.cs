using BoardGameTracker.Application.Identity.DTO;
using FluentValidation;
using MediatR;
using System.Security.Claims;

namespace BoardGameTracker.Application.Identity.Query;

public record class GetUserIdQuery(string Token) : IRequest<GetUserIdResponse>;

public class GetUserIdQueryValidator : AbstractValidator<GetUserIdQuery>
{
    public GetUserIdQueryValidator()
    {
        RuleFor(x => x.Token).NotNull().WithMessage("Token is required");
    }
}

public class GetUserIdQueryHandler : IRequestHandler<GetUserIdQuery, GetUserIdResponse>
{
    private readonly IValidator<GetUserIdQuery> validator;

    public GetUserIdQueryHandler(IValidator<GetUserIdQuery> validator)
    {
        this.validator = validator;
    }

    public async Task<GetUserIdResponse> Handle(GetUserIdQuery query, CancellationToken cancellationToken)
    {
        var validation_result = await validator.ValidateAsync(query, cancellationToken);
        if (!validation_result.IsValid)
            return GetUserIdResponse.Failure(validation_result.Errors.Select(e => e.ErrorMessage));

        var claims = JwtParser.ParseClaimsFromJwt(query.Token);
        var userid = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        return GetUserIdResponse.Success(userid);
    }
}