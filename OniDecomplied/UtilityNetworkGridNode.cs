// Decompiled with JetBrains decompiler
// Type: UtilityNetworkGridNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public struct UtilityNetworkGridNode : IEquatable<UtilityNetworkGridNode>
{
  public UtilityConnections connections;
  public int networkIdx;
  public const int InvalidNetworkIdx = -1;

  public bool Equals(UtilityNetworkGridNode other) => this.connections == other.connections && this.networkIdx == other.networkIdx;

  public override bool Equals(object obj) => ((UtilityNetworkGridNode) obj).Equals(this);

  public override int GetHashCode() => base.GetHashCode();

  public static bool operator ==(UtilityNetworkGridNode x, UtilityNetworkGridNode y) => x.Equals(y);

  public static bool operator !=(UtilityNetworkGridNode x, UtilityNetworkGridNode y) => !x.Equals(y);
}
