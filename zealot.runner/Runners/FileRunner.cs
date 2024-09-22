using Zealot.Interpreter;

namespace Zealot.Runner.Runners;
internal class FileRunner(string fileName, Interpreter.Interpreter interpreter) : IRunner, IDisposable
{
	private readonly StreamReader _stream = new(fileName);
    private readonly Interpreter.Interpreter _interpreter = interpreter;
    private bool _disposed;

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

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _stream.Dispose();
            _disposed = true;
        }
    }

    ~FileRunner()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
