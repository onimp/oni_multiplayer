// Decompiled with JetBrains decompiler
// Type: KSelectableProgressBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class KSelectableProgressBar : KSelectable
{
  [MyCmpGet]
  private ProgressBar progressBar;
  private int scaleAmount = 100;

  public override string GetName() => string.Format("{0} {1}/{2}", (object) this.entityName, (object) (int) ((double) this.progressBar.PercentFull * (double) this.scaleAmount), (object) this.scaleAmount);
}
