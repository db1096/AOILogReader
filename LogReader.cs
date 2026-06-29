using System;
using System.Collections.Generic;
using System.IO;

internal static class LogReader
{
    private static long _lastPosition = 0;

    private static readonly Dictionary<string,
        Dictionary<string,
        Dictionary<int, PanelInfo>>> _data = new();

    private static readonly HashSet<string> _cache = new();

    public static void Read()
    {
        if (!File.Exists(Config.LogFile))
            return;

        try
        {
            using FileStream fs = new FileStream(
                Config.LogFile,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete);

            // RESET DETECT (AOI overwrite file khi full size)
            if (_lastPosition > fs.Length)
            {
                _lastPosition = 0;
                _cache.Clear();
                Logger.Info("AOI LOG RESET DETECTED");
            }

            fs.Seek(_lastPosition, SeekOrigin.Begin);

            using StreamReader sr = new StreamReader(fs);

            string? line;

            while ((line = sr.ReadLine()) != null)
            {
                Process(line);
            }

            _lastPosition = fs.Position;
            PositionStore.Save(_lastPosition);
        }
        catch (IOException)
        {
            // AOI đang ghi file → bỏ qua vòng này (an toàn 24/7)
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
    }

    private static void Process(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;

        if (line.StartsWith("Version")) return;
        if (line.StartsWith("Date")) return;

        var p = line.Split(',');

        if (p.Length < 14)
            return;

        string model = p[2].Trim();
        string layer = p[3].Trim();
        string lot = p[4].Trim();

        if (!int.TryParse(p[5], out int pnl))
            return;

        if (!int.TryParse(p[13], out int defect))
            return;

        string key = $"{model}|{layer}|{lot}|{pnl}";

        if (!_cache.Add(key))
            return;

        // chống RAM tăng vô hạn
        if (_cache.Count > 20000)
            _cache.Clear();

        if (!_data.ContainsKey(model))
            _data[model] = new();

        if (!_data[model].ContainsKey(layer))
            _data[model][layer] = new();

        _data[model][layer][pnl] = new PanelInfo
        {
            PNL = pnl,
            Defect = defect,
            ScanTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
        };

        TxtWriter.Write(model, layer, lot, _data[model][layer]);
    }
}
