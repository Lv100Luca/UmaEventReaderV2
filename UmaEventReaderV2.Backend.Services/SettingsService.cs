using System.Text.Json;
using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Services;

public class SettingsService : ISettingsService
{
    private readonly ILogger<SettingsService> logger;

    private readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
    private readonly string filePath = Path.Combine(AppContext.BaseDirectory, "settings.json");

    private readonly UmaEventReaderSettings currentSettings;

    public SettingsService(ILogger<SettingsService> logger)
    {
        this.logger = logger;

        // Load settings from file if available; otherwise use defaults
        currentSettings = LoadSettingsFromFile();
    }

    public UmaEventReaderSettings Settings => currentSettings;

    public async Task UpdateSettingsAsync(Action<UmaEventReaderSettings> update)
    {
        update(currentSettings);

        logger.LogInformation("Settings updated {@settings}", currentSettings);
        logger.LogInformation("Saving settings to {Location}", filePath);

        var json = JsonSerializer.Serialize(currentSettings, jsonSerializerOptions);
        await File.WriteAllTextAsync(filePath, json);
    }

    private UmaEventReaderSettings LoadSettingsFromFile()
    {
        if (!File.Exists(filePath))
        {
            logger.LogWarning("Settings file not found at {FilePath}, using default settings", filePath);
            return new UmaEventReaderSettings();
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var settings = JsonSerializer.Deserialize<UmaEventReaderSettings>(json);

            if (settings is null)
            {
                logger.LogWarning("Settings file is empty or invalid JSON, using defaults");
                return new UmaEventReaderSettings();
            }

            logger.LogInformation("Settings loaded {@Settings}", settings);
            return settings;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load settings file {FilePath}, using defaults", filePath);
            return new UmaEventReaderSettings();
        }
    }
}