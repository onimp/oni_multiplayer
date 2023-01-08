// Decompiled with JetBrains decompiler
// Type: CrewPortrait
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/CrewPortrait")]
[Serializable]
public class CrewPortrait : KMonoBehaviour
{
  public Image targetImage;
  public bool startTransparent;
  public bool useLabels = true;
  [SerializeField]
  public KBatchedAnimController controller;
  public float animScaleBase = 0.2f;
  public LocText duplicantName;
  public LocText duplicantJob;
  public LocText subTitle;
  public bool useDefaultExpression = true;
  private bool requiresRefresh;
  private bool areEventsRegistered;

  public IAssignableIdentity identityObject { get; private set; }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (this.startTransparent)
      ((MonoBehaviour) this).StartCoroutine(this.AlphaIn());
    this.requiresRefresh = true;
  }

  private IEnumerator AlphaIn()
  {
    this.SetAlpha(0.0f);
    for (float i = 0.0f; (double) i < 1.0; i += Time.unscaledDeltaTime * 4f)
    {
      this.SetAlpha(i);
      yield return (object) 0;
    }
    this.SetAlpha(1f);
  }

  private void OnRoleChanged(object data)
  {
    if (Object.op_Equality((Object) this.controller, (Object) null))
      return;
    CrewPortrait.RefreshHat(this.identityObject, this.controller);
  }

  private void RegisterEvents()
  {
    if (this.areEventsRegistered)
      return;
    KMonoBehaviour identityObject = this.identityObject as KMonoBehaviour;
    if (Object.op_Equality((Object) identityObject, (Object) null))
      return;
    identityObject.Subscribe(540773776, new Action<object>(this.OnRoleChanged));
    this.areEventsRegistered = true;
  }

  private void UnregisterEvents()
  {
    if (!this.areEventsRegistered)
      return;
    this.areEventsRegistered = false;
    KMonoBehaviour identityObject = this.identityObject as KMonoBehaviour;
    if (Object.op_Equality((Object) identityObject, (Object) null))
      return;
    identityObject.Unsubscribe(540773776, new Action<object>(this.OnRoleChanged));
  }

  protected virtual void OnCmpEnable()
  {
    base.OnCmpEnable();
    this.RegisterEvents();
    this.ForceRefresh();
  }

  protected virtual void OnCmpDisable()
  {
    base.OnCmpDisable();
    this.UnregisterEvents();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    this.UnregisterEvents();
  }

  public void SetIdentityObject(IAssignableIdentity identity, bool jobEnabled = true)
  {
    this.UnregisterEvents();
    this.identityObject = identity;
    this.RegisterEvents();
    ((Behaviour) this.targetImage).enabled = true;
    if (this.identityObject != null)
      ((Behaviour) this.targetImage).enabled = false;
    if (this.useLabels)
    {
      switch (identity)
      {
        case MinionIdentity _:
        case MinionAssignablesProxy _:
          this.SetDuplicantJobTitleActive(jobEnabled);
          break;
      }
    }
    this.requiresRefresh = true;
  }

  public void SetSubTitle(string newTitle)
  {
    if (!Object.op_Inequality((Object) this.subTitle, (Object) null))
      return;
    if (string.IsNullOrEmpty(newTitle))
    {
      ((Component) this.subTitle).gameObject.SetActive(false);
    }
    else
    {
      ((Component) this.subTitle).gameObject.SetActive(true);
      ((TMP_Text) this.subTitle).SetText(newTitle);
    }
  }

  public void SetDuplicantJobTitleActive(bool state)
  {
    if (!Object.op_Inequality((Object) this.duplicantJob, (Object) null) || ((Component) this.duplicantJob).gameObject.activeInHierarchy == state)
      return;
    ((Component) this.duplicantJob).gameObject.SetActive(state);
  }

  public void ForceRefresh() => this.requiresRefresh = true;

  public void Update()
  {
    if (!this.requiresRefresh || !Object.op_Equality((Object) this.controller, (Object) null) && !this.controller.enabled)
      return;
    this.requiresRefresh = false;
    this.Rebuild();
  }

  private void Rebuild()
  {
    if (Object.op_Equality((Object) this.controller, (Object) null))
    {
      this.controller = ((Component) this).GetComponentInChildren<KBatchedAnimController>();
      if (Object.op_Equality((Object) this.controller, (Object) null))
      {
        if (Object.op_Inequality((Object) this.targetImage, (Object) null))
          ((Behaviour) this.targetImage).enabled = true;
        Debug.LogWarning((object) ("Controller for [" + ((Object) this).name + "] null"));
        return;
      }
    }
    CrewPortrait.SetPortraitData(this.identityObject, this.controller, this.useDefaultExpression);
    if (!this.useLabels || !Object.op_Inequality((Object) this.duplicantName, (Object) null))
      return;
    ((TMP_Text) this.duplicantName).SetText(!Util.IsNullOrDestroyed((object) this.identityObject) ? this.identityObject.GetProperName() : "");
    if (!(this.identityObject is MinionIdentity) || !Object.op_Inequality((Object) this.duplicantJob, (Object) null))
      return;
    ((TMP_Text) this.duplicantJob).SetText(this.identityObject != null ? ((Component) (this.identityObject as MinionIdentity)).GetComponent<MinionResume>().GetSkillsSubtitle() : "");
    ((Component) this.duplicantJob).GetComponent<ToolTip>().toolTip = ((Component) (this.identityObject as MinionIdentity)).GetComponent<MinionResume>().GetSkillsSubtitle();
  }

  private static void RefreshHat(
    IAssignableIdentity identityObject,
    KBatchedAnimController controller)
  {
    string hat_id = "";
    MinionIdentity minionIdentity = identityObject as MinionIdentity;
    if (Object.op_Inequality((Object) minionIdentity, (Object) null))
      hat_id = ((Component) minionIdentity).GetComponent<MinionResume>().CurrentHat;
    else if (Object.op_Inequality((Object) (identityObject as StoredMinionIdentity), (Object) null))
      hat_id = (identityObject as StoredMinionIdentity).currentHat;
    MinionResume.ApplyHat(hat_id, controller);
  }

  public static void SetPortraitData(
    IAssignableIdentity identityObject,
    KBatchedAnimController controller,
    bool useDefaultExpression = true)
  {
    if (identityObject == null)
    {
      ((Component) controller).gameObject.SetActive(false);
    }
    else
    {
      MinionIdentity identityObject1 = identityObject as MinionIdentity;
      if (Object.op_Equality((Object) identityObject1, (Object) null))
      {
        MinionAssignablesProxy assignablesProxy = identityObject as MinionAssignablesProxy;
        if (Object.op_Inequality((Object) assignablesProxy, (Object) null) && assignablesProxy.target != null)
          identityObject1 = assignablesProxy.target as MinionIdentity;
      }
      ((Component) controller).gameObject.SetActive(true);
      controller.Play(HashedString.op_Implicit("ui_idle"));
      SymbolOverrideController component1 = ((Component) controller).GetComponent<SymbolOverrideController>();
      component1.RemoveAllSymbolOverrides();
      if (Object.op_Inequality((Object) identityObject1, (Object) null))
      {
        Accessorizer component2 = ((Component) identityObject1).GetComponent<Accessorizer>();
        foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
        {
          Accessory accessory = component2.GetAccessory(resource);
          if (accessory != null)
          {
            component1.AddSymbolOverride(HashedString.op_Implicit(resource.targetSymbolId), accessory.symbol);
            controller.SetSymbolVisiblity(resource.targetSymbolId, true);
          }
          else
            controller.SetSymbolVisiblity(resource.targetSymbolId, false);
        }
        component1.AddSymbolOverride(HashedString.op_Implicit(Db.Get().AccessorySlots.HatHair.targetSymbolId), Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component2.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
        CrewPortrait.RefreshHat((IAssignableIdentity) identityObject1, controller);
      }
      else
      {
        StoredMinionIdentity identityObject2 = identityObject as StoredMinionIdentity;
        if (Object.op_Equality((Object) identityObject2, (Object) null))
        {
          MinionAssignablesProxy assignablesProxy = identityObject as MinionAssignablesProxy;
          if (Object.op_Inequality((Object) assignablesProxy, (Object) null) && assignablesProxy.target != null)
            identityObject2 = assignablesProxy.target as StoredMinionIdentity;
        }
        if (Object.op_Inequality((Object) identityObject2, (Object) null))
        {
          foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
          {
            Accessory accessory = identityObject2.GetAccessory(resource);
            if (accessory != null)
            {
              component1.AddSymbolOverride(HashedString.op_Implicit(resource.targetSymbolId), accessory.symbol);
              controller.SetSymbolVisiblity(resource.targetSymbolId, true);
            }
            else
              controller.SetSymbolVisiblity(resource.targetSymbolId, false);
          }
          component1.AddSymbolOverride(HashedString.op_Implicit(Db.Get().AccessorySlots.HatHair.targetSymbolId), Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(identityObject2.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
          CrewPortrait.RefreshHat((IAssignableIdentity) identityObject2, controller);
        }
        else
        {
          ((Component) controller).gameObject.SetActive(false);
          return;
        }
      }
      float num = 0.25f;
      controller.animScale = num;
      string str = "ui_idle";
      controller.Play(HashedString.op_Implicit(str), (KAnim.PlayMode) 0);
      controller.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_neck"), false);
      controller.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_goggles"), false);
    }
  }

  public void SetAlpha(float value)
  {
    if (Object.op_Equality((Object) this.controller, (Object) null) || (double) this.controller.TintColour.a == (double) value)
      return;
    this.controller.TintColour = Color32.op_Implicit(new Color(1f, 1f, 1f, value));
  }
}
