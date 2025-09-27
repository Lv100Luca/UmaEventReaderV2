using System.Text;

namespace UmaEventReaderV2.Models.Entities;

public class UmaEventEntity
{
    public long Id { get; set; }
    public string CharacterName { get; init; } = string.Empty;
    public string EventName { get; init; } = string.Empty;

    public Dictionary<long, UmaEventChoiceEntity> Choices { get; init; } = [];

    override public string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{EventName} ({CharacterName})");
        sb.AppendLine();

        foreach (var choice in Choices.Values)
        {
            sb.AppendLine($"  Choice #{choice.ChoiceNumber}: {choice.ChoiceText} ({choice.SuccessType})");

            if (choice.Outcomes.Count > 0)
            {
                // Use ToString() on each outcome or customize
                sb.AppendLine("    - Outcomes: " + string.Join(", ", choice.Outcomes.Values));
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}