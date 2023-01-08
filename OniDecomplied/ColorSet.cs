// Decompiled with JetBrains decompiler
// Type: ColorSet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ColorSet : ScriptableObject
{
  public string settingName;
  [Header("Logic")]
  public Color32 logicOn;
  public Color32 logicOff;
  public Color32 logicDisconnected;
  public Color32 logicOnText;
  public Color32 logicOffText;
  public Color32 logicOnSidescreen;
  public Color32 logicOffSidescreen;
  [Header("Decor")]
  public Color32 decorPositive;
  public Color32 decorNegative;
  public Color32 decorBaseline;
  public Color32 decorHighlightPositive;
  public Color32 decorHighlightNegative;
  [Header("Crop Overlay")]
  public Color32 cropHalted;
  public Color32 cropGrowing;
  public Color32 cropGrown;
  [Header("Harvest Overlay")]
  public Color32 harvestEnabled;
  public Color32 harvestDisabled;
  [Header("Gameplay Events")]
  public Color32 eventPositive;
  public Color32 eventNegative;
  public Color32 eventNeutral;
  [Header("Notifications")]
  public Color32 NotificationBad;
  public Color32 NotificationEvent;
  [Header("Info Screen Status Items")]
  public Color32 statusItemBad;
  public Color32 statusItemEvent;
  [Header("Germ Overlay")]
  public Color32 germFoodPoisoning;
  public Color32 germPollenGerms;
  public Color32 germSlimeLung;
  public Color32 germZombieSpores;
  public Color32 germRadiationSickness;
  [Header("Room Overlay")]
  public Color32 roomNone;
  public Color32 roomFood;
  public Color32 roomSleep;
  public Color32 roomRecreation;
  public Color32 roomBathroom;
  public Color32 roomHospital;
  public Color32 roomIndustrial;
  public Color32 roomAgricultural;
  public Color32 roomScience;
  public Color32 roomPark;
  [Header("Power Overlay")]
  public Color32 powerConsumer;
  public Color32 powerGenerator;
  public Color32 powerBuildingDisabled;
  public Color32 powerCircuitUnpowered;
  public Color32 powerCircuitSafe;
  public Color32 powerCircuitStraining;
  public Color32 powerCircuitOverloading;
  [Header("Light Overlay")]
  public Color32 lightOverlay;
  [Header("Conduit Overlay")]
  public Color32 conduitNormal;
  public Color32 conduitInsulated;
  public Color32 conduitRadiant;
  [Header("Temperature Overlay")]
  public Color32 temperatureThreshold0;
  public Color32 temperatureThreshold1;
  public Color32 temperatureThreshold2;
  public Color32 temperatureThreshold3;
  public Color32 temperatureThreshold4;
  public Color32 temperatureThreshold5;
  public Color32 temperatureThreshold6;
  public Color32 temperatureThreshold7;
  public Color32 heatflowThreshold0;
  public Color32 heatflowThreshold1;
  public Color32 heatflowThreshold2;
  private Dictionary<string, Color32> namedLookup;

  private void Init()
  {
    if (this.namedLookup != null)
      return;
    this.namedLookup = new Dictionary<string, Color32>();
    foreach (FieldInfo field in typeof (ColorSet).GetFields())
    {
      if (field.FieldType == typeof (Color32))
        this.namedLookup[field.Name] = (Color32) field.GetValue((object) this);
    }
  }

  public Color32 GetColorByName(string name)
  {
    this.Init();
    return this.namedLookup[name];
  }

  public void RefreshLookup()
  {
    this.namedLookup = (Dictionary<string, Color32>) null;
    this.Init();
  }

  public bool IsDefaultColorSet() => Array.IndexOf<ColorSet>(GlobalAssets.Instance.colorSetOptions, this) == 0;
}
