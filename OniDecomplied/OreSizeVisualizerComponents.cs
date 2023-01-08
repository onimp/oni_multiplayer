// Decompiled with JetBrains decompiler
// Type: OreSizeVisualizerComponents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class OreSizeVisualizerComponents : KGameObjectComponentManager<OreSizeVisualizerData>
{
  private static readonly OreSizeVisualizerComponents.MassTier[] MassTiers = new OreSizeVisualizerComponents.MassTier[3]
  {
    new OreSizeVisualizerComponents.MassTier()
    {
      animName = HashedString.op_Implicit("idle1"),
      massRequired = 50f,
      colliderRadius = 0.15f
    },
    new OreSizeVisualizerComponents.MassTier()
    {
      animName = HashedString.op_Implicit("idle2"),
      massRequired = 600f,
      colliderRadius = 0.2f
    },
    new OreSizeVisualizerComponents.MassTier()
    {
      animName = HashedString.op_Implicit("idle3"),
      massRequired = float.MaxValue,
      colliderRadius = 0.25f
    }
  };

  public HandleVector<int>.Handle Add(GameObject go)
  {
    HandleVector<int>.Handle handle = this.Add(go, new OreSizeVisualizerData(go));
    ((KComponentManager<OreSizeVisualizerData>) this).OnPrefabInit(handle);
    return handle;
  }

  protected virtual void OnPrefabInit(HandleVector<int>.Handle handle)
  {
    Action<object> action = (Action<object>) (ev_data => OreSizeVisualizerComponents.OnMassChanged(handle, ev_data));
    OreSizeVisualizerData data = ((KCompactedVector<OreSizeVisualizerData>) this).GetData(handle) with
    {
      onMassChangedCB = action
    };
    data.primaryElement.Subscribe(-2064133523, action);
    data.primaryElement.Subscribe(1335436905, action);
    ((KCompactedVector<OreSizeVisualizerData>) this).SetData(handle, data);
  }

  protected virtual void OnSpawn(HandleVector<int>.Handle handle)
  {
    OreSizeVisualizerData data = ((KCompactedVector<OreSizeVisualizerData>) this).GetData(handle);
    OreSizeVisualizerComponents.OnMassChanged(handle, (object) ((Component) data.primaryElement).GetComponent<Pickupable>());
  }

  protected virtual void OnCleanUp(HandleVector<int>.Handle handle)
  {
    OreSizeVisualizerData data = ((KCompactedVector<OreSizeVisualizerData>) this).GetData(handle);
    if (!Object.op_Inequality((Object) data.primaryElement, (Object) null))
      return;
    Action<object> onMassChangedCb = data.onMassChangedCB;
    data.primaryElement.Unsubscribe(-2064133523, onMassChangedCb);
    data.primaryElement.Unsubscribe(1335436905, onMassChangedCb);
  }

  private static void OnMassChanged(HandleVector<int>.Handle handle, object other_data)
  {
    PrimaryElement primaryElement = ((KCompactedVector<OreSizeVisualizerData>) GameComps.OreSizeVisualizers).GetData(handle).primaryElement;
    float mass = primaryElement.Mass;
    if (other_data != null)
    {
      PrimaryElement component = ((Component) other_data).GetComponent<PrimaryElement>();
      mass += component.Mass;
    }
    OreSizeVisualizerComponents.MassTier massTier = new OreSizeVisualizerComponents.MassTier();
    for (int index = 0; index < OreSizeVisualizerComponents.MassTiers.Length; ++index)
    {
      if ((double) mass <= (double) OreSizeVisualizerComponents.MassTiers[index].massRequired)
      {
        massTier = OreSizeVisualizerComponents.MassTiers[index];
        break;
      }
    }
    ((Component) primaryElement).GetComponent<KBatchedAnimController>().Play(massTier.animName);
    KCircleCollider2D component1 = ((Component) primaryElement).GetComponent<KCircleCollider2D>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      component1.radius = massTier.colliderRadius;
    primaryElement.Trigger(1807976145, (object) null);
  }

  private struct MassTier
  {
    public HashedString animName;
    public float massRequired;
    public float colliderRadius;
  }
}
