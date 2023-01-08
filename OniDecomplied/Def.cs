// Decompiled with JetBrains decompiler
// Type: Def
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Def : ScriptableObject
{
  public string PrefabID;
  public Tag Tag;
  private static Dictionary<Tuple<KAnimFile, string, bool>, Sprite> knownUISprites = new Dictionary<Tuple<KAnimFile, string, bool>, Sprite>();
  public const string DEFAULT_SPRITE = "unknown";

  public virtual void InitDef() => this.Tag = TagManager.Create(this.PrefabID);

  public virtual string Name => (string) null;

  public static Tuple<Sprite, Color> GetUISprite(object item, string animName = "ui", bool centered = false)
  {
    switch (item)
    {
      case Substance _:
        return Def.GetUISprite((object) ElementLoader.FindElementByHash((item as Substance).elementID), animName, centered);
      case Element _:
        if ((item as Element).IsSolid)
          return new Tuple<Sprite, Color>(Def.GetUISpriteFromMultiObjectAnim((item as Element).substance.anim, animName, centered), Color.white);
        if ((item as Element).IsLiquid)
          return new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit("element_liquid")), Color32.op_Implicit((item as Element).substance.uiColour));
        return (item as Element).IsGas ? new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit("element_gas")), Color32.op_Implicit((item as Element).substance.uiColour)) : new Tuple<Sprite, Color>((Sprite) null, Color.clear);
      case AsteroidGridEntity _:
        return new Tuple<Sprite, Color>(((ClusterGridEntity) item).GetUISprite(), Color.white);
      case GameObject _:
        GameObject go = item as GameObject;
        if (ElementLoader.GetElement(go.PrefabID()) != null)
          return Def.GetUISprite((object) ElementLoader.GetElement(go.PrefabID()), animName, centered);
        CreatureBrain component1 = go.GetComponent<CreatureBrain>();
        if (Object.op_Inequality((Object) component1, (Object) null))
          animName = component1.symbolPrefix + "ui";
        SpaceArtifact component2 = go.GetComponent<SpaceArtifact>();
        if (Object.op_Inequality((Object) component2, (Object) null))
          animName = component2.GetUIAnim();
        if (go.HasTag(GameTags.Egg))
        {
          IncubationMonitor.Def def = go.GetDef<IncubationMonitor.Def>();
          if (def != null)
          {
            GameObject prefab = Assets.GetPrefab(def.spawnedCreature);
            if (Object.op_Implicit((Object) prefab))
            {
              CreatureBrain component3 = prefab.GetComponent<CreatureBrain>();
              if (Object.op_Implicit((Object) component3) && !string.IsNullOrEmpty(component3.symbolPrefix))
                animName = component3.symbolPrefix + animName;
            }
          }
        }
        if (go.HasTag(GameTags.MoltShell))
          animName = go.GetComponent<SimpleMassStatusItem>().symbolPrefix + animName;
        KBatchedAnimController component4 = go.GetComponent<KBatchedAnimController>();
        if (Object.op_Implicit((Object) component4))
        {
          Sprite fromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(component4.AnimFiles[0], animName, centered);
          return new Tuple<Sprite, Color>(fromMultiObjectAnim, Object.op_Inequality((Object) fromMultiObjectAnim, (Object) null) ? Color.white : Color.clear);
        }
        if (Object.op_Inequality((Object) go.GetComponent<Building>(), (Object) null))
        {
          Sprite uiSprite = go.GetComponent<Building>().Def.GetUISprite(animName, centered);
          return new Tuple<Sprite, Color>(uiSprite, Object.op_Inequality((Object) uiSprite, (Object) null) ? Color.white : Color.clear);
        }
        Debug.LogWarningFormat("Can't get sprite for type {0} (no KBatchedAnimController)", new object[1]
        {
          (object) item.ToString()
        });
        return new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit("unknown")), Color.grey);
      case string _:
        if (Db.Get().Amounts.Exists(item as string))
          return new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit(Db.Get().Amounts.Get(item as string).uiSprite)), Color.white);
        return Db.Get().Attributes.Exists(item as string) ? new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit(Db.Get().Attributes.Get(item as string).uiSprite)), Color.white) : Def.GetUISprite((object) TagExtensions.ToTag(item as string), animName, centered);
      case Tag _:
        if (ElementLoader.GetElement((Tag) item) != null)
          return Def.GetUISprite((object) ElementLoader.GetElement((Tag) item), animName, centered);
        if (Object.op_Inequality((Object) Assets.GetPrefab((Tag) item), (Object) null))
          return Def.GetUISprite((object) Assets.GetPrefab((Tag) item), animName, centered);
        Tag tag = (Tag) item;
        if (Object.op_Inequality((Object) Assets.GetSprite(HashedString.op_Implicit(((Tag) ref tag).Name)), (Object) null))
        {
          tag = (Tag) item;
          return new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit(((Tag) ref tag).Name)), Color.white);
        }
        break;
    }
    DebugUtil.DevAssertArgs(false, new object[2]
    {
      (object) "Can't get sprite for type ",
      (object) item.ToString()
    });
    return new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit("unknown")), Color.grey);
  }

  public static Tuple<Sprite, Color> GetUISprite(Tag prefabID, string facadeID) => Object.op_Inequality((Object) Assets.GetPrefab(prefabID).GetComponent<Equippable>(), (Object) null) && !Util.IsNullOrWhiteSpace(facadeID) ? Db.GetEquippableFacades().Get(facadeID).GetUISprite() : Def.GetUISprite((object) prefabID);

  public static Sprite GetFacadeUISprite(string facadeID) => Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(HashedString.op_Implicit(Db.GetBuildingFacades().Get(facadeID).AnimFile)));

  public static Sprite GetUISpriteFromMultiObjectAnim(
    KAnimFile animFile,
    string animName = "ui",
    bool centered = false,
    string symbolName = "")
  {
    Tuple<KAnimFile, string, bool> key = new Tuple<KAnimFile, string, bool>(animFile, animName, centered);
    if (Def.knownUISprites.ContainsKey(key))
      return Def.knownUISprites[key];
    if (Object.op_Equality((Object) animFile, (Object) null))
    {
      DebugUtil.LogWarningArgs(new object[2]
      {
        (object) animName,
        (object) "missing Anim File"
      });
      return Assets.GetSprite(HashedString.op_Implicit("unknown"));
    }
    KAnimFileData data = animFile.GetData();
    if (data == null)
    {
      DebugUtil.LogWarningArgs(new object[2]
      {
        (object) animName,
        (object) "KAnimFileData is null"
      });
      return Assets.GetSprite(HashedString.op_Implicit("unknown"));
    }
    if (data.build == null)
      return Assets.GetSprite(HashedString.op_Implicit("unknown"));
    KAnim.Anim.Frame frame1 = KAnim.Anim.Frame.InvalidFrame;
    for (int index = 0; index < data.animCount; ++index)
    {
      KAnim.Anim anim = data.GetAnim(index);
      if (anim.name == animName)
        frame1 = anim.GetFrame(data.batchTag, 0);
    }
    if (!((KAnim.Anim.Frame) ref frame1).IsValid())
    {
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) string.Format("missing '{0}' anim in '{1}'", (object) animName, (object) animFile)
      });
      return Assets.GetSprite(HashedString.op_Implicit("unknown"));
    }
    if (data.elementCount == 0)
      return Assets.GetSprite(HashedString.op_Implicit("unknown"));
    KAnim.Anim.FrameElement frameElement = new KAnim.Anim.FrameElement();
    if (string.IsNullOrEmpty(symbolName))
      symbolName = animName;
    KAnimHashedString kanimHashedString;
    // ISSUE: explicit constructor call
    ((KAnimHashedString) ref kanimHashedString).\u002Ector(symbolName);
    KAnim.Build.Symbol symbol = data.build.GetSymbol(kanimHashedString);
    if (symbol == null)
    {
      DebugUtil.LogWarningArgs(new object[5]
      {
        (object) ((Object) animFile).name,
        (object) animName,
        (object) "placeSymbol [",
        (object) frameElement.symbol,
        (object) "] is missing"
      });
      return Assets.GetSprite(HashedString.op_Implicit("unknown"));
    }
    int frame2 = frameElement.frame;
    KAnim.Build.SymbolFrame symbolFrame = symbol.GetFrame(frame2).symbolFrame;
    if (symbolFrame == null)
    {
      DebugUtil.LogWarningArgs(new object[4]
      {
        (object) animName,
        (object) "SymbolFrame [",
        (object) frameElement.frame,
        (object) "] is missing"
      });
      return Assets.GetSprite(HashedString.op_Implicit("unknown"));
    }
    Texture2D texture = data.build.GetTexture(0);
    Debug.Assert(Object.op_Inequality((Object) texture, (Object) null), (object) ("Invalid texture on " + ((Object) animFile).name));
    float x1 = symbolFrame.uvMin.x;
    float x2 = symbolFrame.uvMax.x;
    float y1 = symbolFrame.uvMax.y;
    float y2 = symbolFrame.uvMin.y;
    int num1 = (int) ((double) ((Texture) texture).width * (double) Mathf.Abs(x2 - x1));
    int num2 = (int) ((double) ((Texture) texture).height * (double) Mathf.Abs(y2 - y1));
    float num3 = Mathf.Abs(symbolFrame.bboxMax.x - symbolFrame.bboxMin.x);
    Rect rect = new Rect();
    ((Rect) ref rect).width = (float) num1;
    ((Rect) ref rect).height = (float) num2;
    ((Rect) ref rect).x = (float) (int) ((double) ((Texture) texture).width * (double) x1);
    ((Rect) ref rect).y = (float) (int) ((double) ((Texture) texture).height * (double) y1);
    float num4 = 100f;
    if (num1 != 0)
      num4 = (float) (100.0 / ((double) num3 / (double) num1));
    Sprite fromMultiObjectAnim = Sprite.Create(texture, rect, centered ? new Vector2(0.5f, 0.5f) : Vector2.zero, num4, 0U, (SpriteMeshType) 0);
    ((Object) fromMultiObjectAnim).name = string.Format("{0}:{1}:{2}:{3}", (object) ((Object) texture).name, (object) animName, (object) frameElement.frame.ToString(), (object) centered);
    Def.knownUISprites[key] = fromMultiObjectAnim;
    return fromMultiObjectAnim;
  }
}
