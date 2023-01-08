// Decompiled with JetBrains decompiler
// Type: RequireAttachedComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig]
public class RequireAttachedComponent : ProcessCondition
{
  private string typeNameString;
  private System.Type requiredType;
  private AttachableBuilding myAttachable;

  public System.Type RequiredType
  {
    get => this.requiredType;
    set
    {
      this.requiredType = value;
      this.typeNameString = this.requiredType.Name;
    }
  }

  public RequireAttachedComponent(
    AttachableBuilding myAttachable,
    System.Type required_type,
    string type_name_string)
  {
    this.myAttachable = myAttachable;
    this.requiredType = required_type;
    this.typeNameString = type_name_string;
  }

  public override ProcessCondition.Status EvaluateCondition()
  {
    if (Object.op_Inequality((Object) this.myAttachable, (Object) null))
    {
      foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.myAttachable))
      {
        if (Object.op_Implicit((Object) gameObject.GetComponent(this.requiredType)))
          return ProcessCondition.Status.Ready;
      }
    }
    return ProcessCondition.Status.Failure;
  }

  public override string GetStatusMessage(ProcessCondition.Status status) => this.typeNameString;

  public override string GetStatusTooltip(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.INSTALLED_TOOLTIP, (object) this.typeNameString.ToLower()) : string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.MISSING_TOOLTIP, (object) this.typeNameString.ToLower());

  public override bool ShowInUI() => true;
}
