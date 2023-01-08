// Decompiled with JetBrains decompiler
// Type: SubSpeciesInfoScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubSpeciesInfoScreen : KModalScreen
{
  [SerializeField]
  private KButton renameButton;
  [SerializeField]
  private KButton saveButton;
  [SerializeField]
  private KButton discardButton;
  [SerializeField]
  private RectTransform mutationsList;
  [SerializeField]
  private Image plantIcon;
  [SerializeField]
  private GameObject mutationsItemPrefab;
  private List<GameObject> mutationLineItems = new List<GameObject>();
  private GeneticAnalysisStation targetStation;

  public override bool IsModal() => true;

  protected virtual void OnSpawn() => base.OnSpawn();

  private void ClearMutations()
  {
    for (int index = this.mutationLineItems.Count - 1; index >= 0; --index)
      Util.KDestroyGameObject(this.mutationLineItems[index]);
    this.mutationLineItems.Clear();
  }

  public void DisplayDiscovery(Tag speciesID, Tag subSpeciesID, GeneticAnalysisStation station)
  {
    this.SetSubspecies(speciesID, subSpeciesID);
    this.targetStation = station;
  }

  private void SetSubspecies(Tag speciesID, Tag subSpeciesID)
  {
    this.ClearMutations();
    PlantSubSpeciesCatalog.SubSpeciesInfo subSpecies = PlantSubSpeciesCatalog.Instance.GetSubSpecies(speciesID, subSpeciesID);
    this.plantIcon.sprite = Def.GetUISprite((object) Assets.GetPrefab(speciesID)).first;
    foreach (string mutationId in subSpecies.mutationIDs)
    {
      PlantMutation plantMutation = Db.Get().PlantMutations.Get(mutationId);
      GameObject gameObject = Util.KInstantiateUI(this.mutationsItemPrefab, ((Component) this.mutationsList).gameObject, true);
      HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
      ((TMP_Text) component.GetReference<LocText>("nameLabel")).text = plantMutation.Name;
      ((TMP_Text) component.GetReference<LocText>("descriptionLabel")).text = plantMutation.description;
      this.mutationLineItems.Add(gameObject);
    }
  }
}
