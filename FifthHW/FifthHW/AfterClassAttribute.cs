using System;
namespace FifthHW;

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