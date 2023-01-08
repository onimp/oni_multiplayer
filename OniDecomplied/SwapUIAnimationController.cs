// Decompiled with JetBrains decompiler
// Type: SwapUIAnimationController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SwapUIAnimationController : MonoBehaviour
{
  public GameObject AnimationControllerObject_Primary;
  public GameObject AnimationControllerObject_Alternate;

  public void SetState(bool Primary)
  {
    this.AnimationControllerObject_Primary.SetActive(Primary);
    if (!Primary)
    {
      this.AnimationControllerObject_Alternate.GetComponent<KAnimControllerBase>().TintColour = Color32.op_Implicit(new Color(1f, 1f, 1f, 0.5f));
      this.AnimationControllerObject_Primary.GetComponent<KAnimControllerBase>().TintColour = Color32.op_Implicit(Color.clear);
    }
    this.AnimationControllerObject_Alternate.SetActive(!Primary);
    if (!Primary)
      return;
    this.AnimationControllerObject_Primary.GetComponent<KAnimControllerBase>().TintColour = Color32.op_Implicit(Color.white);
    this.AnimationControllerObject_Alternate.GetComponent<KAnimControllerBase>().TintColour = Color32.op_Implicit(Color.clear);
  }
}
