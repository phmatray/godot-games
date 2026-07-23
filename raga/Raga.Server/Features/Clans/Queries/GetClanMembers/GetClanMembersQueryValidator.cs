using FluentValidation;

namespace Raga.Server.Features.Clans.Queries.GetClanMembers;

public class GetClanMembersQueryValidator : AbstractValidator<GetClanMembersQuery>
{
    public GetClanMembersQueryValidator()
    {
        RuleFor(x => x.ClanId)
            .NotEmpty().WithMessage("ClanId is required.");
    }
}