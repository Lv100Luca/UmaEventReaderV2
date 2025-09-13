using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Abstractions;

public interface IUmaEventRepository
{
    IEnumerable<UmaEventEntity> GetAll();
    IEnumerable<UmaEventEntity> GetAllByName(string eventName);
    IEnumerable<UmaEventEntity> GetAllByChoiceText(string choiceText);
}