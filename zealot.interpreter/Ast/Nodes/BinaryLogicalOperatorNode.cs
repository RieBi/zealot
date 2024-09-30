using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;
using Zealot.Interpreter.Tokens;

namespace Zealot.Interpreter.Ast.Nodes;
internal class BinaryLogicalOperatorNode(AbstractNode left, AbstractNode right, TokenKind operatorKind) : AbstractNode
{
    public AbstractNode Left { get; set; } = left;
    public AbstractNode Right { get; set; } = right;
    public TokenKind OperatorKind { get; set; } = operatorKind;

    private const string errorTypeNotBinary = "Cannot perform binary logical operation on non-bool data type.";
    private const string errorUnknownOperator = "Unknown binary operator detected: {0}.";

    public override TypeInfo Evaluate(Scope scope)
    {
        var leftBool = getValue(Left);

        if (OperatorKind == TokenKind.LogicalAndOperator)
        {
            if (!leftBool)
                return new("bool", BoolType.False);

            var rightBool = getValue(Right);
            return new("bool", new BoolType(rightBool));
        }
        else if (OperatorKind == TokenKind.LogicalExclusiveOrOperator)
        {
            var rightBool = getValue(Right);
            return new("bool", new BoolType(leftBool ^ rightBool));
        }
        else if (OperatorKind == TokenKind.LogicalOrOperator)
        {
            if (leftBool)
                return new("bool", BoolType.True);

            var rightBool = getValue(Right);
            return new("bool", new BoolType(rightBool));
        }
        else
            throw new InvalidOperationException(string.Format(errorUnknownOperator, OperatorKind));

        bool getValue(AbstractNode node)
        {
            var val = node.Evaluate(scope);
            if (val.Name != "bool")
                throw new InvalidOperationException(errorTypeNotBinary);

            return ((BoolType)val.Value).Value;
        }
    }
}
