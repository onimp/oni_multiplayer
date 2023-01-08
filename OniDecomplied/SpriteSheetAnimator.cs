// Decompiled with JetBrains decompiler
// Type: SpriteSheetAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetAnimator
{
  private SpriteSheet sheet;
  private Mesh mesh;
  private MaterialPropertyBlock materialProperties;
  private List<SpriteSheetAnimator.AnimInfo> anims = new List<SpriteSheetAnimator.AnimInfo>();
  private List<SpriteSheetAnimator.AnimInfo> rotatedAnims = new List<SpriteSheetAnimator.AnimInfo>();

  public SpriteSheetAnimator(SpriteSheet sheet)
  {
    this.sheet = sheet;
    this.mesh = new Mesh();
    ((Object) this.mesh).name = nameof (SpriteSheetAnimator);
    this.mesh.MarkDynamic();
    this.materialProperties = new MaterialPropertyBlock();
    this.materialProperties.SetTexture("_MainTex", (Texture) sheet.texture);
  }

  public void Play(Vector3 pos, Quaternion rotation, Vector2 size, Color colour)
  {
    if (Quaternion.op_Equality(rotation, Quaternion.identity))
      this.anims.Add(new SpriteSheetAnimator.AnimInfo()
      {
        elapsedTime = 0.0f,
        pos = pos,
        rotation = rotation,
        size = size,
        colour = Color32.op_Implicit(colour)
      });
    else
      this.rotatedAnims.Add(new SpriteSheetAnimator.AnimInfo()
      {
        elapsedTime = 0.0f,
        pos = pos,
        rotation = rotation,
        size = size,
        colour = Color32.op_Implicit(colour)
      });
  }

  private void GetUVs(
    int frame,
    out Vector2 uv_bl,
    out Vector2 uv_br,
    out Vector2 uv_tl,
    out Vector2 uv_tr)
  {
    int num1 = frame / this.sheet.numXFrames;
    int num2;
    float num3 = (float) (num2 = frame % this.sheet.numXFrames) * this.sheet.uvFrameSize.x;
    float num4 = (float) (num2 + 1) * this.sheet.uvFrameSize.x;
    float num5 = (float) (1.0 - (double) (num1 + 1) * (double) this.sheet.uvFrameSize.y);
    float num6 = (float) (1.0 - (double) num1 * (double) this.sheet.uvFrameSize.y);
    uv_bl = new Vector2(num3, num5);
    uv_br = new Vector2(num4, num5);
    uv_tl = new Vector2(num3, num6);
    uv_tr = new Vector2(num4, num6);
  }

  public int GetFrameFromElapsedTime(float elapsed_time) => Mathf.Min(this.sheet.numFrames, (int) ((double) elapsed_time / 0.033333335071802139));

  public int GetFrameFromElapsedTimeLooping(float elapsed_time)
  {
    int elapsedTimeLooping = (int) ((double) elapsed_time / 0.033333335071802139);
    if (elapsedTimeLooping > this.sheet.numFrames)
      elapsedTimeLooping %= this.sheet.numFrames;
    return elapsedTimeLooping;
  }

  public void UpdateAnims(float dt)
  {
    this.UpdateAnims(dt, (IList<SpriteSheetAnimator.AnimInfo>) this.anims);
    this.UpdateAnims(dt, (IList<SpriteSheetAnimator.AnimInfo>) this.rotatedAnims);
  }

  private void UpdateAnims(float dt, IList<SpriteSheetAnimator.AnimInfo> anims)
  {
    int count = anims.Count;
    int index = 0;
    while (index < count)
    {
      SpriteSheetAnimator.AnimInfo anim = anims[index];
      anim.elapsedTime += dt;
      anim.frame = Mathf.Min(this.sheet.numFrames, (int) ((double) anim.elapsedTime / 0.033333335071802139));
      if (anim.frame >= this.sheet.numFrames)
      {
        --count;
        anims[index] = anims[count];
        anims.RemoveAt(count);
      }
      else
      {
        anims[index] = anim;
        ++index;
      }
    }
  }

  public void Render(List<SpriteSheetAnimator.AnimInfo> anim_infos, bool apply_rotation)
  {
    ListPool<Vector3, SpriteSheetAnimManager>.PooledList pooledList1 = ListPool<Vector3, SpriteSheetAnimManager>.Allocate();
    ListPool<Vector2, SpriteSheetAnimManager>.PooledList pooledList2 = ListPool<Vector2, SpriteSheetAnimManager>.Allocate();
    ListPool<Color32, SpriteSheetAnimManager>.PooledList pooledList3 = ListPool<Color32, SpriteSheetAnimManager>.Allocate();
    ListPool<int, SpriteSheetAnimManager>.PooledList pooledList4 = ListPool<int, SpriteSheetAnimManager>.Allocate();
    this.mesh.Clear();
    if (apply_rotation)
    {
      int count = anim_infos.Count;
      for (int index = 0; index < count; ++index)
      {
        SpriteSheetAnimator.AnimInfo animInfo = anim_infos[index];
        Vector2 vector2 = Vector2.op_Multiply(animInfo.size, 0.5f);
        Vector3 vector3_1 = Quaternion.op_Multiply(animInfo.rotation, Vector2.op_Implicit(Vector2.op_UnaryNegation(vector2)));
        Vector3 vector3_2 = Quaternion.op_Multiply(animInfo.rotation, Vector2.op_Implicit(new Vector2(vector2.x, -vector2.y)));
        Vector3 vector3_3 = Quaternion.op_Multiply(animInfo.rotation, Vector2.op_Implicit(new Vector2(-vector2.x, vector2.y)));
        Vector3 vector3_4 = Quaternion.op_Multiply(animInfo.rotation, Vector2.op_Implicit(vector2));
        ((List<Vector3>) pooledList1).Add(Vector3.op_Addition(animInfo.pos, vector3_1));
        ((List<Vector3>) pooledList1).Add(Vector3.op_Addition(animInfo.pos, vector3_2));
        ((List<Vector3>) pooledList1).Add(Vector3.op_Addition(animInfo.pos, vector3_4));
        ((List<Vector3>) pooledList1).Add(Vector3.op_Addition(animInfo.pos, vector3_3));
        Vector2 uv_bl;
        Vector2 uv_br;
        Vector2 uv_tl;
        Vector2 uv_tr;
        this.GetUVs(animInfo.frame, out uv_bl, out uv_br, out uv_tl, out uv_tr);
        ((List<Vector2>) pooledList2).Add(uv_bl);
        ((List<Vector2>) pooledList2).Add(uv_br);
        ((List<Vector2>) pooledList2).Add(uv_tr);
        ((List<Vector2>) pooledList2).Add(uv_tl);
        ((List<Color32>) pooledList3).Add(animInfo.colour);
        ((List<Color32>) pooledList3).Add(animInfo.colour);
        ((List<Color32>) pooledList3).Add(animInfo.colour);
        ((List<Color32>) pooledList3).Add(animInfo.colour);
        int num = index * 4;
        ((List<int>) pooledList4).Add(num);
        ((List<int>) pooledList4).Add(num + 1);
        ((List<int>) pooledList4).Add(num + 2);
        ((List<int>) pooledList4).Add(num);
        ((List<int>) pooledList4).Add(num + 2);
        ((List<int>) pooledList4).Add(num + 3);
      }
    }
    else
    {
      int count = anim_infos.Count;
      for (int index = 0; index < count; ++index)
      {
        SpriteSheetAnimator.AnimInfo animInfo = anim_infos[index];
        Vector2 vector2 = Vector2.op_Multiply(animInfo.size, 0.5f);
        Vector3 vector3_5 = Vector2.op_Implicit(Vector2.op_UnaryNegation(vector2));
        Vector3 vector3_6 = Vector2.op_Implicit(new Vector2(vector2.x, -vector2.y));
        Vector3 vector3_7 = Vector2.op_Implicit(new Vector2(-vector2.x, vector2.y));
        Vector3 vector3_8 = Vector2.op_Implicit(vector2);
        ((List<Vector3>) pooledList1).Add(Vector3.op_Addition(animInfo.pos, vector3_5));
        ((List<Vector3>) pooledList1).Add(Vector3.op_Addition(animInfo.pos, vector3_6));
        ((List<Vector3>) pooledList1).Add(Vector3.op_Addition(animInfo.pos, vector3_8));
        ((List<Vector3>) pooledList1).Add(Vector3.op_Addition(animInfo.pos, vector3_7));
        Vector2 uv_bl;
        Vector2 uv_br;
        Vector2 uv_tl;
        Vector2 uv_tr;
        this.GetUVs(animInfo.frame, out uv_bl, out uv_br, out uv_tl, out uv_tr);
        ((List<Vector2>) pooledList2).Add(uv_bl);
        ((List<Vector2>) pooledList2).Add(uv_br);
        ((List<Vector2>) pooledList2).Add(uv_tr);
        ((List<Vector2>) pooledList2).Add(uv_tl);
        ((List<Color32>) pooledList3).Add(animInfo.colour);
        ((List<Color32>) pooledList3).Add(animInfo.colour);
        ((List<Color32>) pooledList3).Add(animInfo.colour);
        ((List<Color32>) pooledList3).Add(animInfo.colour);
        int num = index * 4;
        ((List<int>) pooledList4).Add(num);
        ((List<int>) pooledList4).Add(num + 1);
        ((List<int>) pooledList4).Add(num + 2);
        ((List<int>) pooledList4).Add(num);
        ((List<int>) pooledList4).Add(num + 2);
        ((List<int>) pooledList4).Add(num + 3);
      }
    }
    this.mesh.SetVertices((List<Vector3>) pooledList1);
    this.mesh.SetUVs(0, (List<Vector2>) pooledList2);
    this.mesh.SetColors((List<Color32>) pooledList3);
    this.mesh.SetTriangles((List<int>) pooledList4, 0);
    Graphics.DrawMesh(this.mesh, Vector3.zero, Quaternion.identity, this.sheet.material, this.sheet.renderLayer, (Camera) null, 0, this.materialProperties);
    pooledList4.Recycle();
    pooledList3.Recycle();
    pooledList2.Recycle();
    pooledList1.Recycle();
  }

  public void Render()
  {
    this.Render(this.anims, false);
    this.Render(this.rotatedAnims, true);
  }

  public struct AnimInfo
  {
    public int frame;
    public float elapsedTime;
    public Vector3 pos;
    public Quaternion rotation;
    public Vector2 size;
    public Color32 colour;
  }
}
