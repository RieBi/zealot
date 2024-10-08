﻿namespace Zealot.Interpreter;
public interface IRunner
{
    /// <summary>
    /// Ask the runner to get the next line of code, without which the interpreter cannot continue execution.
    /// </summary>
    /// <returns></returns>
    string GetNextLine();

    string ReturnPreviousLine();

    void Run();
}
