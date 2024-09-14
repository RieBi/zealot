using zealot.interpreter;

namespace zealot.runner.Runners;
internal class FileRunner(string fileName, Interpreter interpreter) : IRunner, IDisposable
{
	private readonly StreamReader _stream = new(fileName);
    private readonly Interpreter _interpreter = interpreter;

    public void Dispose()
    {
        _stream.Dispose();
    }

    public void Run()
    {
        while (!_stream.EndOfStream)
        {
            var line = _stream.ReadLine()!;
            var result = _interpreter.InterpretLine(line, this);
            if (result is not null)
                Console.WriteLine(result);
        }
    }

    public string GetNextLine()
    {
        return _stream.ReadLine() ?? throw new InvalidOperationException("Unexpected end of file.");
    }
}
