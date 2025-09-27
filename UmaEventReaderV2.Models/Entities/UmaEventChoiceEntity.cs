using UmaEventReaderV2.Models.Enums;

namespace UmaEventReaderV2.Models.Entities;

public class UmaEventChoiceEntity
{
    public long Id { get; set; }
    public int ChoiceNumber { get; set; }
    public string ChoiceText { get; set; } = string.Empty;
    public SuccessType SuccessType { get; set; }

    public Dictionary<long, UmaEventChoiceOutcomeEntity> Outcomes { get; set; } = new();
}