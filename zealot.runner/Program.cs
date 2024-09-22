using Zealot.Interpreter;
using Zealot.Runner.Runners;

namespace Zealot.Runner;

public static class Program
{
    public static void Main(string[] args)
    {
        var commandDetails = CommandParser.ParseInput(args);
        var interpreter = new Interpreter.Interpreter();

        if (commandDetails.FileName is not null)
        {
            var fileRunner = new FileRunner(commandDetails.FileName, interpreter);
            fileRunner.Run();
        }

        if (commandDetails.IsInteractiveMode)
        {
            var consoleRunner = new ConsoleRunner(interpreter);
            consoleRunner.Run();
        }
    }
}
