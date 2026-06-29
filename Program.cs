using System;
using System.IO;
using System.Threading;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.Title = "AOI Log Reader";

        try
        {
            Config.Load("config.ini");

            Directory.CreateDirectory(Config.Output);

            Logger.Info("=======================================");
            Logger.Info(" AOI LOG READER STARTED");
            Logger.Info("=======================================");
            Logger.Info("LOG FILE : " + Config.LogFile);
            Logger.Info("OUTPUT   : " + Config.Output);
            Logger.Info("INTERVAL : " + Config.IntervalMs + " ms");
            Logger.Info("=======================================");

            Console.WriteLine("=======================================");
            Console.WriteLine(" AOI LOG READER STARTED");
            Console.WriteLine("=======================================");
            Console.WriteLine("LOG FILE : " + Config.LogFile);
            Console.WriteLine("OUTPUT   : " + Config.Output);
            Console.WriteLine("INTERVAL : " + Config.IntervalMs + " ms");
            Console.WriteLine("=======================================");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        while (true)
        {
            try
            {
                LogReader.Read();
            }
            catch (IOException ex)
            {
                Logger.Error("IO ERROR : " + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error("ACCESS ERROR : " + ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

            Thread.Sleep(Config.IntervalMs);
        }
    }
}