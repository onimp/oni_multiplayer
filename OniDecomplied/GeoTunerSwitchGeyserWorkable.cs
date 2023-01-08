// Decompiled with JetBrains decompiler
// Type: GeoTunerSwitchGeyserWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class GeoTunerSwitchGeyserWorkable : Workable
{
  private const string animName = "anim_use_remote_kanim";

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_use_remote_kanim"))
    };
    this.faceTargetWhenWorking = true;
    this.synchronizeAnims = false;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.SetWorkTime(3f);
  }
}
