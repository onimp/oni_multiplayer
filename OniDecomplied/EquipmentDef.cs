// Decompiled with JetBrains decompiler
// Type: EquipmentDef
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;

public class EquipmentDef : Def
{
  public string Id;
  public string Slot;
  public string FabricatorId;
  public float FabricationTime;
  public string RecipeTechUnlock;
  public SimHashes OutputElement;
  public Dictionary<string, float> InputElementMassMap;
  public float Mass;
  public KAnimFile Anim;
  public string SnapOn;
  public string SnapOn1;
  public KAnimFile BuildOverride;
  public int BuildOverridePriority;
  public bool IsBody;
  public List<AttributeModifier> AttributeModifiers;
  public string RecipeDescription;
  public List<Effect> EffectImmunites = new List<Effect>();
  public Action<Equippable> OnEquipCallBack;
  public Action<Equippable> OnUnequipCallBack;
  public EntityTemplates.CollisionShape CollisionShape;
  public float width;
  public float height = 0.325f;
  public Tag[] AdditionalTags;
  public string wornID;
  public List<Descriptor> additionalDescriptors = new List<Descriptor>();

  public override string Name => StringEntry.op_Implicit(Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.Id.ToUpper() + ".NAME"));

  public string GenericName => StringEntry.op_Implicit(Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.Id.ToUpper() + ".GENERICNAME"));

  public string WornName => StringEntry.op_Implicit(Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.Id.ToUpper() + ".WORN_NAME"));

  public string WornDesc => StringEntry.op_Implicit(Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.Id.ToUpper() + ".WORN_DESC"));
}
