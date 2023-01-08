// Decompiled with JetBrains decompiler
// Type: ConsumablesTableScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsumablesTableScreen : TableScreen
{
  private const int CONSUMABLE_COLUMNS_BEFORE_SCROLL = 12;

  protected override void OnActivate()
  {
    this.title = (string) STRINGS.UI.CONSUMABLESSCREEN.TITLE;
    base.OnActivate();
    this.AddPortraitColumn("Portrait", new Action<IAssignableIdentity, GameObject>(((TableScreen) this).on_load_portrait), (Comparison<IAssignableIdentity>) null);
    this.AddButtonLabelColumn("Names", new Action<IAssignableIdentity, GameObject>(((TableScreen) this).on_load_name_label), new Func<IAssignableIdentity, GameObject, string>(((TableScreen) this).get_value_name_label), (Action<GameObject>) (widget_go => this.GetWidgetRow(widget_go).SelectMinion()), (Action<GameObject>) (widget_go => this.GetWidgetRow(widget_go).SelectAndFocusMinion()), new Comparison<IAssignableIdentity>(((TableScreen) this).compare_rows_alphabetical), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_name), new Action<IAssignableIdentity, GameObject, ToolTip>(((TableScreen) this).on_tooltip_sort_alphabetically));
    this.AddLabelColumn("QOLExpectations", new Action<IAssignableIdentity, GameObject>(this.on_load_qualityoflife_expectations), new Func<IAssignableIdentity, GameObject, string>(this.get_value_qualityoflife_label), new Comparison<IAssignableIdentity>(this.compare_rows_qualityoflife_expectations), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_qualityoflife_expectations), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_sort_qualityoflife_expectations), 96, true);
    List<IConsumableUIItem> consumableUiItemList = new List<IConsumableUIItem>();
    for (int index = 0; index < EdiblesManager.GetAllFoodTypes().Count; ++index)
      consumableUiItemList.Add((IConsumableUIItem) EdiblesManager.GetAllFoodTypes()[index]);
    List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(GameTags.Medicine);
    for (int index = 0; index < prefabsWithTag.Count; ++index)
    {
      MedicinalPillWorkable component = prefabsWithTag[index].GetComponent<MedicinalPillWorkable>();
      if (Object.op_Implicit((Object) component))
        consumableUiItemList.Add((IConsumableUIItem) component);
      else
        DebugUtil.DevLogErrorFormat("Prefab tagged Medicine does not have MedicinalPill component: {0}", new object[1]
        {
          (object) prefabsWithTag[index]
        });
    }
    consumableUiItemList.Sort((Comparison<IConsumableUIItem>) ((a, b) =>
    {
      int num = a.MajorOrder.CompareTo(b.MajorOrder);
      if (num == 0)
        num = a.MinorOrder.CompareTo(b.MinorOrder);
      return num;
    }));
    ConsumerManager.instance.OnDiscover += new Action<Tag>(this.OnConsumableDiscovered);
    List<ConsumableInfoTableColumn> consumableInfoTableColumnList1 = new List<ConsumableInfoTableColumn>();
    List<DividerColumn> dividerColumnList = new List<DividerColumn>();
    List<ConsumableInfoTableColumn> consumableInfoTableColumnList2 = new List<ConsumableInfoTableColumn>();
    this.StartScrollableContent("consumableScroller");
    int num1 = 0;
    for (int index = 0; index < consumableUiItemList.Count; ++index)
    {
      if (consumableUiItemList[index].Display)
      {
        if (consumableUiItemList[index].MajorOrder != num1 && index != 0)
        {
          string id = "QualityDivider_" + consumableUiItemList[index].MajorOrder.ToString();
          ConsumableInfoTableColumn[] quality_group_columns = consumableInfoTableColumnList2.ToArray();
          DividerColumn new_column = new DividerColumn((Func<bool>) (() =>
          {
            if (quality_group_columns == null || quality_group_columns.Length == 0)
              return true;
            foreach (TableColumn tableColumn in quality_group_columns)
            {
              if (tableColumn.isRevealed)
                return true;
            }
            return false;
          }), "consumableScroller");
          dividerColumnList.Add(new_column);
          this.RegisterColumn(id, (TableColumn) new_column);
          consumableInfoTableColumnList2.Clear();
        }
        ConsumableInfoTableColumn consumableInfoTableColumn = this.AddConsumableInfoColumn(consumableUiItemList[index].ConsumableId, consumableUiItemList[index], new Action<IAssignableIdentity, GameObject>(this.on_load_consumable_info), new Func<IAssignableIdentity, GameObject, TableScreen.ResultValues>(this.get_value_consumable_info), new Action<GameObject>(this.on_click_consumable_info), new Action<GameObject, TableScreen.ResultValues>(this.set_value_consumable_info), new Comparison<IAssignableIdentity>(this.compare_consumable_info), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_consumable_info), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_sort_consumable_info));
        consumableInfoTableColumnList1.Add(consumableInfoTableColumn);
        num1 = consumableUiItemList[index].MajorOrder;
        consumableInfoTableColumnList2.Add(consumableInfoTableColumn);
      }
    }
    this.AddSuperCheckboxColumn("SuperCheckConsumable", (CheckboxTableColumn[]) consumableInfoTableColumnList1.ToArray(), new Action<IAssignableIdentity, GameObject>(((TableScreen) this).on_load_value_checkbox_column_super), new Func<IAssignableIdentity, GameObject, TableScreen.ResultValues>(((TableScreen) this).get_value_checkbox_column_super), new Action<GameObject>(((TableScreen) this).on_press_checkbox_column_super), new Action<GameObject, TableScreen.ResultValues>(((TableScreen) this).set_value_checkbox_column_super), (Comparison<IAssignableIdentity>) null, new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_consumable_info_super));
  }

  private void refresh_scrollers()
  {
    int num = 0;
    foreach (EdiblesManager.FoodInfo allFoodType in EdiblesManager.GetAllFoodTypes())
    {
      if (DebugHandler.InstantBuildMode || ConsumerManager.instance.isDiscovered(TagExtensions.ToTag(allFoodType.ConsumableId)))
        ++num;
    }
    foreach (TableRow row in this.rows)
    {
      GameObject scroller = row.GetScroller("consumableScroller");
      if (Object.op_Inequality((Object) scroller, (Object) null))
      {
        ScrollRect component = ((Component) scroller.transform.parent).GetComponent<ScrollRect>();
        if (Object.op_Inequality((Object) component.horizontalScrollbar, (Object) null))
        {
          ((Component) component.horizontalScrollbar).gameObject.SetActive(num >= 12);
          row.GetScrollerBorder("consumableScroller").gameObject.SetActive(num >= 12);
        }
        component.horizontal = num >= 12;
        ((Behaviour) component).enabled = num >= 12;
      }
    }
  }

  private void on_load_qualityoflife_expectations(IAssignableIdentity minion, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
    if (minion != null)
      ((TMP_Text) componentInChildren).text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
    else
      ((TMP_Text) componentInChildren).text = widgetRow.isDefault ? "" : STRINGS.UI.VITALSSCREEN.QUALITYOFLIFE_EXPECTATIONS.ToString();
  }

  private string get_value_qualityoflife_label(IAssignableIdentity minion, GameObject widget_go)
  {
    string qualityoflifeLabel = "";
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    if (widgetRow.rowType == TableRow.RowType.Minion)
      qualityoflifeLabel = Db.Get().Attributes.QualityOfLife.Lookup((Component) (minion as MinionIdentity)).GetFormattedValue();
    else if (widgetRow.rowType == TableRow.RowType.StoredMinon)
      qualityoflifeLabel = (string) STRINGS.UI.TABLESCREENS.NA;
    return qualityoflifeLabel;
  }

  private int compare_rows_qualityoflife_expectations(IAssignableIdentity a, IAssignableIdentity b)
  {
    MinionIdentity cmp1 = a as MinionIdentity;
    MinionIdentity cmp2 = b as MinionIdentity;
    if (Object.op_Equality((Object) cmp1, (Object) null) && Object.op_Equality((Object) cmp2, (Object) null))
      return 0;
    if (Object.op_Equality((Object) cmp1, (Object) null))
      return -1;
    return Object.op_Equality((Object) cmp2, (Object) null) ? 1 : Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) cmp1).GetTotalValue().CompareTo(Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) cmp2).GetTotalValue());
  }

  protected void on_tooltip_qualityoflife_expectations(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Minion:
        MinionIdentity cmp = minion as MinionIdentity;
        if (!Object.op_Inequality((Object) cmp, (Object) null))
          break;
        tooltip.AddMultiStringTooltip(Db.Get().Attributes.QualityOfLife.Lookup((Component) cmp).GetAttributeValueTooltip(), (TextStyleSetting) null);
        break;
      case TableRow.RowType.StoredMinon:
        this.StoredMinionTooltip(minion, tooltip);
        break;
    }
  }

  protected void on_tooltip_sort_qualityoflife_expectations(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Header:
        tooltip.AddMultiStringTooltip((string) STRINGS.UI.TABLESCREENS.COLUMN_SORT_BY_EXPECTATIONS, (TextStyleSetting) null);
        break;
    }
  }

  private TableScreen.ResultValues get_value_food_info_super(
    MinionIdentity minion,
    GameObject widget_go)
  {
    SuperCheckboxTableColumn widgetColumn = this.GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    bool flag1 = true;
    bool flag2 = true;
    bool flag3 = false;
    bool flag4 = false;
    foreach (CheckboxTableColumn column in widgetColumn.columns_affected)
    {
      switch (column.get_value_action(widgetRow.GetIdentity(), widgetRow.GetWidget((TableColumn) column)))
      {
        case TableScreen.ResultValues.False:
          flag2 = false;
          if (!flag1)
          {
            flag4 = true;
            break;
          }
          break;
        case TableScreen.ResultValues.Partial:
          flag3 = true;
          flag4 = true;
          break;
        case TableScreen.ResultValues.True:
          flag1 = false;
          if (!flag2)
          {
            flag4 = true;
            break;
          }
          break;
      }
      if (flag4)
        break;
    }
    if (flag3)
      return TableScreen.ResultValues.Partial;
    if (flag2)
      return TableScreen.ResultValues.True;
    return flag1 ? TableScreen.ResultValues.False : TableScreen.ResultValues.Partial;
  }

  private void set_value_consumable_info(GameObject widget_go, TableScreen.ResultValues new_value)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    if (Object.op_Equality((Object) widgetRow, (Object) null))
    {
      Debug.LogWarning((object) "Row is null");
    }
    else
    {
      ConsumableInfoTableColumn widgetColumn = this.GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
      IAssignableIdentity identity = widgetRow.GetIdentity();
      IConsumableUIItem consumableInfo = widgetColumn.consumable_info;
      switch (widgetRow.rowType)
      {
        case TableRow.RowType.Header:
          this.set_value_consumable_info(this.default_row.GetComponent<TableRow>().GetWidget((TableColumn) widgetColumn), new_value);
          ((MonoBehaviour) this).StartCoroutine(this.CascadeSetColumnCheckBoxes(this.all_sortable_rows, (CheckboxTableColumn) widgetColumn, new_value, widget_go));
          break;
        case TableRow.RowType.Default:
          if (new_value == TableScreen.ResultValues.True)
            ConsumerManager.instance.DefaultForbiddenTagsList.Remove(TagExtensions.ToTag(consumableInfo.ConsumableId));
          else
            ConsumerManager.instance.DefaultForbiddenTagsList.Add(TagExtensions.ToTag(consumableInfo.ConsumableId));
          widgetColumn.on_load_action(identity, widget_go);
          using (Dictionary<TableRow, GameObject>.Enumerator enumerator = widgetColumn.widgets_by_row.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<TableRow, GameObject> current = enumerator.Current;
              if (current.Key.rowType == TableRow.RowType.Header)
              {
                widgetColumn.on_load_action((IAssignableIdentity) null, current.Value);
                break;
              }
            }
            break;
          }
        case TableRow.RowType.Minion:
          MinionIdentity minionIdentity = identity as MinionIdentity;
          if (!Object.op_Inequality((Object) minionIdentity, (Object) null))
            break;
          ConsumableConsumer component = ((Component) minionIdentity).GetComponent<ConsumableConsumer>();
          if (Object.op_Equality((Object) component, (Object) null))
          {
            Debug.LogError((object) "Could not find minion identity / row associated with the widget");
            break;
          }
          switch (new_value)
          {
            case TableScreen.ResultValues.False:
            case TableScreen.ResultValues.Partial:
              component.SetPermitted(consumableInfo.ConsumableId, false);
              break;
            case TableScreen.ResultValues.True:
            case TableScreen.ResultValues.ConditionalGroup:
              component.SetPermitted(consumableInfo.ConsumableId, true);
              break;
          }
          widgetColumn.on_load_action(widgetRow.GetIdentity(), widget_go);
          using (Dictionary<TableRow, GameObject>.Enumerator enumerator = widgetColumn.widgets_by_row.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<TableRow, GameObject> current = enumerator.Current;
              if (current.Key.rowType == TableRow.RowType.Header)
              {
                widgetColumn.on_load_action((IAssignableIdentity) null, current.Value);
                break;
              }
            }
            break;
          }
      }
    }
  }

  private void on_click_consumable_info(GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    IAssignableIdentity identity = widgetRow.GetIdentity();
    ConsumableInfoTableColumn widgetColumn = this.GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
    switch (widgetRow.rowType)
    {
      case TableRow.RowType.Header:
        switch (this.get_value_consumable_info((IAssignableIdentity) null, widget_go))
        {
          case TableScreen.ResultValues.False:
          case TableScreen.ResultValues.Partial:
          case TableScreen.ResultValues.ConditionalGroup:
            widgetColumn.on_set_action(widget_go, TableScreen.ResultValues.True);
            break;
          case TableScreen.ResultValues.True:
            widgetColumn.on_set_action(widget_go, TableScreen.ResultValues.False);
            break;
        }
        widgetColumn.on_load_action((IAssignableIdentity) null, widget_go);
        break;
      case TableRow.RowType.Default:
        IConsumableUIItem consumableInfo1 = widgetColumn.consumable_info;
        bool flag1 = !ConsumerManager.instance.DefaultForbiddenTagsList.Contains(TagExtensions.ToTag(consumableInfo1.ConsumableId));
        widgetColumn.on_set_action(widget_go, flag1 ? TableScreen.ResultValues.False : TableScreen.ResultValues.True);
        break;
      case TableRow.RowType.Minion:
        MinionIdentity minionIdentity = identity as MinionIdentity;
        if (!Object.op_Inequality((Object) minionIdentity, (Object) null))
          break;
        IConsumableUIItem consumableInfo2 = widgetColumn.consumable_info;
        ConsumableConsumer component = ((Component) minionIdentity).GetComponent<ConsumableConsumer>();
        if (Object.op_Equality((Object) component, (Object) null))
        {
          Debug.LogError((object) "Could not find minion identity / row associated with the widget");
          break;
        }
        bool flag2 = component.IsPermitted(consumableInfo2.ConsumableId);
        widgetColumn.on_set_action(widget_go, flag2 ? TableScreen.ResultValues.False : TableScreen.ResultValues.True);
        break;
      case TableRow.RowType.StoredMinon:
        StoredMinionIdentity storedMinionIdentity = identity as StoredMinionIdentity;
        if (!Object.op_Inequality((Object) storedMinionIdentity, (Object) null))
          break;
        IConsumableUIItem consumableInfo3 = widgetColumn.consumable_info;
        bool consume = storedMinionIdentity.IsPermittedToConsume(consumableInfo3.ConsumableId);
        widgetColumn.on_set_action(widget_go, consume ? TableScreen.ResultValues.False : TableScreen.ResultValues.True);
        break;
    }
  }

  private void on_tooltip_consumable_info(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    ConsumableInfoTableColumn widgetColumn = this.GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    EdiblesManager.FoodInfo consumableInfo = widgetColumn.consumable_info as EdiblesManager.FoodInfo;
    int num = 0;
    if (consumableInfo != null)
    {
      int quality = consumableInfo.Quality;
      MinionIdentity cmp = minion as MinionIdentity;
      if (Object.op_Inequality((Object) cmp, (Object) null))
      {
        AttributeInstance attributeInstance = cmp.GetAttributes().Get(Db.Get().Attributes.FoodExpectation);
        quality += Mathf.RoundToInt(attributeInstance.GetTotalValue());
      }
      string effectForFoodQuality = Edible.GetEffectForFoodQuality(quality);
      foreach (AttributeModifier selfModifier in Db.Get().effects.Get(effectForFoodQuality).SelfModifiers)
      {
        if (selfModifier.AttributeId == Db.Get().Attributes.QualityOfLife.Id)
          num += Mathf.RoundToInt(selfModifier.Value);
      }
    }
    switch (widgetRow.rowType)
    {
      case TableRow.RowType.Header:
        tooltip.AddMultiStringTooltip(widgetColumn.consumable_info.ConsumableName, (TextStyleSetting) null);
        if (consumableInfo != null)
        {
          tooltip.AddMultiStringTooltip(string.Format((string) STRINGS.UI.CONSUMABLESSCREEN.FOOD_AVAILABLE, (object) GameUtil.GetFormattedCalories(ClusterManager.Instance.activeWorld.worldInventory.GetAmount(TagExtensions.ToTag(widgetColumn.consumable_info.ConsumableId), false) * consumableInfo.CaloriesPerUnit)), (TextStyleSetting) null);
          tooltip.AddMultiStringTooltip(string.Format((string) STRINGS.UI.CONSUMABLESSCREEN.FOOD_QUALITY, (object) GameUtil.AddPositiveSign(num.ToString(), num > 0)), (TextStyleSetting) null);
          tooltip.AddMultiStringTooltip("\n" + consumableInfo.Description, (TextStyleSetting) null);
          break;
        }
        tooltip.AddMultiStringTooltip(string.Format((string) STRINGS.UI.CONSUMABLESSCREEN.FOOD_AVAILABLE, (object) GameUtil.GetFormattedUnits(ClusterManager.Instance.activeWorld.worldInventory.GetAmount(TagExtensions.ToTag(widgetColumn.consumable_info.ConsumableId), false))), (TextStyleSetting) null);
        break;
      case TableRow.RowType.Default:
        if (widgetColumn.get_value_action(minion, widget_go) == TableScreen.ResultValues.True)
        {
          tooltip.AddMultiStringTooltip(string.Format((string) STRINGS.UI.CONSUMABLESSCREEN.NEW_MINIONS_FOOD_PERMISSION_ON, (object) widgetColumn.consumable_info.ConsumableName), (TextStyleSetting) null);
          break;
        }
        tooltip.AddMultiStringTooltip(string.Format((string) STRINGS.UI.CONSUMABLESSCREEN.NEW_MINIONS_FOOD_PERMISSION_OFF, (object) widgetColumn.consumable_info.ConsumableName), (TextStyleSetting) null);
        break;
      case TableRow.RowType.Minion:
      case TableRow.RowType.StoredMinon:
        if (minion == null)
          break;
        if (widgetColumn.get_value_action(minion, widget_go) == TableScreen.ResultValues.True)
          tooltip.AddMultiStringTooltip(string.Format((string) STRINGS.UI.CONSUMABLESSCREEN.FOOD_PERMISSION_ON, (object) minion.GetProperName(), (object) widgetColumn.consumable_info.ConsumableName), (TextStyleSetting) null);
        else
          tooltip.AddMultiStringTooltip(string.Format((string) STRINGS.UI.CONSUMABLESSCREEN.FOOD_PERMISSION_OFF, (object) minion.GetProperName(), (object) widgetColumn.consumable_info.ConsumableName), (TextStyleSetting) null);
        if (consumableInfo != null && Object.op_Inequality((Object) (minion as MinionIdentity), (Object) null))
        {
          tooltip.AddMultiStringTooltip(string.Format((string) STRINGS.UI.CONSUMABLESSCREEN.FOOD_QUALITY_VS_EXPECTATION, (object) GameUtil.AddPositiveSign(num.ToString(), num > 0), (object) minion.GetProperName()), (TextStyleSetting) null);
          break;
        }
        if (!Object.op_Inequality((Object) (minion as StoredMinionIdentity), (Object) null))
          break;
        tooltip.AddMultiStringTooltip(string.Format((string) STRINGS.UI.CONSUMABLESSCREEN.CANNOT_ADJUST_PERMISSIONS, (object) (minion as StoredMinionIdentity).GetStorageReason()), (TextStyleSetting) null);
        break;
    }
  }

  private void on_tooltip_sort_consumable_info(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
  }

  private void on_tooltip_consumable_info_super(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Header:
        tooltip.AddMultiStringTooltip(STRINGS.UI.CONSUMABLESSCREEN.TOOLTIP_TOGGLE_ALL.text, (TextStyleSetting) null);
        break;
      case TableRow.RowType.Default:
        tooltip.AddMultiStringTooltip((string) STRINGS.UI.CONSUMABLESSCREEN.NEW_MINIONS_TOOLTIP_TOGGLE_ROW, (TextStyleSetting) null);
        break;
      case TableRow.RowType.Minion:
        if (!Object.op_Inequality((Object) (minion as MinionIdentity), (Object) null))
          break;
        tooltip.AddMultiStringTooltip(string.Format(STRINGS.UI.CONSUMABLESSCREEN.TOOLTIP_TOGGLE_ROW.text, (object) ((Component) (minion as MinionIdentity)).gameObject.GetProperName()), (TextStyleSetting) null);
        break;
    }
  }

  private void on_load_consumable_info(IAssignableIdentity minion, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    TableColumn widgetColumn = this.GetWidgetColumn(widget_go);
    IConsumableUIItem consumableInfo = (widgetColumn as ConsumableInfoTableColumn).consumable_info;
    EdiblesManager.FoodInfo foodInfo = consumableInfo as EdiblesManager.FoodInfo;
    MultiToggle component1 = widget_go.GetComponent<MultiToggle>();
    if (!widgetColumn.isRevealed)
    {
      widget_go.SetActive(false);
    }
    else
    {
      if (!widget_go.activeSelf)
        widget_go.SetActive(true);
      switch (widgetRow.rowType)
      {
        case TableRow.RowType.Header:
          GameObject prefab = Assets.GetPrefab(TagExtensions.ToTag(consumableInfo.ConsumableId));
          if (Object.op_Equality((Object) prefab, (Object) null))
            return;
          KBatchedAnimController component2 = prefab.GetComponent<KBatchedAnimController>();
          Image reference1 = widget_go.GetComponent<HierarchyReferences>().GetReference("PortraitImage") as Image;
          if (component2.AnimFiles.Length != 0)
          {
            Sprite fromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(component2.AnimFiles[0]);
            reference1.sprite = fromMultiObjectAnim;
          }
          ((Graphic) reference1).color = Color.white;
          ((Graphic) reference1).material = (double) ClusterManager.Instance.activeWorld.worldInventory.GetAmount(TagExtensions.ToTag(consumableInfo.ConsumableId), false) > 0.0 ? Assets.UIPrefabs.TableScreenWidgets.DefaultUIMaterial : Assets.UIPrefabs.TableScreenWidgets.DesaturatedUIMaterial;
          break;
        case TableRow.RowType.Default:
          switch (this.get_value_consumable_info(minion, widget_go))
          {
            case TableScreen.ResultValues.False:
              component1.ChangeState(0);
              break;
            case TableScreen.ResultValues.True:
              component1.ChangeState(1);
              break;
            case TableScreen.ResultValues.ConditionalGroup:
              component1.ChangeState(2);
              break;
          }
          break;
        case TableRow.RowType.Minion:
        case TableRow.RowType.StoredMinon:
          switch (this.get_value_consumable_info(minion, widget_go))
          {
            case TableScreen.ResultValues.False:
              component1.ChangeState(0);
              break;
            case TableScreen.ResultValues.True:
              component1.ChangeState(1);
              break;
            case TableScreen.ResultValues.ConditionalGroup:
              component1.ChangeState(2);
              break;
          }
          if (foodInfo != null && Object.op_Inequality((Object) (minion as MinionIdentity), (Object) null))
          {
            Image reference2 = widget_go.GetComponent<HierarchyReferences>().GetReference("BGImage") as Image;
            Color color1;
            // ISSUE: explicit constructor call
            ((Color) ref color1).\u002Ector(0.721568644f, 0.443137258f, 0.5803922f, Mathf.Max((float) ((double) foodInfo.Quality - (double) Db.Get().Attributes.FoodExpectation.Lookup((Component) (minion as MinionIdentity)).GetTotalValue() + 1.0), 0.0f) * 0.25f);
            Color color2 = color1;
            ((Graphic) reference2).color = color2;
            break;
          }
          break;
      }
      this.refresh_scrollers();
    }
  }

  private int compare_consumable_info(IAssignableIdentity a, IAssignableIdentity b) => 0;

  private TableScreen.ResultValues get_value_consumable_info(
    IAssignableIdentity minion,
    GameObject widget_go)
  {
    ConsumableInfoTableColumn widgetColumn = this.GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
    IConsumableUIItem consumableInfo = widgetColumn.consumable_info;
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    TableScreen.ResultValues valueConsumableInfo = TableScreen.ResultValues.Partial;
    switch (widgetRow.rowType)
    {
      case TableRow.RowType.Header:
        bool flag1 = true;
        bool flag2 = true;
        bool flag3 = false;
        bool flag4 = false;
        foreach (KeyValuePair<TableRow, GameObject> keyValuePair in widgetColumn.widgets_by_row)
        {
          GameObject gameObject = keyValuePair.Value;
          if (!Object.op_Equality((Object) gameObject, (Object) widget_go) && !Object.op_Equality((Object) gameObject, (Object) null))
          {
            switch (widgetColumn.get_value_action(keyValuePair.Key.GetIdentity(), gameObject))
            {
              case TableScreen.ResultValues.False:
                flag2 = false;
                if (!flag1)
                {
                  flag4 = true;
                  break;
                }
                break;
              case TableScreen.ResultValues.Partial:
                flag3 = true;
                flag4 = true;
                break;
              case TableScreen.ResultValues.True:
                flag1 = false;
                if (!flag2)
                {
                  flag4 = true;
                  break;
                }
                break;
            }
            if (flag4)
              break;
          }
        }
        valueConsumableInfo = !flag3 ? (!flag2 ? (!flag1 ? TableScreen.ResultValues.Partial : TableScreen.ResultValues.False) : TableScreen.ResultValues.True) : TableScreen.ResultValues.Partial;
        break;
      case TableRow.RowType.Default:
        valueConsumableInfo = ConsumerManager.instance.DefaultForbiddenTagsList.Contains(TagExtensions.ToTag(consumableInfo.ConsumableId)) ? TableScreen.ResultValues.False : TableScreen.ResultValues.True;
        break;
      case TableRow.RowType.Minion:
        valueConsumableInfo = !Object.op_Inequality((Object) (minion as MinionIdentity), (Object) null) ? TableScreen.ResultValues.True : (((Component) minion).GetComponent<ConsumableConsumer>().IsPermitted(consumableInfo.ConsumableId) ? TableScreen.ResultValues.True : TableScreen.ResultValues.False);
        break;
      case TableRow.RowType.StoredMinon:
        valueConsumableInfo = !Object.op_Inequality((Object) (minion as StoredMinionIdentity), (Object) null) ? TableScreen.ResultValues.True : (((StoredMinionIdentity) minion).IsPermittedToConsume(consumableInfo.ConsumableId) ? TableScreen.ResultValues.True : TableScreen.ResultValues.False);
        break;
    }
    return valueConsumableInfo;
  }

  protected void on_tooltip_name(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Minion:
        if (minion == null)
          break;
        tooltip.AddMultiStringTooltip(string.Format((string) STRINGS.UI.TABLESCREENS.GOTO_DUPLICANT_BUTTON, (object) minion.GetProperName()), (TextStyleSetting) null);
        break;
    }
  }

  protected ConsumableInfoTableColumn AddConsumableInfoColumn(
    string id,
    IConsumableUIItem consumable_info,
    Action<IAssignableIdentity, GameObject> load_value_action,
    Func<IAssignableIdentity, GameObject, TableScreen.ResultValues> get_value_action,
    Action<GameObject> on_press_action,
    Action<GameObject, TableScreen.ResultValues> set_value_action,
    Comparison<IAssignableIdentity> sort_comparison,
    Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip,
    Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip)
  {
    ConsumableInfoTableColumn new_column = new ConsumableInfoTableColumn(consumable_info, load_value_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip, on_sort_tooltip, (Func<GameObject, string>) (widget_go => ""));
    new_column.scrollerID = "consumableScroller";
    return this.RegisterColumn(id, (TableColumn) new_column) ? new_column : (ConsumableInfoTableColumn) null;
  }

  private void OnConsumableDiscovered(Tag tag) => this.MarkRowsDirty();

  private void StoredMinionTooltip(IAssignableIdentity minion, ToolTip tooltip)
  {
    StoredMinionIdentity storedMinionIdentity = minion as StoredMinionIdentity;
    if (!Object.op_Inequality((Object) storedMinionIdentity, (Object) null))
      return;
    tooltip.AddMultiStringTooltip(string.Format((string) STRINGS.UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, (object) storedMinionIdentity.GetStorageReason(), (object) storedMinionIdentity.GetProperName()), (TextStyleSetting) null);
  }
}
