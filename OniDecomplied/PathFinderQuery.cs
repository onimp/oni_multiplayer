// Decompiled with JetBrains decompiler
// Type: PathFinderQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class PathFinderQuery
{
  protected int resultCell;
  private NavType resultNavType;

  public virtual bool IsMatch(int cell, int parent_cell, int cost) => true;

  public void SetResult(int cell, int cost, NavType nav_type)
  {
    this.resultCell = cell;
    this.resultNavType = nav_type;
  }

  public void ClearResult() => this.resultCell = -1;

  public virtual int GetResultCell() => this.resultCell;

  public NavType GetResultNavType() => this.resultNavType;
}
