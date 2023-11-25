namespace WorkLogger.Ui.ConsoleUi;

public static class ConsoleExt
{
    private static readonly ConsoleColor _warningColor = ConsoleColor.Yellow;

    public static void WriteColor(string text, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
    {
        var oldForeground = Console.ForegroundColor;
        var oldBackground = Console.BackgroundColor;

        Console.ForegroundColor = foreground;
        Console.BackgroundColor = background;
        Console.Write(text);

        Console.ForegroundColor = oldForeground;
        Console.BackgroundColor = oldBackground;
    }

    public static void WriteLineColor(string text, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
    {
        WriteColor(text, foreground, background);
        Console.WriteLine();
    }

    public static void WriteLineWarning(string text)
    {
        WriteLineColor(text, _warningColor, ConsoleColor.Black);
    }

    public static void WriteWarning(string text)
    {
        WriteColor(text, _warningColor, ConsoleColor.Black);
    }
}
