// Decompiled with JetBrains decompiler
// Type: MushBarConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class MushBarConfig : IEntityConfig
{
  public const string ID = "MushBar";
  public static ComplexRecipe recipe;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject food = EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("MushBar", (string) ITEMS.FOOD.MUSHBAR.NAME, (string) ITEMS.FOOD.MUSHBAR.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("mushbar_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true), TUNING.FOOD.FOOD_TYPES.MUSHBAR);
    ComplexRecipeManager.Get().GetRecipe(MushBarConfig.recipe.id).FabricationVisualizer = MushBarConfig.CreateFabricationVisualizer(food);
    return food;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }

  public static GameObject CreateFabricationVisualizer(GameObject result)
  {
    KBatchedAnimController component = result.GetComponent<KBatchedAnimController>();
    GameObject fabricationVisualizer = new GameObject();
    ((Object) fabricationVisualizer).name = ((Object) result).name + "Visualizer";
    fabricationVisualizer.SetActive(false);
    TransformExtensions.SetLocalPosition(fabricationVisualizer.transform, Vector3.zero);
    KBatchedAnimController kbatchedAnimController = fabricationVisualizer.AddComponent<KBatchedAnimController>();
    kbatchedAnimController.AnimFiles = component.AnimFiles;
    kbatchedAnimController.initialAnim = "fabricating";
    kbatchedAnimController.isMovable = true;
    KBatchedAnimTracker kbatchedAnimTracker = fabricationVisualizer.AddComponent<KBatchedAnimTracker>();
    kbatchedAnimTracker.symbol = new HashedString("meter_ration");
    kbatchedAnimTracker.offset = Vector3.zero;
    Object.DontDestroyOnLoad((Object) fabricationVisualizer);
    return fabricationVisualizer;
  }
}
