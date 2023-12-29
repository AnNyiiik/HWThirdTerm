using System;
namespace FifthHW;

/// <summary>
/// Attribute for test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class MyTestAttribute : Attribute
{
    private readonly string? reasonForIgnore;
    private readonly Type? typeOfExpectedException;

    public MyTestAttribute(string? reasonForIgnore = null,
        Type? typeOfExpectedException = null)
    {
        this.reasonForIgnore = reasonForIgnore;
        this.typeOfExpectedException = typeOfExpectedException;
        ReasonForIgnore = reasonForIgnore;
        TypeOfExpectedException = typeOfExpectedException;
    }

    public string? ReasonForIgnore { get; }

    public Type? TypeOfExpectedException { get; }
}

/// <summary>
/// Attribute for BeforeClass method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClassAttribute : Attribute
{
    public BeforeClassAttribute()
    {

    }
}

/// <summary>
/// Attribute for AfterClass method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterClassAttribute : Attribute
{
    public AfterClassAttribute()
    {

    }
}

/// <summary>
/// Attribute for Before method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeAttribute : Attribute
{
    public BeforeAttribute()
    {

    }
}

/// <summary>
/// Attribute for After method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterAttribute : Attribute
{
    public AfterAttribute()
    {

    }
}