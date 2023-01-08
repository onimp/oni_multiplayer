// Decompiled with JetBrains decompiler
// Type: TreeBud
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TreeBud")]
public class TreeBud : KMonoBehaviour, IWiltCause
{
  [MyCmpReq]
  private Growing growing;
  [MyCmpReq]
  private StandardCropPlant crop;
  [Serialize]
  public Ref<BuddingTrunk> buddingTrunk;
  [Serialize]
  private int trunkPosition;
  [Serialize]
  public int growingPos;
  private int trunkWiltHandle = -1;
  private int trunkWiltRecoverHandle = -1;
  private static StandardCropPlant.AnimSet[] animSets = new StandardCropPlant.AnimSet[7]
  {
    new StandardCropPlant.AnimSet()
    {
      grow = "branch_a_grow",
      grow_pst = "branch_a_grow_pst",
      idle_full = "branch_a_idle_full",
      wilt_base = "branch_a_wilt",
      harvest = "branch_a_harvest"
    },
    new StandardCropPlant.AnimSet()
    {
      grow = "branch_b_grow",
      grow_pst = "branch_b_grow_pst",
      idle_full = "branch_b_idle_full",
      wilt_base = "branch_b_wilt",
      harvest = "branch_b_harvest"
    },
    new StandardCropPlant.AnimSet()
    {
      grow = "branch_c_grow",
      grow_pst = "branch_c_grow_pst",
      idle_full = "branch_c_idle_full",
      wilt_base = "branch_c_wilt",
      harvest = "branch_c_harvest"
    },
    new StandardCropPlant.AnimSet()
    {
      grow = "branch_d_grow",
      grow_pst = "branch_d_grow_pst",
      idle_full = "branch_d_idle_full",
      wilt_base = "branch_d_wilt",
      harvest = "branch_d_harvest"
    },
    new StandardCropPlant.AnimSet()
    {
      grow = "branch_e_grow",
      grow_pst = "branch_e_grow_pst",
      idle_full = "branch_e_idle_full",
      wilt_base = "branch_e_wilt",
      harvest = "branch_e_harvest"
    },
    new StandardCropPlant.AnimSet()
    {
      grow = "branch_f_grow",
      grow_pst = "branch_f_grow_pst",
      idle_full = "branch_f_idle_full",
      wilt_base = "branch_f_wilt",
      harvest = "branch_f_harvest"
    },
    new StandardCropPlant.AnimSet()
    {
      grow = "branch_g_grow",
      grow_pst = "branch_g_grow_pst",
      idle_full = "branch_g_idle_full",
      wilt_base = "branch_g_wilt",
      harvest = "branch_g_harvest"
    }
  };
  private static Vector3[] animOffset = new Vector3[7]
  {
    new Vector3(1f, 0.0f, 0.0f),
    new Vector3(1f, -1f, 0.0f),
    new Vector3(1f, -2f, 0.0f),
    new Vector3(0.0f, -2f, 0.0f),
    new Vector3(-1f, -2f, 0.0f),
    new Vector3(-1f, -1f, 0.0f),
    new Vector3(-1f, 0.0f, 0.0f)
  };
  private static readonly EventSystem.IntraObjectHandler<TreeBud> OnHarvestDelegate = new EventSystem.IntraObjectHandler<TreeBud>((Action<TreeBud, object>) ((component, data) => component.OnHarvest(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.simRenderLoadBalance = true;
    int cell = Grid.PosToCell(((Component) this).gameObject);
    GameObject gameObject = Grid.Objects[cell, 5];
    if (Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) gameObject, (Object) ((Component) this).gameObject))
      Util.KDestroyGameObject(((Component) this).gameObject);
    else
      this.SetOccupyGridSpace(true);
    this.Subscribe<TreeBud>(1272413801, TreeBud.OnHarvestDelegate);
  }

