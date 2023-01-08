// Decompiled with JetBrains decompiler
// Type: Database.Dreams
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public class Dreams : ResourceSet<Dream>
  {
    public Dream CommonDream;

    public Dreams(ResourceSet parent)
      : base(nameof (Dreams), parent)
    {
      this.CommonDream = new Dream(nameof (CommonDream), (ResourceSet) this, "dream_tear_swirly_kanim", new string[1]
      {
        "dreamIcon_journal"
      });
    }
  }
}
