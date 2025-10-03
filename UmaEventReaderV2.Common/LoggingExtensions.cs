using Microsoft.Extensions.Logging;

namespace UmaEventReaderV2.Common;

public static class LoggerExtensions
{
    public static void LogSuccess(this ILogger logger, string message)
    {
        ArgumentNullException.ThrowIfNull(logger);

        var originalColor = Console.ForegroundColor;

        try
        {
            Console.ForegroundColor = ConsoleColor.Green;
            logger.LogInformation(message); // Still logs as Information, color is just for console
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }

    // Optional overload with params
    public static void LogSuccess(this ILogger logger, string message, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(logger);

        var formattedMessage = string.Format(message, args);
        LogSuccess(logger, formattedMessage);
    }
}