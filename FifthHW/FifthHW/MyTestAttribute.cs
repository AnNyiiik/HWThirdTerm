using System;
namespace FifthHW;

/// <summary>
/// Attribute for test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class MyTestAttribute : Attribute
{

    public MyTestAttribute(string? reasonForIgnore = null,
        Type? typeOfExpectedException = null)
    {
        ReasonForIgnore = reasonForIgnore;
        TypeOfExpectedException = typeOfExpectedException;
    }

    public string? ReasonForIgnore { get; private set; }

    public Type? TypeOfExpectedException { get; private set; }
}
