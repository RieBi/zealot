using System.Text;

namespace Zealot.Tests;
public class RunnerTests
{
    public static TheoryData<List<string>, List<string>> GetBasicOperationsData()
    {
        return new()
        {
            // Constant numbers
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
            },
            // Addition, subtraction
            {
                ["2 + 5"],
                ["7"]
            },
            {
                ["2 + 4 + 10 - 2 - 1"],
                ["13"]
            },
            {
                ["0 - 10"],
                ["-10"]
            },
            {
                ["2.5+2.5"],
                ["5"]
            },
            // Unary minus
            {
                ["-5 + 5"],
                ["0"]
            },
            {
                ["-2 - -2"],
                ["0"]
            },
            // Multiplication, division
            {
                ["2 + 2 * 2"],
                ["6"]
            },
            {
                ["2 * 10 / 4 * 3"],
                ["15"]
            },
            {
                ["1/2"],
                ["0.5"]
            },
            // Exponentiation
            {
                ["2$3"],
                ["8"]
            },
            {
                ["0$0"],
                ["1"]
            },
            {
                ["2 $ 2 $ 3"],
                ["256"]
            },
            // Parentheses
            {
                ["((2))+(((1)))"],
                ["3"]
            },
            {
                ["(2 + 2) * 2"],
                ["8"]
            },
            {
                ["(2 + 2 * (1 + 2) - ((1) - 0))"],
                ["7"]
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
