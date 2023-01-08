// Decompiled with JetBrains decompiler
// Type: ClusterFXEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class ClusterFXEntity : ClusterGridEntity
{
  [SerializeField]
  public string kAnimName;
  [SerializeField]
  public string animName;
  public KAnim.PlayMode animPlayMode = (KAnim.PlayMode) 1;
  public Vector3 animOffset;

  public override string Name => (string) UI.SPACEDESTINATIONS.TELESCOPE_TARGET.NAME;

  public override EntityLayer Layer => EntityLayer.FX;

  public override List<ClusterGridEntity.AnimConfig> AnimConfigs => new List<ClusterGridEntity.AnimConfig>()
  {
    new ClusterGridEntity.AnimConfig()
    {
      animFile = Assets.GetAnim(HashedString.op_Implicit(this.kAnimName)),
      initialAnim = this.animName,
      playMode = this.animPlayMode,
      animOffset = this.animOffset
    }
  };

  public override bool IsVisible => true;

  public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;

  public void Init(AxialI location, Vector3 animOffset)
  {
    this.Location = location;
    this.animOffset = animOffset;
  }
}
