// Decompiled with JetBrains decompiler
// Type: TargetMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;

public abstract class TargetMessage : Message
{
  [Serialize]
  private MessageTarget target;

  protected TargetMessage()
  {
  }

  public TargetMessage(KPrefabID prefab_id) => this.target = new MessageTarget(prefab_id);

  public MessageTarget GetTarget() => this.target;

  public override void OnCleanUp() => this.target.OnCleanUp();
}
