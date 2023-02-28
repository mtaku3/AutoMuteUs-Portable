using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AutoMuteUsPortable.UI.Main.Controls;

public partial class CustomFrame : ContentControl
{
    /// <summary>
    ///     Defines the <see cref="EntryStack" /> property
    /// </summary>
    public static readonly DirectProperty<CustomFrame, IDictionary<string, CustomPageStackEntry>> EntryStackProperty =
        AvaloniaProperty.RegisterDirect<CustomFrame, IDictionary<string, CustomPageStackEntry>>(nameof(EntryStack),
            x => x.EntryStack);

    /// <summary>
    ///     Defines the <see cref="IsNavigationStackEnabled" /> property
    /// </summary>
    public static readonly StyledProperty<bool> IsNavigationStackEnabledProperty =
        AvaloniaProperty.Register<CustomFrame, bool>(nameof(IsNavigationStackEnabled),
            true);

    /// <summary>
    ///     Gets a collection of <see cref="CustomPageStackEntry" /> instances representing the
    ///     forward navigation history of the Frame.
    /// </summary>
    public IDictionary<string, CustomPageStackEntry> EntryStack
    {
        get => _entryStack;
        private set => SetAndRaise(EntryStackProperty, ref _entryStack, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates whether navigation is recorded in the Frame's
    ///     <see cref="EntryStack" />
    /// </summary>
    public bool IsNavigationStackEnabled
    {
        get => GetValue(IsNavigationStackEnabledProperty);
        set => SetValue(IsNavigationStackEnabledProperty, value);
    }

    internal CustomPageStackEntry CurrentEntry { get; set; }

    /// <summary>
    ///     Occurs when the content that is being navigated to has been found and is available
    ///     from the Content property, although it may not have completed loading.
    /// </summary>
    public event NavigatedEventHandler Navigated;

    /// <summary>
    ///     Occurs when a new navigation is requested.
    /// </summary>
    public event NavigatingCancelEventHandler Navigating;

    /// <summary>
    ///     Occurs when an error is raised while navigating to the requested content.
    /// </summary>
    public event NavigationFailedEventHandler NavigationFailed;

    /// <summary>
    ///     Occurs when a new navigation is requested while a current navigation is in progress.
    /// </summary>
    public event NavigationStoppedEventHandler NavigationStopped;

    /// <summary>
    ///     Indicates to a page that it is being navigated away from. Takes the place of
    ///     Microsoft.UI.Xaml.Controls.Page.OnNavigatingFrom() method
    /// </summary>
    public static readonly RoutedEvent<NavigatingCancelEventArgs> NavigatingFromEvent =
        RoutedEvent.Register<Control, NavigatingCancelEventArgs>("NavigatingFrom",
            RoutingStrategies.Direct);

    /// <summary>
    ///     Indiates to a page that it has been navigated away from. Takes the place of
    ///     Microsoft.UI.Xaml.Controls.Page.OnNavigatedFrom() method
    /// </summary>
    public static readonly RoutedEvent<NavigationEventArgs> NavigatedFromEvent =
        RoutedEvent.Register<Control, NavigationEventArgs>("NavigatedFrom",
            RoutingStrategies.Direct);

    /// <summary>
    ///     Indiates to a page that it is being navigated to. Takes the place of
    ///     Microsoft.UI.Xaml.Controls.Page.OnNavigatedTo() method
    /// </summary>
    public static readonly RoutedEvent<NavigationEventArgs> NavigatedToEvent =
        RoutedEvent.Register<Control, NavigationEventArgs>("NavigatedTo",
            RoutingStrategies.Direct);

    private IDictionary<string, CustomPageStackEntry> _entryStack;
}