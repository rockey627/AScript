namespace AScript.MewUIEditor;

/// <summary>
/// Which TextMate scope each xshd colour name takes its colour from. This is a property of the
/// syntax definitions, not of any one theme, so a palette resolved to these scopes themes every
/// definition listed here.
/// </summary>
/// <remarks>
/// Two limits are inherent to mapping a TextMate theme onto a regex tokenizer. The xshd definitions
/// split keywords far more finely than a theme does, so several names fold onto one scope and lose
/// a distinction the definition drew. And VS Code paints much of C# from semantic highlighting,
/// which no regex tokenizer produces; what a mapped theme gives is its colours as VS Code would
/// paint them with semantic highlighting off.
/// </remarks>
internal static class XshdScopeMap
{
    /// <summary>
    /// Each scope with the xshd colour names drawn in it. A scope may be listed more than once,
    /// and a name left out keeps the colour its own definition carries.
    /// </summary>
    public static readonly (string Scope, string[] Names)[] Entries =
    [
        // C#. Which set gets which colour follows the words the set actually holds: Keywords is
        // else/if/switch/for/while, which a theme paints as control flow, not as a plain keyword.
        ("keyword.control", ["Keywords", "GotoKeywords", "ExceptionKeywords"]),
        ("keyword", [
            "NamespaceKeywords", "GetSetAddRemove", "ContextKeywords", "OperatorKeywords",
            "CheckedKeyword", "UnsafeKeywords", "SemanticKeywords"]),
        ("storage.modifier", ["Modifiers", "Visibility", "ParameterModifiers"]),
        ("constant.language", ["TrueFalse", "NullOrValueKeywords"]),
        ("entity.name.type", ["ValueTypeKeywords", "ReferenceTypeKeywords", "TypeKeywords"]),
        ("variable.language", ["ThisOrBaseReference"]),
        ("entity.name.function", ["MethodCall"]),
        ("meta.preprocessor", ["Preprocessor", "PreprocessorSet"]),

        // Shared across definitions. Every definition names its own scopes, so a colour has to be
        // listed under each name that means the same thing; a name left out draws in the colour its
        // definition carries, which is what makes a language look untouched.
        ("comment", [
            "Comment", "DocCommentMarker", "CommentMarkerSet", "CommentTags", "JavaDocTags",
            "DocComment", "DocCommentSet", "KnownDocTags", "XmlPunctuation"]),
        ("string", ["String", "Char", "Character", "StringInterpolation", "XmlString"]),
        ("constant.numeric", ["NumberLiteral", "Number", "Digits", "DateLiteral"]),
        ("keyword.control", [
            "ControlFlow", "LoopKeywords", "JumpKeywords", "IterationStatements",
            "SelectionStatements", "JumpStatements", "ControlStatements", "ExceptionHandling",
            "ExceptionHandlingStatements", "CompoundKeywords"]),
        ("storage.modifier", ["AccessModifiers", "AccessKeywords", "Friend", "FunctionKeywords"]),
        ("entity.name.type", ["ValueTypes", "ReferenceTypes", "DataTypes", "OtherTypes", "Void"]),
        ("entity.name.function", ["MethodName", "FunctionCall", "Command"]),
        ("constant.language", ["Literals", "Constants", "BooleanConstants"]),
        ("meta.preprocessor", ["Package", "Namespace"]),
        ("variable.language", ["This"]),
        ("variable", ["Variable"]),
        ("keyword.operator", ["Operators"]),
        ("constant.character.escape", ["Escape", "EscapeSequence"]),

        // XML.
        ("entity.name.tag", ["XmlTag"]),
        ("entity.other.attribute-name", ["AttributeName"]),
        ("string", ["AttributeValue"]),
        ("keyword", ["Entity"]),
        ("invalid.illegal", ["BrokenEntity"]),
        ("punctuation.definition.tag", ["CData", "DocType", "XmlDeclaration"]),

        // HTML and ASPX, which name the same things differently from XML.
        ("entity.name.tag", ["HtmlTag", "Tags", "ASPSectionStartEndTags"]),
        ("entity.other.attribute-name", ["Attributes"]),
        ("invalid.illegal", ["UnknownAttribute", "UnknownScriptTag"]),
        ("keyword", ["Entities", "EntityReference", "EntityReferenceSet"]),
        ("punctuation.definition.tag", ["Assignment", "Slash"]),
        ("keyword.control", ["ScriptTag", "JavaScriptTag", "VBScriptTag", "JScriptTag", "ASPSection"]),

        // CSS.
        ("entity.other.attribute-name.class.css", ["Selector", "Class"]),
        ("support.type.property-name", ["Property"]),
        ("string", ["Value"]),
        ("punctuation.definition.tag", ["Colon", "CurlyBraces"]),

        // JSON. The definition draws braces and brackets through Object/Array/Expression, which a
        // theme has no rule for, so those keep the definition's colour.
        ("support.type.property-name", ["FieldName"]),
        ("constant.language", ["Bool", "Null"]),

        // JavaScript, for the HTML and script samples. Built-ins take the support colours a theme
        // keeps apart from user-defined names.
        ("keyword.control", ["JavaScriptKeyWords"]),
        ("constant.language", ["JavaScriptLiterals"]),
        ("support.function", ["JavaScriptGlobalFunctions"]),
        ("entity.name.type", ["JavaScriptIntrinsics"]),
        ("string.regexp", ["Regex"]),

        // Markdown.
        ("markup.heading", ["Heading", "Code"]),
        ("markup.bold", ["Emphasis", "StrongEmphasis"]),
        ("markup.inserted", ["BlockQuote"]),
        ("entity.name.function", ["Link", "Image"]),

        // Patch and diff.
        ("markup.inserted", ["AddedText"]),
        ("invalid.illegal", ["RemovedText"]),
        ("markup.heading", ["Header", "FileName"]),
        ("meta.diff.range", ["Position"]),
    ];
}
