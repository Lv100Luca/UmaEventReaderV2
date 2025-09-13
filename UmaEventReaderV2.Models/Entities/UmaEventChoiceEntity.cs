using UmaEventReader.Abstractions.Model.Enums;

namespace UmaEventReaderV2.Models.Entities;

public class UmaEventChoiceEntity
{
    public long Id { get; set; }
    public int ChoiceNumber { get; set; }
    public string ChoiceText { get; set; } = string.Empty;
    public SuccessType SuccessType { get; set; }

    public long UmaEventId { get; set; }   // FK
    public UmaEventEntity UmaEvent { get; set; } = null!;

    public List<UmaEventChoiceOutcomeEntity> Outcomes { get; set; } = new();
}