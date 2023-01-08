// Decompiled with JetBrains decompiler
// Type: MushroomConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class MushroomConfig : IEntityConfig
{
  public static float SEEDS_PER_FRUIT_CHANCE = 0.05f;
  public static string ID = "Mushroom";
  private static readonly EventSystem.IntraObjectHandler<Edible> OnEatCompleteDelegate = new EventSystem.IntraObjectHandler<Edible>((Action<Edible, object>) ((component, data) => MushroomConfig.OnEatComplete(component)));

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity(MushroomConfig.ID, (string) ITEMS.FOOD.MUSHROOM.NAME, (string) ITEMS.FOOD.MUSHROOM.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("funguscap_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.77f, 0.48f, true), TUNING.FOOD.FOOD_TYPES.MUSHROOM);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst) => EventExtensions.Subscribe<Edible>(inst, -10536414, MushroomConfig.OnEatCompleteDelegate);

  private static void OnEatComplete(Edible edible)
  {
    if (!Object.op_Inequality((Object) edible, (Object) null))
      return;
    int num1 = 0;
    double unitsConsumed = (double) edible.unitsConsumed;
    int num2 = Mathf.FloorToInt((float) unitsConsumed);
    if ((double) Random.value < unitsConsumed % 1.0)
      ++num2;
    for (int index = 0; index < num2; ++index)
    {
      if ((double) Random.value < (double) MushroomConfig.SEEDS_PER_FRUIT_CHANCE)
        ++num1;
    }
    if (num1 <= 0)
      return;
    Vector3 posCcc = Grid.CellToPosCCC(Grid.PosToCell(Vector3.op_Addition(TransformExtensions.GetPosition(edible.transform), new Vector3(0.0f, 0.05f, 0.0f))), Grid.SceneLayer.Ore);
    GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("MushroomSeed")), posCcc, Grid.SceneLayer.Ore);
    PrimaryElement component1 = ((Component) edible).GetComponent<PrimaryElement>();
    PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
    component2.Temperature = component1.Temperature;
    component2.Units = (float) num1;
    gameObject.SetActive(true);
  }
}
