using System.Text;

namespace UmaEventReaderV2.Models.Entities;

public class UmaEventEntity
{
    public long Id { get; set; }
    public string CharacterName { get; set; }
    public string EventName { get; set; }

    public List<UmaEventChoiceEntity> Choices { get; set; } = [];

    override public string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"{EventName} ({CharacterName})");
        sb.AppendLine();

        foreach (var choice in Choices)
        {
            sb.AppendLine($"  Choice #{choice.ChoiceNumber}: {choice.ChoiceText} ({choice.SuccessType})");

            if (choice.Outcomes.Count > 0)
            {
                // Use ToString() on each outcome or customize
                sb.AppendLine("    - Outcomes: " + string.Join(", ", choice.Outcomes));
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}