using zealot.interpreter.Ast.State;
using zealot.interpreter.Ast.Types;

namespace zealot.interpreter.Ast.Nodes;
internal class ValueNode(TypeInfo typeInfo) : AbstractNode
{
    private readonly TypeInfo _value = typeInfo;

    public override TypeInfo Evaluate(Scope scope) => _value;
}
