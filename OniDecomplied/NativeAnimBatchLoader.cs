// Decompiled with JetBrains decompiler
// Type: NativeAnimBatchLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class NativeAnimBatchLoader : MonoBehaviour
{
  public bool performTimeUpdate;
  public bool performUpdate;
  public bool performRender;
  public bool setTimeScale;
  public bool destroySelf;
  public bool generateObjects;
  public GameObject[] enableObjects;

  private void Start()
  {
    if (this.generateObjects)
    {
      for (int index = 0; index < this.enableObjects.Length; ++index)
      {
        if (Object.op_Inequality((Object) this.enableObjects[index], (Object) null))
        {
          this.enableObjects[index].GetComponent<KBatchedAnimController>().visibilityType = KAnimControllerBase.VisibilityType.Always;
          this.enableObjects[index].SetActive(true);
        }
      }
    }
    if (this.setTimeScale)
      Time.timeScale = 1f;
    if (!this.destroySelf)
      return;
    Object.Destroy((Object) this);
  }

  private void LateUpdate()
  {
    if (this.destroySelf)
      return;
    if (this.performUpdate)
    {
      KAnimBatchManager.Instance().UpdateActiveArea(new Vector2I(0, 0), new Vector2I(9999, 9999));
      KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
    }
    if (!this.performRender)
      return;
    KAnimBatchManager.Instance().Render();
  }
}
