namespace zealot.runner;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"Total args: {args.Length}");
        Console.WriteLine(string.Join('\n', args));

        Console.WriteLine($"Working directory: {Environment.CurrentDirectory}");
    }
}
