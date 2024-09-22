using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal abstract class AbstractNode
{
    public abstract TypeInfo Evaluate(Scope scope);
}
