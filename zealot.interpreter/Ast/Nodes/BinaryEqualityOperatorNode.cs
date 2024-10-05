using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;
using Zealot.Interpreter.Tokens;

namespace Zealot.Interpreter.Ast.Nodes;
internal class BinaryEqualityOperatorNode(AbstractNode left, AbstractNode right, TokenKind operatorKind) : AbstractNode
{
    public AbstractNode Left { get; set; } = left;
    public AbstractNode Right { get; set; } = right;
    public TokenKind OperatorKind { get; set; } = operatorKind;

    private const string unknownOperatorErrorMessage = "Unknown equality operator detected.";

    public override TypeInfo Evaluate(Scope scope)
    {
        var leftResult = Left.Evaluate(scope);
        var rightResult = Right.Evaluate(scope);

        bool result;

        if (OperatorKind == TokenKind.EqualOperator)
            result = leftResult.Value.Equals(rightResult.Value);
        else if (OperatorKind == TokenKind.NotEqualOperator)
            result = !leftResult.Value.Equals(rightResult.Value);
        else
            throw new InvalidOperationException(unknownOperatorErrorMessage);

        return new("bool", new BoolType(result));
    }
}
