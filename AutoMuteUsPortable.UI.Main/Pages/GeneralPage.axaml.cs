using System;
using System.Text;
using AutoMuteUsPortable.UI.Main.Common;
using AutoMuteUsPortable.UI.Main.ViewModels;
using Avalonia.ReactiveUI;

namespace AutoMuteUsPortable.UI.Main.Pages;

public partial class GeneralPage : ReactiveUserControl<GeneralPageViewModel>
{
    public GeneralPage()
    {
        InitializeComponent();
        ViewModel = new GeneralPageViewModel();

        var config = AppHost.Instance.ConfigRepository.ActiveConfig;
        if (!config.serverConfiguration.IsSimpleSettingsUsed) throw new NotImplementedException();

        var token = config.serverConfiguration.simpleSettings!.discordToken;
        var encodedClientId = token.Split(".")[0];
        if (encodedClientId.Length % 4 > 0)
        {
            var padding = 4 - encodedClientId.Length % 4;
            encodedClientId = encodedClientId.PadRight(encodedClientId.Length + padding, '=');
        }

        var decodedClientId = Encoding.UTF8.GetString(Convert.FromBase64String(encodedClientId));


        DiscordBotInviteHyperlinkButton.NavigateUri = new Uri(
            $"https://discord.com/oauth2/authorize?client_id={decodedClientId}&permissions=12905472&scope=applications.commands%20bot");
    }
}