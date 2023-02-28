using Avalonia.Interactivity;
using FluentAvalonia.UI.Media.Animation;

namespace AutoMuteUsPortable.UI.Main.Controls;

/// <summary>
///     Represents the method that will handle the Navigated event.
/// </summary>
/// <param name="sender">The object where the handler is attached.</param>
/// <param name="e">Event data for the event.</param>
public delegate void NavigatedEventHandler(object sender, NavigationEventArgs e);

/// <summary>
///     Provides event data for the NavigationStopped event.
/// </summary>
/// <param name="sender">The object where the handler is attached.</param>
/// <param name="e">Event data for the event.</param>
public delegate void NavigationStoppedEventHandler(object sender, NavigationEventArgs e);

/// <summary>
///     Provides data for navigation methods and event handlers that cannot cancel the navigation request.
/// </summary>
public class NavigationEventArgs : RoutedEventArgs
{
    internal NavigationEventArgs(object content,
        NavigationTransitionInfo navInfo, object param,
        string key)
    {
        Content = content;
        NavigationTransitionInfo = navInfo;
        Parameter = param;
        Key = key;
    }

    //public Uri Uri { get; set; }

    /// <summary>
    ///     Gets the root node of the target page's content.
    /// </summary>
    public object Content { get; }

    /// <summary>
    ///     Gets any "Parameter" object passed to the target page for the navigation.
    /// </summary>
    public object Parameter { get; }

    /// <summary>
    ///     Gets the data type of the source page.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Gets a value that indicates the animated transition associated with the navigation.
    /// </summary>
    public NavigationTransitionInfo NavigationTransitionInfo { get; }
}