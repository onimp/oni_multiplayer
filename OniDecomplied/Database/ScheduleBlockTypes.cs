// Decompiled with JetBrains decompiler
// Type: Database.ScheduleBlockTypes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public class ScheduleBlockTypes : ResourceSet<ScheduleBlockType>
  {
    public ScheduleBlockType Sleep;
    public ScheduleBlockType Eat;
    public ScheduleBlockType Work;
    public ScheduleBlockType Hygiene;
    public ScheduleBlockType Recreation;

    public ScheduleBlockTypes(ResourceSet parent)
      : base(nameof (ScheduleBlockTypes), parent)
    {
      this.Sleep = this.Add(new ScheduleBlockType(nameof (Sleep), (ResourceSet) this, (string) UI.SCHEDULEBLOCKTYPES.SLEEP.NAME, (string) UI.SCHEDULEBLOCKTYPES.SLEEP.DESCRIPTION, new Color(0.9843137f, 0.992156863f, 0.270588249f)));
      this.Eat = this.Add(new ScheduleBlockType(nameof (Eat), (ResourceSet) this, (string) UI.SCHEDULEBLOCKTYPES.EAT.NAME, (string) UI.SCHEDULEBLOCKTYPES.EAT.DESCRIPTION, new Color(0.807843149f, 0.5294118f, 0.113725491f)));
      this.Work = this.Add(new ScheduleBlockType(nameof (Work), (ResourceSet) this, (string) UI.SCHEDULEBLOCKTYPES.WORK.NAME, (string) UI.SCHEDULEBLOCKTYPES.WORK.DESCRIPTION, new Color(0.9372549f, 0.129411772f, 0.129411772f)));
      this.Hygiene = this.Add(new ScheduleBlockType(nameof (Hygiene), (ResourceSet) this, (string) UI.SCHEDULEBLOCKTYPES.HYGIENE.NAME, (string) UI.SCHEDULEBLOCKTYPES.HYGIENE.DESCRIPTION, new Color(0.458823532f, 0.1764706f, 0.345098048f)));
      this.Recreation = this.Add(new ScheduleBlockType(nameof (Recreation), (ResourceSet) this, (string) UI.SCHEDULEBLOCKTYPES.RECREATION.NAME, (string) UI.SCHEDULEBLOCKTYPES.RECREATION.DESCRIPTION, new Color(0.458823532f, 0.372549027f, 0.1882353f)));
    }
  }
}
