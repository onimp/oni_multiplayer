// Decompiled with JetBrains decompiler
// Type: SpriteSheetAnimManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SpriteSheetAnimManager")]
public class SpriteSheetAnimManager : KMonoBehaviour, IRenderEveryTick
{
  public const float SECONDS_PER_FRAME = 0.0333333351f;
  [SerializeField]
  private SpriteSheet[] sheets;
  private Dictionary<int, SpriteSheetAnimator> nameIndexMap = new Dictionary<int, SpriteSheetAnimator>();
  public static SpriteSheetAnimManager instance;

  public static void DestroyInstance() => SpriteSheetAnimManager.instance = (SpriteSheetAnimManager) null;

  protected virtual void OnPrefabInit() => SpriteSheetAnimManager.instance = this;

  protected virtual void OnSpawn()
  {
    for (int index = 0; index < this.sheets.Length; ++index)
      this.nameIndexMap[Hash.SDBMLower(this.sheets[index].name)] = new SpriteSheetAnimator(this.sheets[index]);
  }

  public void Play(string name, Vector3 pos, Vector2 size, Color32 colour) => this.Play(Hash.SDBMLower(name), pos, Quaternion.identity, size, colour);

  public void Play(string name, Vector3 pos, Quaternion rotation, Vector2 size, Color32 colour) => this.Play(Hash.SDBMLower(name), pos, rotation, size, colour);

  public void Play(int name_hash, Vector3 pos, Quaternion rotation, Vector2 size, Color32 colour) => this.nameIndexMap[name_hash].Play(pos, rotation, size, Color32.op_Implicit(colour));

  public void RenderEveryTick(float dt)
  {
    this.UpdateAnims(dt);
    this.Render();
  }

  public void UpdateAnims(float dt)
  {
    foreach (KeyValuePair<int, SpriteSheetAnimator> nameIndex in this.nameIndexMap)
      nameIndex.Value.UpdateAnims(dt);
  }

  public void Render()
  {
    Vector3 zero = Vector3.zero;
    foreach (KeyValuePair<int, SpriteSheetAnimator> nameIndex in this.nameIndexMap)
      nameIndex.Value.Render();
  }

  public SpriteSheetAnimator GetSpriteSheetAnimator(HashedString name) => this.nameIndexMap[((HashedString) ref name).HashValue];
}