  private void OnHarvest(object data)
  {
    if (!Object.op_Inequality((Object) this.buddingTrunk.Get(), (Object) null))
      return;
    this.buddingTrunk.Get().TryRollNewSeed();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.buddingTrunk != null && Object.op_Inequality((Object) this.buddingTrunk.Get(), (Object) null))
    {
      this.SubscribeToTrunk();
      this.UpdateAnimationSet();
    }
    else
    {
      Debug.LogWarning((object) "TreeBud loaded with missing trunk reference. Destroying...");
      Util.KDestroyGameObject(((Component) this).gameObject);
    }
  }

  protected virtual void OnCleanUp()
  {
    this.UnsubscribeToTrunk();
    this.SetOccupyGridSpace(false);
    base.OnCleanUp();
  }

  private void SetOccupyGridSpace(bool active)
  {
    int cell = Grid.PosToCell(((Component) this).gameObject);
    if (active)
    {
      GameObject gameObject = Grid.Objects[cell, 5];
      if (Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) gameObject, (Object) ((Component) this).gameObject))
        Debug.LogWarningFormat((Object) ((Component) this).gameObject, "TreeBud.SetOccupyGridSpace already occupied by {0}", new object[1]
        {
          (object) gameObject
        });
      Grid.Objects[cell, 5] = ((Component) this).gameObject;
    }
    else
    {
      if (!Object.op_Equality((Object) Grid.Objects[cell, 5], (Object) ((Component) this).gameObject))
        return;
      Grid.Objects[cell, 5] = (GameObject) null;
    }
  }

  private void SubscribeToTrunk()
  {
    if (this.trunkWiltHandle != -1 || this.trunkWiltRecoverHandle != -1)
      return;
    Debug.Assert(this.buddingTrunk != null, (object) "buddingTrunk null");
    BuddingTrunk buddingTrunk = this.buddingTrunk.Get();
    Debug.Assert(Object.op_Inequality((Object) buddingTrunk, (Object) null), (object) "tree_trunk null");
    this.trunkWiltHandle = buddingTrunk.Subscribe(-724860998, new Action<object>(this.OnTrunkWilt));
    this.trunkWiltRecoverHandle = buddingTrunk.Subscribe(712767498, new Action<object>(this.OnTrunkRecover));
    this.Trigger(912965142, (object) !((Component) buddingTrunk).GetComponent<WiltCondition>().IsWilting());
    ((Component) this).GetComponent<ReceptacleMonitor>().SetReceptacle(((Component) buddingTrunk).GetComponent<ReceptacleMonitor>().GetReceptacle());
    Vector3 position = ((Component) this).gameObject.transform.position;
    position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront) - 0.1f * (float) this.trunkPosition;
    TransformExtensions.SetPosition(((Component) this).gameObject.transform, position);
    ((Component) this).GetComponent<BudUprootedMonitor>().SetParentObject(((Component) buddingTrunk).GetComponent<KPrefabID>());
  }

  private void UnsubscribeToTrunk()
  {
    if (this.buddingTrunk == null)
    {
      Debug.LogWarning((object) "buddingTrunk null", (Object) ((Component) this).gameObject);
    }
    else
    {
      BuddingTrunk buddingTrunk = this.buddingTrunk.Get();
      if (Object.op_Equality((Object) buddingTrunk, (Object) null))
      {
        Debug.LogWarning((object) "tree_trunk null", (Object) ((Component) this).gameObject);
      }
      else
      {
        buddingTrunk.Unsubscribe(this.trunkWiltHandle);
        buddingTrunk.Unsubscribe(this.trunkWiltRecoverHandle);
        buddingTrunk.OnBranchRemoved(this.trunkPosition, this);
      }
    }
  }

  public void SetTrunkPosition(BuddingTrunk budding_trunk, int idx)
  {
    this.buddingTrunk = new Ref<BuddingTrunk>(budding_trunk);
    this.trunkPosition = idx;
    this.SubscribeToTrunk();
    this.UpdateAnimationSet();
  }

  private void OnTrunkWilt(object data = null) => this.Trigger(912965142, (object) false);

  private void OnTrunkRecover(object data = null) => this.Trigger(912965142, (object) true);

  private void UpdateAnimationSet()
  {
    this.crop.anims = TreeBud.animSets[this.trunkPosition];
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    component.Offset = TreeBud.animOffset[this.trunkPosition];
    component.Play(HashedString.op_Implicit(this.crop.anims.grow), (KAnim.PlayMode) 2);
    this.crop.RefreshPositionPercent();
  }

  public string WiltStateString => "    • " + (string) DUPLICANTS.STATS.TRUNKHEALTH.NAME;

  public WiltCondition.Condition[] Conditions => new WiltCondition.Condition[1]
  {
    WiltCondition.Condition.UnhealthyRoot
  };
}
