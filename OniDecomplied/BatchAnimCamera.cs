// Decompiled with JetBrains decompiler
// Type: BatchAnimCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class BatchAnimCamera : MonoBehaviour
{
  private static readonly float pan_speed = 5f;
  private static readonly float zoom_speed = 5f;
  public static Bounds bounds = new Bounds(new Vector3(0.0f, 0.0f, -50f), new Vector3(0.0f, 0.0f, 50f));
  private float zoom_min = 1f;
  private float zoom_max = 100f;
  private Camera cam;
  private bool do_pan;
  private Vector3 last_pan;

  private void Awake() => this.cam = ((Component) this).GetComponent<Camera>();

  private void Update()
  {
    if (Input.GetKey((KeyCode) 275))
      TransformExtensions.SetPosition(((Component) this).transform, Vector3.op_Addition(TransformExtensions.GetPosition(((Component) this).transform), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.right, BatchAnimCamera.pan_speed), Time.deltaTime)));
    if (Input.GetKey((KeyCode) 276))
      TransformExtensions.SetPosition(((Component) this).transform, Vector3.op_Addition(TransformExtensions.GetPosition(((Component) this).transform), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.left, BatchAnimCamera.pan_speed), Time.deltaTime)));
    if (Input.GetKey((KeyCode) 273))
      TransformExtensions.SetPosition(((Component) this).transform, Vector3.op_Addition(TransformExtensions.GetPosition(((Component) this).transform), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.up, BatchAnimCamera.pan_speed), Time.deltaTime)));
    if (Input.GetKey((KeyCode) 274))
      TransformExtensions.SetPosition(((Component) this).transform, Vector3.op_Addition(TransformExtensions.GetPosition(((Component) this).transform), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.down, BatchAnimCamera.pan_speed), Time.deltaTime)));
    this.ClampToBounds();
    if (Input.GetKey((KeyCode) 304))
    {
      if (Input.GetMouseButtonDown(0))
      {
        this.do_pan = true;
        this.last_pan = KInputManager.GetMousePos();
      }
      else if (Input.GetMouseButton(0) && this.do_pan)
      {
        Vector3 viewportPoint = this.cam.ScreenToViewportPoint(Vector3.op_Subtraction(this.last_pan, KInputManager.GetMousePos()));
        Vector3 vector3;
        // ISSUE: explicit constructor call
        ((Vector3) ref vector3).\u002Ector(viewportPoint.x * BatchAnimCamera.pan_speed, viewportPoint.y * BatchAnimCamera.pan_speed, 0.0f);
        ((Component) this).transform.Translate(vector3, (Space) 0);
        this.ClampToBounds();
        this.last_pan = KInputManager.GetMousePos();
      }
    }
    if (Input.GetMouseButtonUp(0))
      this.do_pan = false;
    float axis = Input.GetAxis("Mouse ScrollWheel");
    if ((double) axis == 0.0)
      return;
    this.cam.fieldOfView = Mathf.Clamp(this.cam.fieldOfView - axis * BatchAnimCamera.zoom_speed, this.zoom_min, this.zoom_max);
  }

  private void ClampToBounds()
  {
    Vector3 position = TransformExtensions.GetPosition(((Component) this).transform);
    position.x = Mathf.Clamp(TransformExtensions.GetPosition(((Component) this).transform).x, ((Bounds) ref BatchAnimCamera.bounds).min.x, ((Bounds) ref BatchAnimCamera.bounds).max.x);
    position.y = Mathf.Clamp(TransformExtensions.GetPosition(((Component) this).transform).y, ((Bounds) ref BatchAnimCamera.bounds).min.y, ((Bounds) ref BatchAnimCamera.bounds).max.y);
    position.z = Mathf.Clamp(TransformExtensions.GetPosition(((Component) this).transform).z, ((Bounds) ref BatchAnimCamera.bounds).min.z, ((Bounds) ref BatchAnimCamera.bounds).max.z);
    TransformExtensions.SetPosition(((Component) this).transform, position);
  }

  private void OnDrawGizmosSelected() => DebugExtension.DebugBounds(BatchAnimCamera.bounds, Color.red, 0.0f, true);
}
