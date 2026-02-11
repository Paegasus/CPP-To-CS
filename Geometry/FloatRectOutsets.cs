namespace UI.Geometry;

public struct FloatRectOutsets
{
    private float m_Top;
    private float m_Right;
    private float m_Bottom;
    private float m_Left;

    public float Top { readonly get => m_Top; set => m_Top = value; }
    public float Right { readonly get => m_Right; set => m_Right = value; }
    public float Bottom { readonly get => m_Bottom; set => m_Bottom = value; }
    public float Left { readonly get => m_Left; set => m_Left = value; }

    public FloatRectOutsets()
    {
        m_Top = 0;
        m_Right = 0;
        m_Bottom = 0;
        m_Left = 0;
    }

    public FloatRectOutsets(float top, float right, float bottom, float left)
    {
        m_Top = top;
        m_Right = right;
        m_Bottom = bottom;
        m_Left = left;
    }

    // Change outsets to be at least as large as other.
    public void Unite(ref FloatRectOutsets other) // C++: const FloatRectOutsets& other
    {
        m_Top = Math.Max(m_Top, other.m_Top);
        m_Right = Math.Max(m_Right, other.m_Right);
        m_Bottom = Math.Max(m_Bottom, other.m_Bottom);
        m_Left = Math.Max(m_Left, other.m_Left);
    }
}
