namespace SvgClassDiagramGenerator.Parser
{
    public abstract class LanguageParser
    {
        public string Text { get; }

        public LanguageParser(string text)
        {
            Text = text;
        }

        public abstract ParsedSyntaxInfo Parse();
    }
}
