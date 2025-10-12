using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Abstractions;

/// <summary>
/// Service for managing the settings
/// </summary>
public interface ISettingsService
{
    UmaEventReaderSettings Settings { get; }

    Task UpdateSettingsAsync(Action<UmaEventReaderSettings> settings);
}