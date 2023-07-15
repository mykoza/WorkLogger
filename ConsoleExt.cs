namespace Namespace;
public static class ConsoleExt
{
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
}
