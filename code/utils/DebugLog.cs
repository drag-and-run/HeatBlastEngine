namespace HeatBlastEngine;

public static class DebugLog
{
    private static void Log(string msg)
    {
        Console.WriteLine($"[{Time.Elapsed}]: {msg}");
    }

    public static void Error(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Log(msg);
        Console.ResetColor();
    }
    public static void Stats(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Log(msg);
        Console.ResetColor();
    }
    public static void Warning(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Log(msg);
        Console.ResetColor();
    }
    public static void Msg(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Log(msg);
        Console.ResetColor();
    }
}