// Decompiled with JetBrains decompiler
// Type: FaceGraph
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FaceGraph")]
public class FaceGraph : KMonoBehaviour
{
  private List<Expression> expressions = new List<Expression>();
  [MyCmpGet]
  private KBatchedAnimController m_controller;
  [MyCmpGet]
  private Accessorizer m_accessorizer;
  [MyCmpGet]
  private SymbolOverrideController m_symbolOverrideController;
  private BlinkMonitor.Instance m_blinkMonitor;
  private SpeechMonitor.Instance m_speechMonitor;
  private static HashedString HASH_HEAD_MASTER_SWAP_KANIM = HashedString.op_Implicit("head_master_swap_kanim");
  private static KAnimHashedString ANIM_HASH_SNAPTO_EYES = KAnimHashedString.op_Implicit("snapto_eyes");
  private static KAnimHashedString ANIM_HASH_SNAPTO_MOUTH = KAnimHashedString.op_Implicit("snapto_mouth");
  private static KAnimHashedString ANIM_HASH_NEUTRAL = KAnimHashedString.op_Implicit("neutral");
  private static int FIRST_SIDEWAYS_FRAME = 29;

  public IEnumerator<Expression> GetEnumerator() => (IEnumerator<Expression>) this.expressions.GetEnumerator();

  public Expression overrideExpression { get; private set; }

  public Expression currentExpression { get; private set; }

  public void AddExpression(Expression expression)
  {
    if (this.expressions.Contains(expression))
      return;
    this.expressions.Add(expression);
    this.UpdateFace();
  }

  public void RemoveExpression(Expression expression)
  {
    if (!this.expressions.Remove(expression))
      return;
    this.UpdateFace();
  }

  public void SetOverrideExpression(Expression expression)
  {
    if (expression == this.overrideExpression)
      return;
    this.overrideExpression = expression;
    this.UpdateFace();
  }

  public void ApplyShape()
  {
    KAnimFile anim = Assets.GetAnim(FaceGraph.HASH_HEAD_MASTER_SWAP_KANIM);
    bool should_use_sideways_symbol = this.ShouldUseSidewaysSymbol(this.m_controller);
    if (this.m_blinkMonitor == null)
      this.m_blinkMonitor = ((Component) this.m_accessorizer).GetSMI<BlinkMonitor.Instance>();
    if (this.m_speechMonitor == null)
      this.m_speechMonitor = ((Component) this.m_accessorizer).GetSMI<SpeechMonitor.Instance>();
    if (this.m_blinkMonitor.IsNullOrStopped() || !this.m_blinkMonitor.IsBlinking())
      this.ApplyShape(this.m_accessorizer.GetAccessory(Db.Get().AccessorySlots.Eyes).symbol, this.m_controller, anim, FaceGraph.ANIM_HASH_SNAPTO_EYES, should_use_sideways_symbol);
    if (this.m_speechMonitor.IsNullOrStopped() || !this.m_speechMonitor.IsPlayingSpeech())
      this.ApplyShape(this.m_accessorizer.GetAccessory(Db.Get().AccessorySlots.Mouth).symbol, this.m_controller, anim, FaceGraph.ANIM_HASH_SNAPTO_MOUTH, should_use_sideways_symbol);
    else
      this.m_speechMonitor.DrawMouth();
  }

  private bool ShouldUseSidewaysSymbol(KBatchedAnimController controller)
  {
    KAnim.Anim currentAnim = controller.GetCurrentAnim();
    if (currentAnim == null)
      return false;
    int currentFrameIndex = controller.GetCurrentFrameIndex();
    if (currentFrameIndex <= 0)
      return false;
    KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(currentAnim.animFile.animBatchTag);
    KAnim.Anim.Frame frame = batchGroupData.GetFrame(currentFrameIndex);
    for (int index = 0; index < frame.numElements; ++index)
    {
      KAnim.Anim.FrameElement frameElement = batchGroupData.GetFrameElement(frame.firstElementIdx + index);
      if (KAnimHashedString.op_Equality(frameElement.symbol, FaceGraph.ANIM_HASH_SNAPTO_EYES) && frameElement.frame >= FaceGraph.FIRST_SIDEWAYS_FRAME)
        return true;
    }
    return false;
  }

