
namespace SvgClassDiagramGenerator.Parser;

public class ParsedSyntaxInfo
{
    public required List<ClassSyntax> Classes { get; init; } = new List<ClassSyntax>();
}

public class ClassSyntax
{
    public required string Name { get; init; }

    public string? BaseClasses { get; init; }

    public List<DataMember> Properties { get; init; } = new();
    public List<MemberFunction> Functions { get; init; } = new();

}

public abstract record Member
{
    /// <summary>
    /// Is modifier public
    /// </summary>
    public required bool IsPublic { get; init; }
    /// <summary>
    /// Type of property or return type of function
    /// </summary>
    public required string Type { get; init; }
    /// <summary>
    /// Name if the Property or field or method
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The Class that can be linked to it to draw conenction line
    /// </summary>
    public ClassSyntax? LinksClass { get; init; }
}

public record MemberFunction : Member
{
    public List<Parameter> Arguments { get; init; } = new List<Parameter>();
}

public record DataMember : Member;

public record AttributeInfo(bool IsPublic);

public record Parameter
{
    /// <summary>
    /// Type of property or return type of function
    /// </summary>
    public required string Type { get; init; }
    /// <summary>
    /// Name if the Property or field or method
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The Class that can be linked to it to draw conenction line
    /// </summary>
    public ClassSyntax? LinksClass { get; init; }
}