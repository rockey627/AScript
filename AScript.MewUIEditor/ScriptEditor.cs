using Aprillz.MewUI;
using Aprillz.MewUI.Controls;
using Aprillz.MewUI.MewvalonEdit;
using Aprillz.MewUI.MewvalonEdit.CodeCompletion;
using Aprillz.MewUI.MewvalonEdit.Folding;
using Aprillz.MewUI.MewvalonEdit.Highlighting;
using Aprillz.MewUI.MewvalonEdit.Search;
using Aprillz.MewUI.MewvalonEdit.Snippets;
using AScript.Lang.JavaScript;
using AScript.Lang.Lua;
using AScript.Lang.Python3;
using AScript.Lang.Sql;
using System.Collections;
using System.Text;

namespace AScript.MewUIEditor;

public sealed class ScriptEditor : Window
{
    private readonly EditorTheme _theme = EditorTheme.Load("2026-palette.json");
    private readonly ButtonGroup _group;
    private readonly TextEditor _editor;
    private readonly MultiLineTextBox _result;
    private readonly SearchPanel _search;
    private readonly FoldingManager _foldingManager;
    private readonly BraceFoldingStrategy _braceFolding = new();
    private readonly XmlFoldingStrategy _xmlFolding = new() { ShowAttributesWhenFolded = true };
    private readonly TextBlock _foldingState = new();
    private readonly TextBlock _position = new();
    private readonly TextBlock _selection = new();
    private readonly TextBlock _documentState = new();
    private readonly TextBlock _encoding = new() { Text = "UTF-8" };
    private readonly Border _optionsPanel;
    private string langName = "CSharp";

    public ScriptEditor()
    {
        Script.Langs.Set("JavaScript", JavaScriptLang.Instance);
        Script.Langs.Set("Python", Python3Lang.Instance);
        Script.Langs.Set("Lua", LuaLang.Instance);
        Script.Langs.Set("Sql", SqlLang.Instance);

        Title = "脚本编辑器（Ascript MewUIEditor）";
        FontSize = 16;
        WindowSize = WindowSize.Resizable(1200, 800);

        // Before the editor exists, so the first document is already drawn in the theme's colours:
        // the palette is consulted while painting but nothing repaints on a later swap.
        _theme.Install();

        _group = new ButtonGroup()
            .Items("CSharp", "JavaScript", "Python", "Lua", "Sql")
            .PrepareContainer<string>((seg, name, index) =>
            {
                seg.Click += () =>
                {
                    langName = name;
                    LoadBuiltIn(SampleText.Scripts[index], name);
                };
            })
            .Left();
        _editor = new TextEditor
        {
            // Comma lists resolve to the first installed family: Consolas ships with Windows,
            // Menlo with macOS.
            FontFamily = "Consolas, Menlo, DejaVu Sans Mono",
            FontSize = 18,
            ShowLineNumbers = true,
            WordWrap = false
        };
        _editor.WithTheme((theme, editor) => _theme.Apply(editor, theme.IsDark));
        _result = new MultiLineTextBox
        {
            Placeholder = "脚本运行结果",
            IsReadOnly = true,
            AcceptTab = true
        };
        _search = SearchPanel.Install(_editor.TextArea);
        // Ctrl+Space opens the completion window at the current word, VS-style.
        _editor.InputMap.Map(
            new KeyGesture(Key.Space, ModifierKeys.Control), CompleteCurrentWord);
        // Installing the manager attaches the folding margin beside the line numbers; its boxes
        // toggle a section on click.
        _foldingManager = FoldingManager.Install(_editor.TextArea);
        _foldingManager.FoldingsChanged += (_, _) => UpdateFoldingState();
        // URLs and mail addresses underline and open on Ctrl+Click without registering anything:
        // the editor builds both generators from EnableHyperlinks and EnableEmailHyperlinks.
        // Neither carries a colour, so appearance comes from the view.
        _optionsPanel = CreateOptionsPanel();
        _optionsPanel.IsVisible = false;

        _editor.TextArea.Caret.PositionChanged += (_, _) => UpdateStatus();
        _editor.TextArea.SelectionChanged += (_, _) => UpdateStatus();
        _editor.TextArea.TextEntered += _ => UpdateStatus();
        _editor.TextChanged += (_, _) =>
        {
            UpdateDocumentState();
            UpdateFoldings(_editor.SyntaxHighlighting?.Name);
        };

        LoadBuiltIn(SampleText.Scripts[0], "CSharp");
        Content = new DockPanel()
            .Spacing(8)
            .Children(
                CreateToolbar().DockTop(),
                CreateStatusBar().DockBottom(),
                _optionsPanel.DockRight(),
                _result.DockBottom(),
                _editor);
    }

