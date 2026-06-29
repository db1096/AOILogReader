using System;
using System.IO;

internal static class Logger
{
    private static readonly object _lock = new();

    private static readonly string LogPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

    static Logger()
    {
        Directory.CreateDirectory(LogPath);
    }

    private static string LogFile =>
        Path.Combine(LogPath, $"AOILog_{DateTime.Now:yyyyMMdd}.log");

    public static void Info(string message)
    {
        Write("INFO", message);
    }

    public static void Error(string message)
    {
        Write("ERROR", message);
    }

    private static void Write(string level, string message)
    {
        try
        {
            lock (_lock)
            {
                string text =
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

                Console.WriteLine(text);

                File.AppendAllText(
                    LogFile,
                    text + Environment.NewLine);
            }
        }
        catch
        {
            // Không để Logger làm chương trình dừng
        }
    }
}