using BlazorBindGen;
using Microsoft.JSInterop;
using SvgClassDiagramGenerator.Renderer.Elements;
using static BlazorBindGen.BindGen;

namespace SvgClassDiagramGenerator.Renderer;

public class SvgRenderer
{
    public IJSRuntime JSRuntime { get; }
#nullable disable
    private JObjPtr _drawingPtr;
#nullable restore

    public SvgRenderer(IJSRuntime jSRuntime)
    {
        JSRuntime = jSRuntime;
    }

    public async ValueTask CreateSvgCanvas(string id, double width, double height)
    {
        await InitAsync(JSRuntime);
        _drawingPtr = Window.CallRef("createSvg", id, width, height);
    }
    public Text Plain(string text)
    {
        return new(_drawingPtr.CallRef("plain",text));
    }
    public Text Text(string text)
    {
        return new(_drawingPtr.CallRef("text", text));
    }
    public Rect Rect(double width, double height)
    {
        return new(_drawingPtr.CallRef("rect", width,height));
    }
    public Line Line(double x1,double y1, double x2, double y2)
    {
        return new(_drawingPtr.CallRef("line", x1, y1, x2, y2));
    }
}
