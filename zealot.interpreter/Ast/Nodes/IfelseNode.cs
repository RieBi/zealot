using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class IfelseNode((AbstractNode condition, ScopedBlockNode body) ifBlock, IList<(AbstractNode condition, ScopedBlockNode body)> elseifBlock, AbstractNode? elseBody) : AbstractNode
{
    public (AbstractNode condition, ScopedBlockNode body) IfBlock { get; set; } = ifBlock;
    public IList<(AbstractNode condition, ScopedBlockNode body)> ElseifBlock { get; set; } = elseifBlock;
    public AbstractNode? ElseBody { get; set; } = elseBody;

    private const string notBoolConditionErrorMessage = "The type of condition in if-else statement is not bool.";

    public override TypeInfo Evaluate(Scope scope)
    {
        var ifConditionResult = IfBlock.condition.Evaluate(scope);
        if (ifConditionResult.Name != "bool")
            throw new InvalidOperationException(notBoolConditionErrorMessage);

        if (((BoolType)ifConditionResult.Value).Value)
        {
            var ifScope = new Scope(ScopeType.Ifelse, scope);
            IfBlock.body.Evaluate(ifScope);
            return new("empty", new());
        }
        else
        {
            for (int i = 0; i < ElseifBlock.Count; i++)
            {
                var (condition, body) = ElseifBlock[i];
                var elseifConditionResult = condition.Evaluate(scope);
                if (elseifConditionResult.Name != "bool")
                    throw new InvalidOperationException(notBoolConditionErrorMessage);

                if (((BoolType)elseifConditionResult.Value).Value)
                {
                    var elseIfScope = new Scope(ScopeType.Ifelse, scope);
                    body.Evaluate(elseIfScope);
                    return new("empty", new());
                }
            }

            if (ElseBody is not null)
            {
                var elseScope = new Scope(ScopeType.Ifelse, scope);
                ElseBody.Evaluate(elseScope);
            }

            return new("empty", new());
        }
    }
}
