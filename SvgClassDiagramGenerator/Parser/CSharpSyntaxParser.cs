using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SvgClassDiagramGenerator.Parser;

public class CSharpSyntaxParser: LanguageParser
{
    private SyntaxTree SyntaxTree { get; init; }
    private SyntaxNode? _root;
    private SyntaxNode Root
    {
        get
        {
            if (_root == null)
                _root = SyntaxTree.GetCompilationUnitRoot();
            return _root;
        }
    }

    public CSharpSyntaxParser(string text):base(text)
    {
        SyntaxTree = CSharpSyntaxTree.ParseText(text);
    }

    public override ParsedSyntaxInfo Parse()
    {
        return new ParsedSyntaxInfo()
        {
            Classes = GetAllClasses()
        };
    }

    private List<ClassSyntax> GetAllClasses()
    {
        var classes = Root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        List<ClassSyntax> classSyntaxes = new();

        foreach (var classDeclarationSyntax in classes)
        {
            classSyntaxes.Add(ParseClass(classDeclarationSyntax));
        }

        return classSyntaxes;
    }

    private ClassSyntax ParseClass(ClassDeclarationSyntax @class)
    {
        return new ClassSyntax()
        {
            Name = GetClassName(@class),
            BaseClasses = GetBaseClass(@class),
            Functions = GetFunctions(@class),
            Properties = GetProperties(@class)
        };
    }

    private List<DataMember> GetProperties(ClassDeclarationSyntax @class)
    {
        List<DataMember> properties = new List<DataMember>();

        foreach (var prop in @class.DescendantNodes().OfType<PropertyDeclarationSyntax>())
        {
            properties.Add(GetPropertyMember(prop));
        }

        foreach (var f in @class.DescendantNodes().OfType<FieldDeclarationSyntax>())
        {
            properties.AddRange(GetFieldMembers(f));
        }

        return properties;
    }

    private DataMember GetPropertyMember(PropertyDeclarationSyntax prop)
    {
        return new DataMember()
        {
            IsPublic = IsPublic(prop.Modifiers),
            Name = prop.Identifier.ValueText,
            Type = prop.Type.ToString(),
        };
    }

    private List<DataMember> GetFieldMembers(FieldDeclarationSyntax field)
    {
        List<DataMember> dataMembers = new List<DataMember>();
        foreach (var f in field.Declaration.Variables)
        {
            dataMembers.Add(new DataMember()
            {
                IsPublic = IsPublic(field.Modifiers),
                Name = f.Identifier.Text,
                Type = field.Declaration.Type.ToString(),
            });
        }
        return dataMembers;
    }

    private List<MemberFunction> GetFunctions(ClassDeclarationSyntax @class)
    {
        List<MemberFunction> functions = new();

        foreach (var func in @class.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            functions.Add(GetFunction(func));
        }
        return functions;
    }

    private MemberFunction GetFunction(MethodDeclarationSyntax func)
    {
        return new MemberFunction()
        {
            IsPublic = IsPublic(func.Modifiers),
            Name = func.Identifier.Text,
            Type = func.ReturnType.ToString(),
            Arguments = GetArguments(func)
        };
    }

    private List<Parameter> GetArguments(MethodDeclarationSyntax func)
    {
        List<Parameter> arguments = new();

        if (func.ParameterList == null)
            return arguments;

        foreach (var arg in func.ParameterList.DescendantNodes().OfType<ParameterSyntax>())
        {
            arguments.Add(new Parameter()
            {
                Name = arg.Identifier.Text,
                Type = arg.Type!.ToString()
            });
        }

        return arguments;
    }

    private bool IsPublic(SyntaxTokenList token)
    {
        return token.Any(x => x.IsKind(SyntaxKind.PublicKeyword));
    }

    private string? GetBaseClass(ClassDeclarationSyntax @class)
    {
        if (@class.BaseList == null)
            return string.Empty;

        StringBuilder stringBuilder = new StringBuilder(string.Empty);

        var baseNodes = @class.BaseList.DescendantNodes().OfType<BaseTypeSyntax>().ToList();
        for (int i = 0; i < baseNodes.Count; i++)
        {
            stringBuilder.Append(baseNodes[i].ToString());

            if (i != baseNodes.Count - 1)
                stringBuilder.Append(", ");
        }
        return stringBuilder.ToString();
    }

    private string GetClassName(ClassDeclarationSyntax @class)
    {
        return @class.Identifier.ValueText;
    }
}
