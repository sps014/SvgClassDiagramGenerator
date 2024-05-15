using BlazorBindGen;
using SvgClassDiagramGenerator.Renderer.Elements;

namespace SvgClassDiagramGenerator.Renderer;

public abstract class SvgObjectBase<T> where T:class
{
    private JObjPtr SvgElementPtr { get; }
    public T This => (this as T)!;
    public SvgObjectBase(JObjPtr _svgElementPtr)
    {
        SvgElementPtr = _svgElementPtr;
    }
    public T Move(int x,int y)
    {
        SvgElementPtr.CallVoid("move", x, y);
        return This;
    }
    public T Fill(string color)
    {
        SvgElementPtr.CallVoid("fill", color);
        return This;
    }
    public T Stroke(double width,string color)
    {
        SvgElementPtr.CallVoid("stroke", new {
            width,
            color
        });
        return This;
    }
    public T Stroke(string color)
    {
        SvgElementPtr.CallVoid("stroke", new
        {
            width=1,
            color
        });
        return This;
    }
    public T Stroke(double width)
    {
        SvgElementPtr.CallVoid("stroke", new
        {
            width
        });
        return This;
    }
    public SvgRect GetBoundingBox()
    {
        return SvgElementPtr.Call<SvgRect>("bbox");
    }
    public T Size(double x, double y)
    {
        SvgElementPtr.CallVoid("size", x,y);
        return This;
    }
    public T Merge<G>(SvgObjectBase<G> other) where G : class
    {
        SvgElementPtr.CallVoid("merge",other.SvgElementPtr);
        return This;
    }
    public T MaskWith<G>(SvgObjectBase<G> other) where G : class
    {
        SvgElementPtr.CallVoid("maskWith", other.SvgElementPtr);
        return This;
    }
}
