using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class ScopedBlockNode(IList<AbstractNode> statements) : AbstractNode
{
    public IList<AbstractNode> Statements { get; set; } = statements;

    public override TypeInfo Evaluate(Scope scope)
    {
        if (Statements.Count == 0)
            throw new InvalidOperationException("Cannot execute a block of 0 statements.");

        for (int i = 0; i < Statements.Count - 1; i++)
            Statements[i].Evaluate(scope);

        if (scope.Kind == ScopeType.Function)
            return Statements[^1].Evaluate(scope);
        else
            Statements[^1].Evaluate(scope);

        return new("empty", new());
    }
}
