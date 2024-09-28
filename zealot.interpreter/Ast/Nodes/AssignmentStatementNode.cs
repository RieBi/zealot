using Zealot.Interpreter.Ast.State;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.Nodes;
internal class AssignmentStatementNode(IdentifierNode variableName, AbstractNode rightSide) : AbstractNode
{
    public IdentifierNode VariableName { get; set; } = variableName;
    public AbstractNode RightSide { get; set; } = rightSide;


    public override TypeInfo Evaluate(Scope scope)
    {
        if (!scope.HasVariable(VariableName.Identifier))
            throw new InvalidOperationException($"Cannot reassign a nonexisting variable {VariableName.Identifier}.");

        var rightSideValue = RightSide.Evaluate(scope);
        scope.RedefineClosestVariable(VariableName.Identifier, rightSideValue);

        return rightSideValue;
    }
}
