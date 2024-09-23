using Zealot.Interpreter.Ast.Nodes;
using Zealot.Interpreter.Ast.Types;
using Zealot.Interpreter.Tokens;

namespace Zealot.Interpreter;
internal class Parser(List<Token> tokens)
{
    private readonly List<Token> _tokens = tokens;
    private int _pos = 0;

    public AbstractNode ParseLine() => ParseVariableAssignment();

    public AbstractNode ParseBasicExpression()
    {
        if (IsAt(TokenKind.ConstantNumberInteger) || IsAt(TokenKind.ConstantNumberDouble))
        {
            var token = Next();
            var info = new TypeInfo("number", double.Parse(token.Value));
            return new ValueNode(info);
        }
        else if (IsAt(TokenKind.Identifier))
        {
            var token = Next();
            var identifier = new IdentifierNode(token.Value);
            return identifier;
        }
        
        throw new InvalidOperationException("Invalid basic expression detected.");
    }

    public AbstractNode ParseParentheses()
    {
        if (IsAt(TokenKind.ParantheseOpen))
        {
            Next();
            var expression = ParseLine();

            Expect(TokenKind.ParantheseClosed);
            return expression;
        }

        return ParseBasicExpression();
    }

    public AbstractNode ParseExponentiationOperator()
    {
        AbstractNode left = ParseParentheses();

        while (IsAt(TokenKind.ExponentiationOperator))
        {
            var token = Next();

            var right = ParseExponentiationOperator();
            left = new BinaryOperatorNode(left, right, token.Kind);
        }

        return left;
    }

    public AbstractNode ParseUnaryMinus()
    {
        if (IsAt(TokenKind.SubtractionOperator))
        {
            Next();

            var right = ParseExponentiationOperator();
            return new UnaryMinusNode(right);
        }

        return ParseExponentiationOperator();
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

    public AbstractNode ParseVariableDefinition()
    {
        if (IsAt(TokenKind.VariableDefinition))
        {
            Next();

            var identifier = ParseBasicExpression();
            if (identifier is not IdentifierNode identifierNode)
                throw new InvalidOperationException("Invalid name when defining a variable.");

            Expect(TokenKind.AssignmentOperator);
            var right = ParseAdditionOperator();

            return new VariableDefinitionNode(identifierNode, right);
        }

        return ParseAdditionOperator();
    }

    public AbstractNode ParseVariableAssignment()
    {
        var left = ParseVariableDefinition();
        if (IsAt(TokenKind.AssignmentOperator))
        {
            Next();

            if (left is not IdentifierNode identifier)
                throw new InvalidOperationException("Cannot assign a value to a non-variable.");

            var right = ParseAdditionOperator();
            return new AssignmentStatementNode(identifier, right);
        }

        return left;
    }

    /// <summary>
    /// Returns the next token in the sequence.
    /// </summary>
    /// <returns></returns>
    private Token At() => _pos < _tokens.Count ? _tokens[_pos] : Token.EndOfLineToken;

    /// <summary>
    /// Determines whether the next token in the sequence is of type <paramref name="tokenKind"/>.
    /// </summary>
    /// <param name="tokenKind"></param>
    /// <returns></returns>
    private bool IsAt(TokenKind tokenKind) => At().Kind == tokenKind;

    /// <summary>
    /// Advances from the current token to the next one and returns the current one (before advancing).
    /// </summary>
    /// <returns></returns>
    private Token Next() => _pos < _tokens.Count ? _tokens[_pos++] : Token.EndOfLineToken;

    /// <summary>
    /// Throws InvalidOperationException if the current token is not of type <paramref name="tokenKind"/>. Advances to the next token on success.
    /// </summary>
    /// <param name="tokenKind"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void Expect(TokenKind tokenKind)
    {
        if (!IsAt(tokenKind))
            throw new InvalidOperationException($"Expected token: {tokenKind}. Got: {At()}");

        Next();
    }
}
