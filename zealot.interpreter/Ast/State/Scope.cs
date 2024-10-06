using System.Diagnostics.CodeAnalysis;
using Zealot.Interpreter.Ast.Nodes;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.State;
internal class Scope(ScopeType kind)
{
    public Scope? ParentScope { get; set; }

    public ScopeType Kind { get; set; } = kind;

    private readonly Dictionary<string, TypeInfo> _variables = [];

    private readonly Dictionary<string, FunctionDefinitionNode> _functions = [];

    public Scope(ScopeType kind, Scope parentScope) : this(kind)
    {
        ParentScope = parentScope;
    }

    public static Scope CreateGlobal()
    {
        var scope = new Scope(ScopeType.Global);
        foreach (var funcInfo in BuiltinFunctions.Functions)
        {
            var functionNode = new ExternalFunctionNode(funcInfo.function);
            var definition = new FunctionDefinitionNode(funcInfo.name, funcInfo.parameters, functionNode);
            scope.DefineFunction(definition);
        }

        return scope;
    }

    public void DefineVariable(string identifier, TypeInfo value, bool replaceExisting = false)
    {
        if (HasFunctionScopedVariable(identifier, out var containingScope) && (containingScope != this || !replaceExisting))
            throw new InvalidOperationException($"Cannot define a variable {identifier}: it is already defined.");

        _variables[identifier] = value;
    }

    public void RedefineClosestVariable(string identifier, TypeInfo value)
    {
        var scope = this;
        while (scope is not null && !scope.HasOwnVariable(identifier))
            scope = scope.ParentScope;

        if (scope is null)
            throw new InvalidOperationException($"Cannot redefine a variable {identifier}: it does not exist.");

        scope.DefineVariable(identifier, value, true);
    }

    public IList<TypeInfo> GetAllOwnValues()
    {
        return _variables.Select(f => f.Value).ToList();
    }

    /// <summary>
    /// Returns whether the scope has a variable named <paramref name="identifier"/> defined in it, excluding parent scopes.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool HasOwnVariable(string identifier) => _variables.ContainsKey(identifier);

    /// <summary>
    /// Returns whether the scope or any of its parent scopes has a variable named <paramref name="identifier"/> defined in it.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool HasVariable(string identifier) => HasOwnVariable(identifier) || ParentScope?.HasVariable(identifier) == true;

    /// <summary>
    /// Returns whether the scope or any of its parent scopes up to a scope of function or global level has a variable named <paramref name="identifier"/> defined in it.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool HasFunctionScopedVariable(string identifier, out Scope containingScope)
    {
        var scope = this;
        while (scope.Kind != ScopeType.Global && scope.Kind != ScopeType.Function)
        {
            if (scope.HasOwnVariable(identifier))
            {
                containingScope = scope;
                return true;
            }

            scope = scope.ParentScope!;
        }

        containingScope = scope;
        return scope.HasOwnVariable(identifier);
    }

    /// <summary>
    /// Gets the value of a variable named <paramref name="identifier"/> defined in the scope, excluding parent scopes.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetOwnVariable(string identifier, [MaybeNullWhen(false)] out TypeInfo value) => _variables.TryGetValue(identifier, out value);

    /// <summary>
    /// Gets the value of a variable named <paramref name="identifier"/> in the scope or any of its parent scopes.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetVariable(string identifier, [MaybeNullWhen(false)] out TypeInfo value) => TryGetOwnVariable(identifier, out value) || ParentScope?.TryGetVariable(identifier, out value) == true;

    /// <summary>
    /// Returns whether the scope has a function named <paramref name="identifier"/> defined in it, excluding parent scopes.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool HasOwnFunction(string identifier) => _functions.ContainsKey(identifier);

    /// <summary>
    /// Returns whether the scope or any of its parent scopes has a function named <paramref name="identifier"/> defined in it.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool HasFunction(string identifier) => HasOwnFunction(identifier) || ParentScope?.HasFunction(identifier) == true;

    /// <summary>
    /// Gets the value of a function named <paramref name="identifier"/> defined in the scope, excluding parent scopes.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="function"></param>
    /// <returns></returns>
    public bool TryGetOwnFunction(string identifier, [MaybeNullWhen(false)] out FunctionDefinitionNode function)
        => _functions.TryGetValue(identifier, out function);

    /// <summary>
    /// Gets the value of a variable named <paramref name="identifier"/> in the scope or any of its parent scopes.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="function"></param>
    /// <returns></returns>
    public bool TryGetFunction(string identifier, [MaybeNullWhen(false)] out FunctionDefinitionNode function)
        => TryGetOwnFunction(identifier, out function) || ParentScope?.TryGetFunction(identifier, out function) == true;

    public void DefineFunction(FunctionDefinitionNode function)
    {
        if (HasFunction(function.Identifier))
            throw new InvalidOperationException($"Cannot define function '{function.Identifier}': it is already defined.");

        _functions[function.Identifier] = function;
    }
}
