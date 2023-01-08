// Decompiled with JetBrains decompiler
// Type: WorldDamage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/WorldDamage")]
public class WorldDamage : KMonoBehaviour
{
  public KBatchedAnimController leakEffect;
  [SerializeField]
  private FMODAsset leakSound;
  [SerializeField]
  private EventReference leakSoundMigrated;
  private float damageAmount = 0.000833333354f;
  private const float SPAWN_DELAY = 1f;
  private Dictionary<int, float> spawnTimes = new Dictionary<int, float>();
  private List<int> expiredCells = new List<int>();

  public static WorldDamage Instance { get; private set; }

  public static void DestroyInstance() => WorldDamage.Instance = (WorldDamage) null;

  protected virtual void OnPrefabInit() => WorldDamage.Instance = this;

  public void RestoreDamageToValue(int cell, float amount)
  {
    if ((double) Grid.Damage[cell] <= (double) amount)
      return;
    Grid.Damage[cell] = amount;
  }

  public float ApplyDamage(Sim.WorldDamageInfo damage_info) => this.ApplyDamage(damage_info.gameCell, this.damageAmount, damage_info.damageSourceOffset, (string) BUILDINGS.DAMAGESOURCES.LIQUID_PRESSURE, (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.LIQUID_PRESSURE);

  public float ApplyDamage(
    int cell,
    float amount,
    int src_cell,
    string source_name = null,
    string pop_text = null)
  {
    float num1 = 0.0f;
    if (Grid.Solid[cell])
    {
      float num2 = Grid.Damage[cell];
      num1 = Mathf.Min(amount, 1f - num2);
      float num3 = num2 + amount;
      bool flag = (double) num3 > 0.15000000596046448;
      if (flag)
      {
        GameObject gameObject = Grid.Objects[cell, 9];
        if (Object.op_Inequality((Object) gameObject, (Object) null))
        {
          BuildingHP component = gameObject.GetComponent<BuildingHP>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            if (!component.invincible)
            {
              int num4 = Mathf.RoundToInt(Mathf.Max((float) component.HitPoints - (1f - num3) * (float) component.MaxHitPoints, 0.0f));
              EventExtensions.Trigger(gameObject, -794517298, (object) new BuildingHP.DamageSourceInfo()
              {
                damage = num4,
                source = source_name,
                popString = pop_text
              });
            }
            else
              num3 = 0.0f;
          }
        }
      }
      Grid.Damage[cell] = Mathf.Min(1f, num3);
      if ((double) Grid.Damage[cell] >= 1.0)
        this.DestroyCell(cell);
      else if (Grid.IsValidCell(src_cell) & flag)
      {
        Element elem = Grid.Element[src_cell];
        if (elem.IsLiquid && (double) Grid.Mass[src_cell] > 1.0)
        {
          int offset = cell - src_cell;
          switch (offset)
          {
            case -1:
            case 1:
              int index = cell + offset;
              if (Grid.IsValidCell(index))
              {
                Element element = Grid.Element[index];
                if (!element.IsSolid && (!element.IsLiquid || element.id == elem.id && (double) Grid.Mass[index] <= 100.0) && ((int) Grid.Properties[index] & 2) == 0 && !this.spawnTimes.ContainsKey(index))
                {
                  this.spawnTimes[index] = Time.realtimeSinceStartup;
                  ushort idx = elem.idx;
                  float temperature = Grid.Temperature[src_cell];
                  ((MonoBehaviour) this).StartCoroutine(this.DelayedSpawnFX(src_cell, index, offset, elem, idx, temperature));
                  break;
                }
                break;
              }
              break;
            default:
              if (offset == Grid.WidthInCells || offset == -Grid.WidthInCells)
                goto case -1;
              else
                break;
          }
        }
      }
    }
    return num1;
  }

  private void ReleaseGO(GameObject go) => TracesExtesions.DeleteObject(go);

