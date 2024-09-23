using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class IdentifierNode(string identifier) : AbstractNode
{
    public string Identifier { get; set; } = identifier;

    public override TypeInfo Evaluate(Scope scope)
    {
        if (scope.TryGetVariable(Identifier, out var value))
            return value;
        else
            throw new InvalidOperationException($"Unknown identifier: {Identifier}");
    }
}
