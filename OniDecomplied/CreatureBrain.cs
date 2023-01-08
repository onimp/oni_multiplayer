// Decompiled with JetBrains decompiler
// Type: CreatureBrain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CreatureBrain : Brain
{
  public string symbolPrefix;
  public Tag species;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    Navigator component = ((Component) this).GetComponent<Navigator>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetAbilities((PathFinderAbilities) new CreaturePathFinderAbilities(component));
  }
}
