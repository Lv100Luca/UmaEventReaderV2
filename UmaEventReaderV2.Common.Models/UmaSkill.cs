using UmaEventReaderV2.Common.Models.Enums;
using UmaEventReaderV2.Common.Models.Enums.Skills;

namespace UmaEventReaderV2.Common.Models;

public class UmaSkill
{
    public string Name { get; init; } = string.Empty;

    public SkillCondition Condition { get; init; }
    public SkillType Type { get; init; }
    public List<SkillTag> Tags { get; init; } = [];
}