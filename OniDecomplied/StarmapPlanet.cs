// Decompiled with JetBrains decompiler
// Type: StarmapPlanet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/StarmapPlanet")]
public class StarmapPlanet : KMonoBehaviour
{
  public List<StarmapPlanetVisualizer> visualizers;

  public void SetSprite(Sprite sprite, Color color)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
    {
      visualizer.image.sprite = sprite;
      ((Graphic) visualizer.image).color = color;
    }
  }

  public void SetFillAmount(float amount)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
      visualizer.image.fillAmount = amount;
  }

  public void SetUnknownBGActive(bool active, Color color)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
    {
      ((Component) visualizer.unknownBG).gameObject.SetActive(active);
      ((Graphic) visualizer.unknownBG).color = color;
    }
  }

  public void SetSelectionActive(bool active)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
      ((Component) visualizer.selection).gameObject.SetActive(active);
  }

  public void SetAnalysisActive(bool active)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
      visualizer.analysisSelection.SetActive(active);
  }

  public void SetLabel(string text)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
    {
      ((TMP_Text) visualizer.label).text = text;
      this.ShowLabel(false);
    }
  }

  public void ShowLabel(bool show)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
      ((Component) visualizer.label).gameObject.SetActive(show);
  }

  public void SetOnClick(System.Action del)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
      visualizer.button.onClick = del;
  }

  public void SetOnEnter(System.Action del)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
      visualizer.button.onEnter = del;
  }

  public void SetOnExit(System.Action del)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
      visualizer.button.onExit = del;
  }

  public void AnimateSelector(float time)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
      visualizer.selection.anchoredPosition = new Vector2(0.0f, (float) (25.0 + (double) Mathf.Sin(time * 4f) * 5.0));
  }

  public void ShowAsCurrentRocketDestination(bool show)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
    {
      RectTransform rectTransform = Util.rectTransform(visualizer.rocketIconContainer);
      if (((Transform) rectTransform).childCount > 0)
        ((Graphic) ((Component) ((Transform) rectTransform).GetChild(((Transform) rectTransform).childCount - 1)).GetComponent<HierarchyReferences>().GetReference<Image>("fg")).color = show ? new Color(0.117647059f, 0.8627451f, 0.3137255f) : Color.white;
    }
  }

  public void SetRocketIcons(int numRockets, GameObject iconPrefab)
  {
    foreach (StarmapPlanetVisualizer visualizer in this.visualizers)
    {
      RectTransform rectTransform1 = Util.rectTransform(visualizer.rocketIconContainer);
      for (int childCount = ((Transform) rectTransform1).childCount; childCount < numRockets; ++childCount)
        Util.KInstantiateUI(iconPrefab, visualizer.rocketIconContainer, true);
      for (int childCount = ((Transform) rectTransform1).childCount; childCount > numRockets; --childCount)
        Object.Destroy((Object) ((Component) ((Transform) rectTransform1).GetChild(childCount - 1)).gameObject);
      int num = 0;
      foreach (RectTransform rectTransform2 in (Transform) rectTransform1)
      {
        rectTransform2.anchoredPosition = new Vector2((float) num * -10f, 0.0f);
        ++num;
      }
    }
  }
}
