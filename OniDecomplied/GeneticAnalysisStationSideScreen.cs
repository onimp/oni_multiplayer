// Decompiled with JetBrains decompiler
// Type: GeneticAnalysisStationSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneticAnalysisStationSideScreen : SideScreenContent
{
  [SerializeField]
  private LocText message;
  [SerializeField]
  private GameObject contents;
  [SerializeField]
  private GameObject rowContainer;
  [SerializeField]
  private HierarchyReferences rowPrefab;
  private List<HierarchyReferences> rows = new List<HierarchyReferences>();
  private GeneticAnalysisStation.StatesInstance target;
  private Dictionary<Tag, bool> expandedSeeds = new Dictionary<Tag, bool>();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Refresh();
  }

  public override bool IsValidForTarget(GameObject target) => target.GetSMI<GeneticAnalysisStation.StatesInstance>() != null;

  public override void SetTarget(GameObject target)
  {
    this.target = target.GetSMI<GeneticAnalysisStation.StatesInstance>();
    target.GetComponent<GeneticAnalysisStationWorkable>();
    this.Refresh();
  }

  private void Refresh()
  {
    if (this.target == null)
      return;
    this.DrawPickerMenu();
  }

  private void DrawPickerMenu()
  {
    Dictionary<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> dictionary = new Dictionary<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>>();
    foreach (Tag allDiscoveredSpecy in PlantSubSpeciesCatalog.Instance.GetAllDiscoveredSpecies())
      dictionary[allDiscoveredSpecy] = new List<PlantSubSpeciesCatalog.SubSpeciesInfo>((IEnumerable<PlantSubSpeciesCatalog.SubSpeciesInfo>) PlantSubSpeciesCatalog.Instance.GetAllSubSpeciesForSpecies(allDiscoveredSpecy));
    int index1 = 0;
    foreach (KeyValuePair<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> keyValuePair in dictionary)
    {
      if (PlantSubSpeciesCatalog.Instance.GetAllSubSpeciesForSpecies(keyValuePair.Key).Count > 1)
      {
        GameObject prefab = Assets.GetPrefab(keyValuePair.Key);
        if (!Object.op_Equality((Object) prefab, (Object) null))
        {
          SeedProducer component = prefab.GetComponent<SeedProducer>();
          if (!Object.op_Equality((Object) component, (Object) null))
          {
            Tag tag = TagExtensions.ToTag(component.seedInfo.seedId);
            if (DiscoveredResources.Instance.IsDiscovered(tag))
            {
              HierarchyReferences button;
              if (index1 < this.rows.Count)
              {
                button = this.rows[index1];
              }
              else
              {
                button = Util.KInstantiateUI<HierarchyReferences>(((Component) this.rowPrefab).gameObject, this.rowContainer, false);
                this.rows.Add(button);
              }
              this.ConfigureButton(button, keyValuePair.Key);
              ((Component) this.rows[index1]).gameObject.SetActive(true);
              ++index1;
            }
          }
        }
      }
    }
    for (int index2 = index1; index2 < this.rows.Count; ++index2)
      ((Component) this.rows[index2]).gameObject.SetActive(false);
    if (index1 == 0)
    {
      ((TMP_Text) this.message).text = (string) STRINGS.UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.NONE_DISCOVERED;
      this.contents.gameObject.SetActive(false);
    }
    else
    {
      ((TMP_Text) this.message).text = (string) STRINGS.UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.SELECT_SEEDS;
      this.contents.gameObject.SetActive(true);
    }
  }

  private void ConfigureButton(HierarchyReferences button, Tag speciesID)
  {
    LocText reference1 = button.GetReference<LocText>("Label");
    Image reference2 = button.GetReference<Image>("Icon");
    LocText reference3 = button.GetReference<LocText>("ProgressLabel");
    button.GetReference<ToolTip>("ToolTip");
    Tag seedID = this.GetSeedIDFromPlantID(speciesID);
    bool isForbidden = this.target.GetSeedForbidden(seedID);
    string str = seedID.ProperName();
    ((TMP_Text) reference1).text = str;
    reference2.sprite = Def.GetUISprite((object) seedID).first;
    if (PlantSubSpeciesCatalog.Instance.GetAllSubSpeciesForSpecies(speciesID).Count > 0)
      ((TMP_Text) reference3).text = (string) (isForbidden ? STRINGS.UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.SEED_FORBIDDEN : STRINGS.UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.SEED_ALLOWED);
    else
      ((TMP_Text) reference3).text = (string) STRINGS.UI.UISIDESCREENS.GENETICANALYSISSIDESCREEN.SEED_NO_MUTANTS;
    KToggle component = ((Component) button).GetComponent<KToggle>();
    component.isOn = !isForbidden;
    component.ClearOnClick();
    component.onClick += (System.Action) (() =>
    {
      this.target.SetSeedForbidden(seedID, !isForbidden);
      this.Refresh();
    });
  }

  private Tag GetSeedIDFromPlantID(Tag speciesID) => Tag.op_Implicit(Assets.GetPrefab(speciesID).GetComponent<SeedProducer>().seedInfo.seedId);
}
