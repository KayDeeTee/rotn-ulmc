using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UIPlugin;

class ProxyRectTransform
{
    RectTransform target;
    [MoonSharpHidden]
    public ProxyRectTransform(RectTransform t)
    {
        target = t;
    }

    //
    // Create new components/gameObjects as a child of this gameObject
    //
    public RectTransform add_child(string name)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.transform.SetParent(target.transform, false);
        return gameObject.GetComponent<RectTransform>();
    }
    public Image add_image(string sprite)
    {
        Image img = target.gameObject.AddComponent<UnityEngine.UI.Image>();
        img.sprite = LuaManager.Sprites[sprite];
        return img;
    }
    //this gets added but no text shows up
    public TextMeshProUGUI add_tmpro()
    {
        TextMeshProUGUI tmpro = target.gameObject.AddComponent<TextMeshProUGUI>();
        tmpro.font = RRStageControllerPatch.instance._stageUIView._scoreText.font;
        tmpro.fontSize = 24;
        return tmpro;
    }

    //
    // Get other components on this gameObject
    //
    public Image get_image() { return target.GetComponent<Image>(); }
    public TextMeshProUGUI get_tmpro() { return target.GetComponent<TextMeshProUGUI>(); }

    //
    // Set rect transform properties
    //
    public Dictionary<string, float> get_anchor_position() { return LuaManager.Vec2Dict(target.anchoredPosition); }
    public void set_anchor_position(float x, float y) { target.anchoredPosition = new Vector2(x, y); }

    public Dictionary<string, float> get_anchor_max() { return LuaManager.Vec2Dict(target.anchorMax); }
    public void set_anchor_max(float x, float y) { target.anchorMax = new Vector2(x, y); }

    public Dictionary<string, float> get_anchor_min() { return LuaManager.Vec2Dict(target.anchorMin); }
    public void set_anchor_min(float x, float y) { target.anchorMin = new Vector2(x, y); }

    public Dictionary<string, float> get_offset_max() { return LuaManager.Vec2Dict(target.offsetMax); }
    public void set_offset_max(float x, float y) { target.offsetMax = new Vector2(x, y); }

    public Dictionary<string, float> get_offset_min() { return LuaManager.Vec2Dict(target.offsetMin); }
    public void set_offset_min(float x, float y) { target.offsetMin = new Vector2(x, y); }
    public Dictionary<string, float> get_size_delta() { return LuaManager.Vec2Dict(target.sizeDelta); }
    public void set_size_delta(float x, float y) { target.sizeDelta = new Vector2(x, y); }
    public Dictionary<string, float> get_rotation() { return LuaManager.Vec3Dict(target.eulerAngles); }
    public void set_rotation(float x, float y, float z) { target.eulerAngles = new Vector3(x, y, z); }

    //
    // Set gameObject properties
    //
    public void set_active(bool v) { target.gameObject.SetActive(v); }
    public void disable_layout()
    {
        LayoutElement le = target.gameObject.AddComponent<LayoutElement>();
        le.ignoreLayout = true;
    }

}