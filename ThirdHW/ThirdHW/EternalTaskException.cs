namespace ThirdHW;

using System;

/// <summary>
/// Thrown if the task hasn't been completed for some exact time after Shutdown was requested.
/// </summary>
public class EternalTaskException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EternalTaskException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    public EternalTaskException(string message)
        : base(message)
    {
    }
}