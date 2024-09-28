using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class FunctionDefinitionNode(string identifier, IList<string> parameterIdentifiers, ScopedBlockNode body) : AbstractNode
{
    public string Identifier { get; set; } = identifier;
    public IList<string> ParameterIdentifiers { get; set; } = parameterIdentifiers;
    public ScopedBlockNode Body { get; set; } = body;

    public override TypeInfo Evaluate(Scope scope)
    {
        if (ParameterIdentifiers.Distinct().Count() != ParameterIdentifiers.Count)
            throw new InvalidOperationException("Cannot declare a function with duplicate parameters.");

        scope.DefineFunction(this);
        return new("empty", new());
    }
}