  private IEnumerator DelayedSpawnFX(
    int src_cell,
    int dest_cell,
    int offset,
    Element elem,
    ushort idx,
    float temperature)
  {
    WorldDamage worldDamage = this;
    yield return (object) new WaitForSeconds(Random.value * 0.25f);
    Vector3 posCcc = Grid.CellToPosCCC(dest_cell, Grid.SceneLayer.Front);
    GameObject gameObject = GameUtil.KInstantiate(((Component) worldDamage.leakEffect).gameObject, posCcc, Grid.SceneLayer.Front);
    KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
    component.TintColour = elem.substance.colour;
    component.onDestroySelf = new Action<GameObject>(worldDamage.ReleaseGO);
    SimMessages.AddRemoveSubstance(src_cell, idx, CellEventLogger.Instance.WorldDamageDelayedSpawnFX, -1f, temperature, byte.MaxValue, 0);
    if (offset == -1)
    {
      component.Play(HashedString.op_Implicit("side"));
      component.FlipX = true;
      component.enabled = false;
      component.enabled = true;
      TransformExtensions.SetPosition(gameObject.transform, Vector3.op_Addition(TransformExtensions.GetPosition(gameObject.transform), Vector3.op_Multiply(Vector3.right, 0.5f)));
      FallingWater.instance.AddParticle(dest_cell, idx, 1f, temperature, byte.MaxValue, 0, true);
    }
    else if (offset == Grid.WidthInCells)
    {
      TransformExtensions.SetPosition(gameObject.transform, Vector3.op_Subtraction(TransformExtensions.GetPosition(gameObject.transform), Vector3.op_Multiply(Vector3.up, 0.5f)));
      component.Play(HashedString.op_Implicit("floor"));
      component.enabled = false;
      component.enabled = true;
      SimMessages.AddRemoveSubstance(dest_cell, idx, CellEventLogger.Instance.WorldDamageDelayedSpawnFX, 1f, temperature, byte.MaxValue, 0);
    }
    else if (offset == -Grid.WidthInCells)
    {
      component.Play(HashedString.op_Implicit("ceiling"));
      component.enabled = false;
      component.enabled = true;
      TransformExtensions.SetPosition(gameObject.transform, Vector3.op_Addition(TransformExtensions.GetPosition(gameObject.transform), Vector3.op_Multiply(Vector3.up, 0.5f)));
      FallingWater.instance.AddParticle(dest_cell, idx, 1f, temperature, byte.MaxValue, 0, true);
    }
    else
    {
      component.Play(HashedString.op_Implicit("side"));
      component.enabled = false;
      component.enabled = true;
      TransformExtensions.SetPosition(gameObject.transform, Vector3.op_Subtraction(TransformExtensions.GetPosition(gameObject.transform), Vector3.op_Multiply(Vector3.right, 0.5f)));
      FallingWater.instance.AddParticle(dest_cell, idx, 1f, temperature, byte.MaxValue, 0, true);
    }
    if (CameraController.Instance.IsAudibleSound(TransformExtensions.GetPosition(gameObject.transform), worldDamage.leakSoundMigrated))
      SoundEvent.PlayOneShot(worldDamage.leakSoundMigrated, TransformExtensions.GetPosition(gameObject.transform));
    yield return (object) null;
  }

  private void Update()
  {
    this.expiredCells.Clear();
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    foreach (KeyValuePair<int, float> spawnTime in this.spawnTimes)
    {
      if ((double) realtimeSinceStartup - (double) spawnTime.Value > 1.0)
        this.expiredCells.Add(spawnTime.Key);
    }
    foreach (int expiredCell in this.expiredCells)
      this.spawnTimes.Remove(expiredCell);
    this.expiredCells.Clear();
  }

  public void DestroyCell(int cell)
  {
    if (!Grid.Solid[cell])
      return;
    SimMessages.Dig(cell);
  }

  public void OnSolidStateChanged(int cell) => Grid.Damage[cell] = 0.0f;

  public void OnDigComplete(
    int cell,
    float mass,
    float temperature,
    ushort element_idx,
    byte disease_idx,
    int disease_count)
  {
    Vector3 pos = Grid.CellToPos(cell, (CellAlignment) 5, Grid.SceneLayer.Ore);
    Element element = ElementLoader.elements[(int) element_idx];
    Grid.Damage[cell] = 0.0f;
    WorldDamage.Instance.PlaySoundForSubstance(element, pos);
    float mass1 = mass * 0.5f;
    if ((double) mass1 <= 0.0)
      return;
    GameObject gameObject = element.substance.SpawnResource(pos, mass1, temperature, disease_idx, disease_count);
    Pickupable component = gameObject.GetComponent<Pickupable>();
    if (!Object.op_Inequality((Object) component, (Object) null) || !Object.op_Inequality((Object) component.GetMyWorld(), (Object) null) || !component.GetMyWorld().worldInventory.IsReachable(component))
      return;
    PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, Mathf.RoundToInt(mass1).ToString() + " " + element.name, gameObject.transform);
  }

  private void PlaySoundForSubstance(Element element, Vector3 pos)
  {
    string sound = GlobalAssets.GetSound("Break_" + (element.substance.GetMiningBreakSound() ?? (!element.HasTag(GameTags.RefinedMetal) ? (!element.HasTag(GameTags.Metal) ? "Rock" : "RawMetal") : "RefinedMetal")));
    if (!Object.op_Implicit((Object) CameraController.Instance) || !CameraController.Instance.IsAudibleSound(pos, HashedString.op_Implicit(sound)))
      return;
    KFMOD.PlayOneShot(sound, CameraController.Instance.GetVerticallyScaledPosition(pos), 1f);
  }
}
