using System;
using System.IO;

internal static class PositionStore
{
    private static readonly string FilePath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "position.dat");

    public static long Load()
    {
        try
        {
            if (!File.Exists(FilePath))
                return 0;

            return long.Parse(File.ReadAllText(FilePath));
        }
        catch
        {
            return 0;
        }
    }

    public static void Save(long pos)
    {
        try
        {
            File.WriteAllText(FilePath, pos.ToString());
        }
        catch
        {
        }
    }

    public static void Reset()
    {
        try
        {
            File.WriteAllText(FilePath, "0");
        }
        catch
        {
        }
    }
}
