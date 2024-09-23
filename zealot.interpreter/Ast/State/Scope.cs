using System.Diagnostics.CodeAnalysis;
using Zealot.Interpreter.Ast.Types;

namespace Zealot.Interpreter.Ast.State;
internal class Scope
{
    public Scope? ParentScope { get; set; }

    private readonly Dictionary<string, TypeInfo> _variables = [];

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
}
