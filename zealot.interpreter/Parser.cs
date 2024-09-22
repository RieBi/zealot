using Zealot.Interpreter.Ast.Nodes;
using Zealot.Interpreter.Ast.Types;
using Zealot.Interpreter.Tokens;

namespace Zealot.Interpreter;
internal class Parser(List<Token> tokens)
{
    private readonly List<Token> _tokens = tokens;
    private int _pos = 0;

    public AbstractNode ParseLine() => ParseAdditionOperator();

    public ValueNode ParseConstantNumber()
    {
        if (IsAt(TokenKind.ConstantNumberInteger) || IsAt(TokenKind.ConstantNumberDouble))
        {
            var token = Next();
            var info = new TypeInfo("number", double.Parse(token.Value));
            return new(info);
        }
        
        throw new InvalidOperationException("Invalid constant number detected.");
    }

    public AbstractNode ParseUnaryMinus()
    {
        if (IsAt(TokenKind.SubtractionOperator))
        {
            Next();

            var right = ParseConstantNumber();
            return new UnaryMinusNode(right);
        }

        return ParseConstantNumber();
    }

    public AbstractNode ParseMultiplicationOperator()
    {
        AbstractNode left = ParseUnaryMinus();

        while (IsAt(TokenKind.MultiplicationOperator) || IsAt(TokenKind.DivisionOperator))
        {
            var token = Next();

            var right = ParseUnaryMinus();
            left = new BinaryOperatorNode(left, right, token.Kind);
        }

        return left;
    }

    public AbstractNode ParseAdditionOperator()
    {
        AbstractNode left = ParseMultiplicationOperator();

        while (IsAt(TokenKind.AdditionOperator) || IsAt(TokenKind.SubtractionOperator))
        {
            var token = Next();

            var right = ParseMultiplicationOperator();
            left = new BinaryOperatorNode(left, right, token.Kind);
        }

        return left;
    }

    private Token At() => _pos < _tokens.Count ? _tokens[_pos] : Token.EndOfLineToken;

    private bool IsAt(TokenKind tokenKind) => At().Kind == tokenKind;

    private Token Next() => _pos < _tokens.Count ? _tokens[_pos++] : Token.EndOfLineToken;
}
