// Decompiled with JetBrains decompiler
// Type: SelectToolHoverTextCard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectToolHoverTextCard : HoverTextConfiguration
{
  public static int maxNumberOfDisplayedSelectableWarnings = 10;
  private Dictionary<HashedString, Func<bool>> overlayFilterMap;
  public int recentNumberOfDisplayedSelectables;
  public int currentSelectedSelectableIndex;
  [NonSerialized]
  public Sprite iconWarning;
  [NonSerialized]
  public Sprite iconDash;
  [NonSerialized]
  public Sprite iconHighlighted;
  [NonSerialized]
  public Sprite iconActiveAutomationPort;
  public HoverTextConfiguration.TextStylePair Styles_LogicActive;
  public HoverTextConfiguration.TextStylePair Styles_LogicStandby;
  public TextStyleSetting Styles_LogicSignalInactive;
  public static List<GameObject> highlightedObjects = new List<GameObject>();
  private static readonly List<System.Type> hiddenChoreConsumerTypes = new List<System.Type>()
  {
    typeof (KSelectableHealthBar)
  };
  private int maskOverlay;
  private string cachedTemperatureString;
  private float cachedTemperature;
  private List<KSelectable> overlayValidHoverObjects;
  private Dictionary<HashedString, Func<KSelectable, bool>> modeFilters;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.overlayFilterMap.Add(OverlayModes.Oxygen.ID, (Func<bool>) (() =>
    {
      int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
      return Grid.Element[cell].IsGas;
    }));
    this.overlayFilterMap.Add(OverlayModes.GasConduits.ID, (Func<bool>) (() =>
    {
      int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
      return Grid.Element[cell].IsGas;
    }));
    this.overlayFilterMap.Add(OverlayModes.Radiation.ID, (Func<bool>) (() =>
    {
      int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
      return (double) Grid.Radiation[cell] > 0.0;
    }));
    this.overlayFilterMap.Add(OverlayModes.LiquidConduits.ID, (Func<bool>) (() =>
    {
      int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
      return Grid.Element[cell].IsLiquid;
    }));
    this.overlayFilterMap.Add(OverlayModes.Decor.ID, (Func<bool>) (() => false));
    this.overlayFilterMap.Add(OverlayModes.Rooms.ID, (Func<bool>) (() => false));
    this.overlayFilterMap.Add(OverlayModes.Logic.ID, (Func<bool>) (() => false));
    this.overlayFilterMap.Add(OverlayModes.TileMode.ID, (Func<bool>) (() =>
    {
      int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
      Element element = Grid.Element[cell];
      foreach (Tag tileOverlayFilter in Game.Instance.tileOverlayFilters)
      {
        if (element.HasTag(tileOverlayFilter))
          return true;
      }
      return false;
    }));
  }

  public override void ConfigureHoverScreen()
  {
    base.ConfigureHoverScreen();
    HoverTextScreen instance = HoverTextScreen.Instance;
    this.iconWarning = instance.GetSprite("iconWarning");
    this.iconDash = instance.GetSprite("dash");
    this.iconHighlighted = instance.GetSprite("dash_arrow");
    this.iconActiveAutomationPort = instance.GetSprite("current_automation_state_arrow");
    this.maskOverlay = LayerMask.GetMask(new string[2]
    {
      "MaskedOverlay",
      "MaskedOverlayBG"
    });
  }

  private bool IsStatusItemWarning(StatusItemGroup.Entry item) => item.item.notificationType == NotificationType.Bad || item.item.notificationType == NotificationType.BadMinor || item.item.notificationType == NotificationType.DuplicantThreatening;

  public override void UpdateHoverElements(List<KSelectable> hoverObjects)
  {
    if (Object.op_Equality((Object) this.iconWarning, (Object) null))
      this.ConfigureHoverScreen();
    int cell1 = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
    if (Object.op_Equality((Object) OverlayScreen.Instance, (Object) null) || !Grid.IsValidCell(cell1))
      return;
    HoverTextDrawer drawer = HoverTextScreen.Instance.BeginDrawing();
    this.overlayValidHoverObjects.Clear();
    foreach (KSelectable hoverObject in hoverObjects)
    {
      if (this.ShouldShowSelectableInCurrentOverlay(hoverObject))
        this.overlayValidHoverObjects.Add(hoverObject);
    }
    this.currentSelectedSelectableIndex = -1;
    if (SelectToolHoverTextCard.highlightedObjects.Count > 0)
      SelectToolHoverTextCard.highlightedObjects.Clear();
    HashedString mode = SimDebugView.Instance.GetMode();
    bool flag1 = HashedString.op_Equality(mode, OverlayModes.Disease.ID);
    bool flag2 = true;
    if (Grid.DupePassable[cell1] && Grid.Solid[cell1])
      flag2 = false;
    bool flag3 = Grid.IsVisible(cell1);
    if ((int) Grid.WorldIdx[cell1] != ClusterManager.Instance.activeWorldId)
      flag3 = false;
    if (!flag3)
      flag2 = false;
    foreach (KeyValuePair<HashedString, Func<bool>> overlayFilter in this.overlayFilterMap)
    {
      if (HashedString.op_Equality(OverlayScreen.Instance.GetMode(), overlayFilter.Key))
      {
        if (!overlayFilter.Value())
        {
          flag2 = false;
          break;
        }
        break;
      }
    }
    string str1 = "";
    if (HashedString.op_Equality(mode, OverlayModes.Temperature.ID) && Game.Instance.temperatureOverlayMode == Game.TemperatureOverlayModes.HeatFlow)
    {
      if (!Grid.Solid[cell1] & flag3)
      {
        float thermalComfort1 = GameUtil.GetThermalComfort(cell1, 0.0f);
        float thermalComfort2 = GameUtil.GetThermalComfort(cell1);
        float num = 0.0f;
        if ((double) thermalComfort2 * (1.0 / 1000.0) > -0.27893334627151489 - (double) num && (double) thermalComfort2 * (1.0 / 1000.0) < 0.27893334627151489 + (double) num)
          str1 = (string) UI.OVERLAYS.HEATFLOW.NEUTRAL;
        else if ((double) thermalComfort2 <= (double) ExternalTemperatureMonitor.GetExternalColdThreshold((Attributes) null))
          str1 = (string) UI.OVERLAYS.HEATFLOW.COOLING;
        else if ((double) thermalComfort2 >= (double) ExternalTemperatureMonitor.GetExternalWarmThreshold((Attributes) null))
          str1 = (string) UI.OVERLAYS.HEATFLOW.HEATING;
        float dtu_s = 1f * thermalComfort1;
        string text = str1 + " (" + GameUtil.GetFormattedHeatEnergyRate(dtu_s) + ")";
        drawer.BeginShadowBar();
        drawer.DrawText((string) UI.OVERLAYS.HEATFLOW.HOVERTITLE, this.Styles_Title.Standard);
        drawer.NewLine();
        drawer.DrawText(text, this.Styles_BodyText.Standard);
        drawer.EndShadowBar();
      }
    }
    else if (HashedString.op_Equality(mode, OverlayModes.Decor.ID))
    {
      List<DecorProvider> event_data = new List<DecorProvider>();
      GameScenePartitioner.Instance.TriggerEvent(cell1, GameScenePartitioner.Instance.decorProviderLayer, (object) event_data);
      float decorAtCell = GameUtil.GetDecorAtCell(cell1);
      drawer.BeginShadowBar();
      drawer.DrawText((string) UI.OVERLAYS.DECOR.HOVERTITLE, this.Styles_Title.Standard);
      drawer.NewLine();
      drawer.DrawText((string) UI.OVERLAYS.DECOR.TOTAL + GameUtil.GetFormattedDecor(decorAtCell, true), this.Styles_BodyText.Standard);
      if (!Grid.Solid[cell1] & flag3)
      {
        List<EffectorEntry> effectorEntryList1 = new List<EffectorEntry>();
        List<EffectorEntry> effectorEntryList2 = new List<EffectorEntry>();
        foreach (DecorProvider decorProvider in event_data)
        {
          float decorForCell = decorProvider.GetDecorForCell(cell1);
          if ((double) decorForCell != 0.0)
          {
            string name = decorProvider.GetName();
            KMonoBehaviour component = ((Component) decorProvider).GetComponent<KMonoBehaviour>();
            if (Object.op_Inequality((Object) component, (Object) null) && Object.op_Inequality((Object) ((Component) component).gameObject, (Object) null))
            {
              SelectToolHoverTextCard.highlightedObjects.Add(((Component) component).gameObject);
              if (Object.op_Inequality((Object) ((Component) component).GetComponent<MonumentPart>(), (Object) null) && ((Component) component).GetComponent<MonumentPart>().IsMonumentCompleted())
              {
                name = (string) MISC.MONUMENT_COMPLETE.NAME;
                foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) component).GetComponent<AttachableBuilding>()))
                  SelectToolHoverTextCard.highlightedObjects.Add(gameObject);
              }
            }
            bool flag4 = false;
            if ((double) decorForCell > 0.0)
            {
              for (int index = 0; index < effectorEntryList1.Count; ++index)
              {
                if (effectorEntryList1[index].name == name)
                {
                  EffectorEntry effectorEntry = effectorEntryList1[index];
                  ++effectorEntry.count;
                  effectorEntry.value += decorForCell;
                  effectorEntryList1[index] = effectorEntry;
                  flag4 = true;
                  break;
                }
              }
              if (!flag4)
                effectorEntryList1.Add(new EffectorEntry(name, decorForCell));
            }
            else
            {
              for (int index = 0; index < effectorEntryList2.Count; ++index)
              {
                if (effectorEntryList2[index].name == name)
                {
                  EffectorEntry effectorEntry = effectorEntryList2[index];
                  ++effectorEntry.count;
                  effectorEntry.value += decorForCell;
                  effectorEntryList2[index] = effectorEntry;
                  flag4 = true;
                  break;
                }
              }
              if (!flag4)
                effectorEntryList2.Add(new EffectorEntry(name, decorForCell));
            }
          }
        }
        int lightDecorBonus = DecorProvider.GetLightDecorBonus(cell1);
        if (lightDecorBonus > 0)
          effectorEntryList1.Add(new EffectorEntry((string) UI.OVERLAYS.DECOR.LIGHTING, (float) lightDecorBonus));
        effectorEntryList1.Sort((Comparison<EffectorEntry>) ((x, y) => y.value.CompareTo(x.value)));
        if (effectorEntryList1.Count > 0)
        {
          drawer.NewLine();
          drawer.DrawText((string) UI.OVERLAYS.DECOR.HEADER_POSITIVE, this.Styles_BodyText.Standard);
        }
        foreach (EffectorEntry effectorEntry in effectorEntryList1)
        {
          drawer.NewLine(18);
          drawer.DrawIcon(this.iconDash);
          drawer.DrawText(effectorEntry.ToString(), this.Styles_BodyText.Standard);
        }
        effectorEntryList2.Sort((Comparison<EffectorEntry>) ((x, y) => Mathf.Abs(y.value).CompareTo(Mathf.Abs(x.value))));
        if (effectorEntryList2.Count > 0)
        {
          drawer.NewLine();
          drawer.DrawText((string) UI.OVERLAYS.DECOR.HEADER_NEGATIVE, this.Styles_BodyText.Standard);
        }
        foreach (EffectorEntry effectorEntry in effectorEntryList2)
        {
          drawer.NewLine(18);
          drawer.DrawIcon(this.iconDash);
          drawer.DrawText(effectorEntry.ToString(), this.Styles_BodyText.Standard);
        }
      }
      drawer.EndShadowBar();
    }
    else if (HashedString.op_Equality(mode, OverlayModes.Rooms.ID))
    {
      CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell1);
      if (cavityForCell != null)
      {
        Room room = cavityForCell.room;
        RoomType roomType = (RoomType) null;
        string text1;
        if (room != null)
        {
          roomType = room.roomType;
          text1 = roomType.Name;
        }
        else
          text1 = (string) UI.OVERLAYS.ROOMS.NOROOM.HEADER;
        drawer.BeginShadowBar();
        drawer.DrawText(text1, this.Styles_Title.Standard);
        if (room != null)
        {
          string text2 = RoomDetails.EFFECT.resolve_string_function(room);
          string text3 = RoomDetails.ASSIGNED_TO.resolve_string_function(room);
          string text4 = RoomConstraints.RoomCriteriaString(room);
          string text5 = RoomDetails.EFFECTS.resolve_string_function(room);
          if (text2 != "")
          {
            drawer.NewLine();
            drawer.DrawText(text2, this.Styles_BodyText.Standard);
          }
          if (text3 != "" && roomType != Db.Get().RoomTypes.Neutral)
          {
            drawer.NewLine();
            drawer.DrawText(text3, this.Styles_BodyText.Standard);
          }
          drawer.NewLine(22);
          drawer.DrawText(RoomDetails.RoomDetailString(room), this.Styles_BodyText.Standard);
          if (text4 != "")
          {
            drawer.NewLine();
            drawer.DrawText(text4, this.Styles_BodyText.Standard);
          }
          if (text5 != "")
          {
            drawer.NewLine();
            drawer.DrawText(text5, this.Styles_BodyText.Standard);
          }
        }
        else
        {
          string text6 = (string) UI.OVERLAYS.ROOMS.NOROOM.DESC;
          int maxRoomSize = TuningData<RoomProber.Tuning>.Get().maxRoomSize;
          if (cavityForCell.numCells > maxRoomSize)
            text6 = text6 + "\n" + string.Format((string) UI.OVERLAYS.ROOMS.NOROOM.TOO_BIG, (object) cavityForCell.numCells, (object) maxRoomSize);
          drawer.NewLine();
          drawer.DrawText(text6, this.Styles_BodyText.Standard);
        }
        drawer.EndShadowBar();
      }
    }
    else if (HashedString.op_Equality(mode, OverlayModes.Light.ID))
    {
      if (flag3)
      {
        string text = str1 + string.Format((string) UI.OVERLAYS.LIGHTING.DESC, (object) Grid.LightIntensity[cell1]) + " (" + GameUtil.GetLightDescription(Grid.LightIntensity[cell1]) + ")";
        drawer.BeginShadowBar();
        drawer.DrawText((string) UI.OVERLAYS.LIGHTING.HOVERTITLE, this.Styles_Title.Standard);
        drawer.NewLine();
        drawer.DrawText(text, this.Styles_BodyText.Standard);
        drawer.EndShadowBar();
      }
    }
    else if (HashedString.op_Equality(mode, OverlayModes.Radiation.ID))
    {
      if (flag3)
      {
        flag2 = true;
        string text7 = str1 + UI.OVERLAYS.RADIATION.DESC.Replace("{rads}", GameUtil.GetFormattedRads(Grid.Radiation[cell1])).Replace("{description}", GameUtil.GetRadiationDescription(Grid.Radiation[cell1]));
        string text8 = UI.OVERLAYS.RADIATION.SHIELDING_DESC.Replace("{radiationAbsorptionFactor}", GameUtil.GetFormattedPercent(GameUtil.GetRadiationAbsorptionPercentage(cell1) * 100f));
        drawer.BeginShadowBar();
        drawer.DrawText((string) UI.OVERLAYS.RADIATION.HOVERTITLE, this.Styles_Title.Standard);
        drawer.NewLine();
        drawer.DrawText(text7, this.Styles_BodyText.Standard);
        drawer.NewLine();
        drawer.DrawText(text8, this.Styles_BodyText.Standard);
        drawer.EndShadowBar();
      }
    }
    else if (HashedString.op_Equality(mode, OverlayModes.Logic.ID))
    {
      foreach (KSelectable hoverObject in hoverObjects)
      {
        LogicPorts component1 = ((Component) hoverObject).GetComponent<LogicPorts>();
        LogicPorts.Port port1;
        bool isInput;
        if (Object.op_Inequality((Object) component1, (Object) null) && component1.TryGetPortAtCell(cell1, out port1, out isInput))
        {
          bool flag5 = component1.IsPortConnected(port1.id);
          drawer.BeginShadowBar();
          int num;
          if (isInput)
          {
            string str2 = port1.displayCustomName ? port1.description : UI.LOGIC_PORTS.PORT_INPUT_DEFAULT_NAME.text;
            num = component1.GetInputValue(port1.id);
            drawer.DrawText(UI.TOOLS.GENERIC.LOGIC_INPUT_HOVER_FMT.Replace("{Port}", str2.ToUpper()).Replace("{Name}", hoverObject.GetProperName().ToUpper()), this.Styles_Title.Standard);
          }
          else
          {
            string str3 = port1.displayCustomName ? port1.description : UI.LOGIC_PORTS.PORT_OUTPUT_DEFAULT_NAME.text;
            num = component1.GetOutputValue(port1.id);
            drawer.DrawText(UI.TOOLS.GENERIC.LOGIC_OUTPUT_HOVER_FMT.Replace("{Port}", str3.ToUpper()).Replace("{Name}", hoverObject.GetProperName().ToUpper()), this.Styles_Title.Standard);
          }
          drawer.NewLine();
          TextStyleSetting style1 = !flag5 ? this.Styles_LogicActive.Standard : (num == 1 ? this.Styles_LogicActive.Selected : this.Styles_LogicSignalInactive);
          this.DrawLogicIcon(drawer, num == 1 & flag5 ? this.iconActiveAutomationPort : this.iconDash, style1);
          this.DrawLogicText(drawer, port1.activeDescription, style1);
          drawer.NewLine();
          TextStyleSetting style2 = !flag5 ? this.Styles_LogicStandby.Standard : (num == 0 ? this.Styles_LogicStandby.Selected : this.Styles_LogicSignalInactive);
          this.DrawLogicIcon(drawer, num == 0 & flag5 ? this.iconActiveAutomationPort : this.iconDash, style2);
          this.DrawLogicText(drawer, port1.inactiveDescription, style2);
          drawer.EndShadowBar();
        }
        LogicGate component2 = ((Component) hoverObject).GetComponent<LogicGate>();
        LogicGateBase.PortId port2;
        if (Object.op_Inequality((Object) component2, (Object) null) && component2.TryGetPortAtCell(cell1, out port2))
        {
          int portValue = component2.GetPortValue(port2);
          bool portConnected = component2.GetPortConnected(port2);
          LogicGate.LogicGateDescriptions.Description portDescription = component2.GetPortDescription(port2);
          drawer.BeginShadowBar();
          if (port2 == LogicGateBase.PortId.OutputOne)
            drawer.DrawText(UI.TOOLS.GENERIC.LOGIC_MULTI_OUTPUT_HOVER_FMT.Replace("{Port}", portDescription.name.ToUpper()).Replace("{Name}", hoverObject.GetProperName().ToUpper()), this.Styles_Title.Standard);
          else
            drawer.DrawText(UI.TOOLS.GENERIC.LOGIC_MULTI_INPUT_HOVER_FMT.Replace("{Port}", portDescription.name.ToUpper()).Replace("{Name}", hoverObject.GetProperName().ToUpper()), this.Styles_Title.Standard);
          drawer.NewLine();
          TextStyleSetting style3 = !portConnected ? this.Styles_LogicActive.Standard : (portValue == 1 ? this.Styles_LogicActive.Selected : this.Styles_LogicSignalInactive);
          this.DrawLogicIcon(drawer, portValue == 1 & portConnected ? this.iconActiveAutomationPort : this.iconDash, style3);
          this.DrawLogicText(drawer, portDescription.active, style3);
          drawer.NewLine();
          TextStyleSetting style4 = !portConnected ? this.Styles_LogicStandby.Standard : (portValue == 0 ? this.Styles_LogicStandby.Selected : this.Styles_LogicSignalInactive);
          this.DrawLogicIcon(drawer, portValue == 0 & portConnected ? this.iconActiveAutomationPort : this.iconDash, style4);
          this.DrawLogicText(drawer, portDescription.inactive, style4);
          drawer.EndShadowBar();
        }
      }
    }
    int num1 = 0;
    ChoreConsumer choreConsumer = (ChoreConsumer) null;
    if (Object.op_Inequality((Object) SelectTool.Instance.selected, (Object) null))
      choreConsumer = ((Component) SelectTool.Instance.selected).GetComponent<ChoreConsumer>();
    for (int index1 = 0; index1 < this.overlayValidHoverObjects.Count; ++index1)
    {
      if (Object.op_Inequality((Object) this.overlayValidHoverObjects[index1], (Object) null) && !CellSelectionObject.IsSelectionObject(((Component) this.overlayValidHoverObjects[index1]).gameObject))
      {
        KSelectable validHoverObject = this.overlayValidHoverObjects[index1];
        if ((!Object.op_Inequality((Object) OverlayScreen.Instance, (Object) null) || !HashedString.op_Inequality(OverlayScreen.Instance.mode, OverlayModes.None.ID) || (((Component) validHoverObject).gameObject.layer & this.maskOverlay) == 0) && flag3)
        {
          PrimaryElement component3 = ((Component) validHoverObject).GetComponent<PrimaryElement>();
          bool selected = Object.op_Equality((Object) SelectTool.Instance.selected, (Object) this.overlayValidHoverObjects[index1]);
          if (selected)
            this.currentSelectedSelectableIndex = index1;
          ++num1;
          drawer.BeginShadowBar(selected);
          string text9 = GameUtil.GetUnitFormattedName(((Component) this.overlayValidHoverObjects[index1]).gameObject, true);
          if (Object.op_Inequality((Object) component3, (Object) null) && Object.op_Inequality((Object) ((Component) validHoverObject).GetComponent<Building>(), (Object) null))
            text9 = StringFormatter.Replace(StringFormatter.Replace((string) UI.TOOLS.GENERIC.BUILDING_HOVER_NAME_FMT, "{Name}", text9), "{Element}", component3.Element.nameUpperCase);
          drawer.DrawText(text9, this.Styles_Title.Standard);
          bool flag6 = false;
          string text10 = (string) UI.OVERLAYS.DISEASE.NO_DISEASE;
          if (flag1)
          {
            if (Object.op_Inequality((Object) component3, (Object) null) && component3.DiseaseIdx != byte.MaxValue)
              text10 = GameUtil.GetFormattedDisease(component3.DiseaseIdx, component3.DiseaseCount, true);
            flag6 = true;
            Storage component4 = ((Component) validHoverObject).GetComponent<Storage>();
            if (Object.op_Inequality((Object) component4, (Object) null) && component4.showInUI)
            {
              List<GameObject> items = component4.items;
              for (int index2 = 0; index2 < items.Count; ++index2)
              {
                GameObject gameObject = items[index2];
                if (Object.op_Inequality((Object) gameObject, (Object) null))
                {
                  PrimaryElement component5 = gameObject.GetComponent<PrimaryElement>();
                  if (component5.DiseaseIdx != byte.MaxValue)
                    text10 += string.Format((string) UI.OVERLAYS.DISEASE.CONTAINER_FORMAT, (object) gameObject.GetComponent<KSelectable>().GetProperName(), (object) GameUtil.GetFormattedDisease(component5.DiseaseIdx, component5.DiseaseCount, true));
                }
              }
            }
          }
          if (flag6)
          {
            drawer.NewLine();
            drawer.DrawIcon(this.iconDash);
            drawer.DrawText(text10, this.Styles_Values.Property.Standard);
          }
          int num2 = 0;
          foreach (StatusItemGroup.Entry entry in this.overlayValidHoverObjects[index1].GetStatusItemGroup())
          {
            if (this.ShowStatusItemInCurrentOverlay(entry.item))
            {
              if (num2 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
              {
                if (entry.category != null && entry.category.Id == "Main" && num2 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
                {
                  TextStyleSetting style = this.IsStatusItemWarning(entry) ? this.HoverTextStyleSettings[1] : this.Styles_BodyText.Standard;
                  Sprite icon = entry.item.sprite != null ? entry.item.sprite.sprite : this.iconWarning;
                  Color color = this.IsStatusItemWarning(entry) ? this.HoverTextStyleSettings[1].textColor : this.Styles_BodyText.Standard.textColor;
                  drawer.NewLine();
                  drawer.DrawIcon(icon, color);
                  drawer.DrawText(entry.GetName(), style);
                  ++num2;
                }
              }
              else
                break;
            }
          }
          foreach (StatusItemGroup.Entry entry in this.overlayValidHoverObjects[index1].GetStatusItemGroup())
          {
            if (this.ShowStatusItemInCurrentOverlay(entry.item))
            {
              if (num2 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
              {
                if ((entry.category == null || entry.category.Id != "Main") && num2 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
                {
                  TextStyleSetting style = this.IsStatusItemWarning(entry) ? this.HoverTextStyleSettings[1] : this.Styles_BodyText.Standard;
                  Sprite icon = entry.item.sprite != null ? entry.item.sprite.sprite : this.iconWarning;
                  Color color = this.IsStatusItemWarning(entry) ? this.HoverTextStyleSettings[1].textColor : this.Styles_BodyText.Standard.textColor;
                  drawer.NewLine();
                  drawer.DrawIcon(icon, color);
                  drawer.DrawText(entry.GetName(), style);
                  ++num2;
                }
              }
              else
                break;
            }
          }
          float temp = 0.0f;
          bool flag7 = true;
          bool flag8 = HashedString.op_Equality(OverlayModes.Temperature.ID, SimDebugView.Instance.GetMode()) && Game.Instance.temperatureOverlayMode != Game.TemperatureOverlayModes.HeatFlow;
          if (Object.op_Implicit((Object) ((Component) validHoverObject).GetComponent<Constructable>()))
            flag7 = false;
          else if (flag8 && Object.op_Implicit((Object) component3))
            temp = component3.Temperature;
          else if (Object.op_Implicit((Object) ((Component) validHoverObject).GetComponent<Building>()) && Object.op_Implicit((Object) component3))
            temp = component3.Temperature;
          else if (CellSelectionObject.IsSelectionObject(((Component) validHoverObject).gameObject))
            temp = ((Component) validHoverObject).GetComponent<CellSelectionObject>().temperature;
          else
            flag7 = false;
          if (HashedString.op_Inequality(mode, OverlayModes.None.ID) && HashedString.op_Inequality(mode, OverlayModes.Temperature.ID))
            flag7 = false;
          if (flag7)
          {
            drawer.NewLine();
            drawer.DrawIcon(this.iconDash);
            drawer.DrawText(GameUtil.GetFormattedTemperature(temp), this.Styles_BodyText.Standard);
          }
          BuildingComplete component6 = ((Component) validHoverObject).GetComponent<BuildingComplete>();
          if (Object.op_Inequality((Object) component6, (Object) null) && component6.Def.IsFoundation && Grid.Element[cell1].IsSolid)
            flag2 = false;
          if (HashedString.op_Equality(mode, OverlayModes.Light.ID) && Object.op_Inequality((Object) choreConsumer, (Object) null))
          {
            bool flag9 = false;
            foreach (System.Type choreConsumerType in SelectToolHoverTextCard.hiddenChoreConsumerTypes)
            {
              if (Object.op_Inequality((Object) ((Component) choreConsumer).gameObject.GetComponent(choreConsumerType), (Object) null))
              {
                flag9 = true;
                break;
              }
            }
            if (!flag9)
              choreConsumer.ShowHoverTextOnHoveredItem(validHoverObject, drawer, this);
          }
          drawer.EndShadowBar();
        }
      }
    }
    if (flag2)
    {
      CellSelectionObject cellSelectionObject = (CellSelectionObject) null;
      if (Object.op_Inequality((Object) SelectTool.Instance.selected, (Object) null))
        cellSelectionObject = ((Component) SelectTool.Instance.selected).GetComponent<CellSelectionObject>();
      bool selected = Object.op_Inequality((Object) cellSelectionObject, (Object) null) && cellSelectionObject.mouseCell == cellSelectionObject.alternateSelectionObject.mouseCell;
      if (selected)
        this.currentSelectedSelectableIndex = this.recentNumberOfDisplayedSelectables - 1;
      Element element1 = Grid.Element[cell1];
      drawer.BeginShadowBar(selected);
      drawer.DrawText(element1.nameUpperCase, this.Styles_Title.Standard);
      if (Grid.DiseaseCount[cell1] > 0 | flag1)
      {
        drawer.NewLine();
        drawer.DrawIcon(this.iconDash);
        drawer.DrawText(GameUtil.GetFormattedDisease(Grid.DiseaseIdx[cell1], Grid.DiseaseCount[cell1], true), this.Styles_Values.Property.Standard);
      }
      if (!element1.IsVacuum)
      {
        drawer.NewLine();
        drawer.DrawIcon(this.iconDash);
        drawer.DrawText(ElementLoader.elements[(int) Grid.ElementIdx[cell1]].GetMaterialCategoryTag().ProperName(), this.Styles_BodyText.Standard);
      }
      string[] strArray = HoverTextHelper.MassStringsReadOnly(cell1);
      drawer.NewLine();
      drawer.DrawIcon(this.iconDash);
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (index >= 3 || !element1.IsVacuum)
          drawer.DrawText(strArray[index], this.Styles_BodyText.Standard);
      }
      if (!element1.IsVacuum)
      {
        drawer.NewLine();
        drawer.DrawIcon(this.iconDash);
        Element element2 = Grid.Element[cell1];
        string str4 = this.cachedTemperatureString;
        float num3 = Grid.Temperature[cell1];
        if ((double) num3 != (double) this.cachedTemperature)
        {
          this.cachedTemperature = num3;
          str4 = GameUtil.GetFormattedTemperature(Grid.Temperature[cell1]);
          this.cachedTemperatureString = str4;
        }
        string text = (double) element2.specificHeatCapacity == 0.0 ? "N/A" : str4;
        drawer.DrawText(text, this.Styles_BodyText.Standard);
      }
      if (CellSelectionObject.IsExposedToSpace(cell1))
      {
        drawer.NewLine();
        drawer.DrawIcon(this.iconDash);
        drawer.DrawText((string) MISC.STATUSITEMS.SPACE.NAME, this.Styles_BodyText.Standard);
      }
      if (((Component) Game.Instance).GetComponent<EntombedItemVisualizer>().IsEntombedItem(cell1))
      {
        drawer.NewLine();
        drawer.DrawIcon(this.iconDash);
        drawer.DrawText((string) MISC.STATUSITEMS.BURIEDITEM.NAME, this.Styles_BodyText.Standard);
      }
      int cell2 = Grid.CellAbove(cell1);
      bool flag10 = element1.IsLiquid && Grid.IsValidCell(cell2) && (Grid.Element[cell2].IsGas || Grid.Element[cell2].IsVacuum);
      if (element1.sublimateId != (SimHashes) 0 && element1.IsSolid | flag10)
      {
        float mass = Grid.AccumulatedFlow[cell1] / 3f;
        string nameByElementHash1 = GameUtil.GetElementNameByElementHash(element1.id);
        string nameByElementHash2 = GameUtil.GetElementNameByElementHash(element1.sublimateId);
        string text11 = ((string) BUILDING.STATUSITEMS.EMITTINGGASAVG.NAME).Replace("{FlowRate}", GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.PerSecond)).Replace("{Element}", nameByElementHash2);
        drawer.NewLine();
        drawer.DrawIcon(this.iconDash);
        drawer.DrawText(text11, this.Styles_BodyText.Standard);
        bool all_not_gaseous;
        bool all_over_pressure;
        GameUtil.IsEmissionBlocked(cell1, out all_not_gaseous, out all_over_pressure);
        string str5 = (string) null;
        if (all_not_gaseous)
          str5 = (string) MISC.STATUSITEMS.SUBLIMATIONBLOCKED.NAME;
        else if (all_over_pressure)
          str5 = (string) MISC.STATUSITEMS.SUBLIMATIONOVERPRESSURE.NAME;
        if (str5 != null)
        {
          string text12 = str5.Replace("{Element}", nameByElementHash1).Replace("{SubElement}", nameByElementHash2);
          drawer.NewLine();
          drawer.DrawIcon(this.iconDash);
          drawer.DrawText(text12, this.Styles_BodyText.Standard);
        }
      }
      drawer.EndShadowBar();
    }
    else if (!flag3 && (int) Grid.WorldIdx[cell1] == ClusterManager.Instance.activeWorldId)
    {
      drawer.BeginShadowBar();
      drawer.DrawIcon(this.iconWarning);
      drawer.DrawText((string) UI.TOOLS.GENERIC.UNKNOWN, this.Styles_BodyText.Standard);
      drawer.EndShadowBar();
    }
    this.recentNumberOfDisplayedSelectables = num1 + 1;
    drawer.EndDrawing();
  }

  public void DrawLogicIcon(HoverTextDrawer drawer, Sprite icon, TextStyleSetting style) => drawer.DrawIcon(icon, this.GetLogicColorFromStyle(style));

  public void DrawLogicText(HoverTextDrawer drawer, string text, TextStyleSetting style) => drawer.DrawText(text, style, this.GetLogicColorFromStyle(style));

  private Color GetLogicColorFromStyle(TextStyleSetting style)
  {
    ColorSet colorSet = GlobalAssets.Instance.colorSet;
    if (Object.op_Equality((Object) style, (Object) this.Styles_LogicActive.Selected))
      return Color32.op_Implicit(colorSet.logicOnText);
    return Object.op_Equality((Object) style, (Object) this.Styles_LogicStandby.Selected) ? Color32.op_Implicit(colorSet.logicOffText) : style.textColor;
  }

  private bool ShowStatusItemInCurrentOverlay(StatusItem status) => !Object.op_Equality((Object) OverlayScreen.Instance, (Object) null) && ((StatusItem.StatusItemOverlays) status.status_overlays & StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode())) == StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode());

  private bool ShouldShowSelectableInCurrentOverlay(KSelectable selectable)
  {
    bool flag = true;
    if (Object.op_Equality((Object) OverlayScreen.Instance, (Object) null))
      return flag;
    if (Object.op_Equality((Object) selectable, (Object) null))
      return false;
    Func<KSelectable, bool> func;
    if (Object.op_Equality((Object) ((Component) selectable).GetComponent<KPrefabID>(), (Object) null) || !this.modeFilters.TryGetValue(OverlayScreen.Instance.GetMode(), out func))
      return flag;
    flag = func(selectable);
    return flag;
  }

  private static bool ShouldShowOxygenOverlay(KSelectable selectable) => (Object.op_Inequality((Object) ((Component) selectable).GetComponent<AlgaeHabitat>(), (Object) null) ? 1 : (Object.op_Inequality((Object) ((Component) selectable).GetComponent<Electrolyzer>(), (Object) null) ? 1 : 0)) != 0 || Object.op_Inequality((Object) ((Component) selectable).GetComponent<AirFilter>(), (Object) null);

  private static bool ShouldShowLightOverlay(KSelectable selectable) => Object.op_Inequality((Object) ((Component) selectable).GetComponent<Light2D>(), (Object) null);

  private static bool ShouldShowRadiationOverlay(KSelectable selectable) => Object.op_Inequality((Object) ((Component) selectable).GetComponent<HighEnergyParticle>(), (Object) null) || Object.op_Implicit((Object) ((Component) selectable).GetComponent<HighEnergyParticlePort>());

  private static bool ShouldShowGasConduitOverlay(KSelectable selectable)
  {
    if (((((!Object.op_Inequality((Object) ((Component) selectable).GetComponent<Conduit>(), (Object) null) ? 0 : (((Component) selectable).GetComponent<Conduit>().type == ConduitType.Gas ? 1 : 0)) != 0 ? 1 : (!Object.op_Inequality((Object) ((Component) selectable).GetComponent<Filterable>(), (Object) null) ? 0 : (((Component) selectable).GetComponent<Filterable>().filterElementState == Filterable.ElementState.Gas ? 1 : 0))) != 0 ? 1 : (!Object.op_Inequality((Object) ((Component) selectable).GetComponent<Vent>(), (Object) null) ? 0 : (((Component) selectable).GetComponent<Vent>().conduitType == ConduitType.Gas ? 1 : 0))) != 0 ? 1 : (!Object.op_Inequality((Object) ((Component) selectable).GetComponent<Pump>(), (Object) null) ? 0 : (((Component) selectable).GetComponent<Pump>().conduitType == ConduitType.Gas ? 1 : 0))) != 0)
      return true;
    return Object.op_Inequality((Object) ((Component) selectable).GetComponent<ValveBase>(), (Object) null) && ((Component) selectable).GetComponent<ValveBase>().conduitType == ConduitType.Gas;
  }

  private static bool ShouldShowLiquidConduitOverlay(KSelectable selectable)
  {
    if (((((!Object.op_Inequality((Object) ((Component) selectable).GetComponent<Conduit>(), (Object) null) ? 0 : (((Component) selectable).GetComponent<Conduit>().type == ConduitType.Liquid ? 1 : 0)) != 0 ? 1 : (!Object.op_Inequality((Object) ((Component) selectable).GetComponent<Filterable>(), (Object) null) ? 0 : (((Component) selectable).GetComponent<Filterable>().filterElementState == Filterable.ElementState.Liquid ? 1 : 0))) != 0 ? 1 : (!Object.op_Inequality((Object) ((Component) selectable).GetComponent<Vent>(), (Object) null) ? 0 : (((Component) selectable).GetComponent<Vent>().conduitType == ConduitType.Liquid ? 1 : 0))) != 0 ? 1 : (!Object.op_Inequality((Object) ((Component) selectable).GetComponent<Pump>(), (Object) null) ? 0 : (((Component) selectable).GetComponent<Pump>().conduitType == ConduitType.Liquid ? 1 : 0))) != 0)
      return true;
    return Object.op_Inequality((Object) ((Component) selectable).GetComponent<ValveBase>(), (Object) null) && ((Component) selectable).GetComponent<ValveBase>().conduitType == ConduitType.Liquid;
  }

  private static bool ShouldShowPowerOverlay(KSelectable selectable)
  {
    Tag prefabTag = ((Component) selectable).GetComponent<KPrefabID>().PrefabTag;
    return (((OverlayScreen.WireIDs.Contains(prefabTag) ? 1 : (Object.op_Inequality((Object) ((Component) selectable).GetComponent<Battery>(), (Object) null) ? 1 : 0)) != 0 ? 1 : (Object.op_Inequality((Object) ((Component) selectable).GetComponent<PowerTransformer>(), (Object) null) ? 1 : 0)) != 0 ? 1 : (Object.op_Inequality((Object) ((Component) selectable).GetComponent<EnergyConsumer>(), (Object) null) ? 1 : 0)) != 0 || Object.op_Inequality((Object) ((Component) selectable).GetComponent<EnergyGenerator>(), (Object) null);
  }

  private static bool ShouldShowTileOverlay(KSelectable selectable)
  {
    bool flag = false;
    PrimaryElement component = ((Component) selectable).GetComponent<PrimaryElement>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      Element element = component.Element;
      foreach (Tag tileOverlayFilter in Game.Instance.tileOverlayFilters)
      {
        if (element.HasTag(tileOverlayFilter))
        {
          flag = true;
          break;
        }
      }
    }
    return flag;
  }

  private static bool ShouldShowTemperatureOverlay(KSelectable selectable) => Object.op_Inequality((Object) ((Component) selectable).GetComponent<PrimaryElement>(), (Object) null);

  private static bool ShouldShowLogicOverlay(KSelectable selectable)
  {
    Tag prefabTag = ((Component) selectable).GetComponent<KPrefabID>().PrefabTag;
    return OverlayModes.Logic.HighlightItemIDs.Contains(prefabTag) || Object.op_Inequality((Object) ((Component) selectable).GetComponent<LogicPorts>(), (Object) null);
  }

  private static bool ShouldShowSolidConveyorOverlay(KSelectable selectable)
  {
    Tag prefabTag = ((Component) selectable).GetComponent<KPrefabID>().PrefabTag;
    return OverlayScreen.SolidConveyorIDs.Contains(prefabTag);
  }

  private static bool HideInOverlay(KSelectable selectable) => false;

  private static bool ShowOverlayIfHasComponent<T>(KSelectable selectable) => (object) ((Component) selectable).GetComponent<T>() != null;

  private static bool ShouldShowCropOverlay(KSelectable selectable) => Object.op_Inequality((Object) ((Component) selectable).GetComponent<Uprootable>(), (Object) null) || Object.op_Inequality((Object) ((Component) selectable).GetComponent<PlanterBox>(), (Object) null);

  public SelectToolHoverTextCard()
  {
    Dictionary<HashedString, Func<KSelectable, bool>> dictionary = new Dictionary<HashedString, Func<KSelectable, bool>>();
    dictionary.Add(OverlayModes.Oxygen.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowOxygenOverlay));
    dictionary.Add(OverlayModes.Light.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowLightOverlay));
    dictionary.Add(OverlayModes.Radiation.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowRadiationOverlay));
    dictionary.Add(OverlayModes.GasConduits.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowGasConduitOverlay));
    dictionary.Add(OverlayModes.LiquidConduits.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowLiquidConduitOverlay));
    dictionary.Add(OverlayModes.SolidConveyor.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowSolidConveyorOverlay));
    dictionary.Add(OverlayModes.Power.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowPowerOverlay));
    dictionary.Add(OverlayModes.Logic.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowLogicOverlay));
    dictionary.Add(OverlayModes.TileMode.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowTileOverlay));
    dictionary.Add(OverlayModes.Disease.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShowOverlayIfHasComponent<PrimaryElement>));
    dictionary.Add(OverlayModes.Decor.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShowOverlayIfHasComponent<DecorProvider>));
    dictionary.Add(OverlayModes.Crop.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowCropOverlay));
    dictionary.Add(OverlayModes.Temperature.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowTemperatureOverlay));
    this.modeFilters = dictionary;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }
}
