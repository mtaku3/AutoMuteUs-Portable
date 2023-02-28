using System;
using System.Threading;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using FluentAvalonia.UI.Media.Animation;

namespace AutoMuteUsPortable.UI.Main.Controls;

/// <summary>
///     Displays <see cref="UserControl" /> instances (Pages in WinUI), supports navigation to new pages,
///     and maintains a navigation history to support forward and backward navigation.
/// </summary>
[TemplatePart(s_tpContentPresenter, typeof(ContentPresenter))]
public partial class CustomFrame : ContentControl
{
    public CustomFrame()
    {
        var entry = new AvaloniaDictionary<string, CustomPageStackEntry>();

        EntryStack = entry;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ContentProperty)
        {
            if (change.NewValue == null) CurrentEntry = null;
        }
        else if (change.Property == IsNavigationStackEnabledProperty)
        {
            if (!change.GetNewValue<bool>()) _entryStack.Clear();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _presenter = e.NameScope.Find<ContentPresenter>(s_tpContentPresenter);
    }

    protected override bool RegisterContentPresenter(IContentPresenter presenter)
    {
        if (presenter.Name == "ContentPresenter")
            return true;

        return base.RegisterContentPresenter(presenter);
    }

    public bool Navigate(string key)
    {
        var entry = EntryStack[key];

        try
        {
            _isNavigating = true;

            var ea = new NavigatingCancelEventArgs(
                entry.NavigationTransitionInfo,
                entry.Parameter,
                key);

            Navigating?.Invoke(this, ea);

            if (ea.Cancel)
            {
                OnNavigationStopped(key, entry);
                return false;
            }

            // Tell the current page we want to navigate away from it
            if (CurrentEntry?.Instance is Control oldPage)
            {
                ea.RoutedEvent = NavigatingFromEvent;
                oldPage.RaiseEvent(ea);

                if (ea.Cancel)
                {
                    OnNavigationStopped(key, entry);
                    return false;
                }
            }

            var oldEntry = CurrentEntry;
            CurrentEntry = entry;

            var navEA = new NavigationEventArgs(
                CurrentEntry.Instance,
                entry.NavigationTransitionInfo,
                entry.Parameter, key);

            // Old page is now unloaded, raise OnNavigatedFrom
            if (oldEntry != null)
            {
                navEA.RoutedEvent = NavigatedFromEvent;
                oldEntry.Instance.RaiseEvent(navEA);
            }

            SetContentAndAnimate(entry);

            Navigated?.Invoke(this, navEA);

            // New Page is loaded, let's tell the page
            if (entry.Instance is Control newPage)
            {
                navEA.RoutedEvent = NavigatedToEvent;
                newPage.RaiseEvent(navEA);
            }

            //Need to find compatible method for this
            //VisualTreeHelper.CloseAllPopups();

            return true;
        }
        catch (Exception ex)
        {
            NavigationFailed?.Invoke(this, new NavigationFailedEventArgs(ex, key));

            //I don't really want to throw an exception and break things. Just return false
            return false;
        }
        finally
        {
            _isNavigating = false;
        }
    }

    private void OnNavigationStopped(string key, CustomPageStackEntry entry)
    {
        NavigationStopped?.Invoke(this,
            new NavigationEventArgs(entry.Instance, entry.NavigationTransitionInfo, entry.Parameter, key));
    }

    private void SetContentAndAnimate(CustomPageStackEntry entry)
    {
        if (entry == null)
            return;

        Content = entry.Instance;

        if (_presenter != null)
        {
            //Default to entrance transition
            entry.NavigationTransitionInfo = entry.NavigationTransitionInfo ?? new EntranceNavigationTransitionInfo();
            _presenter.Opacity = 0;

            _cts?.Cancel();
            // _cts = new CancellationTokenSource();

            // Post the animation otherwise pages that take slightly longer to load won't
            // have an animation since it will run before layout is complete
            Dispatcher.UIThread.Post(
                () => { entry.NavigationTransitionInfo.RunAnimation(_presenter /* , _cts.Token */); },
                DispatcherPriority.Render);
        }
    }

    private CancellationTokenSource _cts;
    private ContentPresenter _presenter;
    private bool _isNavigating;

    private const string s_tpContentPresenter = "ContentPresenter";
}