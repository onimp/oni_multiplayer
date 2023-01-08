// Decompiled with JetBrains decompiler
// Type: EdiblesManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EdiblesManager")]
public class EdiblesManager : KMonoBehaviour
{
  private static List<EdiblesManager.FoodInfo> s_allFoodTypes = new List<EdiblesManager.FoodInfo>();
  private static Dictionary<string, EdiblesManager.FoodInfo> s_allFoodMap = new Dictionary<string, EdiblesManager.FoodInfo>();

  public static List<EdiblesManager.FoodInfo> GetAllFoodTypes() => EdiblesManager.s_allFoodTypes.Where<EdiblesManager.FoodInfo>((Func<EdiblesManager.FoodInfo, bool>) (x => DlcManager.IsContentActive(x.DlcId))).ToList<EdiblesManager.FoodInfo>();

  public static EdiblesManager.FoodInfo GetFoodInfo(string foodID)
  {
    string key = foodID.Replace("Compost", "");
    EdiblesManager.FoodInfo foodInfo = (EdiblesManager.FoodInfo) null;
    EdiblesManager.s_allFoodMap.TryGetValue(key, out foodInfo);
    return foodInfo;
  }

  public static bool TryGetFoodInfo(string foodID, out EdiblesManager.FoodInfo info)
  {
    info = (EdiblesManager.FoodInfo) null;
    if (string.IsNullOrEmpty(foodID))
      return false;
    info = EdiblesManager.GetFoodInfo(foodID);
    return info != null;
  }

  public class FoodInfo : IConsumableUIItem
  {
    public string Id;
    public string DlcId;
    public string Name;
    public string Description;
    public float CaloriesPerUnit;
    public float PreserveTemperature;
    public float RotTemperature;
    public float StaleTime;
    public float SpoilTime;
    public bool CanRot;
    public int Quality;
    public List<string> Effects;

    public FoodInfo(
      string id,
      string dlcId,
      float caloriesPerUnit,
      int quality,
      float preserveTemperatue,
      float rotTemperature,
      float spoilTime,
      bool can_rot)
    {
      this.Id = id;
      this.DlcId = dlcId;
      this.CaloriesPerUnit = caloriesPerUnit;
      this.Quality = quality;
      this.PreserveTemperature = preserveTemperatue;
      this.RotTemperature = rotTemperature;
      this.StaleTime = spoilTime / 2f;
      this.SpoilTime = spoilTime;
      this.CanRot = can_rot;
      this.Name = StringEntry.op_Implicit(Strings.Get("STRINGS.ITEMS.FOOD." + id.ToUpper() + ".NAME"));
      this.Description = StringEntry.op_Implicit(Strings.Get("STRINGS.ITEMS.FOOD." + id.ToUpper() + ".DESC"));
      this.Effects = new List<string>();
      EdiblesManager.s_allFoodTypes.Add(this);
      EdiblesManager.s_allFoodMap[this.Id] = this;
    }

    public EdiblesManager.FoodInfo AddEffects(List<string> effects, string[] dlcIds)
    {
      if (DlcManager.IsDlcListValidForCurrentContent(dlcIds))
        this.Effects.AddRange((IEnumerable<string>) effects);
      return this;
    }

    public string ConsumableId => this.Id;

    public string ConsumableName => this.Name;

    public int MajorOrder => this.Quality;

    public int MinorOrder => (int) this.CaloriesPerUnit;

    public bool Display => (double) this.CaloriesPerUnit != 0.0;
  }
}
