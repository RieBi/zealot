using Zealot.Interpreter;

namespace Zealot.Runner.Runners;
public class ConsoleRunner(Interpreter.Interpreter interpreter) : IRunner
{
    private readonly Interpreter.Interpreter _interpreter = interpreter;

    public void Run()
    {
        var stopString = char.MinValue.ToString();
        while (true)
        {
            var line = Console.ReadLine();
            if (line is null || line == stopString)
                break;

            var result = _interpreter.InterpretLine(line, this);
            if (result is not null)
                Console.WriteLine(result);
        }
    }

    public string GetNextLine() => Console.ReadLine() ?? throw new InvalidOperationException("Input console stream is unexpectedly closed");
}
