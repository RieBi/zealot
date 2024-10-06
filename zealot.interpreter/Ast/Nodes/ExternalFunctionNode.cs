using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class ExternalFunctionNode(Func<IList<TypeInfo>, TypeInfo> function) : AbstractNode
{
    public Func<IList<TypeInfo>, TypeInfo> Function { get; set; } = function;

    public override TypeInfo Evaluate(Scope scope)
    {
        var functionArguments = scope.GetAllOwnValues();
        var result = Function(functionArguments);

        return result;
    }
}
