using System.Numerics;
using zealot.interpreter.Ast.Nodes;
using zealot.interpreter.Ast.Types;
using zealot.interpreter.Tokens;

namespace zealot.interpreter;
internal class Parser(List<Token> tokens)
{
    private readonly List<Token> _tokens = tokens;
    private int _pos = 0;

    public AbstractNode ParseLine() => ParseConstantNumber();

    public ValueNode ParseConstantNumber()
    {
        if (IsAt(TokenKind.ConstantNumberInteger))
        {
            var token = Next();
            var info = new TypeInfo("integer", BigInteger.Parse(token.Value));
            return new(info);
        }
        else if (IsAt(TokenKind.ConstantNumberDouble))
        {
            var token = Next();
            var info = new TypeInfo("double", double.Parse(token.Value));
        }

        throw new InvalidOperationException("Invalid constant number detected.");
    }

    private Token At() => _pos < _tokens.Count ? _tokens[_pos] : Token.EndOfLineToken;

    private bool IsAt(TokenKind tokenKind) => At().Kind == tokenKind;

    private Token Next() => _pos < _tokens.Count ? _tokens[_pos++] : Token.EndOfLineToken;
}
