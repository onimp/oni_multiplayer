// Decompiled with JetBrains decompiler
// Type: GameSoundEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public static class GameSoundEvents
{
  public static GameSoundEvents.Event BatteryFull = new GameSoundEvents.Event("game_triggered.battery_full");
  public static GameSoundEvents.Event BatteryWarning = new GameSoundEvents.Event("game_triggered.battery_warning");
  public static GameSoundEvents.Event BatteryDischarged = new GameSoundEvents.Event("game_triggered.battery_drained");

  public class Event
  {
    public HashedString Name;

    public Event(string name) => this.Name = HashedString.op_Implicit(name);
  }
}
