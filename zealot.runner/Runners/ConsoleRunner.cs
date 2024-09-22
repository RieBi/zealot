using zealot.interpreter;

namespace zealot.runner.Runners;
internal class ConsoleRunner(Interpreter interpreter) : IRunner
{
    private readonly Interpreter _interpreter = interpreter;

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
