using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;

namespace UIPlugin;

class ProxyImage
{
    Image target;
    [MoonSharpHidden]
    public ProxyImage(Image t)
    {
        target = t;
    }
    public void set_sprite(string v)
    {
        if (LuaManager.Sprites.ContainsKey(v))
        {
            target.sprite = LuaManager.Sprites[v];
        }
    }
    public Transform get_transform() { return target.GetComponent<Transform>(); }
    public void set_color(float r, float g, float b, float a)
    {
        target.color = new Color(r, g, b, a);
    }
    public void set_fill(float amt)
    {
        target.fillAmount = amt;
    }
    public void set_sliced()
    {
        target.type = Image.Type.Sliced;
    }
    public void set_method(int method)
    {
        //hori
        //vert
        //radial 90
        //radial 180
        //radial 360
        target.fillMethod = (Image.FillMethod)method;
    }
}