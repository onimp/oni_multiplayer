// Decompiled with JetBrains decompiler
// Type: Klei.Actions.DigToolActionFactory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.Input;
using System;

namespace Klei.Actions
{
  public class DigToolActionFactory : 
    ActionFactory<DigToolActionFactory, DigAction, DigToolActionFactory.Actions>
  {
    protected override DigAction CreateAction(DigToolActionFactory.Actions action)
    {
      if (action == DigToolActionFactory.Actions.Immediate)
        return (DigAction) new ImmediateDigAction();
      if (action == DigToolActionFactory.Actions.ClearCell)
        return (DigAction) new ClearCellDigAction();
      if (action == DigToolActionFactory.Actions.MarkCell)
        return (DigAction) new MarkCellDigAction();
      throw new InvalidOperationException("Can not create DigAction 'Count'. Please provide a valid action.");
    }

    public enum Actions
    {
      Count = -1427607121, // 0xAAE871AF
      Immediate = -1044758767, // 0xC1BA3F11
      ClearCell = -1011242513, // 0xC3B9A9EF
      MarkCell = 145163119, // 0x08A7036F
    }
  }
}
