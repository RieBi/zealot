using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class FunctionEvaluationNode(string identifier, List<AbstractNode> arguments) : AbstractNode
{
    public string Identifier { get; set; } = identifier;
    public List<AbstractNode> Arguments { get; set; } = arguments;

    public override TypeInfo Evaluate(Scope scope)
    {
        if (scope.TryGetFunction(Identifier, out var function))
        {
            if (function.ParameterIdentifiers.Count != Arguments.Count)
                throw new InvalidOperationException($"Invalid amount of arguments. Expected: {function.ParameterIdentifiers.Count}. Got: {Arguments.Count}.");

            var functionScope = new Scope(ScopeType.Function, scope);
            for (int i = 0; i < Arguments.Count; i++)
            {
                var parameterName = function.ParameterIdentifiers[i];
                var argumentValue = Arguments[i].Evaluate(scope);

                functionScope.DefineVariable(parameterName, argumentValue, true);
            }

            return function.Body.Evaluate(functionScope);
        }
        else
            throw new InvalidOperationException($"Unknown function: {Identifier}.");
    }
}
