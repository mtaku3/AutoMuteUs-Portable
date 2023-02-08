using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMuteUsPortable.UI.Setup.ViewModels;
using Avalonia;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace AutoMuteUsPortable.UI.Setup.Pages;

public partial class CreateDiscordBotPage3 : ReactiveUserControl<CreateDiscordBotPage3ViewModel>
{
    private async Task<bool> ValidateToken(string token)
    {
        using (var client = new HttpClient())
        {
            try
            {
                var request =
                    new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/oauth2/applications/@me");
                request.Headers.Add("Authorization", $"Bot {token}");
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode) return true;
            }
            catch
            {
                // ignored
            }
        }

        return false;
    }

    public CreateDiscordBotPage3()
    {
        InitializeComponent();
        ViewModel = new CreateDiscordBotPage3ViewModel();

        this.WhenActivated(d =>
        {
            d(Observable.Interval(TimeSpan.FromSeconds(.5)).Select(_ => Observable.FromAsync(async () =>
                {
                    var clipboard = Application.Current?.Clipboard;
                    if (clipboard == null) return null;

                    return await clipboard.GetTextAsync();
                }, RxApp.TaskpoolScheduler)).Concat().WhereNotNull().Select(clipboardText =>
                    Observable.FromAsync(
                        async () =>
                        {
                            if (await ValidateToken(clipboardText)) return clipboardText;
                            return null;
                        }, RxApp.TaskpoolScheduler)).Concat().WhereNotNull().ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(token =>
                {
                    ViewModel.Token = token;
                    ViewModel.IsValid = true;
                }));
        });
    }
}