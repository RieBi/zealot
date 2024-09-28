using Zealot.Interpreter.Ast.Nodes;
using Zealot.Interpreter.Ast.Types;
using Zealot.Interpreter.Tokens;

namespace Zealot.Interpreter;
internal class Parser(List<Token> tokens, IRunner runner)
{
    private List<Token> _tokens = tokens;
    private int _pos = 0;
    private readonly IRunner _runner = runner;

    public AbstractNode ParseLine()
    {
        if (_tokens.Count == 0 || _tokens.Count == 1 && _tokens[0].Kind == TokenKind.Indentation)
            return new EmptyLineNode();
        else
            return ParseFunctionDefinition();
    }

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

    public AbstractNode ParseFunctionCall()
    {
        var node = ParseBasicExpression();

        if (node is IdentifierNode identifier && IsAt(TokenKind.ParentheseOpen))
        {
            Next();

            List<AbstractNode> arguments = [];

            while (!IsAt(TokenKind.ParentheseClosed))
            {
                var argument = ParseAdditionOperator();
                arguments.Add(argument);

                if (!IsAt(TokenKind.ParentheseClosed))
                    Expect(TokenKind.CommaSeparator);
            }

            Expect(TokenKind.ParentheseClosed);

            return new FunctionEvaluationNode(identifier.Identifier, arguments);
        }

        return node;
    }

    public AbstractNode ParseParentheses()
    {
        if (IsAt(TokenKind.ParentheseOpen))
        {
            Next();
            var expression = ParseLine();

            Expect(TokenKind.ParentheseClosed);
            return expression;
        }

        return ParseFunctionCall();
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

    public AbstractNode ParseShorthandOperators()
    {
        var left = ParseAdditionOperator();

        if (left is IdentifierNode identifier)
        {
            var at = At().Kind;

            TokenKind? binaryOperator = at switch
            {
                TokenKind.ShortAdditionOperator => TokenKind.AdditionOperator,
                TokenKind.ShortSubtractionOperator => TokenKind.SubtractionOperator,
                TokenKind.ShortMultiplicationOperator => TokenKind.MultiplicationOperator,
                TokenKind.ShortDivisionOperator => TokenKind.DivisionOperator,
                TokenKind.ShortExponentiationOperator => TokenKind.ExponentiationOperator,
                _ => null
            };

            if (binaryOperator is null)
                return left;

            Next();

            var operatorNode = new BinaryOperatorNode(left, ParseAdditionOperator(), binaryOperator.Value);
            return new AssignmentStatementNode(identifier, operatorNode);
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

        return ParseShorthandOperators();
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

    public AbstractNode ParseFunctionDefinition()
    {
        if (IsAt(TokenKind.FunctionDefinition))
        {
            Next();

            var identifier = Expect(TokenKind.Identifier);
            var parameters = new List<string>();

            if (IsAt(TokenKind.ParentheseOpen))
            {
                Next();

                while (IsAt(TokenKind.Identifier))
                {
                    parameters.Add(Next().Value);

                    if (!IsAt(TokenKind.ParentheseClosed))
                        Expect(TokenKind.CommaSeparator);
                }

                Expect(TokenKind.ParentheseClosed);
            }

            var block = ParseScopedBlock(string.Empty);
            return new FunctionDefinitionNode(identifier.Value, parameters, block);
        }

        return ParseVariableAssignment();
    }

    public ScopedBlockNode ParseScopedBlock(string indentation)
    {
        var line = _runner.GetNextLine();
        Reset(line);

        var curIndent = Expect(TokenKind.Indentation).Value;
        if (curIndent == indentation)
            throw new InvalidOperationException("A block cannot start with the same indent level as its parent block.");

        List<AbstractNode> blockLines = [ParseLine()];

        while (true)
        {
            line = _runner.GetNextLine();
                Reset(line);

            if (!IsAt(TokenKind.Indentation) || At().Value != curIndent)
            {
                _runner.ReturnPreviousLine();
                break;
            }

            Next();
            blockLines.Add(ParseLine());
        }

        return new(blockLines);
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
    /// Throws InvalidOperationException if the current token is not of type <paramref name="tokenKind"/>. Returns the token and advances to the next one on success.
    /// </summary>
    /// <param name="tokenKind"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private Token Expect(TokenKind tokenKind)
    {
        if (!IsAt(tokenKind))
            throw new InvalidOperationException($"Expected token: {tokenKind}. Got: {At()}");

        return Next();
    }

    private void Reset(string newLine)
    {
        _tokens = Tokenizer.Tokenize(newLine);
        _pos = 0;
    }
}
