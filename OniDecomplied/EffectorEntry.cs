// Decompiled with JetBrains decompiler
// Type: EffectorEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

internal struct EffectorEntry
{
  public string name;
  public int count;
  public float value;

  public EffectorEntry(string name, float value)
  {
    this.name = name;
    this.value = value;
    this.count = 1;
  }

  public override string ToString()
  {
    string str = "";
    if (this.count > 1)
      str = string.Format((string) UI.OVERLAYS.DECOR.COUNT, (object) this.count);
    return string.Format((string) UI.OVERLAYS.DECOR.ENTRY, (object) GameUtil.GetFormattedDecor(this.value), (object) this.name, (object) str);
  }
}
