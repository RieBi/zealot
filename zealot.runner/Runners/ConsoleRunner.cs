namespace Zealot.Runner.Runners;
public class ConsoleRunner(Interpreter.Interpreter interpreter) : BaseRunner
{
    private readonly Interpreter.Interpreter _interpreter = interpreter;

    public override void Run()
    {
        while (true)
        {
            try
            {
                var line = GetNextLine();
                if (line is null || line == _stopString)
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

    protected override string GetNextLineInternal() => Console.ReadLine() ?? _stopString;
}
