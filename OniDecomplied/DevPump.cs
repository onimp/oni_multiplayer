// Decompiled with JetBrains decompiler
// Type: DevPump
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class DevPump : Filterable, ISim1000ms
{
  public Filterable.ElementState elementState = Filterable.ElementState.Liquid;
  [MyCmpReq]
  private Storage storage;

  private Element element
  {
    get
    {
      Tag selectedTag = this.SelectedTag;
      return ((Tag) ref selectedTag).IsValid ? ElementLoader.GetElement(this.SelectedTag) : ElementLoader.FindElementByHash(SimHashes.Void);
    }
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (this.elementState == Filterable.ElementState.Liquid)
    {
      this.SelectedTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
    }
    else
    {
      if (this.elementState != Filterable.ElementState.Gas)
        return;
      this.SelectedTag = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
    }
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.filterElementState = this.elementState;
  }

  public void Sim1000ms(float dt)
  {
    float mass = 10f - this.storage.GetAmountAvailable(this.element.tag);
    if ((double) mass <= 0.0)
      return;
    if (this.element.IsLiquid)
    {
      this.storage.AddLiquid(this.element.id, mass, this.element.defaultValues.temperature, byte.MaxValue, 0);
    }
    else
    {
      if (!this.element.IsGas)
        return;
      this.storage.AddGasChunk(this.element.id, mass, this.element.defaultValues.temperature, byte.MaxValue, 0, false);
    }
  }
}
