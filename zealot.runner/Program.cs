using zealot.interpreter;
using zealot.runner.Runners;

namespace zealot.runner;

internal static class Program
{
    static void Main(string[] args)
    {
        var commandDetails = CommandParser.ParseInput(args);
        var interpreter = new Interpreter();

        if (commandDetails.FileName is not null)
        {
            var fileRunner = new FileRunner(commandDetails.FileName, interpreter);
            fileRunner.Run();
        }
    }
}
