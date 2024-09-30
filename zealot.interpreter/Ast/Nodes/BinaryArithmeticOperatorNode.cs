using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;
using Zealot.Interpreter.Tokens;

namespace Zealot.Interpreter.Ast.Nodes;
internal class BinaryArithmeticOperatorNode(AbstractNode left, AbstractNode right, TokenKind operatorKind) : AbstractNode
{
    public AbstractNode Left { get; set; } = left;
    public AbstractNode Right { get; set; } = right;
    public TokenKind OperatorKind { get; set; } = operatorKind;

    public override TypeInfo Evaluate(Scope scope)
    {
        var leftVal = Left.Evaluate(scope);
        var rightVal = Right.Evaluate(scope);

        if (leftVal.Name != "number" || rightVal.Name != "number")
            throw new InvalidOperationException("Cannot perform binary operation on non-number data type.");

        var leftNum = (double)leftVal.Value;
        var rightNum = (double)rightVal.Value;

        var result = OperatorKind switch
        {
            TokenKind.AdditionOperator => leftNum + rightNum,
            TokenKind.SubtractionOperator => leftNum - rightNum,
            TokenKind.MultiplicationOperator => leftNum * rightNum,
            TokenKind.DivisionOperator => leftNum / rightNum,
            TokenKind.ExponentiationOperator => Math.Pow(leftNum, rightNum),
            _ => throw new InvalidOperationException($"Unknown binary operator detected: {OperatorKind}.")
        };

        return new("number", result);
    }
}
