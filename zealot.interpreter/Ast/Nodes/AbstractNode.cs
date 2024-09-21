using zealot.interpreter.Ast.State;
using zealot.interpreter.Ast.Types;

namespace zealot.interpreter.Ast.Nodes;
internal abstract class AbstractNode
{
    public abstract TypeInfo Evaluate(Scope scope);
}
