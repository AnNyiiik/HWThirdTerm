using System;
namespace FifthHW;
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