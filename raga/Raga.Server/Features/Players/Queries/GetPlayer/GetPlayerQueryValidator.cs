using FluentValidation;

namespace Raga.Server.Features.Players.Queries.GetPlayer;

public class GetPlayerQueryValidator : AbstractValidator<GetPlayerQuery>
{
    public GetPlayerQueryValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("PlayerId is required.");
    }
}