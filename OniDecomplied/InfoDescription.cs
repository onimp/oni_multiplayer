// Decompiled with JetBrains decompiler
// Type: InfoDescription
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/InfoDescription")]
public class InfoDescription : KMonoBehaviour
{
  public string nameLocString = "";
  private string descriptionLocString = "";
  public string description;
  public string displayName;

  public string DescriptionLocString
  {
    set
    {
      this.descriptionLocString = value;
      if (this.descriptionLocString == null)
        return;
      this.description = StringEntry.op_Implicit(Strings.Get(this.descriptionLocString));
    }
    get => this.descriptionLocString;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (!string.IsNullOrEmpty(this.nameLocString))
      this.displayName = StringEntry.op_Implicit(Strings.Get(this.nameLocString));
    if (string.IsNullOrEmpty(this.descriptionLocString))
      return;
    this.description = StringEntry.op_Implicit(Strings.Get(this.descriptionLocString));
  }
}
