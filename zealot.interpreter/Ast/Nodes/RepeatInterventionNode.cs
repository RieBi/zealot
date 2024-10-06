using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class RepeatInterventionNode(ScopeFlag scopeFlag) : AbstractNode
{
    public ScopeFlag ScopeFlag { get; set; } = scopeFlag;

    public override TypeInfo Evaluate(Scope scope)
    {
        while (scope.Kind != ScopeType.Loop)
        {
            scope.Flags ^= ScopeFlag;
            scope = scope.ParentScope ?? throw new InvalidOperationException("Cannot apply loop intervention statement out of loop");
        }

        scope.Flags ^= ScopeFlag;
        return new("empty", new());
    }
}
