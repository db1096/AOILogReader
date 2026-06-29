using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

internal static class TxtWriter
{
    private static readonly object _lock = new();

    public static void Write(
        string model,
        string layer,
        string lot,
        Dictionary<int, PanelInfo> panels)
    {
        try
        {
            string folder = Path.Combine(Config.Output, model, layer);
            Directory.CreateDirectory(folder);

            string file = Path.Combine(folder, lot + ".txt");

            int pnlScan = panels.Count;
            int totalDefect = panels.Sum(x => x.Value.Defect);

            StringBuilder sb = new();

            sb.AppendLine($"LOT:\t{lot}");
            sb.AppendLine($"LAYER:\t{layer}");
            sb.AppendLine($"MODEL:\t{model}");
            sb.AppendLine();
            sb.AppendLine($"PNL SCAN:\t{pnlScan}");
            sb.AppendLine($"TOTAL DEFECT:\t{totalDefect}");
            sb.AppendLine();
            sb.AppendLine("PNL\tDEFECT\tSCAN TIME");
            sb.AppendLine("--------------------------------");

            foreach (var p in panels.OrderByDescending(x => x.Key))
            {
                sb.AppendLine($"{p.Value.PNL}\t{p.Value.Defect}\t{p.Value.ScanTime}");
            }

            lock (_lock)
            {
                File.WriteAllText(file, sb.ToString(), Encoding.UTF8);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
    }
}
