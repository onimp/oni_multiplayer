// Decompiled with JetBrains decompiler
// Type: SuitDiseaseHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SuitDiseaseHandler")]
public class SuitDiseaseHandler : KMonoBehaviour
{
  private static readonly EventSystem.IntraObjectHandler<SuitDiseaseHandler> OnEquippedDelegate = new EventSystem.IntraObjectHandler<SuitDiseaseHandler>((Action<SuitDiseaseHandler, object>) ((component, data) => component.OnEquipped(data)));
  private static readonly EventSystem.IntraObjectHandler<SuitDiseaseHandler> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<SuitDiseaseHandler>((Action<SuitDiseaseHandler, object>) ((component, data) => component.OnUnequipped(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<SuitDiseaseHandler>(-1617557748, SuitDiseaseHandler.OnEquippedDelegate);
    this.Subscribe<SuitDiseaseHandler>(-170173755, SuitDiseaseHandler.OnUnequippedDelegate);
  }

  private PrimaryElement GetPrimaryElement(object data)
  {
    GameObject targetGameObject = ((Component) data).GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
    return Object.op_Implicit((Object) targetGameObject) ? targetGameObject.GetComponent<PrimaryElement>() : (PrimaryElement) null;
  }

  private void OnEquipped(object data)
  {
    PrimaryElement primaryElement = this.GetPrimaryElement(data);
    if (!Object.op_Inequality((Object) primaryElement, (Object) null))
      return;
    primaryElement.ForcePermanentDiseaseContainer(true);
    primaryElement.RedirectDisease(((Component) this).gameObject);
  }

  private void OnUnequipped(object data)
  {
    PrimaryElement primaryElement = this.GetPrimaryElement(data);
    if (!Object.op_Inequality((Object) primaryElement, (Object) null))
      return;
    primaryElement.ForcePermanentDiseaseContainer(false);
    primaryElement.RedirectDisease((GameObject) null);
  }

  private void OnModifyDiseaseCount(int delta, string reason) => ((Component) this).GetComponent<PrimaryElement>().ModifyDiseaseCount(delta, reason);

  private void OnAddDisease(byte disease_idx, int delta, string reason) => ((Component) this).GetComponent<PrimaryElement>().AddDisease(disease_idx, delta, reason);
}
