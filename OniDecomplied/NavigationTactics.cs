// Decompiled with JetBrains decompiler
// Type: NavigationTactics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public static class NavigationTactics
{
  public static NavTactic ReduceTravelDistance = new NavTactic(0, 0, pathCostPenalty: 4);
  public static NavTactic Range_2_AvoidOverlaps = new NavTactic(2, 6, 12);
  public static NavTactic Range_3_ProhibitOverlap = new NavTactic(3, 6, 9999);
}
