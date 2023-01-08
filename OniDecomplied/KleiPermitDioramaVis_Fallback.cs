// Decompiled with JetBrains decompiler
// Type: KleiPermitDioramaVis_Fallback
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KleiPermitDioramaVis_Fallback : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
  [SerializeField]
  private Image sprite;
  [SerializeField]
  private RectTransform editorOnlyErrorMessageParent;
  [SerializeField]
  private TextMeshProUGUI editorOnlyErrorMessageText;
  private Option<string> error;

  public GameObject GetGameObject() => ((Component) this).gameObject;

  public void ConfigureSetup()
  {
  }

  public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo)
  {
    this.sprite.sprite = PermitPresentationInfo.GetUnknownSprite();
    ((Component) this.editorOnlyErrorMessageParent).gameObject.SetActive(false);
  }

  public KleiPermitDioramaVis_Fallback WithError(string error)
  {
    this.error = (Option<string>) error;
    Debug.Log((object) ("[KleiInventoryScreen Error] Had to use fallback vis. " + error));
    return this;
  }
}
