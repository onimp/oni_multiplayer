// Decompiled with JetBrains decompiler
// Type: SpaceArtifact
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SpaceArtifact")]
public class SpaceArtifact : KMonoBehaviour, IGameObjectEffectDescriptor
{
  public const string ID = "SpaceArtifact";
  private const string charmedPrefix = "entombed_";
  private const string idlePrefix = "idle_";
  [SerializeField]
  private string ui_anim;
  [Serialize]
  private bool loadCharmed = true;
  public ArtifactTier artifactTier;
  public ArtifactType artifactType;
  public string uniqueAnimNameFragment;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.loadCharmed && DlcManager.IsExpansion1Active())
    {
      ((Component) this).gameObject.AddTag(GameTags.CharmedArtifact);
      this.SetEntombedDecor();
    }
    else
    {
      this.loadCharmed = false;
      this.SetAnalyzedDecor();
    }
    this.UpdateStatusItem();
    Components.SpaceArtifacts.Add(this);
    this.UpdateAnim();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Components.SpaceArtifacts.Remove(this);
  }

  public void RemoveCharm()
  {
    ((Component) this).gameObject.RemoveTag(GameTags.CharmedArtifact);
    this.UpdateStatusItem();
    this.loadCharmed = false;
    this.UpdateAnim();
    this.SetAnalyzedDecor();
  }

  private void SetEntombedDecor() => ((Component) this).GetComponent<DecorProvider>().SetValues(TUNING.DECOR.BONUS.TIER0);

  private void SetAnalyzedDecor() => ((Component) this).GetComponent<DecorProvider>().SetValues(this.artifactTier.decorValues);

  public void UpdateStatusItem()
  {
    if (((Component) this).gameObject.HasTag(GameTags.CharmedArtifact))
      ((Component) this).gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.ArtifactEntombed);
    else
      ((Component) this).gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.ArtifactEntombed);
  }

  public void SetArtifactTier(ArtifactTier tier) => this.artifactTier = tier;

  public ArtifactTier GetArtifactTier() => this.artifactTier;

  public void SetUIAnim(string anim) => this.ui_anim = anim;

  public string GetUIAnim() => this.ui_anim;

  public List<Descriptor> GetEffectDescriptions()
  {
    List<Descriptor> effectDescriptions = new List<Descriptor>();
    if (((Component) this).gameObject.HasTag(GameTags.CharmedArtifact))
    {
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector(STRINGS.BUILDINGS.PREFABS.ARTIFACTANALYSISSTATION.PAYLOAD_DROP_RATE.Replace("{chance}", GameUtil.GetFormattedPercent(this.artifactTier.payloadDropChance * 100f)), STRINGS.BUILDINGS.PREFABS.ARTIFACTANALYSISSTATION.PAYLOAD_DROP_RATE_TOOLTIP.Replace("{chance}", GameUtil.GetFormattedPercent(this.artifactTier.payloadDropChance * 100f)), (Descriptor.DescriptorType) 1, false);
      effectDescriptions.Add(descriptor);
    }
    Descriptor descriptor1;
    // ISSUE: explicit constructor call
    ((Descriptor) ref descriptor1).\u002Ector(string.Format("This is an artifact from space"), string.Format("This is the tooltip string"), (Descriptor.DescriptorType) 3, false);
    effectDescriptions.Add(descriptor1);
    return effectDescriptions;
  }

  public List<Descriptor> GetDescriptors(GameObject go) => this.GetEffectDescriptions();

  private void UpdateAnim()
  {
    string str = !((Component) this).gameObject.HasTag(GameTags.CharmedArtifact) ? this.uniqueAnimNameFragment : "entombed_" + this.uniqueAnimNameFragment.Replace("idle_", "");
    ((Component) this).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit(str), (KAnim.PlayMode) 0);
  }

  [OnDeserialized]
  public void OnDeserialize()
  {
    Pickupable component = ((Component) this).GetComponent<Pickupable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.deleteOffGrid = false;
  }
}
