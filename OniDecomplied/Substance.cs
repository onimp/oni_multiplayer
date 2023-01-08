// Decompiled with JetBrains decompiler
// Type: Substance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using Klei;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Substance
{
  public string name;
  public SimHashes elementID;
  internal Tag nameTag;
  public Color32 colour;
  [FormerlySerializedAs("debugColour")]
  public Color32 uiColour;
  [FormerlySerializedAs("overlayColour")]
  public Color32 conduitColour = Color32.op_Implicit(Color.white);
  [NonSerialized]
  internal bool renderedByWorld;
  [NonSerialized]
  internal int idx;
  public Material material;
  public KAnimFile anim;
  [SerializeField]
  internal bool showInEditor = true;
  [NonSerialized]
  internal KAnimFile[] anims;
  [NonSerialized]
  internal ElementsAudio.ElementAudioConfig audioConfig;
  [NonSerialized]
  internal MaterialPropertyBlock propertyBlock;
  public EventReference fallingStartSound;
  public EventReference fallingStopSound;

  public GameObject SpawnResource(
    Vector3 position,
    float mass,
    float temperature,
    byte disease_idx,
    int disease_count,
    bool prevent_merge = false,
    bool forceTemperature = false,
    bool manual_activation = false)
  {
    GameObject gameObject1 = (GameObject) null;
    PrimaryElement primaryElement = (PrimaryElement) null;
    if (!prevent_merge)
    {
      int cell = Grid.PosToCell(position);
      GameObject gameObject2 = Grid.Objects[cell, 3];
      if (Object.op_Inequality((Object) gameObject2, (Object) null))
      {
        Pickupable component1 = gameObject2.GetComponent<Pickupable>();
        if (Object.op_Inequality((Object) component1, (Object) null))
        {
          Tag tag = GameTagExtensions.Create(this.elementID);
          for (ObjectLayerListItem objectLayerListItem = component1.objectLayerListItem; objectLayerListItem != null; objectLayerListItem = objectLayerListItem.nextItem)
          {
            KPrefabID component2 = objectLayerListItem.gameObject.GetComponent<KPrefabID>();
            if (Tag.op_Equality(component2.PrefabTag, tag))
            {
              PrimaryElement component3 = ((Component) component2).GetComponent<PrimaryElement>();
              if ((double) component3.Mass + (double) mass <= (double) PrimaryElement.MAX_MASS)
              {
                gameObject1 = ((Component) component2).gameObject;
                primaryElement = component3;
                temperature = SimUtil.CalculateFinalTemperature(primaryElement.Mass, primaryElement.Temperature, mass, temperature);
                position = TransformExtensions.GetPosition(gameObject1.transform);
                break;
              }
            }
          }
        }
      }
    }
    if (Object.op_Equality((Object) gameObject1, (Object) null))
    {
      gameObject1 = GameUtil.KInstantiate(Assets.GetPrefab(this.nameTag), Grid.SceneLayer.Ore);
      primaryElement = gameObject1.GetComponent<PrimaryElement>();
      primaryElement.Mass = mass;
    }
    else
    {
      Debug.Assert(Object.op_Inequality((Object) primaryElement, (Object) null));
      Pickupable component = ((Component) primaryElement).GetComponent<Pickupable>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.TotalAmount += mass / primaryElement.MassPerUnit;
      else
        primaryElement.Mass += mass;
    }
    primaryElement.InternalTemperature = temperature;
    position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
    TransformExtensions.SetPosition(gameObject1.transform, position);
    if (!manual_activation)
      this.ActivateSubstanceGameObject(gameObject1, disease_idx, disease_count);
    return gameObject1;
  }

  public void ActivateSubstanceGameObject(GameObject obj, byte disease_idx, int disease_count)
  {
    obj.SetActive(true);
    obj.GetComponent<PrimaryElement>().AddDisease(disease_idx, disease_count, "Substances.SpawnResource");
  }

  private void SetTexture(MaterialPropertyBlock block, string texture_name)
  {
    Texture texture = this.material.GetTexture(texture_name);
    if (!Object.op_Inequality((Object) texture, (Object) null))
      return;
    this.propertyBlock.SetTexture(texture_name, texture);
  }

  public void RefreshPropertyBlock()
  {
    if (this.propertyBlock == null)
      this.propertyBlock = new MaterialPropertyBlock();
    if (!Object.op_Inequality((Object) this.material, (Object) null))
      return;
    this.SetTexture(this.propertyBlock, "_MainTex");
    this.propertyBlock.SetFloat("_WorldUVScale", this.material.GetFloat("_WorldUVScale"));
    if (!ElementLoader.FindElementByHash(this.elementID).IsSolid)
      return;
    this.SetTexture(this.propertyBlock, "_MainTex2");
    this.SetTexture(this.propertyBlock, "_HeightTex2");
    this.propertyBlock.SetFloat("_Frequency", this.material.GetFloat("_Frequency"));
    this.propertyBlock.SetColor("_ShineColour", this.material.GetColor("_ShineColour"));
    this.propertyBlock.SetColor("_ColourTint", this.material.GetColor("_ColourTint"));
  }

  internal AmbienceType GetAmbience() => this.audioConfig == null ? AmbienceType.None : this.audioConfig.ambienceType;

  internal SolidAmbienceType GetSolidAmbience() => this.audioConfig == null ? SolidAmbienceType.None : this.audioConfig.solidAmbienceType;

  internal string GetMiningSound() => this.audioConfig == null ? "" : this.audioConfig.miningSound;

  internal string GetMiningBreakSound() => this.audioConfig == null ? "" : this.audioConfig.miningBreakSound;

  internal string GetOreBumpSound() => this.audioConfig == null ? "" : this.audioConfig.oreBumpSound;

  internal string GetFloorEventAudioCategory() => this.audioConfig == null ? "" : this.audioConfig.floorEventAudioCategory;

  internal string GetCreatureChewSound() => this.audioConfig == null ? "" : this.audioConfig.creatureChewSound;
}
