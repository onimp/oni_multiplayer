// Decompiled with JetBrains decompiler
// Type: LogicEventHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class LogicEventHandler : 
  ILogicEventReceiver,
  ILogicNetworkConnection,
  ILogicUIElement,
  IUniformGridObject
{
  private int cell;
  private int value;
  private Action<int> onValueChanged;
  private Action<int, bool> onConnectionChanged;
  private LogicPortSpriteType spriteType;

  public LogicEventHandler(
    int cell,
    Action<int> on_value_changed,
    Action<int, bool> on_connection_changed,
    LogicPortSpriteType sprite_type)
  {
    this.cell = cell;
    this.onValueChanged = on_value_changed;
    this.onConnectionChanged = on_connection_changed;
    this.spriteType = sprite_type;
  }

  public void ReceiveLogicEvent(int value)
  {
    this.TriggerAudio(value);
    this.value = value;
    this.onValueChanged(value);
  }

  public int Value => this.value;

  public int GetLogicUICell() => this.cell;

  public LogicPortSpriteType GetLogicPortSpriteType() => this.spriteType;

  public Vector2 PosMin() => Vector2.op_Implicit(Grid.CellToPos2D(this.cell));

  public Vector2 PosMax() => Vector2.op_Implicit(Grid.CellToPos2D(this.cell));

  public int GetLogicCell() => this.cell;

  private void TriggerAudio(int new_value)
  {
    LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(this.cell);
    SpeedControlScreen instance = SpeedControlScreen.Instance;
    if (networkForCell == null || new_value == this.value || !Object.op_Inequality((Object) instance, (Object) null) || instance.IsPaused || KPlayerPrefs.HasKey(AudioOptionsScreen.AlwaysPlayAutomation) && KPlayerPrefs.GetInt(AudioOptionsScreen.AlwaysPlayAutomation) != 1 && HashedString.op_Inequality(OverlayScreen.Instance.GetMode(), OverlayModes.Logic.ID))
      return;
    string name = "Logic_Building_Toggle";
    if (!CameraController.Instance.IsAudibleSound(Vector2.op_Implicit(Grid.CellToPosCCC(this.cell, Grid.SceneLayer.BuildingFront))))
      return;
    LogicCircuitNetwork.LogicSoundPair logicSoundPair = new LogicCircuitNetwork.LogicSoundPair();
    Dictionary<int, LogicCircuitNetwork.LogicSoundPair> logicSoundRegister = LogicCircuitNetwork.logicSoundRegister;
    int id = networkForCell.id;
    if (!logicSoundRegister.ContainsKey(id))
    {
      logicSoundRegister.Add(id, logicSoundPair);
    }
    else
    {
      logicSoundPair.playedIndex = logicSoundRegister[id].playedIndex;
      logicSoundPair.lastPlayed = logicSoundRegister[id].lastPlayed;
    }
    if (logicSoundPair.playedIndex < 2)
    {
      logicSoundRegister[id].playedIndex = logicSoundPair.playedIndex + 1;
    }
    else
    {
      logicSoundRegister[id].playedIndex = 0;
      logicSoundRegister[id].lastPlayed = Time.time;
    }
    float num = (float) (((double) Time.time - (double) logicSoundPair.lastPlayed) / 3.0);
    EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound(name), Grid.CellToPos(this.cell), 1f);
    ((EventInstance) ref eventInstance).setParameterByName("logic_volumeModifer", num, false);
    ((EventInstance) ref eventInstance).setParameterByName("wireCount", (float) (networkForCell.WireCount % 24), false);
    ((EventInstance) ref eventInstance).setParameterByName("enabled", (float) new_value, false);
    KFMOD.EndOneShot(eventInstance);
  }

  public void OnLogicNetworkConnectionChanged(bool connected)
  {
    if (this.onConnectionChanged == null)
      return;
    this.onConnectionChanged(this.cell, connected);
  }
}
