using Zealot.Interpreter;

namespace Zealot.Runner.Runners;
public abstract class BaseRunner : IRunner
{
    private string? _storedLine = default;
    private bool _returned = false;
    protected readonly string _stopString = char.MinValue.ToString();

    public string GetNextLine()
    {
        if (_returned && _storedLine is not null)
        {
            _returned = false;
            return _storedLine;
        }

        var line = GetNextLineInternal();
        _storedLine = line;
        return line;
    }

    protected abstract string GetNextLineInternal();

    public string ReturnPreviousLine()
    {
        if (_returned || _storedLine is null)
            throw new InvalidOperationException($"Cannot return a previous line before calling {nameof(GetNextLine)}.");

        _returned = true;
        return _storedLine;
    }

    public abstract void Run();
}
