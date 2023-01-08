// Decompiled with JetBrains decompiler
// Type: StarmapPlanetVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/StarmapPlanetVisualizer")]
public class StarmapPlanetVisualizer : KMonoBehaviour
{
  public Image image;
  public LocText label;
  public MultiToggle button;
  public RectTransform selection;
  public GameObject analysisSelection;
  public Image unknownBG;
  public GameObject rocketIconContainer;
}
