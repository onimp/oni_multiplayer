// Decompiled with JetBrains decompiler
// Type: ClusterMapPathDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClusterMapPathDrawer : MonoBehaviour
{
  public ClusterMapPath pathPrefab;
  public Transform pathContainer;

  public ClusterMapPath AddPath()
  {
    ClusterMapPath clusterMapPath = Object.Instantiate<ClusterMapPath>(this.pathPrefab, this.pathContainer);
    clusterMapPath.Init();
    return clusterMapPath;
  }

  public static List<Vector2> GetDrawPathList(Vector2 startLocation, List<AxialI> pathPoints)
  {
    List<Vector2> drawPathList = new List<Vector2>();
    drawPathList.Add(startLocation);
    drawPathList.AddRange(((IEnumerable<AxialI>) pathPoints).Select<AxialI, Vector2>((Func<AxialI, Vector2>) (point => ((AxialI) ref point).ToWorld2D())));
    return drawPathList;
  }
}
