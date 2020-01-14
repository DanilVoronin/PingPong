using UnityEngine;

public static class DrawLine
{
    static Texture2D _Texture;
    static Texture2D _Texture2D
    {
        get
        {
            if (_Texture == null)
            {
                _Texture = new Texture2D(1, 1);
                _Texture.SetPixel(0, 0, Color.white);
                _Texture.Apply();
            }
            return _Texture;
        }
    }
    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, _Texture);
        GUI.color = Color.white;
    }
}
