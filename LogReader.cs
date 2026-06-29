using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class LogReader
{
    static FileStream fs;
    static long lastPos = 0;

    static Dictionary<string,
        Dictionary<string,
        Dictionary<int, PanelInfo>>> data = new();

    static HashSet<string> cache = new();

    public static void Read()
    {
        if (!File.Exists(Config.LogFile))
            return;

        fs ??= new FileStream(
            Config.LogFile,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite);

        // reset file detect (AOI restart log)
        if (lastPos > fs.Length)
            lastPos = 0;

        fs.Seek(lastPos, SeekOrigin.Begin);

        using var sr = new StreamReader(fs, leaveOpen: true);

        string line;
        while ((line = sr.ReadLine()) != null)
        {
            Process(line);
        }

        lastPos = fs.Position;
    }

    static void Process(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return;
        if (line.StartsWith("Version")) return;
        if (line.StartsWith("Date,")) return;

        var p = line.Split(',');

        if (p.Length < 14) return;

        string model = p[2].Trim();
        string layer = p[3].Trim();
        string lot = p[4].Trim();

        if (!int.TryParse(p[5], out int pnl)) return;
        if (!int.TryParse(p[13], out int defect)) return;

        string key = $"{model}|{layer}|{lot}|{pnl}";
        if (cache.Contains(key)) return;

        cache.Add(key);

        // chống full RAM khi chạy lâu
        if (cache.Count > 5000)
            cache.Clear();

        if (!data.ContainsKey(model))
            data[model] = new();

        if (!data[model].ContainsKey(layer))
            data[model][layer] = new();

        data[model][layer][pnl] = new PanelInfo
        {
            PNL = pnl,
            Defect = defect
        };

        TxtWriter.Write(model, layer, lot, data[model][layer]);
    }
}