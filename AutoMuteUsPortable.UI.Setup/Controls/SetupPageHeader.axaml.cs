using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using FluentAvalonia.FluentIcons;

namespace AutoMuteUsPortable.UI.Setup.Controls;

public class SetupPageHeader : TemplatedControl
{
    public static readonly StyledProperty<FluentIconSymbol> SymbolProperty =
        FluentIcon.IconProperty.AddOwner<FluentIcon>();

    public static readonly DirectProperty<SetupPageHeader, string?> TitleProperty =
        TextBlock.TextProperty.AddOwner<SetupPageHeader>(o => o.Title, (o, v) =>
        {
            if (v != null) o.Title = v;
        });

    private string? _title;

    [Content] public string? Title
    {
        get => _title;
        set => SetAndRaise(TitleProperty, ref _title, value);
    }

    public FluentIconSymbol Symbol
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }
}