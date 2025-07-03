using System;
using UnityEngine;
using Verse;

namespace SyrTraitValue;

public class Dialog_ColorPicker : Window
{
    private readonly Action<Color> SetColor;
    private readonly Texture2D[] textures = new Texture2D[6];

    private Color color;

    private float hue;

    private float sat;

    private string tempStr;

    private float value;

    public Dialog_ColorPicker(Color initColor, Action<Color> setColor)
    {
        color = initColor;
        doCloseButton = true;
        SetColor = setColor;
        syncColor();
    }

    public override Vector2 InitialSize => new(350f, 590f);

    public override void PostClose()
    {
        SetColor(color);
        base.PostClose();
    }

    public override void Close(bool doCloseSound = true)
    {
        SetColor(color);
        base.Close(doCloseSound);
    }

    private void generateTextures()
    {
        tempStr =
            $"R {color.r.ToStringByStyle(ToStringStyle.FloatTwo)} | G {color.g.ToStringByStyle(ToStringStyle.FloatTwo)} | B {color.b.ToStringByStyle(ToStringStyle.FloatTwo)}";
        var texture2D = new Texture2D(256, 256);
        for (var i = 0; i <= 256; i++)
        {
            for (var j = 0; j <= 256; j++)
            {
                texture2D.SetPixel(i, j, Color.HSVToRGB(i / 256f, j / 256f, value));
            }
        }

        texture2D.Apply();
        textures[0] = texture2D;
        texture2D = new Texture2D(15, 256);
        for (var k = 0; k <= 15; k++)
        {
            for (var l = 0; l <= 256; l++)
            {
                texture2D.SetPixel(k, l, Color.HSVToRGB(hue, sat, l / 256f));
            }
        }

        texture2D.Apply();
        textures[1] = texture2D;
        texture2D = new Texture2D(256, 16);
        for (var m = 0; m <= 15; m++)
        {
            for (var n = 0; n <= 256; n++)
            {
                texture2D.SetPixel(n, m, new Color(n / 256f, color.g, color.b));
            }
        }

        texture2D.Apply();
        textures[2] = texture2D;
        texture2D = new Texture2D(256, 16);
        for (var num = 0; num <= 15; num++)
        {
            for (var num2 = 0; num2 <= 256; num2++)
            {
                texture2D.SetPixel(num2, num, new Color(color.r, num2 / 256f, color.b));
            }
        }

        texture2D.Apply();
        textures[3] = texture2D;
        texture2D = new Texture2D(256, 16);
        for (var num3 = 0; num3 <= 15; num3++)
        {
            for (var num4 = 0; num4 <= 256; num4++)
            {
                texture2D.SetPixel(num4, num3, new Color(color.r, color.g, num4 / 256f));
            }
        }

        texture2D.Apply();
        textures[4] = texture2D;
        texture2D = new Texture2D(256, 16);
        for (var num5 = 0; num5 <= 15; num5++)
        {
            for (var num6 = 0; num6 <= 256; num6++)
            {
                texture2D.SetPixel(num6, num5, color);
            }
        }

        texture2D.Apply();
        textures[5] = texture2D;
    }

    public override void DoWindowContents(Rect inRect)
    {
        var rect = inRect.ContractedBy(10f);
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(new Rect(rect.x, rect.y, 256f, 30f), "SyrTraitValue_ColorPicker".Translate());
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;
        rect.y += 35f;
        var rect2 = new Rect(rect.x, rect.y, 256f, 256f);
        GUI.DrawTexture(rect2, textures[0], ScaleMode.StretchToFill);
        if (Mouse.IsOver(rect2) && Event.current.isMouse && Event.current.button == 0)
        {
            var mousePosition = Event.current.mousePosition;
            hue = GenMath.RoundTo((mousePosition.x - rect2.x) / 256f, 0.01f);
            sat = GenMath.RoundTo((rect2.yMax - mousePosition.y) / 256f, 0.01f);
            color = Color.HSVToRGB(hue, sat, value);
            generateTextures();
        }

        var rect3 = new Rect(rect2.xMax + 5f, rect.y, 15f, 256f);
        GUI.DrawTexture(rect3, textures[1], ScaleMode.StretchToFill);
        if (Mouse.IsOver(rect3) && Event.current.isMouse && Event.current.button == 0)
        {
            value = GenMath.RoundTo((rect3.yMax - Event.current.mousePosition.y) / 256f, 0.01f);
            color = Color.HSVToRGB(hue, sat, value);
            generateTextures();
        }

        rect.y += 266f;
        rect3 = rect.TopPartPixels(30f);
        rect3.width *= 0.7f;
        GUI.color = Color.grey;
        GUI.enabled = false;
        tempStr = Widgets.TextField(rect3, tempStr);
        GUI.color = Color.white;
        GUI.enabled = true;
        rect3.x = rect3.xMax;
        rect.y += 40f;
        rect3 = new Rect(rect.x + 55f, rect.y, 256f, 15f);
        Widgets.Label(new Rect(rect.x, rect.y, 55f, 25f), "SyrTraitValue_Red".Translate());
        GUI.DrawTexture(rect3, textures[2], ScaleMode.StretchToFill);
        if (Mouse.IsOver(rect3) && Event.current.isMouse && Event.current.button == 0)
        {
            Log.Message(
                $"Mouse: {Event.current.mousePosition.x}, Rect: {rect3.x}, Sub: {Event.current.mousePosition.x - rect3.x}, r will be {(Event.current.mousePosition.x - rect3.x) / 256f}");
            color.r = GenMath.RoundTo((Event.current.mousePosition.x - rect3.x) / 256f, 0.01f);
            syncColor();
        }

        rect.y += 40f;
        rect3 = new Rect(rect.x + 55f, rect.y, 256f, 15f);
        Widgets.Label(new Rect(rect.x, rect.y, 55f, 25f), "SyrTraitValue_Green".Translate());
        GUI.DrawTexture(rect3, textures[3], ScaleMode.StretchToFill);
        if (Mouse.IsOver(rect3) && Event.current.isMouse && Event.current.button == 0)
        {
            color.g = GenMath.RoundTo((Event.current.mousePosition.x - rect3.x) / 256f, 0.01f);
            syncColor();
        }

        rect.y += 40f;
        rect3 = new Rect(rect.x + 55f, rect.y, 256f, 15f);
        Widgets.Label(new Rect(rect.x, rect.y, 55f, 25f), "SyrTraitValue_Blue".Translate());
        GUI.DrawTexture(rect3, textures[4], ScaleMode.StretchToFill);
        if (!Mouse.IsOver(rect3) || !Event.current.isMouse || Event.current.button != 0)
        {
            return;
        }

        color.b = GenMath.RoundTo((Event.current.mousePosition.x - rect3.x) / 256f, 0.01f);
        syncColor();
    }

    private void syncColor()
    {
        Color.RGBToHSV(color, out hue, out sat, out value);
        generateTextures();
    }
}