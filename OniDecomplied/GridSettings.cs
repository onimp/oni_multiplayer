// Decompiled with JetBrains decompiler
// Type: GridSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/GridSettings")]
public class GridSettings : KMonoBehaviour
{
  public const float CellSizeInMeters = 1f;

  public static void Reset(int width, int height)
  {
    Grid.WidthInCells = width;
    Grid.HeightInCells = height;
    Grid.CellCount = width * height;
    Grid.WidthInMeters = 1f * (float) width;
    Grid.HeightInMeters = 1f * (float) height;
    Grid.CellSizeInMeters = 1f;
    Grid.HalfCellSizeInMeters = 0.5f;
    Grid.Element = new Element[Grid.CellCount];
    Grid.VisMasks = new Grid.VisFlags[Grid.CellCount];
    Grid.Visible = new byte[Grid.CellCount];
    Grid.Spawnable = new byte[Grid.CellCount];
    Grid.BuildMasks = new Grid.BuildFlags[Grid.CellCount];
    Grid.LightCount = new int[Grid.CellCount];
    Grid.Damage = new float[Grid.CellCount];
    Grid.NavMasks = new Grid.NavFlags[Grid.CellCount];
    Grid.NavValidatorMasks = new Grid.NavValidatorFlags[Grid.CellCount];
    Grid.Decor = new float[Grid.CellCount];
    Grid.Loudness = new float[Grid.CellCount];
    Grid.GravitasFacility = new bool[Grid.CellCount];
    Grid.WorldIdx = new byte[Grid.CellCount];
    Grid.ObjectLayers = new Dictionary<int, GameObject>[44];
    for (int index = 0; index < Grid.ObjectLayers.Length; ++index)
      Grid.ObjectLayers[index] = new Dictionary<int, GameObject>();
    for (int index = 0; index < Grid.CellCount; ++index)
      Grid.Loudness[index] = 0.0f;
    if (Object.op_Inequality((Object) Game.Instance, (Object) null))
    {
      Game.Instance.gasConduitSystem.Initialize(Grid.WidthInCells, Grid.HeightInCells);
      Game.Instance.liquidConduitSystem.Initialize(Grid.WidthInCells, Grid.HeightInCells);
      Game.Instance.electricalConduitSystem.Initialize(Grid.WidthInCells, Grid.HeightInCells);
      Game.Instance.travelTubeSystem.Initialize(Grid.WidthInCells, Grid.HeightInCells);
      Game.Instance.gasConduitFlow.Initialize(Grid.CellCount);
      Game.Instance.liquidConduitFlow.Initialize(Grid.CellCount);
    }
    for (int index = 0; index < Grid.CellCount; ++index)
      Grid.WorldIdx[index] = byte.MaxValue;
    Grid.OnReveal = (Action<int>) null;
  }

  public static void ClearGrid()
  {
    Grid.WidthInCells = 0;
    Grid.HeightInCells = 0;
    Grid.CellCount = 0;
    Grid.WidthInMeters = 0.0f;
    Grid.HeightInMeters = 0.0f;
    Grid.CellSizeInMeters = 0.0f;
    Grid.HalfCellSizeInMeters = 0.0f;
    Grid.Element = (Element[]) null;
    Grid.VisMasks = (Grid.VisFlags[]) null;
    Grid.Visible = (byte[]) null;
    Grid.Spawnable = (byte[]) null;
    Grid.BuildMasks = (Grid.BuildFlags[]) null;
    Grid.NavValidatorMasks = (Grid.NavValidatorFlags[]) null;
    Grid.LightCount = (int[]) null;
    Grid.Damage = (float[]) null;
    Grid.Decor = (float[]) null;
    Grid.Loudness = (float[]) null;
    Grid.GravitasFacility = (bool[]) null;
    Grid.ObjectLayers = (Dictionary<int, GameObject>[]) null;
    Grid.WorldIdx = (byte[]) null;
    Grid.OnReveal = (Action<int>) null;
    Grid.ResetNavMasksAndDetails();
  }
}
