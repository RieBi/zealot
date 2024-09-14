namespace zealot.interpreter;
public class Interpreter(IRunner runner)
{
	private IRunner _runner = runner;

    /// <summary>
    /// Interprets the <paramref name="line"/> of code and returns returned result, if any.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public string? InterpretLine(string line)
	{
        return line;
	}
}
