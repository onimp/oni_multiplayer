// Decompiled with JetBrains decompiler
// Type: GravityComponents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using UnityEngine;

public class GravityComponents : KGameObjectComponentManager<GravityComponent>
{
  private const float Acceleration = -9.8f;
  private static Tag[] LANDS_ON_FAKEFLOOR = new Tag[3]
  {
    GameTags.Minion,
    GameTags.Creatures.Walker,
    GameTags.Creatures.Hoverer
  };

  public HandleVector<int>.Handle Add(GameObject go, Vector2 initial_velocity, System.Action on_landed = null)
  {
    bool land_on_fake_floors = false;
    KPrefabID component = go.GetComponent<KPrefabID>();
    if (Object.op_Inequality((Object) component, (Object) null))
      land_on_fake_floors = component.HasAnyTags(GravityComponents.LANDS_ON_FAKEFLOOR);
    bool mayLeaveWorld = Object.op_Inequality((Object) go.GetComponent<MinionIdentity>(), (Object) null);
    return this.Add(go, new GravityComponent(go.transform, on_landed, initial_velocity, land_on_fake_floors, mayLeaveWorld));
  }

  public virtual void FixedUpdate(float dt)
  {
    GravityComponents.Tuning tuning = TuningData<GravityComponents.Tuning>.Get();
    float num1 = tuning.maxVelocity * tuning.maxVelocity;
    for (int index = 0; index < ((KCompactedVector<GravityComponent>) this).data.Count; ++index)
    {
      GravityComponent gravityComponent = ((KCompactedVector<GravityComponent>) this).data[index];
      if ((double) gravityComponent.elapsedTime >= 0.0 && !Object.op_Equality((Object) gravityComponent.transform, (Object) null) && !((KComponentManager<GravityComponent>) this).IsInCleanupList(((Component) gravityComponent.transform).gameObject))
      {
        Vector3 position = TransformExtensions.GetPosition(gravityComponent.transform);
        Vector2 pos1 = Vector2.op_Implicit(position);
        Vector2 vector2;
        // ISSUE: explicit constructor call
        ((Vector2) ref vector2).\u002Ector(gravityComponent.velocity.x, gravityComponent.velocity.y + -9.8f * dt);
        float sqrMagnitude = ((Vector2) ref vector2).sqrMagnitude;
        if ((double) sqrMagnitude > (double) num1)
          vector2 = Vector2.op_Multiply(vector2, tuning.maxVelocity / Mathf.Sqrt(sqrMagnitude));
        int cell1 = Grid.PosToCell(pos1);
        bool flag1 = Grid.IsVisiblyInLiquid(Vector2.op_Subtraction(pos1, new Vector2(0.0f, gravityComponent.bottomYOffset)));
        if (flag1)
        {
          flag1 = true;
          float num2 = (float) (((Object) gravityComponent.transform).GetInstanceID() % 1000) / 1000f * 0.25f;
          float num3 = tuning.maxVelocityInLiquid + num2 * tuning.maxVelocityInLiquid;
          if ((double) sqrMagnitude > (double) num3 * (double) num3)
          {
            float num4 = Mathf.Sqrt(sqrMagnitude);
            vector2 = Vector2.op_Multiply(Vector2.op_Division(vector2, num4), Mathf.Lerp(num4, num2, dt * (float) (5.0 + 5.0 * (double) num2)));
          }
        }
        gravityComponent.velocity = vector2;
        gravityComponent.elapsedTime += dt;
        Vector2 pos2 = Vector2.op_Addition(pos1, Vector2.op_Multiply(vector2, dt));
        Vector2 pos3 = pos2;
        pos3.y = pos2.y - gravityComponent.bottomYOffset;
        bool flag2 = Grid.IsVisiblyInLiquid(Vector2.op_Addition(pos2, new Vector2(0.0f, gravityComponent.bottomYOffset)));
        if (!flag1 & flag2)
        {
          KBatchedAnimController effect = FXHelpers.CreateEffect("splash_step_kanim", Vector3.op_Addition(new Vector3(pos2.x, pos2.y, 0.0f), new Vector3(-0.38f, 0.75f, -0.1f)), layer: Grid.SceneLayer.FXFront);
          effect.Play(HashedString.op_Implicit("fx1"));
          effect.destroyOnAnimComplete = true;
        }
        bool flag3 = false;
        int cell2 = Grid.PosToCell(pos3);
        if (Grid.IsValidCell(cell2))
        {
          if ((double) ((Vector2) ref vector2).sqrMagnitude > 0.20000000298023224 && Grid.IsValidCell(cell1) && !Grid.Element[cell1].IsLiquid && Grid.Element[cell2].IsLiquid)
          {
            AmbienceType ambience = Grid.Element[cell2].substance.GetAmbience();
            if (ambience != AmbienceType.None)
            {
              EventReference event_ref = Sounds.Instance.OreSplashSoundsMigrated[(int) ambience];
              if (Object.op_Inequality((Object) CameraController.Instance, (Object) null) && CameraController.Instance.IsAudibleSound(Vector2.op_Implicit(pos2), event_ref))
                SoundEvent.PlayOneShot(event_ref, Vector2.op_Implicit(pos2));
            }
          }
          bool flag4 = Grid.Solid[cell2];
          if (!flag4 && gravityComponent.landOnFakeFloors && Grid.FakeFloor[cell2])
          {
            Navigator component = ((Component) gravityComponent.transform).GetComponent<Navigator>();
            if (Object.op_Implicit((Object) component))
            {
              flag4 = component.NavGrid.NavTable.IsValid(cell2);
              if (!flag4)
              {
                int cell3 = Grid.CellAbove(cell2);
                flag4 = component.NavGrid.NavTable.IsValid(cell3, NavType.Hover);
              }
            }
          }
          if (flag4)
          {
            Vector3 posCbc = Grid.CellToPosCBC(Grid.CellAbove(cell2), Grid.SceneLayer.Move);
            pos2.y = posCbc.y + gravityComponent.bottomYOffset;
            gravityComponent.velocity.x = 0.0f;
            flag3 = true;
          }
          else
          {
            Vector2 pos4 = pos2;
            pos4.x -= gravityComponent.extents.x;
            int cell4 = Grid.PosToCell(pos4);
            if (Grid.IsValidCell(cell4) && Grid.Solid[cell4])
            {
              pos2.x = Mathf.Floor(pos2.x - gravityComponent.extents.x) + (1f + gravityComponent.extents.x);
              gravityComponent.velocity.x = -0.1f * gravityComponent.velocity.x;
            }
            else
            {
              Vector3 pos5 = Vector2.op_Implicit(pos2);
              pos5.x += gravityComponent.extents.x;
              int cell5 = Grid.PosToCell(pos5);
              if (Grid.IsValidCell(cell5) && Grid.Solid[cell5])
              {
                pos2.x = Mathf.Floor(pos2.x + gravityComponent.extents.x) - gravityComponent.extents.x;
                gravityComponent.velocity.x = -0.1f * gravityComponent.velocity.x;
              }
            }
          }
        }
        ((KCompactedVector<GravityComponent>) this).data[index] = gravityComponent;
        int cell6 = Grid.PosToCell(pos2);
        if (gravityComponent.mayLeaveWorld || !Grid.IsValidCell(cell1) || (int) Grid.WorldIdx[cell1] == (int) ClusterManager.INVALID_WORLD_IDX || Grid.IsValidCellInWorld(cell6, (int) Grid.WorldIdx[cell1]))
        {
          TransformExtensions.SetPosition(gravityComponent.transform, new Vector3(pos2.x, pos2.y, position.z));
          if (flag3)
          {
            EventExtensions.Trigger(((Component) gravityComponent.transform).gameObject, 1188683690, (object) vector2);
            if (gravityComponent.onLanded != null)
              gravityComponent.onLanded();
          }
        }
      }
    }
  }

  public class Tuning : TuningData<GravityComponents.Tuning>
  {
    public float maxVelocity;
    public float maxVelocityInLiquid;
  }
}
