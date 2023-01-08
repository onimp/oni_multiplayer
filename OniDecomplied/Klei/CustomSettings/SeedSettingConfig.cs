// Decompiled with JetBrains decompiler
// Type: Klei.CustomSettings.SeedSettingConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Klei.CustomSettings
{
  public class SeedSettingConfig : SettingConfig
  {
    public SeedSettingConfig(
      string id,
      string label,
      string tooltip,
      bool debug_only = false,
      bool triggers_custom_game = true)
      : base(id, label, tooltip, "", "", debug_only: debug_only, triggers_custom_game: triggers_custom_game)
    {
    }

    public override SettingLevel GetLevel(string level_id) => new SettingLevel(level_id, level_id, level_id);

    public override List<SettingLevel> GetLevels() => new List<SettingLevel>();
  }
}
