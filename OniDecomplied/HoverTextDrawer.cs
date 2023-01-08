// Decompiled with JetBrains decompiler
// Type: HoverTextDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoverTextDrawer
{
  public HoverTextDrawer.Skin skin;
  private Vector2 currentPos;
  private Vector2 rootPos;
  private Vector2 shadowStartPos;
  private float maxShadowX;
  private bool firstShadowBar;
  private bool isShadowBarSelected;
  private int minLineHeight;
  private HoverTextDrawer.Pool<LocText> textWidgets;
  private HoverTextDrawer.Pool<Image> iconWidgets;
  private HoverTextDrawer.Pool<Image> shadowBars;
  private HoverTextDrawer.Pool<Image> selectBorders;

  public HoverTextDrawer(HoverTextDrawer.Skin skin, RectTransform parent)
  {
    this.shadowBars = new HoverTextDrawer.Pool<Image>(((Component) skin.shadowBarWidget).gameObject, parent);
    this.selectBorders = new HoverTextDrawer.Pool<Image>(((Component) skin.selectBorderWidget).gameObject, parent);
    this.textWidgets = new HoverTextDrawer.Pool<LocText>(((Component) skin.textWidget).gameObject, parent);
    this.iconWidgets = new HoverTextDrawer.Pool<Image>(((Component) skin.iconWidget).gameObject, parent);
    this.skin = skin;
  }

  public void SetEnabled(bool enabled)
  {
    this.shadowBars.SetEnabled(enabled);
    this.textWidgets.SetEnabled(enabled);
    this.iconWidgets.SetEnabled(enabled);
    this.selectBorders.SetEnabled(enabled);
  }

  public void BeginDrawing(Vector2 root_pos)
  {
    this.rootPos = Vector2.op_Addition(root_pos, this.skin.baseOffset);
    if (this.skin.enableDebugOffset)
      this.rootPos = Vector2.op_Addition(this.rootPos, this.skin.debugOffset);
    this.currentPos = this.rootPos;
    this.textWidgets.BeginDrawing();
    this.iconWidgets.BeginDrawing();
    this.shadowBars.BeginDrawing();
    this.selectBorders.BeginDrawing();
    this.firstShadowBar = true;
    this.minLineHeight = 0;
  }

  public void EndDrawing()
  {
    this.shadowBars.EndDrawing();
    this.iconWidgets.EndDrawing();
    this.textWidgets.EndDrawing();
    this.selectBorders.EndDrawing();
  }

  public void DrawText(string text, TextStyleSetting style, Color color, bool override_color = true)
  {
    if (!this.skin.drawWidgets)
      return;
    LocText widget = this.textWidgets.Draw(this.currentPos).widget;
    Color color1 = Color.white;
    if (Object.op_Inequality((Object) widget.textStyleSetting, (Object) style))
    {
      widget.textStyleSetting = style;
      widget.ApplySettings();
    }
    if (Object.op_Inequality((Object) style, (Object) null))
      color1 = style.textColor;
    if (override_color)
      color1 = color;
    ((Graphic) widget).color = color1;
    if (((TMP_Text) widget).text != text)
    {
      ((TMP_Text) widget).text = text;
      widget.KForceUpdateDirty();
    }
    this.currentPos.x += ((TMP_Text) widget).renderedWidth;
    this.maxShadowX = Mathf.Max(this.currentPos.x, this.maxShadowX);
    this.minLineHeight = (int) Mathf.Max((float) this.minLineHeight, ((TMP_Text) widget).renderedHeight);
  }

  public void DrawText(string text, TextStyleSetting style) => this.DrawText(text, style, Color.white, false);

  public void AddIndent(int width = 36)
  {
    if (!this.skin.drawWidgets)
      return;
    this.currentPos.x += (float) width;
  }

  public void NewLine(int min_height = 26)
  {
    if (!this.skin.drawWidgets)
      return;
    this.currentPos.y -= (float) Math.Max(min_height, this.minLineHeight);
    this.currentPos.x = this.rootPos.x;
    this.minLineHeight = 0;
  }

  public void DrawIcon(Sprite icon, int min_width = 18) => this.DrawIcon(icon, Color.white, min_width);

  public void DrawIcon(Sprite icon, Color color, int image_size = 18, int horizontal_spacing = 2)
  {
    if (!this.skin.drawWidgets)
      return;
    this.AddIndent(horizontal_spacing);
    HoverTextDrawer.Pool<Image>.Entry entry1 = this.iconWidgets.Draw(Vector2.op_Addition(this.currentPos, this.skin.shadowImageOffset));
    entry1.widget.sprite = icon;
    ((Graphic) entry1.widget).color = this.skin.shadowImageColor;
    entry1.rect.sizeDelta = new Vector2((float) image_size, (float) image_size);
    HoverTextDrawer.Pool<Image>.Entry entry2 = this.iconWidgets.Draw(this.currentPos);
    entry2.widget.sprite = icon;
    ((Graphic) entry2.widget).color = color;
    entry2.rect.sizeDelta = new Vector2((float) image_size, (float) image_size);
    this.AddIndent(horizontal_spacing);
    this.currentPos.x += (float) image_size;
    this.maxShadowX = Mathf.Max(this.currentPos.x, this.maxShadowX);
  }

  public void BeginShadowBar(bool selected = false)
  {
    if (!this.skin.drawWidgets)
      return;
    if (this.firstShadowBar)
      this.firstShadowBar = false;
    else
      this.NewLine(22);
    this.isShadowBarSelected = selected;
    this.shadowStartPos = this.currentPos;
    this.maxShadowX = this.rootPos.x;
  }

  public void EndShadowBar()
  {
    if (!this.skin.drawWidgets)
      return;
    this.NewLine(22);
    HoverTextDrawer.Pool<Image>.Entry entry1 = this.shadowBars.Draw(this.currentPos);
    entry1.rect.anchoredPosition = Vector2.op_Addition(this.shadowStartPos, new Vector2(-this.skin.shadowBarBorder.x, this.skin.shadowBarBorder.y));
    entry1.rect.sizeDelta = new Vector2((float) ((double) this.maxShadowX - (double) this.rootPos.x + (double) this.skin.shadowBarBorder.x * 2.0), (float) ((double) this.shadowStartPos.y - (double) this.currentPos.y + (double) this.skin.shadowBarBorder.y * 2.0));
    if (!this.isShadowBarSelected)
      return;
    HoverTextDrawer.Pool<Image>.Entry entry2 = this.selectBorders.Draw(this.currentPos);
    entry2.rect.anchoredPosition = Vector2.op_Addition(this.shadowStartPos, new Vector2(-this.skin.shadowBarBorder.x - this.skin.selectBorder.x, this.skin.shadowBarBorder.y + this.skin.selectBorder.y));
    entry2.rect.sizeDelta = new Vector2((float) ((double) this.maxShadowX - (double) this.rootPos.x + (double) this.skin.shadowBarBorder.x * 2.0 + (double) this.skin.selectBorder.x * 2.0), (float) ((double) this.shadowStartPos.y - (double) this.currentPos.y + (double) this.skin.shadowBarBorder.y * 2.0 + (double) this.skin.selectBorder.y * 2.0));
  }

  public void Cleanup()
  {
    this.shadowBars.Cleanup();
    this.textWidgets.Cleanup();
    this.iconWidgets.Cleanup();
  }

  [Serializable]
  public class Skin
  {
    public Vector2 baseOffset;
    public LocText textWidget;
    public Image iconWidget;
    public Vector2 shadowImageOffset;
    public Color shadowImageColor;
    public Image shadowBarWidget;
    public Image selectBorderWidget;
    public Vector2 shadowBarBorder;
    public Vector2 selectBorder;
    public bool drawWidgets;
    public bool enableProfiling;
    public bool enableDebugOffset;
    public bool drawInProgressHoverText;
    public Vector2 debugOffset;
  }

  private class Pool<WidgetType> where WidgetType : MonoBehaviour
  {
    private GameObject prefab;
    private RectTransform root;
    private List<HoverTextDrawer.Pool<WidgetType>.Entry> entries = new List<HoverTextDrawer.Pool<WidgetType>.Entry>();
    private int drawnWidgets;

    public Pool(GameObject prefab, RectTransform master_root)
    {
      this.prefab = prefab;
      GameObject gameObject = new GameObject(typeof (WidgetType).Name);
      this.root = gameObject.AddComponent<RectTransform>();
      ((Transform) this.root).SetParent((Transform) master_root);
      this.root.anchoredPosition = Vector2.zero;
      this.root.anchorMin = Vector2.zero;
      this.root.anchorMax = Vector2.one;
      this.root.sizeDelta = Vector2.zero;
      gameObject.AddComponent<CanvasGroup>();
    }

    public HoverTextDrawer.Pool<WidgetType>.Entry Draw(Vector2 pos)
    {
      HoverTextDrawer.Pool<WidgetType>.Entry entry;
      if (this.drawnWidgets < this.entries.Count)
      {
        entry = this.entries[this.drawnWidgets];
        if (!((Component) (object) entry.widget).gameObject.activeSelf)
          ((Component) (object) entry.widget).gameObject.SetActive(true);
      }
      else
      {
        GameObject gameObject = Util.KInstantiateUI(this.prefab, ((Component) this.root).gameObject, false);
        gameObject.SetActive(true);
        entry.widget = gameObject.GetComponent<WidgetType>();
        entry.rect = gameObject.GetComponent<RectTransform>();
        this.entries.Add(entry);
      }
      entry.rect.anchoredPosition = new Vector2(pos.x, pos.y);
      ++this.drawnWidgets;
      return entry;
    }

    public void BeginDrawing() => this.drawnWidgets = 0;

    public void EndDrawing()
    {
      for (int drawnWidgets = this.drawnWidgets; drawnWidgets < this.entries.Count; ++drawnWidgets)
      {
        if (((Component) (object) this.entries[drawnWidgets].widget).gameObject.activeSelf)
          ((Component) (object) this.entries[drawnWidgets].widget).gameObject.SetActive(false);
      }
    }

    public void SetEnabled(bool enabled)
    {
      if (enabled)
        ((Component) this.root).gameObject.GetComponent<CanvasGroup>().alpha = 1f;
      else
        ((Component) this.root).gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    public void Cleanup()
    {
      foreach (HoverTextDrawer.Pool<WidgetType>.Entry entry in this.entries)
        Object.Destroy((Object) ((Component) (object) entry.widget).gameObject);
      this.entries.Clear();
    }

    public struct Entry
    {
      public WidgetType widget;
      public RectTransform rect;
    }
  }
}
