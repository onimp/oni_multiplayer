// Decompiled with JetBrains decompiler
// Type: Klei.AI.PlantMutation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Klei.AI
{
  public class PlantMutation : Modifier
  {
    public string desc;
    public string animationSoundEvent;
    public bool originalMutation;
    public List<string> requiredPrefabIDs = new List<string>();
    public List<string> restrictedPrefabIDs = new List<string>();
    private Tag bonusCropID;
    private float bonusCropAmount;
    private byte droppedDiseaseID = byte.MaxValue;
    private int droppedDiseaseOnGrowAmount;
    private int droppedDiseaseContinuousAmount;
    private byte harvestDiseaseID = byte.MaxValue;
    private int harvestDiseaseAmount;
    private bool forcePrefersDarkness;
    private bool forceSelfHarvestOnGrown;
    private PlantElementAbsorber.ConsumeInfo ensureIrrigationInfo;
    private Color plantTint = Color.white;
    private List<string> symbolTintTargets = new List<string>();
    private List<Color> symbolTints = new List<Color>();
    private List<PlantMutation.SymbolOverrideInfo> symbolOverrideInfo;
    private List<string> symbolScaleTargets = new List<string>();
    private List<float> symbolScales = new List<float>();
    private string bGFXAnim;
    private string fGFXAnim;
    private List<string> additionalSoundEvents = new List<string>();

    public List<string> AdditionalSoundEvents => this.additionalSoundEvents;

    public PlantMutation(string id, string name, string desc)
      : base(id, name, desc)
    {
    }

    public void ApplyTo(MutantPlant target)
    {
      this.ApplyFunctionalTo(target);
      if (((Component) target).HasTag(GameTags.Seed) || ((Component) target).HasTag(GameTags.CropSeed) || ((Component) target).HasTag(GameTags.Compostable))
        return;
      this.ApplyVisualTo(target);
    }

    private void ApplyFunctionalTo(MutantPlant target)
    {
      SeedProducer component = ((Component) target).GetComponent<SeedProducer>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.seedInfo.productionType == SeedProducer.ProductionType.Harvest)
        component.Configure(component.seedInfo.seedId, SeedProducer.ProductionType.Sterile, 0);
      if (((Tag) ref this.bonusCropID).IsValid)
        target.Subscribe(-1072826864, new Action<object>(this.OnHarvestBonusCrop));
      if (this.forcePrefersDarkness || this.SelfModifiers.Find((Predicate<Klei.AI.AttributeModifier>) (m => m.AttributeId == Db.Get().PlantAttributes.MinLightLux.Id)) != null)
      {
        IlluminationVulnerable illuminationVulnerable = ((Component) target).GetComponent<IlluminationVulnerable>();
        if (Object.op_Equality((Object) illuminationVulnerable, (Object) null))
          illuminationVulnerable = ((Component) target).gameObject.AddComponent<IlluminationVulnerable>();
        if (this.forcePrefersDarkness)
        {
          if (Object.op_Inequality((Object) illuminationVulnerable, (Object) null))
            illuminationVulnerable.SetPrefersDarkness(true);
        }
        else
        {
          if (Object.op_Inequality((Object) illuminationVulnerable, (Object) null))
            illuminationVulnerable.SetPrefersDarkness();
          ((Component) target).GetComponent<Modifiers>().attributes.Add(Db.Get().PlantAttributes.MinLightLux);
        }
      }
      int droppedDiseaseId = (int) this.droppedDiseaseID;
      if (this.harvestDiseaseID != byte.MaxValue)
        target.Subscribe(35625290, new Action<object>(this.OnCropSpawnedAddDisease));
      int num = ((Tag) ref this.ensureIrrigationInfo.tag).IsValid ? 1 : 0;
      this.AddTo(target.GetAttributes());
    }

    private void ApplyVisualTo(MutantPlant target)
    {
      KBatchedAnimController component1 = ((Component) target).GetComponent<KBatchedAnimController>();
      if (this.symbolOverrideInfo != null && this.symbolOverrideInfo.Count > 0)
      {
        SymbolOverrideController component2 = ((Component) target).GetComponent<SymbolOverrideController>();
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          foreach (PlantMutation.SymbolOverrideInfo symbolOverrideInfo in this.symbolOverrideInfo)
          {
            KAnim.Build.Symbol symbol = Assets.GetAnim(HashedString.op_Implicit(symbolOverrideInfo.sourceAnim)).GetData().build.GetSymbol(KAnimHashedString.op_Implicit(symbolOverrideInfo.sourceSymbol));
            component2.AddSymbolOverride(HashedString.op_Implicit(symbolOverrideInfo.targetSymbolName), symbol);
          }
        }
      }
      if (this.bGFXAnim != null)
        PlantMutation.CreateFXObject(target, this.bGFXAnim, "_BGFX", 0.1f);
      if (this.fGFXAnim != null)
        PlantMutation.CreateFXObject(target, this.fGFXAnim, "_FGFX", -0.1f);
      if (Color.op_Inequality(this.plantTint, Color.white))
        component1.TintColour = Color32.op_Implicit(this.plantTint);
      if (this.symbolTints.Count > 0)
      {
        for (int index = 0; index < this.symbolTints.Count; ++index)
          component1.SetSymbolTint(KAnimHashedString.op_Implicit(this.symbolTintTargets[index]), this.symbolTints[index]);
      }
      if (this.symbolScales.Count > 0)
      {
        for (int index = 0; index < this.symbolScales.Count; ++index)
          component1.SetSymbolScale(KAnimHashedString.op_Implicit(this.symbolScaleTargets[index]), this.symbolScales[index]);
      }
      if (this.additionalSoundEvents.Count <= 0)
        return;
      int num = 0;
      while (num < this.additionalSoundEvents.Count)
        ++num;
    }

    private static void CreateFXObject(
      MutantPlant target,
      string anim,
      string nameSuffix,
      float offset)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(Assets.GetPrefab(Tag.op_Implicit(SimpleFXConfig.ID)));
      ((Object) gameObject).name = ((Object) target).name + nameSuffix;
      gameObject.transform.parent = target.transform;
      gameObject.AddComponent<LoopingSounds>();
      gameObject.GetComponent<KPrefabID>().PrefabTag = new Tag(((Object) gameObject).name);
      Extents extents = ((Component) target).GetComponent<OccupyArea>().GetExtents();
      Vector3 position = TransformExtensions.GetPosition(target.transform);
      position.x = (float) extents.x + (float) extents.width / 2f;
      position.y = (float) extents.y + (float) extents.height / 2f;
      position.z += offset;
      TransformExtensions.SetPosition(gameObject.transform, position);
      KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
      component.AnimFiles = new KAnimFile[1]
      {
        Assets.GetAnim(HashedString.op_Implicit(anim))
      };
      component.initialAnim = "idle";
      component.initialMode = (KAnim.PlayMode) 0;
      component.randomiseLoopedOffset = true;
      component.fgLayer = Grid.SceneLayer.NoLayer;
      if (((Component) target).HasTag(GameTags.Hanging))
        component.Rotation = 180f;
      gameObject.SetActive(true);
    }

    private void OnHarvestBonusCrop(object data) => ((Crop) data).SpawnSomeFruit(this.bonusCropID, this.bonusCropAmount);

    private void OnCropSpawnedAddDisease(object data) => ((GameObject) data).GetComponent<PrimaryElement>().AddDisease(this.harvestDiseaseID, this.harvestDiseaseAmount, this.Name);

    public string GetTooltip()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.desc);
      foreach (Klei.AI.AttributeModifier selfModifier in this.SelfModifiers)
      {
        Attribute attribute = Db.Get().Attributes.TryGet(selfModifier.AttributeId) ?? Db.Get().PlantAttributes.Get(selfModifier.AttributeId);
        if (attribute.ShowInUI != Attribute.Display.Never)
        {
          stringBuilder.Append((string) DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
          stringBuilder.Append(string.Format((string) DUPLICANTS.TRAITS.ATTRIBUTE_MODIFIERS, (object) attribute.Name, (object) selfModifier.GetFormattedString()));
        }
      }
      if (Tag.op_Inequality(this.bonusCropID, Tag.op_Implicit((string) null)))
      {
        string newValue;
        if (GameTags.DisplayAsCalories.Contains(this.bonusCropID))
        {
          EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(((Tag) ref this.bonusCropID).Name);
          DebugUtil.Assert(foodInfo != null, "Eeh? Trying to spawn a bonus crop that is caloric but isn't a food??", this.bonusCropID.ToString());
          newValue = GameUtil.GetFormattedCalories(this.bonusCropAmount * foodInfo.CaloriesPerUnit);
        }
        else
          newValue = !GameTags.DisplayAsUnits.Contains(this.bonusCropID) ? GameUtil.GetFormattedMass(this.bonusCropAmount) : GameUtil.GetFormattedUnits(this.bonusCropAmount, displaySuffix: false);
        stringBuilder.Append((string) DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
        stringBuilder.Append(CREATURES.PLANT_MUTATIONS.BONUS_CROP_FMT.Replace("{Crop}", this.bonusCropID.ProperName()).Replace("{Amount}", newValue));
      }
      if (this.droppedDiseaseID != byte.MaxValue)
      {
        if (this.droppedDiseaseOnGrowAmount > 0)
        {
          stringBuilder.Append((string) DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
          stringBuilder.Append(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_DROPPER_BURST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(this.droppedDiseaseID)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(this.droppedDiseaseOnGrowAmount)));
        }
        if (this.droppedDiseaseContinuousAmount > 0)
        {
          stringBuilder.Append((string) DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
          stringBuilder.Append(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_DROPPER_CONSTANT.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(this.droppedDiseaseID)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(this.droppedDiseaseContinuousAmount, GameUtil.TimeSlice.PerSecond)));
        }
      }
      if (this.harvestDiseaseID != byte.MaxValue)
      {
        stringBuilder.Append((string) DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
        stringBuilder.Append(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_ON_HARVEST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(this.harvestDiseaseID)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(this.harvestDiseaseAmount)));
      }
      if (this.forcePrefersDarkness)
      {
        stringBuilder.Append((string) DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
        stringBuilder.Append((string) UI.GAMEOBJECTEFFECTS.REQUIRES_DARKNESS);
      }
      if (this.forceSelfHarvestOnGrown)
      {
        stringBuilder.Append((string) DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
        stringBuilder.Append((string) UI.UISIDESCREENS.PLANTERSIDESCREEN.AUTO_SELF_HARVEST);
      }
      if (((Tag) ref this.ensureIrrigationInfo.tag).IsValid)
      {
        stringBuilder.Append((string) DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
        stringBuilder.Append(string.Format((string) UI.GAMEOBJECTEFFECTS.IDEAL_FERTILIZER, (object) this.ensureIrrigationInfo.tag.ProperName(), (object) GameUtil.GetFormattedMass(-this.ensureIrrigationInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle), (object) true));
      }
      if (!this.originalMutation)
      {
        stringBuilder.Append((string) DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY);
        stringBuilder.Append((string) UI.GAMEOBJECTEFFECTS.MUTANT_STERILE);
      }
      return stringBuilder.ToString();
    }

    public void GetDescriptors(ref List<Descriptor> descriptors, GameObject go)
    {
      if (this.harvestDiseaseID != byte.MaxValue)
        descriptors.Add(new Descriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_ON_HARVEST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(this.harvestDiseaseID)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(this.harvestDiseaseAmount)), UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.DISEASE_ON_HARVEST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(this.harvestDiseaseID)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(this.harvestDiseaseAmount)), (Descriptor.DescriptorType) 1, false));
      if (!this.forceSelfHarvestOnGrown)
        return;
      descriptors.Add(new Descriptor((string) UI.UISIDESCREENS.PLANTERSIDESCREEN.AUTO_SELF_HARVEST, (string) UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.AUTO_SELF_HARVEST, (Descriptor.DescriptorType) 1, false));
    }

    public PlantMutation Original()
    {
      this.originalMutation = true;
      return this;
    }

    public PlantMutation RequiredPrefabID(string requiredID)
    {
      this.requiredPrefabIDs.Add(requiredID);
      return this;
    }

    public PlantMutation RestrictPrefabID(string restrictedID)
    {
      this.restrictedPrefabIDs.Add(restrictedID);
      return this;
    }

    public PlantMutation AttributeModifier(Attribute attribute, float amount, bool multiplier = false)
    {
      DebugUtil.Assert(!this.forcePrefersDarkness || attribute != Db.Get().PlantAttributes.MinLightLux, "A plant mutation has both darkness and light defined!", this.Id);
      this.Add(new Klei.AI.AttributeModifier(attribute.Id, amount, this.Name, multiplier));
      return this;
    }

    public PlantMutation BonusCrop(Tag cropPrefabID, float bonucCropAmount)
    {
      this.bonusCropID = cropPrefabID;
      this.bonusCropAmount = bonucCropAmount;
      return this;
    }

    public PlantMutation DiseaseDropper(byte diseaseID, int onGrowAmount, int continuousAmount)
    {
      this.droppedDiseaseID = diseaseID;
      this.droppedDiseaseOnGrowAmount = onGrowAmount;
      this.droppedDiseaseContinuousAmount = continuousAmount;
      return this;
    }

    public PlantMutation AddDiseaseToHarvest(byte diseaseID, int amount)
    {
      this.harvestDiseaseID = diseaseID;
      this.harvestDiseaseAmount = amount;
      return this;
    }

    public PlantMutation ForcePrefersDarkness()
    {
      DebugUtil.Assert(this.SelfModifiers.Find((Predicate<Klei.AI.AttributeModifier>) (m => m.AttributeId == Db.Get().PlantAttributes.MinLightLux.Id)) == null, "A plant mutation has both darkness and light defined!", this.Id);
      this.forcePrefersDarkness = true;
      return this;
    }

    public PlantMutation ForceSelfHarvestOnGrown()
    {
      this.forceSelfHarvestOnGrown = true;
      this.AttributeModifier(Db.Get().Amounts.OldAge.maxAttribute, -0.999999f, true);
      return this;
    }

    public PlantMutation EnsureIrrigated(PlantElementAbsorber.ConsumeInfo consumeInfo)
    {
      this.ensureIrrigationInfo = consumeInfo;
      return this;
    }

    public PlantMutation VisualTint(float r, float g, float b)
    {
      Debug.Assert((double) Mathf.Sign(r) == (double) Mathf.Sign(g) && (double) Mathf.Sign(r) == (double) Mathf.Sign(b), (object) "Vales for tints must be all positive or all negative for the shader to work correctly!");
      this.plantTint = (double) r >= 0.0 ? new Color(r, g, b, 0.0f) : Color.op_Addition(Color.white, new Color(r, g, b, 0.0f));
      return this;
    }

    public PlantMutation VisualSymbolTint(string targetSymbolName, float r, float g, float b)
    {
      Debug.Assert((double) Mathf.Sign(r) == (double) Mathf.Sign(g) && (double) Mathf.Sign(r) == (double) Mathf.Sign(b), (object) "Vales for tints must be all positive or all negative for the shader to work correctly!");
      this.symbolTintTargets.Add(targetSymbolName);
      this.symbolTints.Add(Color.op_Addition(Color.white, new Color(r, g, b, 0.0f)));
      return this;
    }

    public PlantMutation VisualSymbolOverride(
      string targetSymbolName,
      string sourceAnim,
      string sourceSymbol)
    {
      if (this.symbolOverrideInfo == null)
        this.symbolOverrideInfo = new List<PlantMutation.SymbolOverrideInfo>();
      this.symbolOverrideInfo.Add(new PlantMutation.SymbolOverrideInfo()
      {
        targetSymbolName = targetSymbolName,
        sourceAnim = sourceAnim,
        sourceSymbol = sourceSymbol
      });
      return this;
    }

    public PlantMutation VisualSymbolScale(string targetSymbolName, float scale)
    {
      this.symbolScaleTargets.Add(targetSymbolName);
      this.symbolScales.Add(scale);
      return this;
    }

    public PlantMutation VisualBGFX(string animName)
    {
      this.bGFXAnim = animName;
      return this;
    }

    public PlantMutation VisualFGFX(string animName)
    {
      this.fGFXAnim = animName;
      return this;
    }

    public PlantMutation AddSoundEvent(string soundEventName)
    {
      this.additionalSoundEvents.Add(soundEventName);
      return this;
    }

    private class SymbolOverrideInfo
    {
      public string targetSymbolName;
      public string sourceAnim;
      public string sourceSymbol;
    }
  }
}
