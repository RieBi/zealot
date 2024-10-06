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
        {
            var result = ParseRepeatLoop();
            if (_pos < _tokens.Count)
                throw new InvalidOperationException("Unused tokens detected at the end of line.");

            return result;
        }
    }

    public AbstractNode ParseBasicExpression()
    {
        if (IsAt(TokenKind.ConstantNumberInteger) || IsAt(TokenKind.ConstantNumberDouble))
        {
            var token = Next();
            var info = new TypeInfo("number", double.Parse(token.Value));
            return new ValueNode(info);
        }
        else if (IsAt(TokenKind.ConstantFalse) || IsAt(TokenKind.ConstantTrue))
        {
            var token = Next();
            var info = new TypeInfo("bool", token.Value == "true" ? BoolType.True : BoolType.False);
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
                var argument = ParseExpression();
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
            var expression = ParseExpression();

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
            left = new BinaryArithmeticOperatorNode(left, right, token.Kind);
        }

        return left;
    }

    public AbstractNode ParseUnaryMinus()
    {
        if (IsAt(TokenKind.SubtractionOperator))
        {
            Next();

            var right = ParseParentheses();
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
            left = new BinaryArithmeticOperatorNode(left, right, token.Kind);
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
            left = new BinaryArithmeticOperatorNode(left, right, token.Kind);
        }

        return left;
    }

    public AbstractNode ParseLogicalNotOperator()
    {
        if (IsAt(TokenKind.LogicalNotOperator))
        {
            Next();

            var right = ParseParentheses();
            return new UnaryLogicalNotNode(right);
        }

        return ParseAdditionOperator();
    }

    public AbstractNode ParseComparisonOperator()
    {
        var left = ParseLogicalNotOperator();

        var at = At();
        if (at.Kind >= TokenKind.LessThanOperator && at.Kind <= TokenKind.GreaterThanOrEqualToOperator)
        {
            Next();
            var right = ParseLogicalNotOperator();

            var node = new BinaryComparisonOperatorNode(left, right, at.Kind);
            return node;
        }

        return left;
    }

    public AbstractNode ParseEqualityOperator()
    {
        var left = ParseComparisonOperator();

        while (IsAt(TokenKind.EqualOperator) || IsAt(TokenKind.NotEqualOperator))
        {
            var token = Next();
            var right = ParseComparisonOperator();

            left = new BinaryEqualityOperatorNode(left, right, token.Kind);
        }

        return left;
    }

    public AbstractNode ParseLogicalOperator()
    {
        var left = ParseEqualityOperator();

        while (IsAt(TokenKind.LogicalAndOperator) || IsAt(TokenKind.LogicalExclusiveOrOperator) || IsAt(TokenKind.LogicalOrOperator))
        {
            var token = Next();

            var right = ParseEqualityOperator();
            left = new BinaryLogicalOperatorNode(left, right, token.Kind);
        }

        return left;
    }

    public AbstractNode ParseExpression() => ParseVariableAssignment();

    public AbstractNode ParseShorthandOperators()
    {
        var left = ParseLogicalOperator();

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

            var operatorNode = new BinaryArithmeticOperatorNode(left, ParseExpression(), binaryOperator.Value);
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
            var right = ParseExpression();

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

            var right = ParseExpression();
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

            Expect(TokenKind.BlockDefinitionOperator);
            var block = ParseScopedBlock(string.Empty);
            return new FunctionDefinitionNode(identifier.Value, parameters, block);
        }

        return ParseVariableAssignment();
    }

    public AbstractNode ParseRepeatLoop()
    {
        if (IsAt(TokenKind.RepeatDefinition))
        {
            Next();
            Expect(TokenKind.ParentheseOpen);

            if (IsAt(TokenKind.ParentheseClosed))
            {
                Next();
                return new RepeatNode(ParseScopedBlock(string.Empty));
            }
            else
            {
                IList<AbstractNode> pre = [];
                AbstractNode condition;
                IList<AbstractNode> post = [];

                var first = ParseExpression();
                if (IsAt(TokenKind.QuestionMark))
                {
                    pre.Add(first);
                    Next();
                    condition = ParseExpression();
                }
                else if (IsAt(TokenKind.CommaSeparator))
                {
                    pre.Add(first);
                    while (IsAt(TokenKind.CommaSeparator))
                    {
                        Next();
                        pre.Add(ParseExpression());
                    }

                    Expect(TokenKind.QuestionMark);
                    condition = ParseExpression();
                }
                else if (IsAt(TokenKind.Colon))
                {
                    condition = first;
                }
                else if (IsAt(TokenKind.ParentheseClosed))
                {
                    return new RepeatNode(pre, first, [], ParseScopedBlock(string.Empty));
                }
                else
                    throw new InvalidOperationException("Invalid token in loop definition detected.");

                if (IsAt(TokenKind.Colon))
                {
                    Next();
                    post.Add(ParseExpression());
                    
                    while (IsAt(TokenKind.CommaSeparator))
                    {
                        Next();
                        post.Add(ParseExpression());
                    }
                }

                Expect(TokenKind.ParentheseClosed);
                Expect(TokenKind.BlockDefinitionOperator);
                return new RepeatNode(pre, condition, post, ParseScopedBlock(string.Empty));
            }
        }

        return ParseFunctionDefinition();
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
                _pos = int.MaxValue;
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
