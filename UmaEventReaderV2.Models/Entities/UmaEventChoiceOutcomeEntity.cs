using UmaEventReaderV2.Models.Enums;

namespace UmaEventReaderV2.Models.Entities;

public class UmaEventChoiceOutcomeEntity
{
    public long Id { get; set; }
    public OutcomeType Type { get; set; }
    public string Value { get; set; } = string.Empty;

    override public string ToString()
    {
        return Type switch
        {
            OutcomeType.Condition or OutcomeType.Unknown or OutcomeType.SkillHint or OutcomeType.EndOfEventChain => Value,
            OutcomeType.Speed or OutcomeType.Stamina or OutcomeType.Power or OutcomeType.Guts or OutcomeType.Wit
                or OutcomeType.Mood or OutcomeType.AllStats or OutcomeType.Friendship or OutcomeType.SkillPts
                or OutcomeType.Energy or OutcomeType.MaxEnergy => $"{Value} {Type}",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}