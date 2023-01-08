// Decompiled with JetBrains decompiler
// Type: DiseaseOverlayWidget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/DiseaseOverlayWidget")]
public class DiseaseOverlayWidget : KMonoBehaviour
{
  [SerializeField]
  private Image progressFill;
  [SerializeField]
  private ToolTip progressToolTip;
  [SerializeField]
  private Image germsImage;
  [SerializeField]
  private Vector3 offset;
  [SerializeField]
  private Image diseasedImage;
  private List<Image> displayedDiseases = new List<Image>();

  public void Refresh(AmountInstance value_src)
  {
    GameObject gameObject = value_src.gameObject;
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return;
    KAnimControllerBase component = gameObject.GetComponent<KAnimControllerBase>();
    TransformExtensions.SetPosition(this.transform, Vector3.op_Addition(Object.op_Inequality((Object) component, (Object) null) ? component.GetWorldPivot() : Vector3.op_Addition(TransformExtensions.GetPosition(gameObject.transform), Vector3.down), this.offset));
    AmountInstance amountInstance = value_src;
    if (amountInstance != null)
    {
      ((Component) ((Component) this.progressFill).transform.parent).gameObject.SetActive(true);
      float num = amountInstance.value / amountInstance.GetMax();
      Vector3 localScale = ((Transform) ((Graphic) this.progressFill).rectTransform).localScale;
      localScale.y = num;
      ((Transform) ((Graphic) this.progressFill).rectTransform).localScale = localScale;
      this.progressToolTip.toolTip = (string) DUPLICANTS.ATTRIBUTES.IMMUNITY.NAME + " " + GameUtil.GetFormattedPercent(num * 100f);
    }
    else
      ((Component) ((Component) this.progressFill).transform.parent).gameObject.SetActive(false);
    int index1 = 0;
    Amounts amounts = gameObject.GetComponent<Modifiers>().GetAmounts();
    foreach (Klei.AI.Disease resource in Db.Get().Diseases.resources)
    {
      float units = amounts.Get(resource.amount).value;
      if ((double) units > 0.0)
      {
        Image image;
        if (index1 < this.displayedDiseases.Count)
        {
          image = this.displayedDiseases[index1];
        }
        else
        {
          image = Util.KInstantiateUI(((Component) this.germsImage).gameObject, ((Component) ((Component) this.germsImage).transform.parent).gameObject, true).GetComponent<Image>();
          this.displayedDiseases.Add(image);
        }
        ((Graphic) image).color = Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(resource.overlayColourName));
        ((Component) image).GetComponent<ToolTip>().toolTip = resource.Name + " " + GameUtil.GetFormattedDiseaseAmount((int) units);
        ++index1;
      }
    }
    for (int index2 = this.displayedDiseases.Count - 1; index2 >= index1; --index2)
    {
      Util.KDestroyGameObject(((Component) this.displayedDiseases[index2]).gameObject);
      this.displayedDiseases.RemoveAt(index2);
    }
    ((Behaviour) this.diseasedImage).enabled = false;
    ((Component) ((Component) this.progressFill).transform.parent).gameObject.SetActive(this.displayedDiseases.Count > 0);
    ((Component) ((Component) this.germsImage).transform.parent).gameObject.SetActive(this.displayedDiseases.Count > 0);
  }
}
