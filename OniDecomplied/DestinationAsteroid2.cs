// Decompiled with JetBrains decompiler
// Type: DestinationAsteroid2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/DestinationAsteroid2")]
public class DestinationAsteroid2 : KMonoBehaviour
{
  [SerializeField]
  private Image asteroidImage;
  [SerializeField]
  private KButton button;
  [SerializeField]
  private KBatchedAnimController animController;
  private ColonyDestinationAsteroidBeltData asteroidData;

  public event Action<ColonyDestinationAsteroidBeltData> OnClicked;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.button.onClick += new System.Action(this.OnClickInternal);
  }

  public void SetAsteroid(ColonyDestinationAsteroidBeltData newAsteroidData)
  {
    if (this.asteroidData != null && !(newAsteroidData.beltPath != this.asteroidData.beltPath))
      return;
    this.asteroidData = newAsteroidData;
    World getStartWorld = newAsteroidData.GetStartWorld;
    KAnimFile anim;
    Assets.TryGetAnim(HashedString.op_Implicit(Util.IsNullOrWhiteSpace(getStartWorld.asteroidIcon) ? AsteroidGridEntity.DEFAULT_ASTEROID_ICON_ANIM : getStartWorld.asteroidIcon), out anim);
    if (DlcManager.FeatureClusterSpaceEnabled() && Object.op_Inequality((Object) anim, (Object) null))
    {
      ((Component) this.asteroidImage).gameObject.SetActive(false);
      this.animController.AnimFiles = new KAnimFile[1]
      {
        anim
      };
      this.animController.initialMode = (KAnim.PlayMode) 0;
      this.animController.initialAnim = "idle_loop";
      ((Component) this.animController).gameObject.SetActive(true);
      if (!this.animController.HasAnimation(HashedString.op_Implicit(this.animController.initialAnim)))
        return;
      this.animController.Play(HashedString.op_Implicit(this.animController.initialAnim), (KAnim.PlayMode) 0);
    }
    else
    {
      ((Component) this.animController).gameObject.SetActive(false);
      ((Component) this.asteroidImage).gameObject.SetActive(true);
      this.asteroidImage.sprite = this.asteroidData.sprite;
    }
  }

  private void OnClickInternal()
  {
    DebugUtil.LogArgs(new object[2]
    {
      (object) "Clicked asteroid belt",
      (object) this.asteroidData.beltPath
    });
    this.OnClicked(this.asteroidData);
  }
}
