using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class VariableDefinitionNode(IdentifierNode variableName, AbstractNode rightSide) : AbstractNode
{
    public IdentifierNode VariableName { get; set; } = variableName;
    public AbstractNode RightSide { get; set; } = rightSide;


    public override TypeInfo Evaluate(Scope scope)
    {
        var rightSideValue = RightSide.Evaluate(scope);
        scope.DefineVariable(VariableName.Identifier, rightSideValue);

        return rightSideValue;
    }
}
