// Decompiled with JetBrains decompiler
// Type: SoundCuller
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public struct SoundCuller
{
  private Vector2 min;
  private Vector2 max;
  private Vector2 cameraPos;
  private float zoomScaler;

  public static bool IsAudibleWorld(Vector2 pos)
  {
    bool flag = false;
    int cell = Grid.PosToCell(pos);
    if (Grid.IsValidCell(cell) && (int) Grid.WorldIdx[cell] == ClusterManager.Instance.activeWorldId)
      flag = true;
    return flag;
  }

  public bool IsAudible(Vector2 pos) => SoundCuller.IsAudibleWorld(pos) && VectorUtil.LessEqual(this.min, pos) && VectorUtil.LessEqual(pos, this.max);

  public bool IsAudibleNoCameraScaling(Vector2 pos, float falloff_distance_sq) => ((double) pos.x - (double) this.cameraPos.x) * ((double) pos.x - (double) this.cameraPos.x) + ((double) pos.y - (double) this.cameraPos.y) * ((double) pos.y - (double) this.cameraPos.y) < (double) falloff_distance_sq;

  public bool IsAudible(Vector2 pos, float falloff_distance_sq)
  {
    if (!SoundCuller.IsAudibleWorld(pos))
      return false;
    pos = Vector2.op_Implicit(this.GetVerticallyScaledPosition(Vector2.op_Implicit(pos)));
    return this.IsAudibleNoCameraScaling(pos, falloff_distance_sq);
  }

  public bool IsAudible(Vector2 pos, HashedString sound_path) => ((HashedString) ref sound_path).IsValid && this.IsAudible(pos, KFMOD.GetSoundEventDescription(sound_path).falloffDistanceSq);

  public Vector3 GetVerticallyScaledPosition(Vector3 pos, bool objectIsSelectedAndVisible = false)
  {
    float num1 = 1f;
    float num2;
    if ((double) pos.y > (double) this.max.y)
      num2 = Mathf.Abs(pos.y - this.max.y);
    else if ((double) pos.y < (double) this.min.y)
    {
      num2 = Mathf.Abs(pos.y - this.min.y);
      num1 = -1f;
    }
    else
      num2 = 0.0f;
    float extraYrange = TuningData<SoundCuller.Tuning>.Get().extraYRange;
    float num3 = (double) num2 < (double) extraYrange ? num2 : extraYrange;
    float num4 = (float) ((double) num3 * (double) num3 / (4.0 * (double) this.zoomScaler)) * num1;
    Vector3 verticallyScaledPosition;
    // ISSUE: explicit constructor call
    ((Vector3) ref verticallyScaledPosition).\u002Ector(pos.x, pos.y + num4, 0.0f);
    if (objectIsSelectedAndVisible)
      verticallyScaledPosition.z = pos.z;
    return verticallyScaledPosition;
  }

  public static SoundCuller CreateCuller()
  {
    SoundCuller culler = new SoundCuller();
    Camera main = Camera.main;
    Vector3 worldPoint1 = main.ViewportToWorldPoint(new Vector3(1f, 1f, TransformExtensions.GetPosition(((Component) Camera.main).transform).z));
    Vector3 worldPoint2 = main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, TransformExtensions.GetPosition(((Component) Camera.main).transform).z));
    culler.min = Vector2.op_Implicit(new Vector3(worldPoint2.x, worldPoint2.y, 0.0f));
    culler.max = Vector2.op_Implicit(new Vector3(worldPoint1.x, worldPoint1.y, 0.0f));
    culler.cameraPos = Vector2.op_Implicit(TransformExtensions.GetPosition(((Component) main).transform));
    Audio audio = Audio.Get();
    float num = (double) (CameraController.Instance.OrthographicSize / (audio.listenerReferenceZ - audio.listenerMinZ)) > 0.0 ? 1f : 2f;
    culler.zoomScaler = num;
    return culler;
  }

  public class Tuning : TuningData<SoundCuller.Tuning>
  {
    public float extraYRange;
  }
}
