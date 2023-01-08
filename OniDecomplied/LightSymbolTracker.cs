// Decompiled with JetBrains decompiler
// Type: LightSymbolTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LightSymbolTracker")]
public class LightSymbolTracker : KMonoBehaviour, IRenderEveryTick
{
  public HashedString targetSymbol;

  public void RenderEveryTick(float dt)
  {
    Vector3 zero = Vector3.zero;
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    Matrix2x3 matrix2x3 = Matrix2x3.op_Multiply(component.GetTransformMatrix(), component.GetSymbolLocalTransform(this.targetSymbol, out bool _));
    Vector3 vector3 = Vector3.op_Subtraction(((Matrix2x3) ref matrix2x3).MultiplyPoint(Vector3.zero), TransformExtensions.GetPosition(this.transform));
    ((Component) this).GetComponent<Light2D>().Offset = Vector2.op_Implicit(vector3);
  }
}
