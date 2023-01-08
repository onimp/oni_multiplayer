// Decompiled with JetBrains decompiler
// Type: ToolParameterMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ToolParameterMenu")]
public class ToolParameterMenu : KMonoBehaviour
{
  public GameObject content;
  public GameObject widgetContainer;
  public GameObject widgetPrefab;
  private Dictionary<string, GameObject> widgets = new Dictionary<string, GameObject>();
  private Dictionary<string, ToolParameterMenu.ToggleState> currentParameters;
  private string lastEnabledFilter;

  public event System.Action onParametersChanged;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.ClearMenu();
  }

  public void PopulateMenu(
    Dictionary<string, ToolParameterMenu.ToggleState> parameters)
  {
    this.ClearMenu();
    this.currentParameters = parameters;
    foreach (KeyValuePair<string, ToolParameterMenu.ToggleState> parameter in parameters)
    {
      GameObject gameObject = Util.KInstantiateUI(this.widgetPrefab, this.widgetContainer, true);
      ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).text = StringEntry.op_Implicit(Strings.Get("STRINGS.UI.TOOLS.FILTERLAYERS." + parameter.Key));
      this.widgets.Add(parameter.Key, gameObject);
      MultiToggle toggle = gameObject.GetComponentInChildren<MultiToggle>();
      switch (parameter.Value)
      {
        case ToolParameterMenu.ToggleState.On:
          toggle.ChangeState(1);
          this.lastEnabledFilter = parameter.Key;
          break;
        case ToolParameterMenu.ToggleState.Disabled:
          toggle.ChangeState(2);
          break;
        default:
          toggle.ChangeState(0);
          break;
      }
      toggle.onClick += (System.Action) (() =>
      {
        foreach (KeyValuePair<string, GameObject> widget in this.widgets)
        {
          if (Object.op_Equality((Object) widget.Value, (Object) ((Component) toggle.transform.parent).gameObject))
          {
            if (this.currentParameters[widget.Key] == ToolParameterMenu.ToggleState.Disabled)
              break;
            this.ChangeToSetting(widget.Key);
            this.OnChange();
            break;
          }
        }
      });
    }
    this.content.SetActive(true);
  }

  public void ClearMenu()
  {
    this.content.SetActive(false);
    foreach (KeyValuePair<string, GameObject> widget in this.widgets)
      Util.KDestroyGameObject(widget.Value);
    this.widgets.Clear();
  }

  private void ChangeToSetting(string key)
  {
    foreach (KeyValuePair<string, GameObject> widget in this.widgets)
    {
      if (this.currentParameters[widget.Key] != ToolParameterMenu.ToggleState.Disabled)
        this.currentParameters[widget.Key] = ToolParameterMenu.ToggleState.Off;
    }
    this.currentParameters[key] = ToolParameterMenu.ToggleState.On;
  }

  private void OnChange()
  {
    foreach (KeyValuePair<string, GameObject> widget in this.widgets)
    {
      switch (this.currentParameters[widget.Key])
      {
        case ToolParameterMenu.ToggleState.On:
          widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(1);
          this.lastEnabledFilter = widget.Key;
          continue;
        case ToolParameterMenu.ToggleState.Off:
          widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(0);
          continue;
        case ToolParameterMenu.ToggleState.Disabled:
          widget.Value.GetComponentInChildren<MultiToggle>().ChangeState(2);
          continue;
        default:
          continue;
      }
    }
    if (this.onParametersChanged == null)
      return;
    this.onParametersChanged();
  }

  public string GetLastEnabledFilter() => this.lastEnabledFilter;

  public class FILTERLAYERS
  {
    public static string BUILDINGS = nameof (BUILDINGS);
    public static string TILES = nameof (TILES);
    public static string WIRES = nameof (WIRES);
    public static string LIQUIDCONDUIT = "LIQUIDPIPES";
    public static string GASCONDUIT = "GASPIPES";
    public static string SOLIDCONDUIT = "SOLIDCONDUITS";
    public static string CLEANANDCLEAR = nameof (CLEANANDCLEAR);
    public static string DIGPLACER = nameof (DIGPLACER);
    public static string LOGIC = nameof (LOGIC);
    public static string BACKWALL = nameof (BACKWALL);
    public static string CONSTRUCTION = nameof (CONSTRUCTION);
    public static string DIG = nameof (DIG);
    public static string CLEAN = nameof (CLEAN);
    public static string OPERATE = nameof (OPERATE);
    public static string METAL = nameof (METAL);
    public static string BUILDABLE = nameof (BUILDABLE);
    public static string FILTER = nameof (FILTER);
    public static string LIQUIFIABLE = nameof (LIQUIFIABLE);
    public static string LIQUID = nameof (LIQUID);
    public static string CONSUMABLEORE = nameof (CONSUMABLEORE);
    public static string ORGANICS = nameof (ORGANICS);
    public static string FARMABLE = nameof (FARMABLE);
    public static string GAS = nameof (GAS);
    public static string HEATFLOW = nameof (HEATFLOW);
    public static string ABSOLUTETEMPERATURE = nameof (ABSOLUTETEMPERATURE);
    public static string ADAPTIVETEMPERATURE = nameof (ADAPTIVETEMPERATURE);
    public static string STATECHANGE = nameof (STATECHANGE);
    public static string ALL = nameof (ALL);
  }

  public enum ToggleState
  {
    On,
    Off,
    Disabled,
  }
}
