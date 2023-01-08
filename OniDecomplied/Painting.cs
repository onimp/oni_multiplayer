// Decompiled with JetBrains decompiler
// Type: Painting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class Painting : Artable
{
  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.SetOffsetTable(OffsetGroups.InvertedStandardTable);
    this.multitoolContext = HashedString.op_Implicit("paint");
    this.multitoolHitEffectTag = Tag.op_Implicit("fx_paint_splash");
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    Components.Paintings.Add(this);
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Components.Paintings.Remove(this);
  }
}
