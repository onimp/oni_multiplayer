// Decompiled with JetBrains decompiler
// Type: DecorProvider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/DecorProvider")]
public class DecorProvider : KMonoBehaviour, IGameObjectEffectDescriptor
{
  public const string ID = "DecorProvider";
  public float baseRadius;
  public float baseDecor;
  public string overrideName;
  public System.Action refreshCallback;
  public Action<object> refreshPartionerCallback;
  public Action<object> onCollectDecorProvidersCallback;
  public AttributeInstance decor;
  public AttributeInstance decorRadius;
  private AttributeModifier baseDecorModifier;
  private AttributeModifier baseDecorRadiusModifier;
  [MyCmpReq]
  public OccupyArea occupyArea;
  [MyCmpGet]
  public Rotatable rotatable;
  [MyCmpGet]
  public SimCellOccupier simCellOccupier;
  private int[] cells;
  private int cellCount;
  public float currDecor;
  private HandleVector<int>.Handle partitionerEntry;
  private HandleVector<int>.Handle solidChangedPartitionerEntry;

  private void AddDecor()
  {
    this.currDecor = 0.0f;
    if (this.decor != null)
      this.currDecor = this.decor.GetTotalValue();
    if (((Component) this).gameObject.HasTag(GameTags.Stored))
      this.currDecor = 0.0f;
    int cell1 = Grid.PosToCell(((Component) this).gameObject);
    if (!Grid.IsValidCell(cell1))
      return;
    if (!Grid.Transparent[cell1] && Grid.Solid[cell1] && Object.op_Equality((Object) this.simCellOccupier, (Object) null))
      this.currDecor = 0.0f;
    if ((double) this.currDecor == 0.0)
      return;
    this.cellCount = 0;
    int num1 = 5;
    if (this.decorRadius != null)
      num1 = (int) this.decorRadius.GetTotalValue();
    Orientation orientation = Orientation.Neutral;
    if (Object.op_Implicit((Object) this.rotatable))
      orientation = this.rotatable.GetOrientation();
    Extents extents = this.occupyArea.GetExtents(orientation);
    extents.x = Mathf.Max(extents.x - num1, 0);
    extents.y = Mathf.Max(extents.y - num1, 0);
    extents.width = Mathf.Min(extents.width + num1 * 2, Grid.WidthInCells - 1);
    extents.height = Mathf.Min(extents.height + num1 * 2, Grid.HeightInCells - 1);
    this.partitionerEntry = GameScenePartitioner.Instance.Add("DecorProvider.SplatCollectDecorProviders", (object) ((Component) this).gameObject, extents, GameScenePartitioner.Instance.decorProviderLayer, this.onCollectDecorProvidersCallback);
    this.solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("DecorProvider.SplatSolidCheck", (object) ((Component) this).gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, this.refreshPartionerCallback);
    int val1_1 = extents.x + extents.width;
    int val1_2 = extents.y + extents.height;
    int x1 = extents.x;
    int y1 = extents.y;
    int x2;
    int y2;
    Grid.CellToXY(cell1, out x2, out y2);
    int num2 = Math.Min(val1_1, Grid.WidthInCells);
    int num3 = Math.Min(val1_2, Grid.HeightInCells);
    int num4 = Math.Max(0, x1);
    int num5 = Math.Max(0, y1);
    int length = (num2 - num4) * (num3 - num5);
    if (this.cells == null || this.cells.Length != length)
      this.cells = new int[length];
    for (int index1 = num4; index1 < num2; ++index1)
    {
      for (int index2 = num5; index2 < num3; ++index2)
      {
        if (Grid.VisibilityTest(x2, y2, index1, index2))
        {
          int cell2 = Grid.XYToCell(index1, index2);
          if (Grid.IsValidCell(cell2))
          {
            Grid.Decor[cell2] += this.currDecor;
            this.cells[this.cellCount++] = cell2;
          }
        }
      }
    }
  }

  public void Clear()
  {
    if ((double) this.currDecor == 0.0)
      return;
    this.RemoveDecor();
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    GameScenePartitioner.Instance.Free(ref this.solidChangedPartitionerEntry);
  }

  private void RemoveDecor()
  {
    if ((double) this.currDecor == 0.0)
      return;
    for (int index = 0; index < this.cellCount; ++index)
    {
      int cell = this.cells[index];
      if (Grid.IsValidCell(cell))
        Grid.Decor[cell] -= this.currDecor;
    }
  }

  public void Refresh()
  {
    this.Clear();
    this.AddDecor();
    KPrefabID component = ((Component) this).GetComponent<KPrefabID>();
    int num1 = component.HasTag(RoomConstraints.ConstraintTags.Decor20) ? 1 : 0;
    bool flag = (double) this.decor.GetTotalValue() >= 20.0;
    int num2 = flag ? 1 : 0;
    if (num1 == num2)
      return;
    if (flag)
      component.AddTag(RoomConstraints.ConstraintTags.Decor20, false);
    else
      component.RemoveTag(RoomConstraints.ConstraintTags.Decor20);
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    if (!Grid.IsValidCell(cell))
      return;
    Game.Instance.roomProber.SolidChangedEvent(cell, true);
  }

