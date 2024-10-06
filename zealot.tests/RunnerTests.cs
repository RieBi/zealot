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
            // Multiplication, division, modulo
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
            {
                ["5 % 1"],
                ["0"]
            },
            {
                ["5 % 2"],
                ["1"]
            },
            {
                ["5 % 3"],
                ["2"]
            },
            {
                ["5 % 4"],
                ["1"]
            },
            {
                ["5 % 5"],
                ["0"]
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
                def num = 1
                define echo(x) =>
                    x

                echo(num)
                """,
                ["1", "1"]
            },
            {
                """
                def num = 1
                define increase =>
                    num = num + 1

                increase()
                num
                """,
                ["1", "2", "2"]
            },
            {
                """
                def num = 1
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
            },
            {
                """
                def a = 5
                a %= 2
                def b = 5
                b %= 3
                """,
                ["5", "1", "5", "2"]
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

    public static TheoryData<string, List<string>> GetRelationalOperatorsData()
    {
        return new()
        {
            {
                """
                1 < 2
                2 < 1
                2 > 1
                1 > 2
                1 <= 1
                2 <= 1
                2 >= 2
                1 >= 2
                """,
                ["true", "false", "true", "false", "true", "false", "true", "false"]
            },
            {
                """
                1 == 1
                2 == 1
                1 != 2
                1 != 1
                true == true
                true == false
                true != false
                true != true
                """,
                ["true", "false", "true", "false", "true", "false", "true", "false"]
            },
            {
                """
                2 > 1 == 5 > 4
                1 > 1 != 1 >= 1
                true && false == true || false
                """,
                ["true", "true", "false"]
            }
        };
    }

    public static TheoryData<string, List<string>> GetBuiltinFunctionsData()
    {
        return new()
        {
            {
                """
                define writeThings =>
                    print(1)
                    print(2)
                    print(3)

                writeThings()
                """,
                ["123"]
            },
            {
                """
                define writeNewLines =>
                    printn(1)
                    printn(2)
                    printn(3)

                writeNewLines()
                """,
                ["1", "2", "3"]
            },
            {
                """
                define writeExpressions =>
                    printn(true)
                    printn(false)
                    printn(true || false)
                    printn(2 + 2 * 2)

                writeExpressions()
                """,
                ["true", "false", "true", "6"]
            }
        };
    }

    public static TheoryData<string, List<string>> GetRepeatLoopData()
    {
        return new()
        {
            // Basic ones
            {
                """
                repeat(5) =>
                    printn(5)
                """,
                ["5", "5", "5", "5", "5"]
            },
            {
                """
                define repeatFunc(outer, inner) =>
                    repeat(outer) =>
                        repeat(inner) =>
                            printn(5)

                repeatFunc(2, 3)
                """,
                ["5", "5", "5", "5", "5", "5"]
            },
            {
                """
                def num = 3
                repeat (num >= 0) =>
                    printn(num)
                    num -= 1
                """,
                ["3", "3", "2", "1", "0"]
            },
            {
                """
                repeat (def num = 1, def num2 = 2 ? num < 10 && num2 < 10 : num += 1, num2 += 3) =>
                    printn(num)
                """,
                ["1", "2", "3"]
            },
            {
                """
                repeat (def num = 0 ? num < 3) =>
                    printn(num)
                    num += 1
                """,
                ["0", "1", "2"]
            },
            {
                """
                def num = 0
                repeat (num < 10 : num += 1, num += 2, num += 3) =>
                    printn(num)
                """,
                ["0", "0", "6"]
            },
            // With break
            {
                """
                def num = 0
                repeat =>
                    printn(num)
                    if (num >= 2) =>
                        break
                    num += 1
                """,
                ["0", "0", "1", "2"]
            },
            {
                """
                repeat(200) =>
                    break
                    printn(0)
                """,
                []
            },
            {
                """
                define printMoreThan5Twice(num) =>
                    repeat(2) =>
                        if (num <= 5) =>
                            break
                        else =>
                            printn(num)

                repeat(def i = 0 ? i <= 7 : i += 1) =>
                    printMoreThan5Twice(i)
                """,
                ["6", "6", "7", "7"]
            },
            // With continue
            {
                """
                repeat (def i = 0 ? i < 10 : i += 1) =>
                    if (i % 2 == 0) =>
                        continue
                    printn(i)
                """,
                ["1", "3", "5", "7", "9"]
            },
            {
                """
                repeat(20) =>
                    continue
                    printn(111)
                """,
                []
            },
            {
                """
                define f(num) =>
                    repeat(3) =>
                        printn(num)
                        repeat(20) =>
                            continue
                            printn(666)
                repeat(def i = 0 ? i < 2 : i += 1) =>
                    f(i)
                """,
                ["0", "0", "0", "1", "1", "1"]
            }
        };
    }

    public static TheoryData<string, List<string>> GetIfElseData()
    {
        return new()
        {
            {
                """
                if (true) =>
                    printn(1)
                """,
                ["1"]
            },
            {
                """
                if (false) =>
                    printn(1)
                """,
                []
            },
            {
                """
                if (true) =>
                    printn(true)
                else =>
                    printn(false)
                """,
                ["true"]
            },
            {
                """
                if (false) =>
                    printn(true)
                else =>
                    printn(false)
                """,
                ["false"]
            },
            {
                """
                def num = 1
                if (num == 0) =>
                    printn(0)
                elseif (num == 1) =>
                    printn(1)
                else =>
                    printn(2)
                """,
                ["1", "1"]
            },
            {
                """
                def num = 2
                if (num == 0) =>
                    printn(0)
                elseif (num == 1) =>
                    printn(1)
                else =>
                    printn(2)
                """,
                ["2", "2"]
            },
            {
                """
                def num = 0
                if (num == 0) =>
                    printn(0)
                elseif (num == 1) =>
                    printn(1)
                else =>
                    printn(2)
                """,
                ["0", "0"]
            },
            {
                """
                def num = 3
                if (num == 1) =>
                    printn(11)
                elseif (num == 2) =>
                    printn(22)
                elseif (num == 3) =>
                    printn(33)
                """,
                ["3", "33"]
            }
        };
    }

    public static TheoryData<string, List<string>> GetStringsData()
    {
        return new()
        {
            {
                """
                printn("Hello, world!")
                """,
                ["Hello, world!"]
            },
            {
                """
                define printStuff =>
                    repeat(3) =>
                        print("a")
                    printn("a")
                    
                    repeat(3) =>
                        print("b")
                    printn("b")
                    
                printStuff()
                """,
                ["aaaa", "bbbb"]
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
    [MemberData(nameof(GetRelationalOperatorsData))]
    [MemberData(nameof(GetBuiltinFunctionsData))]
    [MemberData(nameof(GetRepeatLoopData))]
    [MemberData(nameof(GetIfElseData))]
    [MemberData(nameof(GetStringsData))]
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
