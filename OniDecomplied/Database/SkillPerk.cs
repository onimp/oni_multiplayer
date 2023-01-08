// Decompiled with JetBrains decompiler
// Type: Database.SkillPerk
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

namespace Database
{
  public class SkillPerk : Resource
  {
    public Action<MinionResume> OnApply { get; protected set; }

    public Action<MinionResume> OnRemove { get; protected set; }

    public Action<MinionResume> OnMinionsChanged { get; protected set; }

    public bool affectAll { get; protected set; }

    public SkillPerk(
      string id_str,
      string description,
      Action<MinionResume> OnApply,
      Action<MinionResume> OnRemove,
      Action<MinionResume> OnMinionsChanged,
      bool affectAll = false)
      : base(id_str, description)
    {
      this.OnApply = OnApply;
      this.OnRemove = OnRemove;
      this.OnMinionsChanged = OnMinionsChanged;
      this.affectAll = affectAll;
    }
  }
}
