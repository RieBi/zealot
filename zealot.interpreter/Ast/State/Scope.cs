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

    public void DefineVariable(string identifier, TypeInfo value, bool replaceExisting = false)
    {
        if (!replaceExisting && HasVariable(identifier))
            throw new InvalidOperationException($"Cannot define a variable {identifier}: it is already defined.");

        _variables[identifier] = value;
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

    public bool HasOwnFunction(string identifier) => _functions.ContainsKey(identifier);

    public bool HasFunction(string identifier) => HasOwnFunction(identifier) || ParentScope?.HasFunction(identifier) == true;

    public bool TryGetOwnFunction(string identifier, [MaybeNullWhen(false)] out FunctionDefinitionNode function)
        => _functions.TryGetValue(identifier, out function);

    public bool TryGetFunction(string identifier, [MaybeNullWhen(false)] out FunctionDefinitionNode function)
        => TryGetOwnFunction(identifier, out function) || ParentScope?.TryGetFunction(identifier, out function) == true;

    public void DefineFunction(FunctionDefinitionNode function)
    {
        if (HasFunction(function.Identifier))
            throw new InvalidOperationException($"Cannot define function '{function.Identifier}': it is already defined.");

        _functions[function.Identifier] = function;
    }
}
