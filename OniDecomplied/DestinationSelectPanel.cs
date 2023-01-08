// Decompiled with JetBrains decompiler
// Type: DestinationSelectPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.CustomSettings;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DestinationSelectPanel")]
public class DestinationSelectPanel : KMonoBehaviour
{
  [SerializeField]
  private GameObject asteroidPrefab;
  [SerializeField]
  private KButtonDrag dragTarget;
  [SerializeField]
  private MultiToggle leftArrowButton;
  [SerializeField]
  private MultiToggle rightArrowButton;
  [SerializeField]
  private RectTransform asteroidContainer;
  [SerializeField]
  private float asteroidFocusScale = 2f;
  [SerializeField]
  private float asteroidXSeparation = 240f;
  [SerializeField]
  private float focusScaleSpeed = 0.5f;
  [SerializeField]
  private float centeringSpeed = 0.5f;
  [SerializeField]
  private GameObject moonContainer;
  [SerializeField]
  private GameObject moonPrefab;
  private static int chosenClusterCategorySetting;
  private float offset;
  private int selectedIndex = -1;
  private List<DestinationAsteroid2> asteroids = new List<DestinationAsteroid2>();
  private int numAsteroids;
  private List<string> clusterKeys;
  private Dictionary<string, string> clusterStartWorlds;
  private Dictionary<string, ColonyDestinationAsteroidBeltData> asteroidData = new Dictionary<string, ColonyDestinationAsteroidBeltData>();
  private Vector2 dragStartPos;
  private Vector2 dragLastPos;
  private bool isDragging;
  private const string debugFmt = "{world}: {seed} [{traits}] {{settings}}";

  public static int ChosenClusterCategorySetting
  {
    get => DestinationSelectPanel.chosenClusterCategorySetting;
    set => DestinationSelectPanel.chosenClusterCategorySetting = value;
  }

  public event Action<ColonyDestinationAsteroidBeltData> OnAsteroidClicked;

  private float min
  {
    get
    {
      Rect rect = this.asteroidContainer.rect;
      return ((Rect) ref rect).x + this.offset;
    }
  }

