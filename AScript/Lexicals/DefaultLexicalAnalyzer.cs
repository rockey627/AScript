using AScript.Readers;
using System;
using System.IO;

namespace AScript.Lexicals
{
    public class DefaultLexicalAnalyzer : ILexicalAnalyzer
    {
        public static readonly DefaultLexicalAnalyzer Instance = new DefaultLexicalAnalyzer();

        public virtual ITokenStream Create(string expression)
        {
            return new DefaultTokenStream(expression);
        }

        public virtual ITokenStream Create(Stream expression, bool autoDisposeStream)
        {
            return new DefaultTokenStream(expression, autoDisposeStream);
        }
    }
}
