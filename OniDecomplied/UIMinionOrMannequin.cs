// Decompiled with JetBrains decompiler
// Type: UIMinionOrMannequin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinionOrMannequin : KMonoBehaviour
{
  private UIMinion minion;
  private UIMannequin mannequin;

  public UIMinionOrMannequin.ITarget current { get; private set; }

  protected virtual void OnSpawn() => this.TrySpawn();

  public bool TrySpawn()
  {
    bool flag = false;
    if (Util.IsNullOrDestroyed((object) this.mannequin))
    {
      GameObject go = new GameObject("UIMannequin");
      go.AddOrGet<RectTransform>().Fill(Padding.All(10f));
      go.transform.SetParent(this.transform, false);
      AspectRatioFitter aspectRatioFitter = go.AddOrGet<AspectRatioFitter>();
      aspectRatioFitter.aspectMode = (AspectRatioFitter.AspectMode) 2;
      aspectRatioFitter.aspectRatio = 1f;
      this.mannequin = go.AddOrGet<UIMannequin>();
      this.mannequin.TrySpawn();
      go.SetActive(false);
      flag = true;
    }
    if (Util.IsNullOrDestroyed((object) this.minion))
    {
      GameObject go = new GameObject("UIMinion");
      go.AddOrGet<RectTransform>().Fill(Padding.All(10f));
      go.transform.SetParent(this.transform, false);
      AspectRatioFitter aspectRatioFitter = go.AddOrGet<AspectRatioFitter>();
      aspectRatioFitter.aspectMode = (AspectRatioFitter.AspectMode) 2;
      aspectRatioFitter.aspectRatio = 1f;
      this.minion = go.AddOrGet<UIMinion>();
      this.minion.TrySpawn();
      go.SetActive(false);
      flag = true;
    }
    if (flag)
      this.SetAsMannequin();
    return flag;
  }

  public UIMinionOrMannequin.ITarget SetFrom(Option<Personality> personality) => personality.HasValue ? (UIMinionOrMannequin.ITarget) this.SetAsMinion((Personality) personality) : (UIMinionOrMannequin.ITarget) this.SetAsMannequin();

  public UIMinion SetAsMinion(Personality personality)
  {
    ((Component) this.mannequin).gameObject.SetActive(false);
    ((Component) this.minion).gameObject.SetActive(true);
    this.minion.SetMinion(personality);
    this.current = (UIMinionOrMannequin.ITarget) this.minion;
    return this.minion;
  }

  public UIMannequin SetAsMannequin()
  {
    ((Component) this.minion).gameObject.SetActive(false);
    ((Component) this.mannequin).gameObject.SetActive(true);
    this.current = (UIMinionOrMannequin.ITarget) this.mannequin;
    return this.mannequin;
  }

  public interface ITarget
  {
    GameObject SpawnedAvatar { get; }

    Option<Personality> Personality { get; }

    void SetOutfit(IEnumerable<ClothingItemResource> clothingItems);

    void React(UIMinionOrMannequinReactSource source);
  }
}