  private float max
  {
    get
    {
      double min = (double) this.min;
      Rect rect = this.asteroidContainer.rect;
      double width = (double) ((Rect) ref rect).width;
      return (float) (min + width);
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.dragTarget.onBeginDrag += new System.Action(this.BeginDrag);
    this.dragTarget.onDrag += new System.Action(this.Drag);
    this.dragTarget.onEndDrag += new System.Action(this.EndDrag);
    this.leftArrowButton.onClick += new System.Action(this.ClickLeft);
    this.rightArrowButton.onClick += new System.Action(this.ClickRight);
  }

  private void BeginDrag()
  {
    this.dragStartPos = Vector2.op_Implicit(KInputManager.GetMousePos());
    this.dragLastPos = this.dragStartPos;
    this.isDragging = true;
    KFMOD.PlayUISound(GlobalAssets.GetSound("DestinationSelect_Scroll_Start"));
  }

  private void Drag()
  {
    Vector2 vector2 = Vector2.op_Implicit(KInputManager.GetMousePos());
    float num = vector2.x - this.dragLastPos.x;
    this.dragLastPos = vector2;
    this.offset += num;
    int selectedIndex1 = this.selectedIndex;
    this.selectedIndex = Mathf.RoundToInt(-this.offset / this.asteroidXSeparation);
    this.selectedIndex = Mathf.Clamp(this.selectedIndex, 0, this.clusterStartWorlds.Count - 1);
    int selectedIndex2 = this.selectedIndex;
    if (selectedIndex1 == selectedIndex2)
      return;
    this.OnAsteroidClicked(this.asteroidData[this.clusterKeys[this.selectedIndex]]);
    KFMOD.PlayUISound(GlobalAssets.GetSound("DestinationSelect_Scroll"));
  }

  private void EndDrag()
  {
    this.Drag();
    this.isDragging = false;
    KFMOD.PlayUISound(GlobalAssets.GetSound("DestinationSelect_Scroll_Stop"));
  }

  private void ClickLeft()
  {
    this.selectedIndex = Mathf.Clamp(this.selectedIndex - 1, 0, this.clusterKeys.Count - 1);
    this.OnAsteroidClicked(this.asteroidData[this.clusterKeys[this.selectedIndex]]);
  }

  private void ClickRight()
  {
    this.selectedIndex = Mathf.Clamp(this.selectedIndex + 1, 0, this.clusterKeys.Count - 1);
    this.OnAsteroidClicked(this.asteroidData[this.clusterKeys[this.selectedIndex]]);
  }

  public void Init()
  {
    this.clusterKeys = new List<string>();
    this.clusterStartWorlds = new Dictionary<string, string>();
    this.UpdateDisplayedClusters();
  }

  private void Update()
  {
    if (!this.isDragging)
    {
      float num1 = this.offset + (float) this.selectedIndex * this.asteroidXSeparation;
      float num2 = 0.0f;
      if ((double) num1 != 0.0)
        num2 = -num1;
      float num3 = Mathf.Clamp(num2, (float) (-(double) this.asteroidXSeparation * 2.0), this.asteroidXSeparation * 2f);
      if ((double) num3 != 0.0)
      {
        float num4 = this.centeringSpeed * Time.unscaledDeltaTime;
        float num5 = num3 * this.centeringSpeed * Time.unscaledDeltaTime;
        if ((double) num5 > 0.0 && (double) num5 < (double) num4)
          num5 = Mathf.Min(num4, num3);
        else if ((double) num5 < 0.0 && (double) num5 > -(double) num4)
          num5 = Mathf.Max(-num4, num3);
        this.offset += num5;
      }
    }
    Rect rect = this.asteroidContainer.rect;
    float x1 = ((Rect) ref rect).min.x;
    rect = this.asteroidContainer.rect;
    float x2 = ((Rect) ref rect).max.x;
    this.offset = Mathf.Clamp(this.offset, (float) -(this.clusterStartWorlds.Count - 1) * this.asteroidXSeparation + x1, x2);
    this.RePlaceAsteroids();
    for (int index = 0; index < this.moonContainer.transform.childCount; ++index)
      TransformExtensions.SetLocalPosition(this.moonContainer.transform.GetChild(index).GetChild(0), new Vector3(0.0f, (float) (1.5 + 3.0 * (double) Mathf.Sin((float) (((double) index + (double) Time.realtimeSinceStartup) * 1.25))), 0.0f));
  }

  public void UpdateDisplayedClusters()
  {
    this.clusterKeys.Clear();
    this.clusterStartWorlds.Clear();
    this.asteroidData.Clear();
    foreach (KeyValuePair<string, ClusterLayout> keyValuePair in SettingsCache.clusterLayouts.clusterCache)
    {
      if ((!DlcManager.FeatureClusterSpaceEnabled() || !(keyValuePair.Key == "clusters/SandstoneDefault")) && keyValuePair.Value.clusterCategory == DestinationSelectPanel.ChosenClusterCategorySetting)
      {
        this.clusterKeys.Add(keyValuePair.Key);
        ColonyDestinationAsteroidBeltData asteroidBeltData = new ColonyDestinationAsteroidBeltData(keyValuePair.Value.GetStartWorld(), 0, keyValuePair.Key);
        this.asteroidData[keyValuePair.Key] = asteroidBeltData;
        this.clusterStartWorlds.Add(keyValuePair.Key, keyValuePair.Value.GetStartWorld());
      }
    }
    this.clusterKeys.Sort((Comparison<string>) ((a, b) => SettingsCache.clusterLayouts.clusterCache[a].menuOrder.CompareTo(SettingsCache.clusterLayouts.clusterCache[b].menuOrder)));
  }

  [ContextMenu("RePlaceAsteroids")]
  public void RePlaceAsteroids()
  {
    this.BeginAsteroidDrawing();
    for (int index = 0; index < this.clusterKeys.Count; ++index)
    {
      float num = this.offset + (float) index * this.asteroidXSeparation;
      string clusterKey = this.clusterKeys[index];
      float iconScale = this.asteroidData[clusterKey].GetStartWorld.iconScale;
      TransformExtensions.SetLocalPosition(this.GetAsteroid(clusterKey, index == this.selectedIndex ? this.asteroidFocusScale * iconScale : iconScale).transform, new Vector3(num, index == this.selectedIndex ? (float) (5.0 + 10.0 * (double) Mathf.Sin(Time.realtimeSinceStartup * 1f)) : 0.0f, 0.0f));
    }
    this.EndAsteroidDrawing();
  }

  private void BeginAsteroidDrawing() => this.numAsteroids = 0;

  private void ShowMoons(ColonyDestinationAsteroidBeltData asteroid)
  {
    if (asteroid.worlds.Count > 0)
    {
      while (this.moonContainer.transform.childCount < asteroid.worlds.Count)
        Object.Instantiate<GameObject>(this.moonPrefab, this.moonContainer.transform);
      for (int index1 = 0; index1 < asteroid.worlds.Count; ++index1)
      {
        KBatchedAnimController componentInChildren = ((Component) this.moonContainer.transform.GetChild(index1)).GetComponentInChildren<KBatchedAnimController>();
        int index2 = (index1 - 1 + asteroid.worlds.Count / 2) % asteroid.worlds.Count;
        World world = asteroid.worlds[index2];
        KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(Util.IsNullOrWhiteSpace(world.asteroidIcon) ? AsteroidGridEntity.DEFAULT_ASTEROID_ICON_ANIM : world.asteroidIcon));
        if (Object.op_Inequality((Object) anim, (Object) null))
        {
          componentInChildren.SetVisiblity(true);
          componentInChildren.SwapAnims(new KAnimFile[1]
          {
            anim
          });
          componentInChildren.initialMode = (KAnim.PlayMode) 0;
          componentInChildren.initialAnim = "idle_loop";
          ((Component) componentInChildren).gameObject.SetActive(true);
          if (componentInChildren.HasAnimation(HashedString.op_Implicit(componentInChildren.initialAnim)))
            componentInChildren.Play(HashedString.op_Implicit(componentInChildren.initialAnim), (KAnim.PlayMode) 0);
          ((Component) ((Component) componentInChildren).transform.parent).gameObject.SetActive(true);
        }
      }
      for (int count = asteroid.worlds.Count; count < this.moonContainer.transform.childCount; ++count)
      {
        KBatchedAnimController componentInChildren = ((Component) this.moonContainer.transform.GetChild(count)).GetComponentInChildren<KBatchedAnimController>();
        if (Object.op_Inequality((Object) componentInChildren, (Object) null))
          componentInChildren.SetVisiblity(false);
        ((Component) this.moonContainer.transform.GetChild(count)).gameObject.SetActive(false);
      }
    }
    else
    {
      foreach (KBatchedAnimController componentsInChild in this.moonContainer.GetComponentsInChildren<KBatchedAnimController>())
        componentsInChild.SetVisiblity(false);
    }
  }

