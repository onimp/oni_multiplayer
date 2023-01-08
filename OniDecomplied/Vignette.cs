// Decompiled with JetBrains decompiler
// Type: Vignette
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class Vignette : KMonoBehaviour
{
  [SerializeField]
  private Image image;
  public Color defaultColor;
  public Color redAlertColor = new Color(1f, 0.0f, 0.0f, 0.3f);
  public Color yellowAlertColor = new Color(1f, 1f, 0.0f, 0.3f);
  public static Vignette Instance;
  private LoopingSounds looping_sounds;
  private bool showingRedAlert;
  private bool showingYellowAlert;

  public static void DestroyInstance() => Vignette.Instance = (Vignette) null;

  protected virtual void OnSpawn()
  {
    this.looping_sounds = ((Component) this).GetComponent<LoopingSounds>();
    base.OnSpawn();
    Vignette.Instance = this;
    this.defaultColor = ((Graphic) this.image).color;
    Game.Instance.Subscribe(1983128072, new Action<object>(this.Refresh));
    Game.Instance.Subscribe(1585324898, new Action<object>(this.Refresh));
    Game.Instance.Subscribe(-1393151672, new Action<object>(this.Refresh));
    Game.Instance.Subscribe(-741654735, new Action<object>(this.Refresh));
    Game.Instance.Subscribe(-2062778933, new Action<object>(this.Refresh));
  }

  public void SetColor(Color color) => ((Graphic) this.image).color = color;

  public void Refresh(object data)
  {
    AlertStateManager.Instance alertManager = ClusterManager.Instance.activeWorld.AlertManager;
    if (alertManager == null)
      return;
    if (alertManager.IsYellowAlert())
    {
      this.SetColor(this.yellowAlertColor);
      if (!this.showingYellowAlert)
      {
        this.looping_sounds.StartSound(GlobalAssets.GetSound("YellowAlert_LP"), enable_culling: false);
        this.showingYellowAlert = true;
      }
    }
    else
    {
      this.showingYellowAlert = false;
      this.looping_sounds.StopSound(GlobalAssets.GetSound("YellowAlert_LP"));
    }
    if (alertManager.IsRedAlert())
    {
      this.SetColor(this.redAlertColor);
      if (!this.showingRedAlert)
      {
        this.looping_sounds.StartSound(GlobalAssets.GetSound("RedAlert_LP"), enable_culling: false);
        this.showingRedAlert = true;
      }
    }
    else
    {
      this.showingRedAlert = false;
      this.looping_sounds.StopSound(GlobalAssets.GetSound("RedAlert_LP"));
    }
    if (this.showingRedAlert || this.showingYellowAlert)
      return;
    this.Reset();
  }

  public void Reset()
  {
    this.SetColor(this.defaultColor);
    this.showingRedAlert = false;
    this.showingYellowAlert = false;
    this.looping_sounds.StopSound(GlobalAssets.GetSound("RedAlert_LP"));
    this.looping_sounds.StopSound(GlobalAssets.GetSound("YellowAlert_LP"));
  }
}
