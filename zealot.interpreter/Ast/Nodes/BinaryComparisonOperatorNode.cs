using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;
using Zealot.Interpreter.Tokens;

namespace Zealot.Interpreter.Ast.Nodes;
internal class BinaryComparisonOperatorNode(AbstractNode left, AbstractNode right, TokenKind operatorKind) : AbstractNode
{
    public AbstractNode Left { get; set; } = left;
    public AbstractNode Right { get; set; } = right;
    public TokenKind OperatorKind { get; set; } = operatorKind;

    private const string notComparableErrorMessage = "The operands of a comparison operator are not of a type that can be compared.";
    private const string differentTypesErrorMessage = "The operands of a comparison operator are not of the same type.";
    private const string unknownOperatorErrorMessage = "Unknown comparison operator detected.";

    public override TypeInfo Evaluate(Scope scope)
    {
        var leftResult = Left.Evaluate(scope);
        if (leftResult.Value is not IComparable leftComparable)
            throw new InvalidOperationException(notComparableErrorMessage);

        var rightResult = Right.Evaluate(scope);
        if (rightResult.Value is not IComparable rightComparable)
            throw new InvalidOperationException(notComparableErrorMessage);

        if (leftComparable.GetType() != rightComparable.GetType())
            throw new InvalidOperationException(differentTypesErrorMessage);

        bool result = OperatorKind switch
        {
            TokenKind.LessThanOperator => leftComparable.CompareTo(rightComparable) < 0,
            TokenKind.LessThanOrEqualToOperator => leftComparable.CompareTo(rightComparable) <= 0,
            TokenKind.GreaterThanOperator => leftComparable.CompareTo(rightComparable) > 0,
            TokenKind.GreaterThanOrEqualToOperator => leftComparable.CompareTo(rightComparable) >= 0,
            _ => throw new InvalidOperationException(unknownOperatorErrorMessage)
        };

        return new("bool", new BoolType(result));
    }
}
