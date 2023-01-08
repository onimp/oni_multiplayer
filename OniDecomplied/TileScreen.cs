// Decompiled with JetBrains decompiler
// Type: TileScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileScreen : KScreen
{
  public Text nameLabel;
  public Text symbolLabel;
  public Text massTitleLabel;
  public Text massAmtLabel;
  public Image massIcon;
  public MinMaxSlider temperatureSlider;
  public Text temperatureSliderText;
  public Image temperatureSliderIcon;
  public Image solidIcon;
  public Image liquidIcon;
  public Image gasIcon;
  public Text solidText;
  public Text gasText;
  [SerializeField]
  private Color temperatureDefaultColour;
  [SerializeField]
  private Color temperatureTransitionColour;

  private bool SetSliderColour(float temperature, float transition_temperature)
  {
    if ((double) Mathf.Abs(temperature - transition_temperature) < 5.0)
    {
      ((Graphic) this.temperatureSliderText).color = this.temperatureTransitionColour;
      ((Graphic) this.temperatureSliderIcon).color = this.temperatureTransitionColour;
      return true;
    }
    ((Graphic) this.temperatureSliderText).color = this.temperatureDefaultColour;
    ((Graphic) this.temperatureSliderIcon).color = this.temperatureDefaultColour;
    return false;
  }

  private void DisplayTileInfo()
  {
    Vector3 mousePos = KInputManager.GetMousePos();
    mousePos.z = -TransformExtensions.GetPosition(((Component) Camera.main).transform).z - Grid.CellSizeInMeters;
    int cell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(mousePos));
    if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
    {
      Element element1 = Grid.Element[cell];
      this.nameLabel.text = element1.name;
      float num1 = Grid.Mass[cell];
      string str = "kg";
      if ((double) num1 < 5.0)
      {
        num1 *= 1000f;
        str = "g";
      }
      if ((double) num1 < 5.0)
      {
        num1 *= 1000f;
        str = "mg";
      }
      if ((double) num1 < 5.0)
      {
        float num2 = num1 * 1000f;
        str = "mcg";
        num1 = Mathf.Floor(num2);
      }
      this.massAmtLabel.text = string.Format("{0:0.0} {1}", (object) num1, (object) str);
      this.massTitleLabel.text = "mass";
      float num3 = Grid.Temperature[cell];
      if (element1.IsSolid)
      {
        ((Component) ((Component) this.solidIcon).gameObject.transform.parent).gameObject.SetActive(true);
        ((Component) ((Component) this.gasIcon).gameObject.transform.parent).gameObject.SetActive(false);
        this.massIcon.sprite = this.solidIcon.sprite;
        this.solidText.text = ((int) element1.highTemp).ToString();
        this.gasText.text = "";
        ((Transform) ((Graphic) this.liquidIcon).rectTransform).SetParent(((Component) this.solidIcon).transform.parent, true);
        TransformExtensions.SetLocalPosition((Transform) ((Graphic) this.liquidIcon).rectTransform, new Vector3(0.0f, 64f));
        this.SetSliderColour(num3, element1.highTemp);
        this.temperatureSlider.SetMinMaxValue(element1.highTemp, Mathf.Min(element1.highTemp + 100f, 4000f), Mathf.Max(element1.highTemp - 100f, 0.0f), Mathf.Min(element1.highTemp + 100f, 4000f));
      }
      else if (element1.IsLiquid)
      {
        ((Component) ((Component) this.solidIcon).gameObject.transform.parent).gameObject.SetActive(true);
        ((Component) ((Component) this.gasIcon).gameObject.transform.parent).gameObject.SetActive(true);
        this.massIcon.sprite = this.liquidIcon.sprite;
        this.solidText.text = ((int) element1.lowTemp).ToString();
        this.gasText.text = ((int) element1.highTemp).ToString();
        ((Transform) ((Graphic) this.liquidIcon).rectTransform).SetParent(this.temperatureSlider.transform.parent, true);
        TransformExtensions.SetLocalPosition((Transform) ((Graphic) this.liquidIcon).rectTransform, new Vector3(-80f, 0.0f));
        if (!this.SetSliderColour(num3, element1.lowTemp))
          this.SetSliderColour(num3, element1.highTemp);
        this.temperatureSlider.SetMinMaxValue(element1.lowTemp, element1.highTemp, Mathf.Max(element1.lowTemp - 100f, 0.0f), Mathf.Min(element1.highTemp + 100f, 5200f));
      }
      else if (element1.IsGas)
      {
        this.solidText.text = "";
        this.gasText.text = ((int) element1.lowTemp).ToString();
        ((Component) ((Component) this.solidIcon).gameObject.transform.parent).gameObject.SetActive(false);
        ((Component) ((Component) this.gasIcon).gameObject.transform.parent).gameObject.SetActive(true);
        this.massIcon.sprite = this.gasIcon.sprite;
        this.SetSliderColour(num3, element1.lowTemp);
        ((Transform) ((Graphic) this.liquidIcon).rectTransform).SetParent(((Component) this.gasIcon).transform.parent, true);
        TransformExtensions.SetLocalPosition((Transform) ((Graphic) this.liquidIcon).rectTransform, new Vector3(0.0f, -64f));
        this.temperatureSlider.SetMinMaxValue(0.0f, Mathf.Max(element1.lowTemp - 100f, 0.0f), 0.0f, element1.lowTemp + 100f);
      }
      this.temperatureSlider.SetExtraValue(num3);
      this.temperatureSliderText.text = GameUtil.GetFormattedTemperature((float) (int) num3);
      Dictionary<int, float> info = FallingWater.instance.GetInfo(cell);
      if (info.Count <= 0)
        return;
      List<Element> elements = ElementLoader.elements;
      foreach (KeyValuePair<int, float> keyValuePair in info)
      {
        Element element2 = elements[keyValuePair.Key];
        Text nameLabel = this.nameLabel;
        nameLabel.text = nameLabel.text + "\n" + element2.name + string.Format(" {0:0.00} kg", (object) keyValuePair.Value);
      }
    }
    else
      this.nameLabel.text = "Unknown";
  }

  private void DisplayConduitFlowInfo()
  {
    HashedString mode = OverlayScreen.Instance.GetMode();
    UtilityNetworkManager<FlowUtilityNetwork, Vent> utilityNetworkManager = HashedString.op_Equality(mode, OverlayModes.GasConduits.ID) ? Game.Instance.gasConduitSystem : Game.Instance.liquidConduitSystem;
    ConduitFlow conduitFlow = HashedString.op_Equality(mode, OverlayModes.LiquidConduits.ID) ? Game.Instance.gasConduitFlow : Game.Instance.liquidConduitFlow;
    Vector3 mousePos = KInputManager.GetMousePos();
    mousePos.z = -TransformExtensions.GetPosition(((Component) Camera.main).transform).z - Grid.CellSizeInMeters;
    int cell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(mousePos));
    if (Grid.IsValidCell(cell) && utilityNetworkManager.GetConnections(cell, true) != (UtilityConnections) 0)
    {
      ConduitFlow.ConduitContents contents = conduitFlow.GetContents(cell);
      Element elementByHash = ElementLoader.FindElementByHash(contents.element);
      float mass = contents.mass;
      float temperature = contents.temperature;
      this.nameLabel.text = elementByHash.name;
      string str = "kg";
      if ((double) mass < 5.0)
      {
        mass *= 1000f;
        str = "g";
      }
      this.massAmtLabel.text = string.Format("{0:0.0} {1}", (object) mass, (object) str);
      this.massTitleLabel.text = "mass";
      if (elementByHash.IsLiquid)
      {
        ((Component) ((Component) this.solidIcon).gameObject.transform.parent).gameObject.SetActive(true);
        ((Component) ((Component) this.gasIcon).gameObject.transform.parent).gameObject.SetActive(true);
        this.massIcon.sprite = this.liquidIcon.sprite;
        this.solidText.text = ((int) elementByHash.lowTemp).ToString();
        this.gasText.text = ((int) elementByHash.highTemp).ToString();
        ((Transform) ((Graphic) this.liquidIcon).rectTransform).SetParent(this.temperatureSlider.transform.parent, true);
        TransformExtensions.SetLocalPosition((Transform) ((Graphic) this.liquidIcon).rectTransform, new Vector3(-80f, 0.0f));
        if (!this.SetSliderColour(temperature, elementByHash.lowTemp))
          this.SetSliderColour(temperature, elementByHash.highTemp);
        this.temperatureSlider.SetMinMaxValue(elementByHash.lowTemp, elementByHash.highTemp, Mathf.Max(elementByHash.lowTemp - 100f, 0.0f), Mathf.Min(elementByHash.highTemp + 100f, 5200f));
      }
      else if (elementByHash.IsGas)
      {
        this.solidText.text = "";
        this.gasText.text = ((int) elementByHash.lowTemp).ToString();
        ((Component) ((Component) this.solidIcon).gameObject.transform.parent).gameObject.SetActive(false);
        ((Component) ((Component) this.gasIcon).gameObject.transform.parent).gameObject.SetActive(true);
        this.massIcon.sprite = this.gasIcon.sprite;
        this.SetSliderColour(temperature, elementByHash.lowTemp);
        ((Transform) ((Graphic) this.liquidIcon).rectTransform).SetParent(((Component) this.gasIcon).transform.parent, true);
        TransformExtensions.SetLocalPosition((Transform) ((Graphic) this.liquidIcon).rectTransform, new Vector3(0.0f, -64f));
        this.temperatureSlider.SetMinMaxValue(0.0f, Mathf.Max(elementByHash.lowTemp - 100f, 0.0f), 0.0f, elementByHash.lowTemp + 100f);
      }
      this.temperatureSlider.SetExtraValue(temperature);
      this.temperatureSliderText.text = GameUtil.GetFormattedTemperature((float) (int) temperature);
    }
    else
    {
      this.nameLabel.text = "No Conduit";
      this.symbolLabel.text = "";
      this.massAmtLabel.text = "";
      this.massTitleLabel.text = "";
    }
  }

  private void Update()
  {
    TransformExtensions.SetPosition(((KMonoBehaviour) this).transform, KInputManager.GetMousePos());
    HashedString mode = OverlayScreen.Instance.GetMode();
    if (HashedString.op_Equality(mode, OverlayModes.GasConduits.ID) || HashedString.op_Equality(mode, OverlayModes.LiquidConduits.ID))
      this.DisplayConduitFlowInfo();
    else
      this.DisplayTileInfo();
  }
}
