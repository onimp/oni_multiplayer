// Decompiled with JetBrains decompiler
// Type: KleiItemsStatusRefresher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public static class KleiItemsStatusRefresher
{
  public static bool Active = false;
  public const double SECONDS_PER_MINUTE = 60.0;
  public const double MINIMUM_SECONDS_BETWEEN_REFRESH_REQUESTS = 360.0;
  public static double realtimeOfLastServerRequest = -720.0;
  public static HashSet<KleiItemsStatusRefresher.UIListener> listeners = new HashSet<KleiItemsStatusRefresher.UIListener>();

  [RuntimeInitializeOnLoadMethod]
  private static void Initialize() => KleiItems.AddInventoryRefreshCallback(new KleiItems.InventoryRefreshCallback((object) null, __methodptr(OnRefreshResponseFromServer)));

  public static void RequestRefreshFromServer()
  {
    if (!KleiItemsStatusRefresher.Active)
      return;
    double sinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
    if (sinceStartupAsDouble - KleiItemsStatusRefresher.realtimeOfLastServerRequest < 360.0)
      return;
    KleiItems.AddRequestInventoryRefresh();
    KleiItemsStatusRefresher.realtimeOfLastServerRequest = sinceStartupAsDouble;
  }

  private static void OnRefreshResponseFromServer()
  {
    KleiItemsStatusRefresher.Active = false;
    KleiItemsStatusRefresher.RefreshUI();
  }

  public static void RefreshUI()
  {
    foreach (KleiItemsStatusRefresher.UIListener listener in KleiItemsStatusRefresher.listeners)
      listener.Internal_RefreshUI();
  }

  public static KleiItemsStatusRefresher.UIListener AddOrGetListener(Component component) => KleiItemsStatusRefresher.AddOrGetListener(component.gameObject);

  public static KleiItemsStatusRefresher.UIListener AddOrGetListener(GameObject onGameObject) => onGameObject.AddOrGet<KleiItemsStatusRefresher.UIListener>();

  public class UIListener : MonoBehaviour
  {
    private System.Action refreshUIFn;

    public void Internal_RefreshUI()
    {
      if (this.refreshUIFn == null)
        return;
      this.refreshUIFn();
    }

    public void OnRefreshUI(System.Action fn) => this.refreshUIFn = fn;

    private void OnEnable() => KleiItemsStatusRefresher.listeners.Add(this);

    private void OnDisable() => KleiItemsStatusRefresher.listeners.Remove(this);
  }
}
