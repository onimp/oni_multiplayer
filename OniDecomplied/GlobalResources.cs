// Decompiled with JetBrains decompiler
// Type: GlobalResources
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using UnityEngine;

public class GlobalResources : ScriptableObject
{
  public Material AnimMaterial;
  public Material AnimUIMaterial;
  public Material AnimPlaceMaterial;
  public Material AnimMaterialUIDesaturated;
  public Material AnimSimpleMaterial;
  public Material AnimOverlayMaterial;
  public Texture2D WhiteTexture;
  public EventReference ConduitOverlaySoundLiquid;
  public EventReference ConduitOverlaySoundGas;
  public EventReference ConduitOverlaySoundSolid;
  public EventReference AcousticDisturbanceSound;
  public EventReference AcousticDisturbanceBubbleSound;
  public EventReference WallDamageLayerSound;
  public Sprite sadDupeAudio;
  public Sprite sadDupe;
  public Sprite baseGameLogoSmall;
  public Sprite expansion1LogoSmall;
  private static GlobalResources _Instance;

  public static GlobalResources Instance()
  {
    if (Object.op_Equality((Object) GlobalResources._Instance, (Object) null))
      GlobalResources._Instance = Resources.Load<GlobalResources>(nameof (GlobalResources));
    return GlobalResources._Instance;
  }
}
