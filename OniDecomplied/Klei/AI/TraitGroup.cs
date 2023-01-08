// Decompiled with JetBrains decompiler
// Type: Klei.AI.TraitGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Klei.AI
{
  public class TraitGroup : ModifierGroup<Trait>
  {
    public bool IsSpawnTrait;

    public TraitGroup(string id, string name, bool is_spawn_trait)
      : base(id, name)
    {
      this.IsSpawnTrait = is_spawn_trait;
    }
  }
}
