// Decompiled with JetBrains decompiler
// Type: TechInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class TechInstance
{
  public Tech tech;
  private bool complete;
  public ResearchPointInventory progressInventory = new ResearchPointInventory();

  public TechInstance(Tech tech) => this.tech = tech;

  public bool IsComplete() => this.complete;

  public void Purchased()
  {
    if (this.complete)
      return;
    this.complete = true;
  }

  public float PercentageCompleteResearchType(string type) => !this.tech.RequiresResearchType(type) ? 1f : Mathf.Clamp01(this.progressInventory.PointsByTypeID[type] / this.tech.costsByResearchTypeID[type]);

  public TechInstance.SaveData Save()
  {
    string[] array1 = new string[this.progressInventory.PointsByTypeID.Count];
    this.progressInventory.PointsByTypeID.Keys.CopyTo(array1, 0);
    float[] array2 = new float[this.progressInventory.PointsByTypeID.Count];
    this.progressInventory.PointsByTypeID.Values.CopyTo(array2, 0);
    return new TechInstance.SaveData()
    {
      techId = this.tech.Id,
      complete = this.complete,
      inventoryIDs = array1,
      inventoryValues = array2
    };
  }

  public void Load(TechInstance.SaveData save_data)
  {
    this.complete = save_data.complete;
    for (int index = 0; index < save_data.inventoryIDs.Length; ++index)
      this.progressInventory.AddResearchPoints(save_data.inventoryIDs[index], save_data.inventoryValues[index]);
  }

  public struct SaveData
  {
    public string techId;
    public bool complete;
    public string[] inventoryIDs;
    public float[] inventoryValues;
  }
}
