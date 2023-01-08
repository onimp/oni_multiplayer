// Decompiled with JetBrains decompiler
// Type: EightDirectionUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

[SerializationConfig]
public class EightDirectionUtil
{
  public static readonly Vector3[] normals;

  public static int GetDirectionIndex(EightDirection direction) => (int) direction;

  public static EightDirection AngleToDirection(int angle) => (EightDirection) Mathf.Floor((float) angle / 45f);

  public static Vector3 GetNormal(EightDirection direction) => EightDirectionUtil.normals[EightDirectionUtil.GetDirectionIndex(direction)];

  public static float GetAngle(EightDirection direction) => (float) (45 * EightDirectionUtil.GetDirectionIndex(direction));

  static EightDirectionUtil()
  {
    Vector3[] vector3Array = new Vector3[8]
    {
      Vector3.up,
      null,
      null,
      null,
      null,
      null,
      null,
      null
    };
    Vector3 vector3_1 = Vector3.op_Addition(Vector3.up, Vector3.left);
    vector3Array[1] = ((Vector3) ref vector3_1).normalized;
    vector3Array[2] = Vector3.left;
    Vector3 vector3_2 = Vector3.op_Addition(Vector3.down, Vector3.left);
    vector3Array[3] = ((Vector3) ref vector3_2).normalized;
    vector3Array[4] = Vector3.down;
    Vector3 vector3_3 = Vector3.op_Addition(Vector3.down, Vector3.right);
    vector3Array[5] = ((Vector3) ref vector3_3).normalized;
    vector3Array[6] = Vector3.right;
    Vector3 vector3_4 = Vector3.op_Addition(Vector3.up, Vector3.right);
    vector3Array[7] = ((Vector3) ref vector3_4).normalized;
    EightDirectionUtil.normals = vector3Array;
  }
}
