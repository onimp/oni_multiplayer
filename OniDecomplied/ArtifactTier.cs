// Decompiled with JetBrains decompiler
// Type: ArtifactTier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class ArtifactTier
{
  public EffectorValues decorValues;
  public StringKey name_key;
  public float payloadDropChance;

  public ArtifactTier(StringKey str_key, EffectorValues values, float payload_drop_chance)
  {
    this.decorValues = values;
    this.name_key = str_key;
    this.payloadDropChance = payload_drop_chance;
  }
}
