// Decompiled with JetBrains decompiler
// Type: CO2Manager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CO2Manager")]
public class CO2Manager : KMonoBehaviour, ISim33ms
{
  private const float CO2Lifetime = 3f;
  [SerializeField]
  private Vector3 acceleration;
  [SerializeField]
  private CO2 prefab;
  [SerializeField]
  private GameObject breathPrefab;
  [SerializeField]
  private Color tintColour;
  private List<CO2> co2Items = new List<CO2>();
  private GameObjectPool breathPool;
  private GameObjectPool co2Pool;
  public static CO2Manager instance;

  public static void DestroyInstance() => CO2Manager.instance = (CO2Manager) null;

  protected virtual void OnPrefabInit()
  {
    CO2Manager.instance = this;
    ((Component) this.prefab).gameObject.SetActive(false);
    this.breathPrefab.SetActive(false);
    this.co2Pool = new GameObjectPool(new Func<GameObject>(this.InstantiateCO2), 16);
    this.breathPool = new GameObjectPool(new Func<GameObject>(this.InstantiateBreath), 16);
  }

  private GameObject InstantiateCO2()
  {
    GameObject gameObject = GameUtil.KInstantiate((Component) this.prefab, Grid.SceneLayer.Front);
    gameObject.SetActive(false);
    return gameObject;
  }

  private GameObject InstantiateBreath()
  {
    GameObject gameObject = GameUtil.KInstantiate(this.breathPrefab, Grid.SceneLayer.Front);
    gameObject.SetActive(false);
    return gameObject;
  }

  public void Sim33ms(float dt)
  {
    Vector2I xy1 = new Vector2I();
    Vector2I xy2 = new Vector2I();
    Vector3 vector3 = Vector3.op_Multiply(this.acceleration, dt);
    int count = this.co2Items.Count;
    for (int index = 0; index < count; ++index)
    {
      CO2 co2Item = this.co2Items[index];
      CO2 co2 = co2Item;
      co2.velocity = Vector3.op_Addition(co2.velocity, vector3);
      co2Item.lifetimeRemaining -= dt;
      Grid.PosToXY(TransformExtensions.GetPosition(co2Item.transform), out xy1);
      TransformExtensions.SetPosition(co2Item.transform, Vector3.op_Addition(TransformExtensions.GetPosition(co2Item.transform), Vector3.op_Multiply(co2Item.velocity, dt)));
      Grid.PosToXY(TransformExtensions.GetPosition(co2Item.transform), out xy2);
      int num1 = Grid.XYToCell(xy1.x, xy1.y);
      for (int y = xy1.y; y >= xy2.y; --y)
      {
        int cell1 = Grid.XYToCell(xy1.x, y);
        bool flag1 = !Grid.IsValidCell(cell1) || (double) co2Item.lifetimeRemaining <= 0.0;
        if (!flag1)
        {
          Element element = Grid.Element[cell1];
          flag1 = element.IsLiquid || element.IsSolid || ((uint) Grid.Properties[cell1] & 1U) > 0U;
        }
        if (flag1)
        {
          int gameCell = cell1;
          bool flag2 = false;
          if (num1 != cell1)
          {
            gameCell = num1;
            flag2 = true;
          }
          else
          {
            bool flag3 = false;
            int num2 = -1;
            int num3 = -1;
            foreach (CellOffset offset in OxygenBreather.DEFAULT_BREATHABLE_OFFSETS)
            {
              int cell2 = Grid.OffsetCell(cell1, offset);
              if (Grid.IsValidCell(cell2))
              {
                Element element = Grid.Element[cell2];
                if (element.id == SimHashes.CarbonDioxide || element.HasTag(GameTags.Breathable))
                {
                  num2 = cell2;
                  flag3 = true;
                  flag2 = true;
                  break;
                }
                if (element.IsGas)
                {
                  num3 = cell2;
                  flag2 = true;
                }
              }
            }
            if (flag2)
              gameCell = !flag3 ? num3 : num2;
          }
          co2Item.TriggerDestroy();
          if (flag2)
          {
            SimMessages.ModifyMass(gameCell, co2Item.mass, byte.MaxValue, 0, CellEventLogger.Instance.CO2ManagerFixedUpdate, co2Item.temperature, SimHashes.CarbonDioxide);
            --count;
            this.co2Items[index] = this.co2Items[count];
            this.co2Items.RemoveAt(count);
            break;
          }
          break;
        }
        num1 = cell1;
      }
    }
  }

  public void SpawnCO2(Vector3 position, float mass, float temperature, bool flip)
  {
    position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
    GameObject instance = this.co2Pool.GetInstance();
    TransformExtensions.SetPosition(instance.transform, position);
    instance.SetActive(true);
    CO2 component1 = instance.GetComponent<CO2>();
    component1.mass = mass;
    component1.temperature = temperature;
    component1.velocity = Vector3.zero;
    component1.lifetimeRemaining = 3f;
    KBatchedAnimController component2 = ((Component) component1).GetComponent<KBatchedAnimController>();
    component2.TintColour = Color32.op_Implicit(this.tintColour);
    component2.onDestroySelf = new Action<GameObject>(this.OnDestroyCO2);
    component2.FlipX = flip;
    component1.StartLoop();
    this.co2Items.Add(component1);
  }

  public void SpawnBreath(Vector3 position, float mass, float temperature, bool flip)
  {
    position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
    this.SpawnCO2(position, mass, temperature, flip);
    GameObject instance = this.breathPool.GetInstance();
    TransformExtensions.SetPosition(instance.transform, position);
    instance.SetActive(true);
    KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
    component.TintColour = Color32.op_Implicit(this.tintColour);
    component.onDestroySelf = new Action<GameObject>(this.OnDestroyBreath);
    component.FlipX = flip;
    component.Play(HashedString.op_Implicit("breath"));
  }

  private void OnDestroyCO2(GameObject co2_go)
  {
    co2_go.SetActive(false);
    this.co2Pool.ReleaseInstance(co2_go);
  }

  private void OnDestroyBreath(GameObject breath_go)
  {
    breath_go.SetActive(false);
    this.breathPool.ReleaseInstance(breath_go);
  }
}
