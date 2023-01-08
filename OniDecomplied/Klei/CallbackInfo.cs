// Decompiled with JetBrains decompiler
// Type: Klei.CallbackInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Klei
{
  public struct CallbackInfo
  {
    private HandleVector<Game.CallbackInfo>.Handle handle;

    public CallbackInfo(HandleVector<Game.CallbackInfo>.Handle h) => this.handle = h;

    public void Release()
    {
      if (!this.handle.IsValid())
        return;
      Game.CallbackInfo callbackInfo = Game.Instance.callbackManager.GetItem(this.handle);
      System.Action cb = callbackInfo.cb;
      if (!callbackInfo.manuallyRelease)
        Game.Instance.callbackManager.Release(this.handle);
      cb();
    }
  }
}
