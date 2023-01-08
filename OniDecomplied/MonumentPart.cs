// Decompiled with JetBrains decompiler
// Type: MonumentPart
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using KSerialization;
using System.Runtime.Serialization;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MonumentPart")]
public class MonumentPart : KMonoBehaviour
{
  public MonumentPartResource.Part part;
  public string stateUISymbol;
  [Serialize]
  private string chosenState;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Components.MonumentParts.Add(this);
    if (!string.IsNullOrEmpty(this.chosenState))
      this.SetState(this.chosenState);
    this.UpdateMonumentDecor();
  }

  [OnDeserialized]
  private void OnDeserializedMethod()
  {
    if (Db.GetMonumentParts().TryGet(this.chosenState) != null)
      return;
    string str = "";
    if (this.part == MonumentPartResource.Part.Bottom)
      str = "bottom_" + this.chosenState;
    else if (this.part == MonumentPartResource.Part.Middle)
      str = "mid_" + this.chosenState;
    else if (this.part == MonumentPartResource.Part.Top)
      str = "top_" + this.chosenState;
    if (Db.GetMonumentParts().TryGet(str) == null)
      return;
    this.chosenState = str;
  }

  protected virtual void OnCleanUp()
  {
    Components.MonumentParts.Remove(this);
    this.RemoveMonumentPiece();
    base.OnCleanUp();
  }

  public void SetState(string state)
  {
    MonumentPartResource monumentPartResource = Db.GetMonumentParts().Get(state);
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    component.SwapAnims(new KAnimFile[1]
    {
      monumentPartResource.AnimFile
    });
    component.Play(HashedString.op_Implicit(monumentPartResource.State));
    this.chosenState = state;
  }

  public bool IsMonumentCompleted()
  {
    int num1 = Object.op_Inequality((Object) this.GetMonumentPart(MonumentPartResource.Part.Top), (Object) null) ? 1 : 0;
    bool flag = Object.op_Inequality((Object) this.GetMonumentPart(MonumentPartResource.Part.Middle), (Object) null);
    int num2 = Object.op_Inequality((Object) this.GetMonumentPart(MonumentPartResource.Part.Bottom), (Object) null) ? 1 : 0;
    return (num1 & num2 & (flag ? 1 : 0)) != 0;
  }

  public void UpdateMonumentDecor()
  {
    GameObject monumentPart = this.GetMonumentPart(MonumentPartResource.Part.Middle);
    if (!this.IsMonumentCompleted())
      return;
    monumentPart.GetComponent<DecorProvider>().SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.COMPLETE);
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this).GetComponent<AttachableBuilding>()))
    {
      if (Object.op_Inequality((Object) gameObject, (Object) monumentPart))
        gameObject.GetComponent<DecorProvider>().SetValues(BUILDINGS.DECOR.NONE);
    }
  }

  public void RemoveMonumentPiece()
  {
    if (!this.IsMonumentCompleted())
      return;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this).GetComponent<AttachableBuilding>()))
    {
      if (Object.op_Inequality((Object) gameObject.GetComponent<MonumentPart>(), (Object) this))
        gameObject.GetComponent<DecorProvider>().SetValues(BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE);
    }
  }

  private GameObject GetMonumentPart(MonumentPartResource.Part requestPart)
  {
    foreach (GameObject monumentPart in AttachableBuilding.GetAttachedNetwork(((Component) this).GetComponent<AttachableBuilding>()))
    {
      MonumentPart component = monumentPart.GetComponent<MonumentPart>();
      if (!Object.op_Equality((Object) component, (Object) null) && component.part == requestPart)
        return monumentPart;
    }
    return (GameObject) null;
  }
}
