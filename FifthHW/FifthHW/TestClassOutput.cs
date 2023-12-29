using System;
namespace FifthHW;

/// <summary>
/// Class for all test methods execution data in class.
/// </summary>
public class TestClassOutput
{
    public TestClassOutput(string className,
                           List<string> incorrectTestElementsNames,
                           List<string> incorrectTestNames,
                           List<TestOutput> testResults)
    {
        ClassName = className;
        IncorrectTestElementsNames = incorrectTestElementsNames;
        IncorrectTestNames = incorrectTestNames;
        TestResults = testResults;
    }

    public string ClassName
    {
        get;
    }

    public List<string> IncorrectTestNames { get; }

    public List<string> IncorrectTestElementsNames { get; }

    public List<TestOutput> TestResults { get; }

}