  private void ApplyShape(
    KAnim.Build.Symbol variation_symbol,
    KBatchedAnimController controller,
    KAnimFile shapes_file,
    KAnimHashedString symbol_name_in_shape_file,
    bool should_use_sideways_symbol)
  {
    HashedString hashedString = HashedString.op_Implicit(FaceGraph.ANIM_HASH_NEUTRAL);
    if (this.currentExpression != null)
      hashedString = this.currentExpression.face.hash;
    KAnim.Anim anim1 = (KAnim.Anim) null;
    KAnim.Anim.FrameElement frameElement = new KAnim.Anim.FrameElement();
    bool flag1 = false;
    bool flag2 = false;
    for (int index1 = 0; index1 < shapes_file.GetData().animCount && !flag1; ++index1)
    {
      KAnim.Anim anim2 = shapes_file.GetData().GetAnim(index1);
      if (HashedString.op_Equality(anim2.hash, hashedString))
      {
        anim1 = anim2;
        KAnim.Anim.Frame frame = anim1.GetFrame(shapes_file.GetData().build.batchTag, 0);
        for (int index2 = 0; index2 < frame.numElements; ++index2)
        {
          frameElement = KAnimBatchManager.Instance().GetBatchGroupData(shapes_file.GetData().animBatchTag).GetFrameElement(frame.firstElementIdx + index2);
          if (!KAnimHashedString.op_Inequality(frameElement.symbol, symbol_name_in_shape_file))
          {
            if (flag2 || !should_use_sideways_symbol)
              flag1 = true;
            flag2 = true;
            break;
          }
        }
      }
    }
    if (anim1 == null)
      DebugUtil.Assert(false, "Could not find shape for expression: " + HashCache.Get().Get(hashedString));
    if (!flag2)
      DebugUtil.Assert(false, "Could not find shape element for shape:" + HashCache.Get().Get(variation_symbol.hash));
    KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(controller.batchGroupID).GetSymbol(symbol_name_in_shape_file);
    KAnim.Build.SymbolFrameInstance symbolFrameInstance = KAnimBatchManager.Instance().GetBatchGroupData(variation_symbol.build.batchTag).symbolFrameInstances[variation_symbol.firstFrameIdx + frameElement.frame];
    symbolFrameInstance.buildImageIdx = this.m_symbolOverrideController.GetAtlasIdx(variation_symbol.build.GetTexture(0));
    controller.SetSymbolOverride(symbol.firstFrameIdx, ref symbolFrameInstance);
  }

  private void UpdateFace()
  {
    Expression expression1 = (Expression) null;
    if (this.overrideExpression != null)
      expression1 = this.overrideExpression;
    else if (this.expressions.Count > 0)
    {
      this.expressions.Sort((Comparison<Expression>) ((a, b) => b.priority.CompareTo(a.priority)));
      expression1 = this.expressions[0];
    }
    if (expression1 != this.currentExpression || expression1 == null)
    {
      this.currentExpression = expression1;
      this.m_symbolOverrideController.MarkDirty();
    }
    AccessorySlot headEffects = Db.Get().AccessorySlots.HeadEffects;
    if (this.currentExpression != null)
    {
      Accessory accessory1 = this.m_accessorizer.GetAccessory(Db.Get().AccessorySlots.HeadEffects);
      HashedString full_id = HashedString.Invalid;
      foreach (Expression expression2 in this.expressions)
      {
        if (((HashedString) ref expression2.face.headFXHash).IsValid)
        {
          full_id = expression2.face.headFXHash;
          break;
        }
      }
      Accessory accessory2 = HashedString.op_Inequality(full_id, HashedString.Invalid) ? headEffects.Lookup(full_id) : (Accessory) null;
      if (accessory1 != accessory2)
      {
        if (accessory1 != null)
          this.m_accessorizer.RemoveAccessory(accessory1);
        if (accessory2 != null)
          this.m_accessorizer.AddAccessory(accessory2);
      }
      this.m_controller.SetSymbolVisiblity(headEffects.targetSymbolId, accessory2 != null);
    }
    else
      this.m_controller.SetSymbolVisiblity(headEffects.targetSymbolId, false);
  }
}
