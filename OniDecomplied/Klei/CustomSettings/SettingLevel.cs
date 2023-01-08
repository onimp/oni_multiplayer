// Decompiled with JetBrains decompiler
// Type: Klei.CustomSettings.SettingLevel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Klei.CustomSettings
{
  public class SettingLevel
  {
    public SettingLevel(
      string id,
      string label,
      string tooltip,
      long coordinate_offset = 0,
      object userdata = null)
    {
      this.id = id;
      this.label = label;
      this.tooltip = tooltip;
      this.userdata = userdata;
      this.coordinate_offset = coordinate_offset;
    }

    public string id { get; private set; }

    public string tooltip { get; private set; }

    public string label { get; private set; }

    public object userdata { get; private set; }

    public long coordinate_offset { get; private set; }
  }
}
