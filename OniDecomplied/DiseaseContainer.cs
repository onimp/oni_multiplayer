// Decompiled with JetBrains decompiler
// Type: DiseaseContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public struct DiseaseContainer
{
  public AutoDisinfectable autoDisinfectable;
  public ushort elemIdx;
  public bool isContainer;
  public ConduitType conduitType;
  public KBatchedAnimController controller;
  public GameObject visualDiseaseProvider;
  public int overpopulationCount;
  public float instanceGrowthRate;
  public float accumulatedError;

  public DiseaseContainer(GameObject go, ushort elemIdx)
  {
    this.elemIdx = elemIdx;
    this.isContainer = go.GetComponent<IUserControlledCapacity>() != null && Object.op_Inequality((Object) go.GetComponent<Storage>(), (Object) null);
    Conduit component = go.GetComponent<Conduit>();
    this.conduitType = !Object.op_Inequality((Object) component, (Object) null) ? ConduitType.None : component.type;
    this.controller = go.GetComponent<KBatchedAnimController>();
    this.overpopulationCount = 1;
    this.instanceGrowthRate = 1f;
    this.accumulatedError = 0.0f;
    this.visualDiseaseProvider = (GameObject) null;
    this.autoDisinfectable = go.GetComponent<AutoDisinfectable>();
    if (!Object.op_Inequality((Object) this.autoDisinfectable, (Object) null))
      return;
    AutoDisinfectableManager.Instance.AddAutoDisinfectable(this.autoDisinfectable);
  }

  public void Clear() => this.controller = (KBatchedAnimController) null;
}
