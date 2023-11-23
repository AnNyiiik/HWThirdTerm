using System;
namespace FifthHW;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class MyTestAttribute : Attribute
{
    private readonly string? reasonForIgnore;
    private readonly Type? typeOfExpectedException;

    public MyTestAttribute(string? reasonForIgnore, Type? typeOfExpectedException)
    {
        this.reasonForIgnore = reasonForIgnore;
        this.typeOfExpectedException = typeOfExpectedException;
        ReasonForIgnore = reasonForIgnore;
        TypeOfExpectedException = typeOfExpectedException;
    }

    public string? ReasonForIgnore { get; }

    public Type? TypeOfExpectedException { get; }
}

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClassAttribute : Attribute
{
    public BeforeClassAttribute()
    {

    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterClassAttribute : Attribute
{
    public AfterClassAttribute()
    {

    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeAttribute : Attribute
{
    public BeforeAttribute()
    {

    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterAttribute : Attribute
{
    public AfterAttribute()
    {

    }
}