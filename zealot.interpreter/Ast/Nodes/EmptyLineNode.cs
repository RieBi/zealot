using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class EmptyLineNode : AbstractNode
{
    public override TypeInfo Evaluate(Scope scope) => new("empty", new());
}
