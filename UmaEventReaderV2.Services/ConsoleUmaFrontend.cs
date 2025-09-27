// using UmaEventReaderV2.Abstractions;
// using UmaEventReaderV2.Models.Entities;
//
// namespace UmaEventReaderV2.Services;
//
// public class ConsoleUmaFrontend : IUmaFrontend
// {
//     public async Task ShowEventAsync(UmaEventEntity umaEvent)
//     {
//         await Console.Out.WriteLineAsync(umaEvent.ToString());
//     }
//
//     // career info not supported using the regular console
//     public Task ShowCareerAsync(string careerInfo)
//     {
//         return Task.CompletedTask;
//     }
//
//     public async Task LogAsync(string message)
//     {
//         await Console.Out.WriteLineAsync(message);
//     }
//
//     public string GetSearchQuery()
//     {
//         throw new NotImplementedException();
//     }
//
//     public void ResetSearchQuery()
//     {
//         throw new NotImplementedException();
//     }
// }

