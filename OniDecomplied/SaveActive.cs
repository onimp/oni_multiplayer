// Decompiled with JetBrains decompiler
// Type: SaveActive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class SaveActive : KScreen
{
  [MyCmpGet]
  private KBatchedAnimController controller;
  private Game.CansaveCB readyForSaveCallback;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Game.Instance.SetAutoSaveCallbacks(new Game.SavingPreCB(this.ActiveateSaveIndicator), new Game.SavingActiveCB(this.SetActiveSaveIndicator), new Game.SavingPostCB(this.DeactivateSaveIndicator));
  }

  private void DoCallBack(HashedString name)
  {
    this.controller.onAnimComplete -= new KAnimControllerBase.KAnimEvent(this.DoCallBack);
    this.readyForSaveCallback();
    this.readyForSaveCallback = (Game.CansaveCB) null;
  }

  private void ActiveateSaveIndicator(Game.CansaveCB cb)
  {
    this.readyForSaveCallback = cb;
    this.controller.onAnimComplete += new KAnimControllerBase.KAnimEvent(this.DoCallBack);
    this.controller.Play(HashedString.op_Implicit("working_pre"));
  }

  private void SetActiveSaveIndicator() => this.controller.Play(HashedString.op_Implicit("working_loop"));

  private void DeactivateSaveIndicator() => this.controller.Play(HashedString.op_Implicit("working_pst"));

  public virtual void OnKeyDown(KButtonEvent e)
  {
  }
}
