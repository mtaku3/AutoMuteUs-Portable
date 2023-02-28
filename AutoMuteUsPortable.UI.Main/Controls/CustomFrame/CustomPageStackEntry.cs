using Avalonia.Controls;
using FluentAvalonia.UI.Media.Animation;

namespace AutoMuteUsPortable.UI.Main.Controls;

/// <summary>
///     Represents an entry in the BackStack or ForwardStack of a Frame.
/// </summary>
public class CustomPageStackEntry
{
    /// <summary>
    ///     Initializes a new instance of the PageStackEntry class.
    /// </summary>
    /// <param name="sourcePageType">The type of page associated with the navigation entry, as a type reference</param>
    /// <param name="parameter">The navigation parameter associated with the navigation entry.</param>
    /// <param name="navigationTransitionInfo">Info about the animated transition associated with the navigation entry.</param>
    public CustomPageStackEntry(object parameter,
        NavigationTransitionInfo navigationTransitionInfo)
    {
        NavigationTransitionInfo = navigationTransitionInfo;
        Parameter = parameter;
    }

    /// <summary>
    ///     Gets a value that indicates the animated transition associated with the navigation entry.
    /// </summary>
    public NavigationTransitionInfo NavigationTransitionInfo { get; internal set; }

    /// <summary>
    ///     Gets the navigation parameter associated with this navigation entry.
    /// </summary>
    public object Parameter { get; set; }


    internal Control Instance { get; set; }
}