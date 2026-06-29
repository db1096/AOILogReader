using System;
using System.IO;

internal static class PositionStore
{
    private static readonly string PositionFile =
        Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "position.dat");

    public static long Load()
    {
        try
        {
            if (!File.Exists(PositionFile))
                return 0;

            string txt = File.ReadAllText(PositionFile).Trim();

            if (long.TryParse(txt, out long pos))
                return pos;
        }
        catch
        {
        }

        return 0;
    }

    public static void Save(long position)
    {
        try
        {
            File.WriteAllText(
                PositionFile,
                position.ToString());
        }
        catch
        {
        }
    }

    public static void Reset()
    {
        try
        {
            File.WriteAllText(PositionFile, "0");
        }
        catch
        {
        }
    }
}