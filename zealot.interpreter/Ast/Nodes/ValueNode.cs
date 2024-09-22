using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class ValueNode(TypeInfo typeInfo) : AbstractNode
{
    private readonly TypeInfo _value = typeInfo;

    public override TypeInfo Evaluate(Scope scope) => _value;
}