  public float GetDecorForCell(int cell)
  {
    for (int index = 0; index < this.cellCount; ++index)
    {
      if (this.cells[index] == cell)
        return this.currDecor;
    }
    return 0.0f;
  }

  public void SetValues(EffectorValues values)
  {
    this.baseDecor = (float) values.amount;
    this.baseRadius = (float) values.radius;
    if (!this.IsInitialized())
      return;
    this.UpdateBaseDecorModifiers();
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.decor = this.GetAttributes().Add(Db.Get().BuildingAttributes.Decor);
    this.decorRadius = this.GetAttributes().Add(Db.Get().BuildingAttributes.DecorRadius);
    this.UpdateBaseDecorModifiers();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.refreshCallback = new System.Action(this.Refresh);
    this.refreshPartionerCallback = (Action<object>) (data => this.Refresh());
    this.onCollectDecorProvidersCallback = new Action<object>(this.OnCollectDecorProviders);
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange), "DecorProvider.OnSpawn");
    this.decor.OnDirty += this.refreshCallback;
    this.decorRadius.OnDirty += this.refreshCallback;
    this.Refresh();
  }

  private void UpdateBaseDecorModifiers()
  {
    Attributes attributes = this.GetAttributes();
    if (this.baseDecorModifier != null)
    {
      attributes.Remove(this.baseDecorModifier);
      attributes.Remove(this.baseDecorRadiusModifier);
      this.baseDecorModifier = (AttributeModifier) null;
      this.baseDecorRadiusModifier = (AttributeModifier) null;
    }
    if ((double) this.baseDecor == 0.0)
      return;
    this.baseDecorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, this.baseDecor, (string) UI.TOOLTIPS.BASE_VALUE);
    this.baseDecorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, this.baseRadius, (string) UI.TOOLTIPS.BASE_VALUE);
    attributes.Add(this.baseDecorModifier);
    attributes.Add(this.baseDecorRadiusModifier);
  }

  private void OnCellChange() => this.Refresh();

  private void OnCollectDecorProviders(object data) => ((List<DecorProvider>) data).Add(this);

  public string GetName() => string.IsNullOrEmpty(this.overrideName) ? ((Component) this).GetComponent<KSelectable>().GetName() : this.overrideName;

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if (this.isSpawned)
    {
      this.decor.OnDirty -= this.refreshCallback;
      this.decorRadius.OnDirty -= this.refreshCallback;
      Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange));
    }
    this.Clear();
  }

  public List<Descriptor> GetEffectDescriptions()
  {
    List<Descriptor> effectDescriptions = new List<Descriptor>();
    if (this.decor != null && this.decorRadius != null)
    {
      float totalValue1 = this.decor.GetTotalValue();
      float totalValue2 = this.decorRadius.GetTotalValue();
      string str1 = (double) this.baseDecor > 0.0 ? "produced" : "consumed";
      string format = (string) ((double) this.baseDecor > 0.0 ? UI.BUILDINGEFFECTS.TOOLTIPS.DECORPROVIDED : UI.BUILDINGEFFECTS.TOOLTIPS.DECORDECREASED) + "\n\n" + this.decor.GetAttributeValueTooltip();
      string str2 = GameUtil.AddPositiveSign(totalValue1.ToString(), (double) totalValue1 > 0.0);
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector(string.Format((string) UI.BUILDINGEFFECTS.DECORPROVIDED, (object) str1, (object) str2, (object) totalValue2), string.Format(format, (object) str2, (object) totalValue2), (Descriptor.DescriptorType) 1, false);
      effectDescriptions.Add(descriptor);
    }
    else if ((double) this.baseDecor != 0.0)
    {
      string str3 = (double) this.baseDecor >= 0.0 ? "produced" : "consumed";
      string format = (string) ((double) this.baseDecor >= 0.0 ? UI.BUILDINGEFFECTS.TOOLTIPS.DECORPROVIDED : UI.BUILDINGEFFECTS.TOOLTIPS.DECORDECREASED);
      string str4 = GameUtil.AddPositiveSign(this.baseDecor.ToString(), (double) this.baseDecor > 0.0);
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector(string.Format((string) UI.BUILDINGEFFECTS.DECORPROVIDED, (object) str3, (object) str4, (object) this.baseRadius), string.Format(format, (object) str4, (object) this.baseRadius), (Descriptor.DescriptorType) 1, false);
      effectDescriptions.Add(descriptor);
    }
    return effectDescriptions;
  }

  public static int GetLightDecorBonus(int cell) => Grid.LightIntensity[cell] > 0 ? TUNING.DECOR.LIT_BONUS : 0;

  public List<Descriptor> GetDescriptors(GameObject go) => this.GetEffectDescriptions();
}
