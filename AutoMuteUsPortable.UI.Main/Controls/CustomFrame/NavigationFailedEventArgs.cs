using System;

namespace AutoMuteUsPortable.UI.Main.Controls;

/// <summary>
///     Represents a method that will handle the Frame.NavigationFailed event.
/// </summary>
/// <param name="sender">The object where the handler is attached.</param>
/// <param name="e">Event data for the event.</param>
public delegate void NavigationFailedEventHandler(object sender, NavigationFailedEventArgs e);

/// <summary>
///     Provides event data for the Frame.NavigationFailed event.
/// </summary>
public class NavigationFailedEventArgs : EventArgs
{
    internal NavigationFailedEventArgs(Exception ex, string key)
    {
        Exception = ex;
        Key = key;
    }

    /// <summary>
    ///     Gets or sets a value that indicates whether the failure event has been handled.
    /// </summary>
    public bool Handled { get; set; }

    /// <summary>
    ///     Gets the result code for the exception that is associated with the failed navigation.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    ///     Gets the data type of the target page.
    /// </summary>
    public string Key { get; }
}