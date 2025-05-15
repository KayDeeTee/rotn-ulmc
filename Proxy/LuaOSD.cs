using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UIPlugin;

public class LuaOSDMessage
{
    public enum MessageLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public MessageLevel level;
    public float duration;
    public string message;
    public int count = 1;

    public LuaOSDMessage(MessageLevel mLevel, string messageString, float messageDuration)
    {
        level = mLevel;
        message = messageString;
        duration = messageDuration;
    }

    public string GetStringFormatted()
    {
        string levelString = "";
        Color messageCol = new Color(1, 1, 1, 1);
        switch (level)
        {
            case MessageLevel.Debug:
                levelString = "Debug";
                messageCol = new Color(1, 1, 1, 1);
                break;
            case MessageLevel.Info:
                levelString = "Info";
                messageCol = new Color(0.8f, 0.8f, 0.8f, 1);
                break;
            case MessageLevel.Warn:
                levelString = "Warn";
                messageCol = new Color(1.0f, 0.5f, 0.0f, 1);
                break;
            case MessageLevel.Error:
                levelString = "Error";
                messageCol = new Color(1.0f, 0.0f, 0.0f, 1);
                break;
            case MessageLevel.Fatal:
                levelString = "Fatal";
                messageCol = new Color(0.6f, 0.0f, 0.0f, 1);
                break;
                
        }
        float alpha = 1;
        if (duration > 0 && duration < 1)
        {
            alpha = duration / 1.0f;
        }
        messageCol.a = alpha;
        string col = ColorUtility.ToHtmlStringRGBA(messageCol);
        string counter = "";
        if( count > 1 ){ counter = string.Format(" (x{0})", count); }
        return string.Format("<color=#{0}>[{1}] {2}{3}</color>", col, levelString, message, counter);
    }
}

public class LuaOSD
{
    public LuaOSD(Transform parent)
    {
        RectTransform screen = (RectTransform)parent.Find("RhythmRiftCanvas/ScreenContainer");

        GameObject cont = new GameObject("LuaOSD", typeof(RectTransform));
        cont.transform.SetParent(screen, false);

        RectTransform cont_transform = (RectTransform)cont.transform;
        cont_transform.anchorMin = new Vector2(0, 0);
        cont_transform.anchorMax = new Vector2(1, 1);
        cont_transform.offsetMax = new Vector2(0, 0);
        cont_transform.offsetMin = new Vector2(0, 0);
        cont_transform.sizeDelta = new Vector2(0, 0);

        //create text
        GameObject text = new GameObject("LuaOSDText", typeof(RectTransform), typeof(CanvasRenderer));
        text.transform.SetParent(cont.transform, false);
        TextObj = text.gameObject.AddComponent<TextMeshProUGUI>();
        TextObj.font = RRStageControllerPatch.instance._stageUIView._scoreText.font;
        TextObj.fontSize = 32;

        RectTransform text_transform = (RectTransform)text.transform;
        text_transform.anchorMin = new Vector2(0.05f, 0.15f);
        text_transform.anchorMax = new Vector2(0.95f, 0.85f);
        text_transform.offsetMax = new Vector2(0, 0);
        text_transform.offsetMin = new Vector2(0, 0);
        text_transform.sizeDelta = new Vector2(0, 0);

        TextObj.enableWordWrapping = true;
        TextObj.overflowMode = TextOverflowModes.Overflow;
        TextObj.outlineColor = Color.black;
        TextObj.outlineWidth = 0.5f;
        TextObj.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0.5f);

        TextObj.text = "";

        Messages = new List<LuaOSDMessage>();
    }

    public TextMeshProUGUI TextObj;
    public List<LuaOSDMessage> Messages;
    public bool requiresReordering = false;
    public void AddMessage(LuaOSDMessage.MessageLevel messageLevel, string message, float duration)
    {
        message = message.Replace(RRStageControllerPatch.LuaPath, "UI"); //Remove path before UI/*.lua
        foreach (LuaOSDMessage oSDMessage in Messages)
        {
            if (oSDMessage.message == message)
            {
                oSDMessage.duration = duration;
                oSDMessage.count += 1;
                return;
            }
        }
        Messages.Add(new LuaOSDMessage(messageLevel, message, duration));
        requiresReordering = true;
    }
    public void Update(float deltaTime)
    {
        List<LuaOSDMessage> toRemove = new List<LuaOSDMessage>();
        foreach (LuaOSDMessage message in Messages)
        {
            if (message.duration == -1) continue;
            message.duration -= deltaTime;
            if (message.duration <= 0) toRemove.Add(message);
        }
        foreach (LuaOSDMessage message in toRemove)
        {
            Messages.Remove(message);
        }
        if (requiresReordering) { Messages.Sort((x, y) => x.duration.CompareTo(y.duration)); }

        string osdText = "";
        foreach (LuaOSDMessage message in Messages)
        {
            string m = message.GetStringFormatted();
            osdText += m + "\n";
        }
        TextObj.text = osdText;
    }
}