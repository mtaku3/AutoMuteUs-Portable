using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

namespace AutoMuteUsPortable.UI.Setup.Controls;

public class DiscordStyledButton : TemplatedControl
{
    public static readonly DirectProperty<DiscordStyledButton, string?> TextProperty =
        TextBlock.TextProperty.AddOwner<DiscordStyledButton>(o => o.Text, (o, v) =>
        {
            if (v != null) o.Text = v;
        });

    private string? _text;

    [Content]
    public string? Text
    {
        get => _text;
        set => SetAndRaise(TextProperty, ref _text, value);
    }
}