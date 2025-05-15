//
//  c# will compain about snake_case usage, but functions / vars in here are for lua which does usually use snake_case
//
#pragma warning disable IDE1006

using MoonSharp.Interpreter;
using UnityEngine;
using TMPro;

namespace UIPlugin;

class ProxyTestMeshProUGUI
{
    TextMeshProUGUI target;
    [MoonSharpHidden]
    public ProxyTestMeshProUGUI(TextMeshProUGUI t)
    {
        target = t;
    }

    //
    // Get transform
    //
    public Transform get_transform() { return target.GetComponent<Transform>(); }

    //
    //  Set TMPro properties
    //
    public string get_text() { return target.text; }
    public void set_text(string v) { target.text = v; }
    public void set_font_size(float v) { target.fontSize = v; }
    public void set_outline(float r, float g, float b, float a, float thickness, float dilation)
    {
        target.outlineColor = new Color(r, g, b, a);
        target.outlineWidth = thickness;
        target.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilation);
    }
    public void set_colour(float r, float g, float b, float a)
    {
        target.color = new Color(r, g, b, a);
    }
}