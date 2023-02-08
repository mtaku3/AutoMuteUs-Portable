using System.Collections;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;

namespace AutoMuteUsPortable.UI.Setup.Controls;

public class SetupPageContainer : TemplatedControl
{
    public static readonly DirectProperty<SetupPageContainer, IEnumerable> BodyProperty =
        AvaloniaProperty.RegisterDirect<SetupPageContainer, IEnumerable>(nameof(Body), o => o.Body,
            (o, v) => o.Body = v);

    private IEnumerable _body = new AvaloniaList<object>();

    public IEnumerable Body
    {
        get => _body;
        set => SetAndRaise(BodyProperty, ref _body, value);
    }

    public static readonly DirectProperty<SetupPageContainer, IEnumerable> FooterProperty =
        AvaloniaProperty.RegisterDirect<SetupPageContainer, IEnumerable>(
            nameof(Footer), o => o._footer, (o, v) => o.Footer = v);

    private IEnumerable _footer = new AvaloniaList<object>();

    public IEnumerable Footer
    {
        get => _footer;
        set => SetAndRaise(FooterProperty, ref _footer, value);
    }
}