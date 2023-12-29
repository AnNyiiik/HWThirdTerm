using System;
namespace FifthHW;
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