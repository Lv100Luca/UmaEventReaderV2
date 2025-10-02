using UmaEventReaderV2.Common.Models.Enums;

namespace UmaEventReaderV2.Common.Models;

public class UmaEventChoiceOutcome
{
    public OutcomeType Type { get; set; }
    public string Value { get; set; } = string.Empty;

    override public string ToString()
    {
        return Type switch
        {
            OutcomeType.GoodCondition or OutcomeType.BadCondition or OutcomeType.Unknown or OutcomeType.SkillHint or OutcomeType.EndOfEventChain => Value,
            OutcomeType.Speed or OutcomeType.Stamina or OutcomeType.Power or OutcomeType.Guts or OutcomeType.Wit
                or OutcomeType.Mood or OutcomeType.AllStats or OutcomeType.Friendship or OutcomeType.SkillPts
                or OutcomeType.Energy or OutcomeType.MaxEnergy => $"{Value} {Type}",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}