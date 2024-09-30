using System.Text;
using Xunit.Sdk;

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

    public static TheoryData<List<string>, List<string>> GetVariablesData()
    {
        return new()
        {
            {
                ["def num = 1"],
                ["1"]
            },
            {
                ["def a = 1", "a + 2"],
                ["1", "3"]
            },
            {
                ["def a = 2", "def b = 3", "a + b + 1"],
                ["2", "3", "6"]
            },
            {
                ["def a = 1", "a = 2", "a + 2", "a = 3", "a + 2"],
                ["1", "2", "4", "3", "5"]
            }
        };
    }

    public static TheoryData<string, List<string>> GetFunctionsData()
    {
        return new()
        {
            {
                """
                define one =>
                    1
                """,
                []
            },
            {
                """
                define culminate =>
                    def a = 2
                    def b = 3
                    a + b
                def r = culminate()
                r
                """,
                ["5", "5"]
            },
            {
                """
                define multiply(a, b) =>
                    a * b

                def result = multiply(7, 7)
                result
                """,
                ["49", "49"]
            },
            {
                """
                def num = 1,
                define echo(x) =>
                    x

                echo(num)
                """,
                ["1", "1"]
            },
            {
                """
                def num = 1,
                define increase =>
                    num = num + 1

                increase()
                num
                """,
                ["1", "2", "2"]
            },
            {
                """
                def num = 1,
                define add3(x) =>
                    def num = 3 + x
                    num

                add3(num)
                num
                """,
                ["1", "4", "1"]
            },
            {
                """
                define rec1 =>
                    def a = 1
                    define rec2 =>
                        def a = 2
                        a
                    
                    a + rec2()

                rec1()
                """,
                ["3"]
            },
            {
                """
                def a = 1
                define func(a) =>
                    a = a * 2
                    a

                func(a)
                a
                """,
                ["1", "2", "1"]
            }
        };
    }

    public static TheoryData<string, List<string>> GetShorthandOperatorsData()
    {
        return new()
        {
            {
                """
                def a = 5
                a += 1
                a -= 1
                a *= 5
                a /= 5
                a $= 3
                """,
                ["5", "6", "5", "25", "5", "125"]
            }
        };
    }

    public static TheoryData<string, List<string>> GetLogicalOperatorsData()
    {
        return new()
        {
            {
                "false",
                ["false"]
            },
            {
                "true",
                ["true"]
            },
            {
                """
                !true
                !false
                """,
                ["false", "true"]
            },
            {
                """
                true && true
                true && false
                false && true
                false && false
                """,
                ["true", "false", "false", "false"]
            },
            {
                """
                true ^ true
                true ^ false
                false ^ true
                false ^ false
                """,
                ["false", "true", "true", "false"]
            },
            {
                """
                true || true
                true || false
                false || true
                false || false
                """,
                ["true", "true", "true", "false"]
            },
            {
                """
                def ran = 0

                define increment =>
                    ran += 1
                    true

                false && increment()
                ran
                true && increment()
                ran
                """,
                ["0", "false", "0", "true", "1"]
            },
            {
                """
                def ran = 0

                define increment =>
                    ran += 1
                    true

                false || increment()
                ran
                true || increment()
                ran
                """,
                ["0", "true", "1", "true", "1"]
            },
            {
                """
                false && false || true && true
                !false && !true || !false && !false
                """,
                ["true", "true"]
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetBasicOperationsData))]
    [MemberData(nameof(GetVariablesData))]
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

    [Theory]
    [MemberData(nameof(GetFunctionsData))]
    [MemberData(nameof(GetShorthandOperatorsData))]
    [MemberData(nameof(GetLogicalOperatorsData))]
    public void BlockTestRunner(string inputLines, List<string> outputs)
    {
        var reader = new StringReader(inputLines);
        Console.SetIn(reader);

        var writer = new StringWriter();
        Console.SetOut(writer);

        Runner.Program.Main(["-i"]);

        var outputLines = writer.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(outputs, outputLines);
    }
}
