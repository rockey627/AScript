using Aprillz.MewUI.MewvalonEdit.CodeCompletion;

namespace AScript.MewUIEditor;

internal static class SampleCompletionData
{
    public static IReadOnlyList<ICompletionData> All { get; } = Create();

    private static IReadOnlyList<ICompletionData> Create()
    {
        var items = new List<ICompletionData>();
        Add(items, 3, "C# keyword",
            "async", "await", "class", "const", "default", "delegate", "enum", "event", "false",
            "for", "foreach", "if", "interface", "internal", "namespace", "new", "null", "override",
            "private", "protected", "public", "readonly", "record", "return", "sealed", "static",
            "struct", "switch", "this", "throw", "true", "try", "using", "var", "virtual", "while");
        Add(items, 5, ".NET type",
            "ArgumentException", "CancellationToken", "Console", "DateTime", "Dictionary", "Enumerable",
            "Environment", "File", "Guid", "List", "Math", "Path", "StringBuilder", "Task", "TimeSpan");
        Add(items, 4, "Console member",
            "BackgroundColor", "Beep", "Clear", "ForegroundColor", "ReadKey", "ReadLine", "Title", "Write", "WriteLine");
        return items;
    }

    private static void Add(List<ICompletionData> output, double priority, string category, params string[] values)
    {
        foreach (string value in values)
            output.Add(new CompletionData(value, $"{category}: {value}", priority));
    }
}