    // A click leaves the focus on the button, and the editing features ride the editor's own
    // focus - the completion and snippet keys among them - so every action that drives the
    // editor takes the focus back first.
    Button EditorAction(string content, Action action)
    {
        return new Button().Content(content).OnClick(() => { _editor.Focus(); action(); });
    }

    private FrameworkElement CreateToolbar()
    {
        return new WrapPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Padding = new Thickness(8)
        }.Children(
            _group,
            EditorAction(" 运行 » ", ExecuteScript),
            Divider(),
            EditorAction("撤销", () => _editor.Undo()),
            EditorAction("重做", () => _editor.Redo()),
            Divider(),
            // The panel, the completion window and the snippet all answer their own keys; the
            // buttons are here so the features are visible without knowing them.
            EditorAction("查找", _search.Open),
            EditorAction("补充", CompleteCurrentWord),
            EditorAction("插片", InsertForLoopSnippet),
            Divider(),
            new Button().Content("设置").OnClick(() => _optionsPanel.IsVisible = !_optionsPanel.IsVisible),
            new Button().Content("暗亮").OnClick(ToggleTheme));
    }

    /// <summary>A hairline between toolbar groups.</summary>
    private static FrameworkElement Divider()
        => new Border { Width = 1, Margin = new Thickness(2, 2, 2, 2) }
            .WithTheme((theme, border) => border.Background(theme.Palette.ControlBorder));

    private Border CreateOptionsPanel()
    {
        CheckBox Toggle(string title, bool initial, Action<bool> apply)
            => new CheckBox { IsChecked = initial }
                .Content(title)
                .OnCheckedChanged(value => apply(value == true));

        // The checkbox itself carries the state the theme callback needs, so it is captured rather
        // than mirrored into a field.
        var customSelection = Toggle("Custom selection color", false,
            enabled => ApplySelectionColors(Application.Current.Theme, enabled));
        _editor.WithTheme((theme, _) => ApplySelectionColors(theme, customSelection.IsChecked == true));

        var indentationSize = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 16,
            Step = 1,
            Format = "0",
            Value = _editor.Options.IndentationSize,
            Width = 80
        };
        indentationSize.ValueChanged += value => _editor.Options.IndentationSize = (int)value;

        var rulerPosition = new NumericUpDown
        {
            Minimum = 10,
            Maximum = 200,
            Step = 5,
            Format = "0",
            Value = _editor.Options.ColumnRulerPosition,
            Width = 80
        };
        rulerPosition.ValueChanged += value => _editor.Options.ColumnRulerPosition = (int)value;

        return new Border
        {
            Width = 300,
            Padding = new Thickness(12),
            BorderThickness = 1,
            Child = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 8
            }.Children(
                new TextBlock().Text("Editor options").FontSize(16).Bold(),
                Toggle("Word wrap", _editor.WordWrap, value => _editor.WordWrap = value),
                Toggle("Show line numbers", _editor.ShowLineNumbers, value => _editor.ShowLineNumbers = value),
                Toggle("Show spaces", _editor.Options.ShowSpaces, value => _editor.Options.ShowSpaces = value),
                Toggle("Show tabs", _editor.Options.ShowTabs, value => _editor.Options.ShowTabs = value),
                Toggle("Show end-of-line", _editor.Options.ShowEndOfLine, value => _editor.Options.ShowEndOfLine = value),
                Toggle("Convert tabs to spaces", _editor.Options.ConvertTabsToSpaces,
                    value => _editor.Options.ConvertTabsToSpaces = value),
                new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 }
                    .Children(new TextBlock().Text("Indentation size").Width(180), indentationSize),
                Toggle("Read only", _editor.IsReadOnly, value => _editor.IsReadOnly = value),
                Toggle("Allow Insert to overwrite", _editor.Options.AllowToggleOverstrikeMode,
                    value => _editor.Options.AllowToggleOverstrikeMode = value),
                Toggle("Ctrl+click to follow links", _editor.Options.RequireControlModifierForHyperlinkClick,
                    value => _editor.Options.RequireControlModifierForHyperlinkClick = value),
                Toggle("Enable IME", _editor.Options.EnableImeSupport,
                    value => _editor.Options.EnableImeSupport = value),
                Toggle("Hide cursor while typing", _editor.Options.HideCursorWhileTyping,
                    value => _editor.Options.HideCursorWhileTyping = value),
                Toggle("Highlight current line", _editor.Options.HighlightCurrentLine,
                    value => _editor.Options.HighlightCurrentLine = value),
                Toggle("Show column ruler", _editor.Options.ShowColumnRuler,
                    value => _editor.Options.ShowColumnRuler = value),
                new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 }
                    .Children(new TextBlock().Text("Column ruler position").Width(180), rulerPosition),
                // The colours live on the view, so one setting covers links from every generator.
                Toggle("Custom link color", false, ApplyCustomLinkColor),
                Toggle("Underline links", _editor.TextArea.TextView.LinkTextUnderline,
                    value => _editor.TextArea.TextView.LinkTextUnderline = value),
                // Setting any of these replaces the host's selection layer with the editor's own,
                // which is the one consumer proving layer replacement works.
                customSelection,
                // The caret is the editor's own layer, so its colour is settable. A box selection
                // already colours its own carets; setting these takes that over.
                Toggle("Custom caret color", false, value =>
                {
                    _editor.TextArea.Caret.CaretBrush = value ? Color.FromRgb(220, 60, 60) : null;
                    _editor.TextArea.Caret.SecondaryCaretBrush = value ? Color.FromRgb(200, 140, 40) : null;
                }))
        }.WithTheme((theme, border) => border
            .Background(theme.Palette.ContainerBackground)
            .BorderBrush(theme.Palette.ControlBorder));
    }

    private FrameworkElement CreateStatusBar()
        => new Border()
            .Padding(new Thickness(8, 4, 8, 4))
            .WithTheme((theme, border) => border.Background(theme.Palette.ContainerBackground))
            .Child(new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 18
            }.Children(_position, _selection, _documentState, _foldingState, _encoding));

    /// <summary>
    /// Loads one of the samples built into this executable. The file list holds no row for it, so
    /// its highlighted row goes, which would otherwise claim to be what is open.
    /// </summary>
    private void LoadBuiltIn(string text, string? highlightingName)
    {
        LoadSample(text, highlightingName);
    }

    private void LoadSample(string text, string? highlightingName)
    {
        // The assignment alone puts the caret back and takes the search and the foldings with it;
        // only the strategy that finds foldings in the new language is this host's to pick.
        _editor.Text = text;
        _editor.SyntaxHighlighting = highlightingName is null
            ? null
            : HighlightingManager.Instance.GetDefinition(highlightingName);
        UpdateFoldings(highlightingName);
        UpdateStatus();
        UpdateDocumentState(highlightingName);
    }

    /// <summary>Picks the folding strategy the loaded language has one for.</summary>
    private void UpdateFoldings(string? language)
    {
        switch (language)
        {
            case "CSharp":
            case "JavaScript":
            case "Python":
            case "Lua":
            case "Sql":
                _braceFolding.UpdateFoldings(_foldingManager, _editor.Document);
                break;
            case "XML":
                _xmlFolding.UpdateFoldings(_foldingManager, _editor.Document);
                break;
            default:
                _foldingManager.UpdateFoldings([], -1);
                break;
        }
        UpdateFoldingState();
    }

    private void UpdateFoldingState()
    {
        int total = _foldingManager.AllFoldings.Count();
        int folded = _foldingManager.AllFoldings.Count(static folding => folding.IsFolded);
        _foldingState.Text = $"Foldings: {folded}/{total}";
    }

    /// <summary>
    /// Selection appearance drawn from the accent, so the custom colors stay legible when the theme
    /// flips. Recoloring the glyphs flattens the syntax colors inside the selection, so the text
    /// only leans halfway to the accent instead of taking it whole.
    /// </summary>
    private void ApplySelectionColors(Theme theme, bool enabled)
    {
        var area = _editor.TextArea;
        var palette = theme.Palette;
        area.SelectionBrush = enabled ? palette.Accent.WithAlpha(0x60) : null;
        area.SelectionBorder = enabled ? palette.Accent : null;
        area.SelectionForeground = enabled ? palette.WindowText.Lerp(palette.Accent, 0.5) : null;
        _editor.InvalidateTextView();
    }

    private void ApplyCustomLinkColor(bool enabled)
    {
        var view = _editor.TextArea.TextView;
        view.LinkTextForegroundBrush = enabled ? Color.FromRgb(0xC0, 0x50, 0x20) : null;
        view.LinkTextBackgroundBrush = enabled ? Color.FromArgb(0x20, 0xC0, 0x50, 0x20) : null;
    }

    private void ToggleFirstFolding()
    {
        var folding = _foldingManager.AllFoldings.FirstOrDefault();
        if (folding is not null) folding.IsFolded = !folding.IsFolded;
    }

    private void CompleteCurrentWord()
    {
        int end = _editor.CaretOffset;
        int start = end;
        while (start > 0 && char.IsLetterOrDigit(_editor.Document.GetCharAt(start - 1))) start--;
        var completion = new CompletionWindow(_editor.TextArea)
        {
            StartOffset = start,
            // Ctrl+Space semantics: erasing back to the word start closes the window.
            CloseWhenCaretAtBeginning = true
        };
        foreach (var item in SampleCompletionData.All)
        {
            completion.CompletionList.CompletionData.Add(item);
        }
        completion.Show();
        completion.CompletionList.SelectItem(_editor.Document.GetText(start, end - start));
    }

    private void InsertForLoopSnippet()
    {
        // The counter is one replaceable field mirrored into two bound copies, the limit is a
        // second field, and the caret lands in the body when interactive mode ends. Tab walks the
        // fields, Enter and Escape end the mode.
        var counter = new SnippetReplaceableTextElement { Text = "i" };
        var snippet = new Snippet();
        snippet.Elements.Add(new SnippetTextElement { Text = "for (int " });
        snippet.Elements.Add(counter);
        snippet.Elements.Add(new SnippetTextElement { Text = " = 0; " });
        snippet.Elements.Add(new SnippetBoundElement { TargetElement = counter });
        snippet.Elements.Add(new SnippetTextElement { Text = " < " });
        snippet.Elements.Add(new SnippetReplaceableTextElement { Text = "count" });
        snippet.Elements.Add(new SnippetTextElement { Text = "; " });
        snippet.Elements.Add(new SnippetBoundElement { TargetElement = counter });
        snippet.Elements.Add(new SnippetTextElement { Text = "++)\n{\n\t" });
        snippet.Elements.Add(new SnippetCaretElement());
        snippet.Elements.Add(new SnippetTextElement { Text = "\n}" });

        snippet.Insert(_editor.TextArea);
    }

    private static void ToggleTheme()
    {
        var application = Application.Current;
        application.SetThemeMode(application.Theme.IsDark ? ThemeVariant.Light : ThemeVariant.Dark);
    }

    private void UpdateStatus()
    {
        var caret = _editor.TextArea.Caret;
        _position.Text = $"Ln {caret.Line}, Col {caret.Column} (Offset {caret.Offset})";
        _selection.Text = $"Selection: {_editor.SelectionLength}";
    }

    private void UpdateDocumentState(string? highlightingName = null)
    {
        string language = highlightingName ?? _editor.SyntaxHighlighting?.Name ?? "Plain text";
        _documentState.Text = $"{language} · {_editor.LineCount:N0} lines · {_editor.Document.TextLength:N0} chars";
    }

    void ExecuteScript()
    {
        try
        {
            var list = new[] { new Person("tom", 15), new Person("jim", 10), new Person("san", 20), new Person("qin", 10) };
            var script = new Script();
            script.Context.Langs = [langName];
            script.Context.AddType<Person>();
            script.Context.SetVar("list", list);
            var text = _editor.Text;
            var result =
                langName == "Sql" ?
                script.Eval<IEnumerable<dynamic>>(text).ToList() :
                script.Eval(text);
            _result.Text =
                result == null ?
                "null" :
                $"类型：{result.GetType()}\n结果：{ResultString(result)}";
        }
        catch (Exception ex)
        {
            _result.Text = ex.Message;
        }
    }

    string ResultString(object obj)
    {
        var sb = new StringBuilder();
        if (obj is IList list)
        {
            sb.Append("[ ");
            int i = 0;
            foreach (var o in list)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(ResultString(o));
                i++;
            }
            sb.Append(" ]");
        }
        else
        {
            sb.Append(obj);
        }
        return sb.ToString();
    }
}

public record Person(string Name, int Age);
