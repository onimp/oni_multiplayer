// Decompiled with JetBrains decompiler
// Type: UserNavigation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UserNavigation")]
public class UserNavigation : KMonoBehaviour
{
  [Serialize]
  private List<UserNavigation.NavPoint> hotkeyNavPoints = new List<UserNavigation.NavPoint>();
  [Serialize]
  private Dictionary<int, UserNavigation.NavPoint> worldCameraPositions = new Dictionary<int, UserNavigation.NavPoint>();

  public UserNavigation()
  {
    for (Action action = (Action) 15; action <= 24; action = (Action) (action + 1))
      this.hotkeyNavPoints.Add(UserNavigation.NavPoint.Invalid);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Game.Instance.Subscribe(1983128072, (Action<object>) (worlds =>
    {
      Tuple<int, int> tuple = (Tuple<int, int>) worlds;
      int first = tuple.first;
      int second = tuple.second;
      int cell = Grid.PosToCell(CameraController.Instance.transform.position);
      if (!Grid.IsValidCell(cell) || (int) Grid.WorldIdx[cell] != second)
      {
        WorldContainer world = ClusterManager.Instance.GetWorld(second);
        float num1 = Mathf.Clamp(CameraController.Instance.transform.position.x, world.minimumBounds.x, world.maximumBounds.x);
        float num2 = Mathf.Clamp(CameraController.Instance.transform.position.y, world.minimumBounds.y, world.maximumBounds.y);
        Vector3 pos;
        // ISSUE: explicit constructor call
        ((Vector3) ref pos).\u002Ector(num1, num2, CameraController.Instance.transform.position.z);
        CameraController.Instance.SetPosition(pos);
      }
      this.worldCameraPositions[second] = new UserNavigation.NavPoint()
      {
        pos = CameraController.Instance.transform.position,
        orthoSize = CameraController.Instance.targetOrthographicSize
      };
      if (!this.worldCameraPositions.ContainsKey(first))
      {
        WorldContainer world = ClusterManager.Instance.GetWorld(first);
        Vector2I vector2I = Vector2I.op_Addition(world.WorldOffset, new Vector2I(world.Width / 2, world.Height / 2));
        this.worldCameraPositions.Add(first, new UserNavigation.NavPoint()
        {
          pos = new Vector3((float) vector2I.x, (float) vector2I.y),
          orthoSize = CameraController.Instance.targetOrthographicSize
        });
      }
      CameraController.Instance.SetTargetPosForWorldChange(this.worldCameraPositions[first].pos, this.worldCameraPositions[first].orthoSize, false);
    }));
  }

  public void SetWorldCameraStartPosition(int world_id, Vector3 start_pos)
  {
    if (!this.worldCameraPositions.ContainsKey(world_id))
      this.worldCameraPositions.Add(world_id, new UserNavigation.NavPoint()
      {
        pos = new Vector3(start_pos.x, start_pos.y),
        orthoSize = CameraController.Instance.targetOrthographicSize
      });
    else
      this.worldCameraPositions[world_id] = new UserNavigation.NavPoint()
      {
        pos = new Vector3(start_pos.x, start_pos.y),
        orthoSize = CameraController.Instance.targetOrthographicSize
      };
  }

  private static int GetIndex(Action action)
  {
    int index = -1;
    if (15 <= action && action <= 24)
      index = action - 15;
    else if (25 <= action && action <= 34)
      index = action - 25;
    return index;
  }

  private void SetHotkeyNavPoint(Action action, Vector3 pos, float ortho_size)
  {
    int index = UserNavigation.GetIndex(action);
    if (index < 0)
      return;
    this.hotkeyNavPoints[index] = new UserNavigation.NavPoint()
    {
      pos = pos,
      orthoSize = ortho_size
    };
    EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("UserNavPoint_set"), Vector3.zero, 1f);
    ((EventInstance) ref eventInstance).setParameterByName("userNavPoint_ID", (float) index, false);
    KFMOD.EndOneShot(eventInstance);
  }

  private void GoToHotkeyNavPoint(Action action)
  {
    int index = UserNavigation.GetIndex(action);
    if (index < 0)
      return;
    UserNavigation.NavPoint hotkeyNavPoint = this.hotkeyNavPoints[index];
    if (!hotkeyNavPoint.IsValid())
      return;
    CameraController.Instance.SetTargetPos(hotkeyNavPoint.pos, hotkeyNavPoint.orthoSize, true);
    EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("UserNavPoint_recall"), Vector3.zero, 1f);
    ((EventInstance) ref eventInstance).setParameterByName("userNavPoint_ID", (float) index, false);
    KFMOD.EndOneShot(eventInstance);
  }

  public bool Handle(KButtonEvent e)
  {
    bool flag = false;
    for (Action action = (Action) 25; action <= 34; action = (Action) (action + 1))
    {
      if (e.TryConsume(action))
      {
        this.GoToHotkeyNavPoint(action);
        flag = true;
        break;
      }
    }
    if (!flag)
    {
      for (Action action = (Action) 15; action <= 24; action = (Action) (action + 1))
      {
        if (e.TryConsume(action))
        {
          Camera baseCamera = CameraController.Instance.baseCamera;
          Vector3 position = TransformExtensions.GetPosition(((Component) baseCamera).transform);
          this.SetHotkeyNavPoint(action, position, baseCamera.orthographicSize);
          flag = true;
          break;
        }
      }
    }
    return flag;
  }

  [Serializable]
  private struct NavPoint
  {
    public Vector3 pos;
    public float orthoSize;
    public static readonly UserNavigation.NavPoint Invalid = new UserNavigation.NavPoint()
    {
      pos = Vector3.zero,
      orthoSize = 0.0f
    };

    public bool IsValid() => (double) this.orthoSize != 0.0;
  }
}
