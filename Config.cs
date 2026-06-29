using System;
using System.IO;

internal static class Config
{
    public static string LogFile = "";
    public static string Output = "";
    public static int IntervalMs = 1000;

    public static void Load(string configFile)
    {
        if (!File.Exists(configFile))
            throw new FileNotFoundException($"Không tìm thấy file cấu hình: {configFile}");

        foreach (string line in File.ReadAllLines(configFile))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.TrimStart().StartsWith("#"))
                continue;

            int index = line.IndexOf('=');

            if (index <= 0)
                continue;

            string key = line[..index].Trim();
            string value = line[(index + 1)..].Trim();

            switch (key.ToUpper())
            {
                case "LOGFILE":
                    if (Directory.Exists(value))
                    {
                        LogFile = Path.Combine(value, "InspectionHistory.csv");
                    }
                    else
                    {
                        LogFile = value;
                    }
                    break;

                case "OUTPUT":
                    Output = value;
                    break;

                case "INTERVALMS":
                    if (int.TryParse(value, out int interval))
                        IntervalMs = interval;
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(LogFile))
            throw new Exception("LogFile chưa được cấu hình.");

        if (string.IsNullOrWhiteSpace(Output))
            throw new Exception("Output chưa được cấu hình.");

        Directory.CreateDirectory(Output);
    }
}