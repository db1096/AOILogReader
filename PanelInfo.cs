using System;

internal class PanelInfo
{
    public int PNL { get; set; }

    public int Defect { get; set; }

    public string ScanTime { get; set; } = "";

    public DateTime UpdateTime { get; set; } = DateTime.Now;
}