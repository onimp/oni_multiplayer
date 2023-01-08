// Decompiled with JetBrains decompiler
// Type: OrbitalObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/OrbitalObject")]
[SerializationConfig]
public class OrbitalObject : KMonoBehaviour, IRenderEveryTick
{
  private WorldContainer world;
  private OrbitalData orbitData;
  [Serialize]
  private string animFilename;
  [Serialize]
  private string initialAnim;
  [Serialize]
  private Vector3 worldOrbitingOrigin;
  [Serialize]
  private int orbitingWorldId;
  [Serialize]
  private float angle;
  [Serialize]
  public int timeoffset;
  [Serialize]
  public string orbitalDBId;

  public void Init(
    string orbit_data_name,
    WorldContainer orbiting_world,
    List<Ref<OrbitalObject>> orbiting_obj)
  {
    OrbitalData data = Db.Get().OrbitalTypeCategories.Get(orbit_data_name);
    if (Object.op_Inequality((Object) orbiting_world, (Object) null))
    {
      this.orbitingWorldId = orbiting_world.id;
      this.world = orbiting_world;
      this.worldOrbitingOrigin = this.GetWorldOrigin(this.world, data);
    }
    else
      this.worldOrbitingOrigin = new Vector3((float) Grid.WidthInCells * 0.5f, (float) Grid.HeightInCells * data.yGridPercent, 0.0f);
    this.animFilename = data.animFile;
    this.initialAnim = this.GetInitialAnim(data);
    this.angle = this.GetAngle(data);
    this.timeoffset = this.GetTimeOffset(orbiting_obj);
    this.orbitalDBId = data.Id;
  }

  protected virtual void OnSpawn()
  {
    this.world = ClusterManager.Instance.GetWorld(this.orbitingWorldId);
    this.orbitData = Db.Get().OrbitalTypeCategories.Get(this.orbitalDBId);
    ((Component) this).gameObject.SetActive(false);
    KBatchedAnimController kbatchedAnimController = ((Component) this).gameObject.AddComponent<KBatchedAnimController>();
    kbatchedAnimController.isMovable = true;
    kbatchedAnimController.initialAnim = this.initialAnim;
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit(this.animFilename))
    };
    kbatchedAnimController.initialMode = (KAnim.PlayMode) 0;
    kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
  }

  public void RenderEveryTick(float dt)
  {
    bool behind;
    Vector3 worldPos = this.CalculateWorldPos(GameClock.Instance.GetTime(), out behind);
    Vector3 vector3 = worldPos;
    if ((double) this.orbitData.periodInCycles > 0.0)
    {
      vector3.x = worldPos.x / (float) Grid.WidthInCells;
      vector3.y = worldPos.y / (float) Grid.HeightInCells;
      vector3.x = Camera.main.ViewportToWorldPoint(vector3).x;
      vector3.y = Camera.main.ViewportToWorldPoint(vector3).y;
    }
    bool flag = (!this.orbitData.rotatesBehind || !behind) && (Object.op_Equality((Object) this.world, (Object) null) || ClusterManager.Instance.activeWorldId == this.world.id);
    TransformExtensions.SetPosition(((Component) this).gameObject.transform, vector3);
    if ((double) this.orbitData.periodInCycles > 0.0)
      ((Component) this).gameObject.transform.localScale = Vector3.op_Multiply(Vector3.one, CameraController.Instance.baseCamera.orthographicSize / this.orbitData.distance);
    else
      ((Component) this).gameObject.transform.localScale = Vector3.op_Multiply(Vector3.one, this.orbitData.distance);
    if (((Component) this).gameObject.activeSelf == flag)
      return;
    ((Component) this).gameObject.SetActive(flag);
  }

  private Vector3 CalculateWorldPos(float time, out bool behind)
  {
    Vector3 worldPos;
    if ((double) this.orbitData.periodInCycles > 0.0)
    {
      float num1 = this.orbitData.periodInCycles * 600f;
      float num2 = (float) ((((double) time + (double) this.timeoffset) / (double) num1 - (double) (int) (((double) time + (double) this.timeoffset) / (double) num1)) * 2.0 * 3.1415927410125732);
      float num3 = 0.5f * this.orbitData.radiusScale * (float) this.world.WorldSize.x;
      Vector3 vector3;
      // ISSUE: explicit constructor call
      ((Vector3) ref vector3).\u002Ector(Mathf.Cos(num2), 0.0f, Mathf.Sin(num2));
      behind = (double) vector3.z > (double) this.orbitData.behindZ;
      worldPos = Vector3.op_Addition(this.worldOrbitingOrigin, Quaternion.op_Multiply(Quaternion.Euler(this.angle, 0.0f, 0.0f), Vector3.op_Multiply(vector3, num3)));
      worldPos.z = this.orbitData.renderZ;
    }
    else
    {
      behind = false;
      worldPos = this.worldOrbitingOrigin;
      worldPos.z = this.orbitData.renderZ;
    }
    return worldPos;
  }

  private string GetInitialAnim(OrbitalData data)
  {
    if (!Util.IsNullOrWhiteSpace(data.initialAnim))
      return data.initialAnim;
    KAnimFileData data1 = Assets.GetAnim(HashedString.op_Implicit(data.animFile)).GetData();
    int num = new KRandom().Next(0, data1.animCount - 1);
    return data1.GetAnim(num).name;
  }

  private Vector3 GetWorldOrigin(WorldContainer wc, OrbitalData data) => Object.op_Inequality((Object) wc, (Object) null) ? new Vector3((float) wc.WorldOffset.x + (float) wc.WorldSize.x * data.xGridPercent, (float) wc.WorldOffset.y + (float) wc.WorldSize.y * data.yGridPercent, 0.0f) : new Vector3((float) Grid.WidthInCells * data.xGridPercent, (float) Grid.HeightInCells * data.yGridPercent, 0.0f);

  private float GetAngle(OrbitalData data) => Random.Range(data.minAngle, data.maxAngle);

  private int GetTimeOffset(List<Ref<OrbitalObject>> orbiting_obj)
  {
    List<int> intList = new List<int>();
    foreach (Ref<OrbitalObject> @ref in orbiting_obj)
    {
      if (Object.op_Equality((Object) @ref.Get().world, (Object) this.world))
        intList.Add(@ref.Get().timeoffset);
    }
    int timeOffset = Random.Range(0, 600);
    while (intList.Contains(timeOffset))
      timeOffset = Random.Range(0, 600);
    return timeOffset;
  }
}
