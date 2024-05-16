using System.Drawing;
using SvgClassDiagramGenerator.Parser;
using SvgClassDiagramGenerator.Renderer;
using SvgClassDiagramGenerator.Renderer.Elements;

namespace SvgClassDiagramGenerator.SyntaxDrawer;

public class ClassDiagramRenderer
{
    public SvgRenderer Renderer { get; }
    public int PaddingX { get; set; } = 5;
    public int PaddingY { get; set; } = 0;
    public int DivideLineSpacing { get; set; } = 10;

    public string? ClassName { get; private set; }


    private ClassDiagramRenderer(SvgRenderer renderer) {
        Renderer = renderer;
    }

    public static ClassDiagramRenderer Init(SvgRenderer renderer)
    {
        return new ClassDiagramRenderer(renderer);
    }
    public ValueTask CreateSvgCanvas(string id, double width, double height)
    {
        return Renderer.CreateSvgCanvas(id, width, height);
    }
    public void Draw(ClassSyntax @class)
    {

        List<SizeF> sizes = new List<SizeF>();
        SizeF sizeMid  = SizeF.Empty;

        sizes.Add(sizeMid = DrawAttributes(@class.Properties,PaddingY+20));

        sizes.Add(DrawMethods(@class.Functions, (int)sizes[^1].Height+DivideLineSpacing));

        var maxX = sizes.MaxBy(z => z.Width).Width+PaddingX;

        //draw 
        DrawMidLine(maxX, sizeMid.Height);

        var classSize = DrawClassName(@class,maxX);

        //Draw line below class Name
        DrawMidLine(maxX, classSize.Height);

        //draw class box
        DrawRect(maxX, sizes[^1].Height);

        ClassName = @class.Name;
    }

    private void DrawRect(float maxX,float totalY)
    {
        Renderer.Rect(maxX, totalY).Move(PaddingX ,0).Fill("none").Stroke(1,"black");
    }

    public string Svg()
    {
        return Renderer.Svg();
    }
    private SizeF DrawClassName(ClassSyntax @class,double maxX)
    {
        var str = @class.Name;

        if(@class.BaseClasses != null)
        {
            str += ": " + @class.BaseClasses;
        }
        var text = Renderer.Text(str).Attr("font-weight","bold");


        var rect = text.GetBoundingBox(); 

        int x = (int)(maxX / 2 - rect.Width / 2);  //center of x

        text.Move(x+PaddingX,0);

        return new SizeF((float)rect.Width, (float)rect.Height);
    }
    private void DrawMidLine(double maxX,double midY)
    {
        midY += DivideLineSpacing / 2;
        Renderer.Line(PaddingX, midY, maxX+PaddingX, midY).Stroke(1,"black");
    }

    private SizeF DrawAttributes(List<DataMember> dataMembers,int currentY)
    {
        float totalY = currentY;
        float maxX = 0;

        foreach (DataMember member in dataMembers)
        {
            SizeF dim=DrawAttribute(member,totalY);
            maxX= Math.Max(maxX, dim.Width);
            totalY += dim.Height;
        }

        return new SizeF(maxX,totalY);
    }
    private SizeF DrawMethods(List<MemberFunction> functions, int currentY)
    {
        float totalY = currentY;
        float maxX = 0;

        foreach (MemberFunction func in functions)
        {
            SizeF dim = DrawMethod(func, totalY);
            maxX = Math.Max(maxX, dim.Width);
            totalY += dim.Height;
        }

        return new SizeF(maxX, totalY);
    }

    private SizeF DrawMethod(MemberFunction func, float currentY)
    {
        string accessModifier = func.IsPublic ? "+" : "-";
        string methodArguments = string.Join(",", func.Arguments.Select(arg => $"{arg.Name}:{arg.Type}"));

        string str = $"{accessModifier}{func.Name}({methodArguments}): {func.Type}";

        var rect = Renderer.Text(str).Move(PaddingX, currentY).GetBoundingBox();

        return new SizeF((float)rect.Width, (float)rect.Height);
    }

    private SizeF DrawAttribute(DataMember member, float currentY)
    {
        string str = member.IsPublic ? "+" : "-";
        str += $"{member.Name}: {member.Type} ";

        var rect = Renderer.Text(str).Move(PaddingX, currentY).GetBoundingBox();
        return new SizeF((float)rect.Width, (float)rect.Height);
    }
}
