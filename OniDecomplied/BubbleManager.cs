// Decompiled with JetBrains decompiler
// Type: BubbleManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BubbleManager")]
public class BubbleManager : KMonoBehaviour, ISim33ms, IRenderEveryTick
{
  public static BubbleManager instance;
  private List<BubbleManager.Bubble> bubbles = new List<BubbleManager.Bubble>();

  public static void DestroyInstance() => BubbleManager.instance = (BubbleManager) null;

  protected virtual void OnPrefabInit() => BubbleManager.instance = this;

  public void SpawnBubble(
    Vector2 position,
    Vector2 velocity,
    SimHashes element,
    float mass,
    float temperature)
  {
    this.bubbles.Add(new BubbleManager.Bubble()
    {
      position = position,
      velocity = velocity,
      element = element,
      temperature = temperature,
      mass = mass
    });
  }

  public void Sim33ms(float dt)
  {
    ListPool<BubbleManager.Bubble, BubbleManager>.PooledList collection = ListPool<BubbleManager.Bubble, BubbleManager>.Allocate();
    ListPool<BubbleManager.Bubble, BubbleManager>.PooledList pooledList = ListPool<BubbleManager.Bubble, BubbleManager>.Allocate();
    foreach (BubbleManager.Bubble bubble in this.bubbles)
    {
      bubble.position = Vector2.op_Addition(bubble.position, Vector2.op_Multiply(bubble.velocity, dt));
      bubble.elapsedTime += dt;
      int cell = Grid.PosToCell(bubble.position);
      if (!Grid.IsVisiblyInLiquid(bubble.position) || Grid.Element[cell].id == bubble.element)
        ((List<BubbleManager.Bubble>) pooledList).Add(bubble);
      else
        ((List<BubbleManager.Bubble>) collection).Add(bubble);
    }
    foreach (BubbleManager.Bubble bubble in (List<BubbleManager.Bubble>) pooledList)
      SimMessages.AddRemoveSubstance(Grid.PosToCell(bubble.position), bubble.element, CellEventLogger.Instance.FallingWaterAddToSim, bubble.mass, bubble.temperature, byte.MaxValue, 0);
    this.bubbles.Clear();
    this.bubbles.AddRange((IEnumerable<BubbleManager.Bubble>) collection);
    pooledList.Recycle();
    collection.Recycle();
  }

  public void RenderEveryTick(float dt)
  {
    ListPool<SpriteSheetAnimator.AnimInfo, BubbleManager>.PooledList pooledList = ListPool<SpriteSheetAnimator.AnimInfo, BubbleManager>.Allocate();
    SpriteSheetAnimator spriteSheetAnimator = SpriteSheetAnimManager.instance.GetSpriteSheetAnimator(HashedString.op_Implicit("liquid_splash1"));
    foreach (BubbleManager.Bubble bubble in this.bubbles)
    {
      SpriteSheetAnimator.AnimInfo animInfo = new SpriteSheetAnimator.AnimInfo()
      {
        frame = spriteSheetAnimator.GetFrameFromElapsedTimeLooping(bubble.elapsedTime),
        elapsedTime = bubble.elapsedTime,
        pos = new Vector3(bubble.position.x, bubble.position.y, 0.0f),
        rotation = Quaternion.identity,
        size = Vector2.one,
        colour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)
      };
      ((List<SpriteSheetAnimator.AnimInfo>) pooledList).Add(animInfo);
    }
    pooledList.Recycle();
  }

  private struct Bubble
  {
    public Vector2 position;
    public Vector2 velocity;
    public float elapsedTime;
    public int frame;
    public SimHashes element;
    public float temperature;
    public float mass;
  }
}
