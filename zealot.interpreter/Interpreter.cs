using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Tokens;

namespace Zealot.Interpreter;
public class Interpreter
{
    private readonly Scope _internalScope = Scope.CreateGlobal();

    /// <summary>
    /// Interprets the <paramref name="line"/> of code and returns returned result, if any.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public string? InterpretLine(string line, IRunner runner)
	{
        var tokens = Tokenizer.Tokenize(line);

        var parser = new Parser(tokens, runner);
        var resultNode = parser.ParseLine();

        var result = resultNode.Evaluate(_internalScope);

        return result.Name == "empty" ? null : result.Value.ToString();
	}
}
