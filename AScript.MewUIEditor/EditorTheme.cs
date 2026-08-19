using System.Reflection;
using System.Text.Json;
using Aprillz.MewUI;
using Aprillz.MewUI.MewvalonEdit;
using Aprillz.MewUI.MewvalonEdit.Highlighting;

namespace AScript.MewUIEditor;

/// <summary>
/// A palette file the editor is painted from. The colours are data (Themes/*.json, keyed by the
/// TextMate scope each was resolved from) and which xshd colour name takes which scope is
/// <see cref="XshdScopeMap"/>, so any theme resolved to the same scopes loads without code.
/// </summary>
internal sealed class EditorTheme
{
    private const string RESOURCE_PREFIX = "AScript.MewUIEditor.Themes.";

    private readonly Dictionary<string, PaletteEntry> _tokens;
    private readonly Dictionary<string, (Color Dark, Color Light)> _surface;

    private EditorTheme(
        Dictionary<string, PaletteEntry> tokens,
        Dictionary<string, (Color Dark, Color Light)> surface)
    {
        _tokens = tokens;
        _surface = surface;
    }

    /// <summary>Reads a palette embedded from the Themes folder, named as it is on disk.</summary>
    public static EditorTheme Load(string fileName)
    {
        string resource = RESOURCE_PREFIX + fileName;
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource)
            ?? throw new InvalidOperationException($"The palette resource '{resource}' is missing.");
        using var document = JsonDocument.Parse(stream);

        var tokens = new Dictionary<string, PaletteEntry>(StringComparer.Ordinal);
        foreach (var property in document.RootElement.GetProperty("tokens").EnumerateObject())
        {
            var (dark, light) = ReadPair(property.Value);
            tokens[property.Name] = new PaletteEntry(dark, light);
        }

        var surface = new Dictionary<string, (Color, Color)>(StringComparer.Ordinal);
        foreach (var property in document.RootElement.GetProperty("editor").EnumerateObject())
        {
            surface[property.Name] = ReadPair(property.Value);
        }

        return new EditorTheme(tokens, surface);
    }

    /// <summary>
    /// Makes this the palette the colorizers read. A scope the file leaves out keeps whatever
    /// colour its definition carries, so a definition this theme does not cover still draws.
    /// </summary>
    public void Install()
    {
        var palette = new HighlightingPalette();
        foreach ((string scope, string[] names) in XshdScopeMap.Entries)
        {
            if (!_tokens.TryGetValue(scope, out var entry))
            {
                continue;
            }
            foreach (string name in names)
            {
                palette.Set(name, entry);
            }
        }
        HighlightingPalette.Current = palette;
    }

    /// <summary>
    /// Applies the surface colours the file carries. Only these three are reachable: the selection,
    /// the caret and the current-line highlight come from the MewUI theme and the editor exposes no
    /// way to override them.
    /// </summary>
    public void Apply(TextEditor editor, bool isDark)
    {
        ArgumentNullException.ThrowIfNull(editor);
        if (TryGetSurface("background", isDark, out var background))
        {
            editor.Background = background;
        }
        if (TryGetSurface("foreground", isDark, out var foreground))
        {
            editor.Foreground = foreground;
        }
        if (TryGetSurface("lineNumber", isDark, out var lineNumber))
        {
            editor.LineNumbersForeground = lineNumber;
        }
    }

    private bool TryGetSurface(string key, bool isDark, out Color color)
    {
        if (!_surface.TryGetValue(key, out var pair))
        {
            color = default;
            return false;
        }
        color = isDark ? pair.Dark : pair.Light;
        return true;
    }

    private static (Color Dark, Color Light) ReadPair(JsonElement element) => (
        Color.FromHex(element.GetProperty("dark").GetString()),
        Color.FromHex(element.GetProperty("light").GetString()));
}
