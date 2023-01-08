// Decompiled with JetBrains decompiler
// Type: TreeFilterableSideScreenElement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/TreeFilterableSideScreenElement")]
public class TreeFilterableSideScreenElement : KMonoBehaviour
{
  [SerializeField]
  private LocText elementName;
  [SerializeField]
  private MultiToggle checkBox;
  [SerializeField]
  private KImage elementImg;
  private KImage checkBoxImg;
  private Tag elementTag;
  private TreeFilterableSideScreen parent;
  private bool initialized;

  public Tag GetElementTag() => this.elementTag;

  public bool IsSelected => this.checkBox.CurrentState == 1;

  public event Action<Tag, bool> OnSelectionChanged;

  public MultiToggle GetCheckboxToggle() => this.checkBox;

  public TreeFilterableSideScreen Parent
  {
    get => this.parent;
    set => this.parent = value;
  }

  private void Initialize()
  {
    if (this.initialized)
      return;
    this.checkBoxImg = KMonoBehaviourExtensions.GetComponentInChildrenOnly<KImage>(((Component) this.checkBox).gameObject);
    this.checkBox.onClick = new System.Action(this.CheckBoxClicked);
    this.initialized = true;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Initialize();
  }

  public Sprite GetStorageObjectSprite(Tag t)
  {
    Sprite storageObjectSprite = (Sprite) null;
    GameObject prefab = Assets.GetPrefab(t);
    if (Object.op_Inequality((Object) prefab, (Object) null))
    {
      KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
      if (Object.op_Inequality((Object) component, (Object) null))
        storageObjectSprite = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0]);
    }
    return storageObjectSprite;
  }

  public void SetSprite(Tag t)
  {
    Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) t);
    ((Image) this.elementImg).sprite = uiSprite.first;
    ((Graphic) this.elementImg).color = uiSprite.second;
    ((Component) this.elementImg).gameObject.SetActive(true);
  }

  public void SetTag(Tag newTag)
  {
    this.Initialize();
    this.elementTag = newTag;
    this.SetSprite(this.elementTag);
    string str = this.elementTag.ProperName();
    if (this.parent.IsStorage)
    {
      float amountInStorage = this.parent.GetAmountInStorage(this.elementTag);
      str = str + ": " + GameUtil.GetFormattedMass(amountInStorage);
    }
    ((TMP_Text) this.elementName).text = str;
  }

  private void CheckBoxClicked() => this.SetCheckBox(!this.parent.IsTagAllowed(this.GetElementTag()));

  public void SetCheckBox(bool checkBoxState)
  {
    this.checkBox.ChangeState(checkBoxState ? 1 : 0);
    ((Behaviour) this.checkBoxImg).enabled = checkBoxState;
    if (this.OnSelectionChanged == null)
      return;
    this.OnSelectionChanged(this.GetElementTag(), checkBoxState);
  }
}
