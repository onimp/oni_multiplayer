// Decompiled with JetBrains decompiler
// Type: CellSelectionInstantiator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CellSelectionInstantiator : MonoBehaviour
{
  public GameObject CellSelectionPrefab;

  private void Awake()
  {
    GameObject gameObject1 = Util.KInstantiate(this.CellSelectionPrefab, (GameObject) null, "WorldSelectionCollider");
    GameObject gameObject2 = Util.KInstantiate(this.CellSelectionPrefab, (GameObject) null, "WorldSelectionCollider");
    CellSelectionObject component1 = gameObject1.GetComponent<CellSelectionObject>();
    CellSelectionObject component2 = gameObject2.GetComponent<CellSelectionObject>();
    component1.alternateSelectionObject = component2;
    component2.alternateSelectionObject = component1;
  }
}
