// Decompiled with JetBrains decompiler
// Type: PlantElementAbsorber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public struct PlantElementAbsorber
{
  public Storage storage;
  public PlantElementAbsorber.LocalInfo localInfo;
  public HandleVector<int>.Handle[] accumulators;
  public PlantElementAbsorber.ConsumeInfo[] consumedElements;

  public void Clear()
  {
    this.storage = (Storage) null;
    this.consumedElements = (PlantElementAbsorber.ConsumeInfo[]) null;
  }

  public struct ConsumeInfo
  {
    public Tag tag;
    public float massConsumptionRate;

    public ConsumeInfo(Tag tag, float mass_consumption_rate)
    {
      this.tag = tag;
      this.massConsumptionRate = mass_consumption_rate;
    }
  }

  public struct LocalInfo
  {
    public Tag tag;
    public float massConsumptionRate;
  }
}
