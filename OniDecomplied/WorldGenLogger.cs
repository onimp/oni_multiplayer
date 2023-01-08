// Decompiled with JetBrains decompiler
// Type: WorldGenLogger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public static class WorldGenLogger
{
  public static void LogException(string message, string stack) => Debug.LogError((object) (message + "\n" + stack));
}
