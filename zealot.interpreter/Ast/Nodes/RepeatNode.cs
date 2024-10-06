using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class RepeatNode : AbstractNode
{
    public IList<AbstractNode> BeforeStatements { get; set; }
    public AbstractNode ConditionExpression { get; set; }
    public IList<AbstractNode> AfterStatements { get; set; }
    public AbstractNode Body { get; set; }

    private static int counter = 0;

    public RepeatNode(AbstractNode body)
    {
        BeforeStatements = [];
        ConditionExpression = new ValueNode(new("bool", BoolType.True));
        AfterStatements = [];
        Body = body;
    }

    public RepeatNode(int iterationCount, AbstractNode body)
    {
        Interlocked.Increment(ref counter);

        var definitionIdentifier = new IdentifierNode($"~loop{counter}");
        var definitionValue = new ValueNode(new("number", 0));
        var definition = new VariableDefinitionNode(definitionIdentifier, definitionValue);

        var conditionValue = new ValueNode(new("number", iterationCount));
        var condition = new BinaryComparisonOperatorNode(definitionIdentifier, conditionValue, Tokens.TokenKind.LessThanOperator);

        var oneValue = new ValueNode(new("number", 1));
        var incrementValue = new BinaryArithmeticOperatorNode(definitionIdentifier, oneValue, Tokens.TokenKind.AdditionOperator);
        var increment = new AssignmentStatementNode(definitionIdentifier, incrementValue);

        BeforeStatements = [definition];
        ConditionExpression = condition;
        AfterStatements = [increment];
        Body = body;
    }

    public RepeatNode(IList<AbstractNode> beforeStatements, AbstractNode conditionExpression, IList<AbstractNode> afterStatements, AbstractNode body)
    {
        BeforeStatements = beforeStatements;
        ConditionExpression = conditionExpression;
        AfterStatements = afterStatements;
        Body = body;
    }

    public override TypeInfo Evaluate(Scope scope)
    {
        var loopScope = new Scope(ScopeType.Loop, scope);

        for (int i = 0; i < BeforeStatements.Count; i++)
            BeforeStatements[i].Evaluate(loopScope);

        var conditionValue = ConditionExpression.Evaluate(loopScope);
        if (conditionValue.Name == "number")
        {
            var iterationCountDouble = (double)conditionValue.Value;
            if (!double.IsInteger(iterationCountDouble))
                throw new InvalidOperationException("Iteration count in repeat loop is not an integer number.");

            var iterationCountInt = (int)iterationCountDouble;

            for (int i = 0; i < iterationCountInt; i++)
                Body.Evaluate(loopScope);
        }
        else if (conditionValue.Name == "bool")
        {
            var going = ((BoolType)conditionValue.Value).Value;

            while (going)
            {
                Body.Evaluate(loopScope);
                for (int i = 0; i < AfterStatements.Count; i++)
                    AfterStatements[i].Evaluate(loopScope);

                going = ((BoolType)ConditionExpression.Evaluate(loopScope).Value).Value;
            }
        }
        else
            throw new InvalidOperationException("Invalid condition expression result type in repeat loop.");

        return new("empty", new());
    }
}
