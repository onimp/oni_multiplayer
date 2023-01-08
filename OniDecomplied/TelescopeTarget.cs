// Decompiled with JetBrains decompiler
// Type: TelescopeTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class TelescopeTarget : ClusterGridEntity
{
  public override string Name => (string) UI.SPACEDESTINATIONS.TELESCOPE_TARGET.NAME;

  public override EntityLayer Layer => EntityLayer.Telescope;

  public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
  {
    new ClusterGridEntity.AnimConfig()
    {
      animFile = Assets.GetAnim(HashedString.op_Implicit("telescope_target_kanim")),
      initialAnim = "idle"
    }
  };

  public override bool IsVisible => true;

  public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;

  public void Init(AxialI location) => this.Location = location;

  public override bool ShowName() => true;

  public override bool ShowProgressBar() => true;

  public override float GetProgress() => ((Component) SaveGame.Instance).GetSMI<ClusterFogOfWarManager.Instance>().GetRevealCompleteFraction(this.Location);
}
