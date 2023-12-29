using System;
namespace FifthHW;
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