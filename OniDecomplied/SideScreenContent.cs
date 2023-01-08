// Decompiled with JetBrains decompiler
// Type: SideScreenContent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public abstract class SideScreenContent : KScreen
{
  [SerializeField]
  protected string titleKey;
  public GameObject ContentContainer;

  public virtual void SetTarget(GameObject target)
  {
  }

  public virtual void ClearTarget()
  {
  }

  public abstract bool IsValidForTarget(GameObject target);

  public virtual int GetSideScreenSortOrder() => 0;

  public virtual string GetTitle() => StringEntry.op_Implicit(Strings.Get(this.titleKey));
}
