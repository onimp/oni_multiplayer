// Decompiled with JetBrains decompiler
// Type: Klei.Input.InterfaceToolConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.Input
{
  [CreateAssetMenu(fileName = "InterfaceToolConfig", menuName = "Klei/Interface Tools/Config")]
  public class InterfaceToolConfig : ScriptableObject
  {
    [SerializeField]
    private DigToolActionFactory.Actions digAction;
    public static InterfaceToolConfig.Comparer ConfigComparer = new InterfaceToolConfig.Comparer();
    [SerializeField]
    [Tooltip("Defines which config will take priority should multiple configs be activated\n0 is the lower bound for this value.")]
    private int priority;
    [SerializeField]
    [Tooltip("This will serve as a key for activating different configs. Currently, these Actionsare how we indicate that different input modes are desired.\nAssigning Action.Invalid to this field will indicate that this is the \"default\" config")]
    private Action inputAction;

    public DigAction DigAction => ActionFactory<DigToolActionFactory, DigAction, DigToolActionFactory.Actions>.GetOrCreateAction(this.digAction);

    public int Priority => this.priority;

    public Action InputAction => this.inputAction;

    public class Comparer : IComparer<InterfaceToolConfig>
    {
      public int Compare(InterfaceToolConfig lhs, InterfaceToolConfig rhs)
      {
        if (lhs.Priority == rhs.Priority)
          return 0;
        return lhs.Priority <= rhs.Priority ? -1 : 1;
      }
    }
  }
}
