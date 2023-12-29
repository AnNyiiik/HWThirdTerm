using System;
using static FifthHW.MyNUnitTest;

namespace FifthHW;

/// <summary>
/// Class for describing test execution data.
/// </summary>
public class TestOutput
{
    public TestOutput(TestStatuses status, string? message, string? name)
    {
        Status = status;
        Message = message;
        Name = name;
    }

    public TestStatuses Status { get; private set; }
    public string? Message { get; private set; }
    public string? Name { get; private set; }
}