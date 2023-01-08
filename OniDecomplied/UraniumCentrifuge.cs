// Decompiled with JetBrains decompiler
// Type: UraniumCentrifuge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class UraniumCentrifuge : ComplexFabricator
{
  private Guid statusHandle;
  private static readonly EventSystem.IntraObjectHandler<UraniumCentrifuge> CheckPipesDelegate = new EventSystem.IntraObjectHandler<UraniumCentrifuge>((Action<UraniumCentrifuge, object>) ((component, data) => component.CheckPipes(data)));
  private static readonly EventSystem.IntraObjectHandler<UraniumCentrifuge> DropEnrichedProductDelegate = new EventSystem.IntraObjectHandler<UraniumCentrifuge>((Action<UraniumCentrifuge, object>) ((component, data) => component.DropEnrichedProducts(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<UraniumCentrifuge>(-1697596308, UraniumCentrifuge.DropEnrichedProductDelegate);
    this.Subscribe<UraniumCentrifuge>(-2094018600, UraniumCentrifuge.CheckPipesDelegate);
  }

  private void DropEnrichedProducts(object data)
  {
    foreach (Storage component in ((Component) this).GetComponents<Storage>())
      component.Drop(ElementLoader.FindElementByHash(SimHashes.EnrichedUranium).tag);
  }

  private void CheckPipes(object data)
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    int cell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), UraniumCentrifugeConfig.outPipeOffset);
    GameObject gameObject = Grid.Objects[cell, 16];
    if (Object.op_Inequality((Object) gameObject, (Object) null))
    {
      if ((double) gameObject.GetComponent<PrimaryElement>().Element.highTemp > (double) ElementLoader.FindElementByHash(SimHashes.MoltenUranium).lowTemp)
        component.RemoveStatusItem(this.statusHandle);
      else
        this.statusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.PipeMayMelt);
    }
    else
      component.RemoveStatusItem(this.statusHandle);
  }
}
