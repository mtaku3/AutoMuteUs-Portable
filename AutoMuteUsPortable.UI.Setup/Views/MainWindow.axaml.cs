using System;
using AutoMuteUsPortable.UI.Setup.Common;
using AutoMuteUsPortable.UI.Setup.Pages;
using AutoMuteUsPortable.UI.Setup.ViewModels;
using Avalonia.ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainWindowViewModel();
    }

    public static MainWindow Instance { get; private set; }

    public static void Initialize()
    {
        if (Instance != null) throw new InvalidOperationException("Main window is already initialized");

        Instance = new MainWindow();
        AppHost.Instance.Frame.Navigate(typeof(ChooseOptionPage));
    }
}