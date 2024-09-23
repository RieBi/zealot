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
            try
            {
                var line = Console.ReadLine();
                if (line is null || line == stopString)
                    break;

                var result = _interpreter.InterpretLine(line, this);
                if (result is not null)
                    Console.WriteLine(result);
            }
            catch (InvalidOperationException exc)
            {
                Console.WriteLine(exc.Message);
            }
            catch (Exception exc)
            {
                Console.WriteLine("FATAL ERROR:");
                Console.WriteLine(exc);
                throw;
            }
        }
    }

    public string GetNextLine() => Console.ReadLine() ?? throw new InvalidOperationException("Input console stream is unexpectedly closed");
}
