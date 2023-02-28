using System;
using AutoMuteUsPortable.UI.Main.ViewModels;
using Avalonia.ReactiveUI;

namespace AutoMuteUsPortable.UI.Main.Pages;

public partial class GeneralPageFrame : ReactiveUserControl<GeneralPageFrameViewModel>
{
    public GeneralPageFrame()
    {
        InitializeComponent();
        ViewModel = new GeneralPageFrameViewModel();
    }

    public static GeneralPageFrame Instance { get; private set; }

    public static void Initialize()
    {
        if (Instance != null) throw new InvalidOperationException("GeneralPageFrame is already initialized");

        Instance = new GeneralPageFrame();
    }
}