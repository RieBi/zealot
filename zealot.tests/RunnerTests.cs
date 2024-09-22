using System.Text;

namespace Zealot.Tests;
public class RunnerTests
{
    public static TheoryData<List<string>, List<string>> GetBasicOperationsData()
    {
        return new()
        {
            {
                ["1"],
                ["1"]
            },
            {
                ["582123"],
                ["582123"]
            },
            {
                ["123.456"],
                ["123.456"]
            },
            {
                ["1.234e3"],
                ["1234"]
            },
            {
                ["1000e-3"],
                ["1"]
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetBasicOperationsData))]
    public void TestRunner(List<string> inputLines, List<string> outputs)
    {
        var str = new StringBuilder();
        for (int i = 0; i < inputLines.Count; i++)
        {
            str.Append(inputLines[i]);
            str.Append(Environment.NewLine);
        }

        var reader = new StringReader(str.ToString());
        Console.SetIn(reader);

        var writer = new StringWriter();
        Console.SetOut(writer);

        Runner.Program.Main(["-i"]);

        var outputLines = writer.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(outputs, outputLines);
    }
}
