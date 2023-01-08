// Decompiled with JetBrains decompiler
// Type: TemperatureCookable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TemperatureCookable")]
public class TemperatureCookable : KMonoBehaviour, ISim1000ms
{
  [MyCmpReq]
  private PrimaryElement element;
  public float cookTemperature = 273150f;
  public string cookedID;

  public void Sim1000ms(float dt)
  {
    if ((double) this.element.Temperature <= (double) this.cookTemperature || this.cookedID == null)
      return;
    this.Cook();
  }

  private void Cook()
  {
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(this.cookedID)), position);
    gameObject.SetActive(true);
    KSelectable component1 = ((Component) this).gameObject.GetComponent<KSelectable>();
    if (Object.op_Inequality((Object) SelectTool.Instance, (Object) null) && Object.op_Inequality((Object) SelectTool.Instance.selected, (Object) null) && Object.op_Equality((Object) SelectTool.Instance.selected, (Object) component1))
      SelectTool.Instance.Select(gameObject.GetComponent<KSelectable>());
    PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
    component2.Temperature = this.element.Temperature;
    component2.Mass = this.element.Mass;
    TracesExtesions.DeleteObject(((Component) this).gameObject);
  }
}
