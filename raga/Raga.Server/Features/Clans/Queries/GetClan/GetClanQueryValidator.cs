using FluentValidation;

namespace Raga.Server.Features.Clans.Queries.GetClan;

public class GetClanQueryValidator : AbstractValidator<GetClanQuery>
{
    public GetClanQueryValidator()
    {
        RuleFor(x => x.ClanId)
            .NotEmpty().WithMessage("ClanId is required.");
    }
}