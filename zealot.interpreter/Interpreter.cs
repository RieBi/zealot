using zealot.interpreter.Ast.State;
using zealot.interpreter.Tokens;

namespace zealot.interpreter;
public class Interpreter
{
    private readonly Scope _internalScope = new();

    /// <summary>
    /// Interprets the <paramref name="line"/> of code and returns returned result, if any.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public string? InterpretLine(string line, IRunner runner)
	{
        var tokens = Tokenizer.Tokenize(line);

        var parser = new Parser(tokens);
        var resultNode = parser.ParseLine();

        return resultNode.Evaluate(_internalScope).Value?.ToString();
	}
}