  private DestinationAsteroid2 GetAsteroid(string name, float scale)
  {
    DestinationAsteroid2 asteroid;
    if (this.numAsteroids < this.asteroids.Count)
    {
      asteroid = this.asteroids[this.numAsteroids];
    }
    else
    {
      asteroid = Util.KInstantiateUI<DestinationAsteroid2>(this.asteroidPrefab, ((Component) this.asteroidContainer).gameObject, false);
      asteroid.OnClicked += this.OnAsteroidClicked;
      this.asteroids.Add(asteroid);
    }
    asteroid.SetAsteroid(this.asteroidData[name]);
    this.asteroidData[name].TargetScale = scale;
    this.asteroidData[name].Scale += (this.asteroidData[name].TargetScale - this.asteroidData[name].Scale) * this.focusScaleSpeed * Time.unscaledDeltaTime;
    asteroid.transform.localScale = Vector3.op_Multiply(Vector3.one, this.asteroidData[name].Scale);
    ++this.numAsteroids;
    return asteroid;
  }

  private void EndAsteroidDrawing()
  {
    for (int index = 0; index < this.asteroids.Count; ++index)
      ((Component) this.asteroids[index]).gameObject.SetActive(index < this.numAsteroids);
  }

  public ColonyDestinationAsteroidBeltData SelectCluster(string name, int seed)
  {
    this.selectedIndex = this.clusterKeys.IndexOf(name);
    this.asteroidData[name].ReInitialize(seed);
    return this.asteroidData[name];
  }

  public string GetDefaultAsteroid() => this.clusterKeys.First<string>();

  public ColonyDestinationAsteroidBeltData SelectDefaultAsteroid(int seed)
  {
    this.selectedIndex = 0;
    string key = this.asteroidData.Keys.First<string>();
    this.asteroidData[key].ReInitialize(seed);
    return this.asteroidData[key];
  }

  public void ScrollLeft() => this.OnAsteroidClicked(this.asteroidData[this.clusterKeys[Mathf.Max(this.selectedIndex - 1, 0)]]);

  public void ScrollRight() => this.OnAsteroidClicked(this.asteroidData[this.clusterKeys[Mathf.Min(this.selectedIndex + 1, this.clusterStartWorlds.Count - 1)]]);

  private void DebugCurrentSetting()
  {
    ColonyDestinationAsteroidBeltData asteroidBeltData = this.asteroidData[this.clusterKeys[this.selectedIndex]];
    string str1 = "{world}: {seed} [{traits}] {{settings}}";
    string startWorldName = asteroidBeltData.startWorldName;
    string newValue1 = asteroidBeltData.seed.ToString();
    string str2 = str1.Replace("{world}", startWorldName).Replace("{seed}", newValue1);
    List<AsteroidDescriptor> traitDescriptors = asteroidBeltData.GetTraitDescriptors();
    string[] strArray = new string[traitDescriptors.Count];
    for (int index = 0; index < traitDescriptors.Count; ++index)
      strArray[index] = traitDescriptors[index].text;
    string newValue2 = string.Join(", ", strArray);
    string str3 = str2.Replace("{traits}", newValue2);
    switch (CustomGameSettings.Instance.customGameMode)
    {
      case CustomGameSettings.CustomGameMode.Survival:
        str3 = str3.Replace("{settings}", "Survival");
        break;
      case CustomGameSettings.CustomGameMode.Nosweat:
        str3 = str3.Replace("{settings}", "Nosweat");
        break;
      case CustomGameSettings.CustomGameMode.Custom:
        List<string> stringList = new List<string>();
        foreach (KeyValuePair<string, SettingConfig> qualitySetting in CustomGameSettings.Instance.QualitySettings)
        {
          if (qualitySetting.Value.coordinate_dimension >= 0L && qualitySetting.Value.coordinate_dimension_width >= 0L)
          {
            SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(qualitySetting.Key);
            if (currentQualitySetting.id != qualitySetting.Value.GetDefaultLevelId())
              stringList.Add(string.Format("{0}={1}", (object) qualitySetting.Value.label, (object) currentQualitySetting.label));
          }
        }
        str3 = str3.Replace("{settings}", string.Join(", ", stringList.ToArray()));
        break;
    }
    Debug.Log((object) str3);
  }
}
