using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class UnaryLogicalNotNode(AbstractNode right) : AbstractNode
{
    public AbstractNode Right { get; set; } = right;

    public override TypeInfo Evaluate(Scope scope)
    {
        var rightVal = Right.Evaluate(scope);

        if (rightVal.Name != "bool")
            throw new InvalidOperationException($"Cannot perform logical and on nun-bool data type. Got: {rightVal.Name}");

        BoolType result = ((BoolType)rightVal.Value).Negate();
        return new("bool", result);
    }
}
