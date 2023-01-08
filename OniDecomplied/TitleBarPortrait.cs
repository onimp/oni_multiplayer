// Decompiled with JetBrains decompiler
// Type: TitleBarPortrait
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/TitleBarPortrait")]
public class TitleBarPortrait : KMonoBehaviour
{
  public GameObject FaceObject;
  public GameObject ImageObject;
  public GameObject PortraitShadow;
  public GameObject AnimControllerObject;
  public Material DefaultMaterial;
  public Material DesatMaterial;

  public void SetSaturation(bool saturated) => ((Graphic) this.ImageObject.GetComponent<Image>()).material = saturated ? this.DefaultMaterial : this.DesatMaterial;

  public void SetPortrait(GameObject selectedTarget)
  {
    MinionIdentity component1 = selectedTarget.GetComponent<MinionIdentity>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      this.SetPortrait(component1);
    }
    else
    {
      Building component2 = selectedTarget.GetComponent<Building>();
      if (Object.op_Inequality((Object) component2, (Object) null))
      {
        this.SetPortrait(component2.Def.GetUISprite());
      }
      else
      {
        MeshRenderer componentInChildren = selectedTarget.GetComponentInChildren<MeshRenderer>();
        if (!Object.op_Implicit((Object) componentInChildren))
          return;
        this.SetPortrait(Sprite.Create((Texture2D) ((Renderer) componentInChildren).material.mainTexture, new Rect(0.0f, 0.0f, (float) ((Renderer) componentInChildren).material.mainTexture.width, (float) ((Renderer) componentInChildren).material.mainTexture.height), new Vector2(0.5f, 0.5f)));
      }
    }
  }

  public void SetPortrait(Sprite image)
  {
    if (Object.op_Implicit((Object) this.PortraitShadow))
      this.PortraitShadow.SetActive(true);
    if (Object.op_Implicit((Object) this.FaceObject))
      this.FaceObject.SetActive(false);
    if (Object.op_Implicit((Object) this.ImageObject))
      this.ImageObject.SetActive(true);
    if (Object.op_Implicit((Object) this.AnimControllerObject))
      this.AnimControllerObject.SetActive(false);
    if (Object.op_Equality((Object) image, (Object) null))
      this.ClearPortrait();
    else
      this.ImageObject.GetComponent<Image>().sprite = image;
  }

  private void SetPortrait(MinionIdentity identity)
  {
    if (Object.op_Implicit((Object) this.PortraitShadow))
      this.PortraitShadow.SetActive(true);
    if (Object.op_Implicit((Object) this.FaceObject))
      this.FaceObject.SetActive(false);
    if (Object.op_Implicit((Object) this.ImageObject))
      this.ImageObject.SetActive(false);
    CrewPortrait component = ((Component) this).GetComponent<CrewPortrait>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      component.SetIdentityObject((IAssignableIdentity) identity);
    }
    else
    {
      if (!Object.op_Implicit((Object) this.AnimControllerObject))
        return;
      this.AnimControllerObject.SetActive(true);
      CrewPortrait.SetPortraitData((IAssignableIdentity) identity, this.AnimControllerObject.GetComponent<KBatchedAnimController>());
    }
  }

  public void ClearPortrait()
  {
    if (Object.op_Implicit((Object) this.PortraitShadow))
      this.PortraitShadow.SetActive(false);
    if (Object.op_Implicit((Object) this.FaceObject))
      this.FaceObject.SetActive(false);
    if (Object.op_Implicit((Object) this.ImageObject))
      this.ImageObject.SetActive(false);
    if (!Object.op_Implicit((Object) this.AnimControllerObject))
      return;
    this.AnimControllerObject.SetActive(false);
  }
}
