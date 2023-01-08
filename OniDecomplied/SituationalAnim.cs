// Decompiled with JetBrains decompiler
// Type: SituationalAnim
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SituationalAnim")]
public class SituationalAnim : KMonoBehaviour
{
  public List<Tuple<SituationalAnim.Situation, string>> anims;
  public Func<int, bool> test;
  public SituationalAnim.MustSatisfy mustSatisfy;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    SituationalAnim.Situation situation = this.GetSituation();
    DebugUtil.LogArgs(new object[2]
    {
      (object) "Situation is",
      (object) situation
    });
    this.SetAnimForSituation(situation);
  }

  private void SetAnimForSituation(SituationalAnim.Situation situation)
  {
    foreach (Tuple<SituationalAnim.Situation, string> anim in this.anims)
    {
      if ((anim.first & situation) == anim.first)
      {
        DebugUtil.LogArgs(new object[3]
        {
          (object) "Chose Anim",
          (object) anim.first,
          (object) anim.second
        });
        this.SetAnim(anim.second);
        break;
      }
    }
  }

  private void SetAnim(string animName) => ((Component) this).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit(animName));

  private SituationalAnim.Situation GetSituation()
  {
    SituationalAnim.Situation situation = (SituationalAnim.Situation) 0;
    Extents extents = ((Component) this).GetComponent<Building>().GetExtents();
    int x = extents.x;
    int maxx = extents.x + extents.width - 1;
    int y = extents.y;
    int maxy = extents.y + extents.height - 1;
    if (this.DoesSatisfy(this.GetSatisfactionForEdge(x, maxx, y - 1, y - 1), this.mustSatisfy))
      situation |= SituationalAnim.Situation.Bottom;
    if (this.DoesSatisfy(this.GetSatisfactionForEdge(x - 1, x - 1, y, maxy), this.mustSatisfy))
      situation |= SituationalAnim.Situation.Left;
    if (this.DoesSatisfy(this.GetSatisfactionForEdge(x, maxx, maxy + 1, maxy + 1), this.mustSatisfy))
      situation |= SituationalAnim.Situation.Top;
    if (this.DoesSatisfy(this.GetSatisfactionForEdge(maxx + 1, maxx + 1, y, maxy), this.mustSatisfy))
      situation |= SituationalAnim.Situation.Right;
    return situation;
  }

  private bool DoesSatisfy(
    SituationalAnim.MustSatisfy result,
    SituationalAnim.MustSatisfy requirement)
  {
    if (requirement == SituationalAnim.MustSatisfy.All)
      return result == SituationalAnim.MustSatisfy.All;
    return requirement == SituationalAnim.MustSatisfy.Any ? result != 0 : result == SituationalAnim.MustSatisfy.None;
  }

  private SituationalAnim.MustSatisfy GetSatisfactionForEdge(
    int minx,
    int maxx,
    int miny,
    int maxy)
  {
    bool flag1 = false;
    bool flag2 = true;
    for (int x = minx; x <= maxx; ++x)
    {
      for (int y = miny; y <= maxy; ++y)
      {
        if (this.test(Grid.XYToCell(x, y)))
          flag1 = true;
        else
          flag2 = false;
      }
    }
    if (flag2)
      return SituationalAnim.MustSatisfy.All;
    return flag1 ? SituationalAnim.MustSatisfy.Any : SituationalAnim.MustSatisfy.None;
  }

  [Flags]
  public enum Situation
  {
    Left = 1,
    Right = 2,
    Top = 4,
    Bottom = 8,
  }

  public enum MustSatisfy
  {
    None,
    Any,
    All,
  }
}
