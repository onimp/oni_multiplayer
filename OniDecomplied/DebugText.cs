// Decompiled with JetBrains decompiler
// Type: DebugText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/DebugText")]
public class DebugText : KMonoBehaviour
{
  public static DebugText Instance;
  private List<DebugText.Entry> entries = new List<DebugText.Entry>();
  private List<Text> texts = new List<Text>();

  public static void DestroyInstance() => DebugText.Instance = (DebugText) null;

  protected virtual void OnPrefabInit() => DebugText.Instance = this;

  public void Draw(string text, Vector3 pos, Color color) => this.entries.Add(new DebugText.Entry()
  {
    text = text,
    pos = pos,
    color = color
  });

  private void LateUpdate()
  {
    foreach (Component text in this.texts)
      Object.Destroy((Object) text.gameObject);
    this.texts.Clear();
    foreach (DebugText.Entry entry in this.entries)
    {
      GameObject gameObject = new GameObject();
      RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
      ((Transform) rectTransform).SetParent((Transform) GameScreenManager.Instance.worldSpaceCanvas.GetComponent<RectTransform>());
      TransformExtensions.SetPosition(gameObject.transform, entry.pos);
      ((Transform) rectTransform).localScale = new Vector3(0.02f, 0.02f, 1f);
      Text text = gameObject.AddComponent<Text>();
      text.font = Assets.DebugFont;
      text.text = entry.text;
      ((Graphic) text).color = entry.color;
      text.horizontalOverflow = (HorizontalWrapMode) 1;
      text.verticalOverflow = (VerticalWrapMode) 1;
      text.alignment = (TextAnchor) 4;
      this.texts.Add(text);
    }
    this.entries.Clear();
  }

  private struct Entry
  {
    public string text;
    public Vector3 pos;
    public Color color;
  }
}
