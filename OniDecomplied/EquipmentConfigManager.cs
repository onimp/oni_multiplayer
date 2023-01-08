// Decompiled with JetBrains decompiler
// Type: EquipmentConfigManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EquipmentConfigManager")]
public class EquipmentConfigManager : KMonoBehaviour
{
  public static EquipmentConfigManager Instance;

  public static void DestroyInstance() => EquipmentConfigManager.Instance = (EquipmentConfigManager) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    EquipmentConfigManager.Instance = this;
  }

  public void RegisterEquipment(IEquipmentConfig config)
  {
    if (!DlcManager.IsDlcListValidForCurrentContent(config.GetDlcIds()))
      return;
    EquipmentDef equipmentDef = config.CreateEquipmentDef();
    GameObject looseEntity1 = EntityTemplates.CreateLooseEntity(equipmentDef.Id, equipmentDef.Name, equipmentDef.RecipeDescription, equipmentDef.Mass, true, equipmentDef.Anim, "object", Grid.SceneLayer.Ore, equipmentDef.CollisionShape, equipmentDef.width, equipmentDef.height, true, element: equipmentDef.OutputElement);
    Equippable equippable = looseEntity1.AddComponent<Equippable>();
    equippable.def = equipmentDef;
    Debug.Assert(Object.op_Inequality((Object) equippable.def, (Object) null));
    equippable.slotID = equipmentDef.Slot;
    Debug.Assert(equippable.slot != null);
    config.DoPostConfigure(looseEntity1);
    Assets.AddPrefab(looseEntity1.GetComponent<KPrefabID>());
    if (equipmentDef.wornID == null)
      return;
    GameObject looseEntity2 = EntityTemplates.CreateLooseEntity(equipmentDef.wornID, equipmentDef.WornName, equipmentDef.WornDesc, equipmentDef.Mass, true, equipmentDef.Anim, "worn_out", Grid.SceneLayer.Ore, equipmentDef.CollisionShape, equipmentDef.width, equipmentDef.height, true);
    RepairableEquipment repairableEquipment = looseEntity2.AddComponent<RepairableEquipment>();
    repairableEquipment.def = equipmentDef;
    Debug.Assert(Object.op_Inequality((Object) repairableEquipment.def, (Object) null));
    SymbolOverrideControllerUtil.AddToPrefab(looseEntity2);
    foreach (Tag additionalTag in equipmentDef.AdditionalTags)
      looseEntity2.GetComponent<KPrefabID>().AddTag(additionalTag, false);
    Assets.AddPrefab(looseEntity2.GetComponent<KPrefabID>());
  }

  private void LoadRecipe(EquipmentDef def, Equippable equippable)
  {
    Recipe recipe = new Recipe(def.Id, recipeDescription: def.RecipeDescription);
    recipe.SetFabricator(def.FabricatorId, def.FabricationTime);
    recipe.TechUnlock = def.RecipeTechUnlock;
    foreach (KeyValuePair<string, float> inputElementMass in def.InputElementMassMap)
      recipe.AddIngredient(new Recipe.Ingredient(inputElementMass.Key, inputElementMass.Value));
  }
}
