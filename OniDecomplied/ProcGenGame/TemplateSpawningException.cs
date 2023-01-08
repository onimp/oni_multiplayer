// Decompiled with JetBrains decompiler
// Type: ProcGenGame.TemplateSpawningException
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

namespace ProcGenGame
{
  public class TemplateSpawningException : Exception
  {
    public readonly string userMessage;

    public TemplateSpawningException(string message, string userMessage)
      : base(message)
    {
      this.userMessage = userMessage;
    }
  }
}
