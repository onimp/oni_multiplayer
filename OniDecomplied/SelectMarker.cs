// Decompiled with JetBrains decompiler
// Type: SelectMarker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SelectMarker")]
public class SelectMarker : KMonoBehaviour
{
  public float animationOffset = 0.1f;
  private Transform targetTransform;

  public void SetTargetTransform(Transform target_transform)
  {
    this.targetTransform = target_transform;
    this.LateUpdate();
  }

  private void LateUpdate()
  {
    if (Object.op_Equality((Object) this.targetTransform, (Object) null))
    {
      ((Component) this).gameObject.SetActive(false);
    }
    else
    {
      Vector3 position = TransformExtensions.GetPosition(this.targetTransform);
      KCollider2D component = ((Component) this.targetTransform).GetComponent<KCollider2D>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        ref Vector3 local1 = ref position;
        Bounds bounds = component.bounds;
        double x = (double) ((Bounds) ref bounds).center.x;
        local1.x = (float) x;
        ref Vector3 local2 = ref position;
        bounds = component.bounds;
        double y = (double) ((Bounds) ref bounds).center.y;
        bounds = component.bounds;
        double num1 = (double) ((Bounds) ref bounds).size.y / 2.0;
        double num2 = y + num1 + 0.10000000149011612;
        local2.y = (float) num2;
      }
      else
        position.y += 2f;
      Vector3 vector3;
      // ISSUE: explicit constructor call
      ((Vector3) ref vector3).\u002Ector(0.0f, (Mathf.Sin(Time.unscaledTime * 4f) + 1f) * this.animationOffset, 0.0f);
      TransformExtensions.SetPosition(this.transform, Vector3.op_Addition(position, vector3));
    }
  }
}
