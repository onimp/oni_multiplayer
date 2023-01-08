// Decompiled with JetBrains decompiler
// Type: CopyBuildingSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CopyBuildingSettings")]
public class CopyBuildingSettings : KMonoBehaviour
{
  [MyCmpReq]
  private KPrefabID id;
  public Tag copyGroupTag;
  private static readonly EventSystem.IntraObjectHandler<CopyBuildingSettings> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<CopyBuildingSettings>((Action<CopyBuildingSettings, object>) ((component, data) => component.OnRefreshUserMenu(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<CopyBuildingSettings>(493375141, CopyBuildingSettings.OnRefreshUserMenuDelegate);
  }

  private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("action_mirror", (string) UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.NAME, new System.Action(this.ActivateCopyTool), (Action) 139, tooltipText: ((string) UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.TOOLTIP)));

  private void ActivateCopyTool()
  {
    CopySettingsTool.Instance.SetSourceObject(((Component) this).gameObject);
    PlayerController.Instance.ActivateTool((InterfaceTool) CopySettingsTool.Instance);
  }

  public static bool ApplyCopy(int targetCell, GameObject sourceGameObject)
  {
    ObjectLayer layer = ObjectLayer.Building;
    Building component1 = (Building) sourceGameObject.GetComponent<BuildingComplete>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      layer = component1.Def.ObjectLayer;
    GameObject gameObject = Grid.Objects[targetCell, (int) layer];
    if (Object.op_Equality((Object) gameObject, (Object) null) || Object.op_Equality((Object) gameObject, (Object) sourceGameObject))
      return false;
    KPrefabID component2 = sourceGameObject.GetComponent<KPrefabID>();
    if (Object.op_Equality((Object) component2, (Object) null))
      return false;
    KPrefabID component3 = gameObject.GetComponent<KPrefabID>();
    if (Object.op_Equality((Object) component3, (Object) null))
      return false;
    CopyBuildingSettings component4 = sourceGameObject.GetComponent<CopyBuildingSettings>();
    if (Object.op_Equality((Object) component4, (Object) null))
      return false;
    CopyBuildingSettings component5 = gameObject.GetComponent<CopyBuildingSettings>();
    if (Object.op_Equality((Object) component5, (Object) null))
      return false;
    if (Tag.op_Inequality(component4.copyGroupTag, Tag.Invalid))
    {
      if (Tag.op_Inequality(component4.copyGroupTag, component5.copyGroupTag))
        return false;
    }
    else if (Tag.op_Inequality(component3.PrefabID(), component2.PrefabID()))
      return false;
    ((KMonoBehaviour) component3).Trigger(-905833192, (object) sourceGameObject);
    PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, (string) UI.COPIED_SETTINGS, gameObject.transform, new Vector3(0.0f, 0.5f, 0.0f));
    return true;
  }
}
