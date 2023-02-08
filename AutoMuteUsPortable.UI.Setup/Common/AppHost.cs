using System;
using System.IO;
using AutoMuteUsPortable.PocketBaseClient;
using AutoMuteUsPortable.UI.Setup.Views;
using FluentAvalonia.UI.Controls;

namespace AutoMuteUsPortable.UI.Setup.Common;

public class AppHost
{
    public AppHost()
    {
        PocketBaseClientApplication = new PocketBaseClientApplication();
    }

    public static AppHost Instance { get; private set; }

    public static readonly string DefaultConfigPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AutoMuteUsPortable.json");

    public static readonly string ProcessPath = Environment.ProcessPath!;

    public PocketBaseClientApplication PocketBaseClientApplication { get; }
    public IConfigurationState ConfigurationState { get; set; }
    public Frame Frame => MainWindow.Instance.Frame;

    public static void Initialize()
    {
        if (Instance != null) throw new InvalidOperationException("AppHost is already initialized");

        Instance = new AppHost();
    }
}