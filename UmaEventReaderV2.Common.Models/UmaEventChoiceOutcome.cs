using System.Text.Json.Serialization;
using UmaEventReaderV2.Common.Models.Enums;

namespace UmaEventReaderV2.Common.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(UmaEventChoiceOutcome), typeDiscriminator: "simple")]
[JsonDerivedType(typeof(SkillOutcome), typeDiscriminator: "skill")]
public abstract class EventChoiceOutcome(OutcomeType type)
{
    public OutcomeType Type { get; init; } = type;
    override public abstract string ToString();
}

public class UmaEventChoiceOutcome(string value, OutcomeType type) : EventChoiceOutcome(type)
{
    public string Value => value;

    override public string ToString()
    {
        return Type switch
        {
            OutcomeType.GoodCondition or OutcomeType.BadCondition or OutcomeType.Unknown or OutcomeType.SkillHint
                or OutcomeType.EndOfEventChain => value,
            OutcomeType.Speed or OutcomeType.Stamina or OutcomeType.Power or OutcomeType.Guts or OutcomeType.Wit
                or OutcomeType.Mood or OutcomeType.AllStats or OutcomeType.Friendship or OutcomeType.SkillPts
                or OutcomeType.Energy or OutcomeType.MaxEnergy => $"{value} {Type}",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public class SkillOutcome(UmaSkill skill, int hints = 1) : EventChoiceOutcome(OutcomeType.SkillHint)
{
    public UmaSkill Skill => skill;
    public int Hints => hints;

    override public string ToString() => $"{Skill.Name} +{Hints}";
}