using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class UnaryMinusNode(AbstractNode right) : AbstractNode
{
    public AbstractNode Right { get; set; } = right;

    public override TypeInfo Evaluate(Scope scope)
    {
        var rightVal = Right.Evaluate(scope);

        if (rightVal.Name != "number")
            throw new InvalidOperationException($"Cannot perform unary minus on nun-number data type. Got: {rightVal.Name}");

        return new("number", -(double)rightVal.Value);
    }
}
