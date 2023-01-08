// Decompiled with JetBrains decompiler
// Type: RocketThrustWidget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/RocketThrustWidget")]
public class RocketThrustWidget : 
  KMonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler
{
  public Image graphBar;
  public Image graphDot;
  public LocText graphDotText;
  public Image hoverMarker;
  public ToolTip hoverTooltip;
  public RectTransform markersContainer;
  public Image markerTemplate;
  private RectTransform rectTransform;
  private float maxMass = 20000f;
  private float totalWidth = 5f;
  private bool mouseOver;
  public CommandModule commandModule;

  protected virtual void OnPrefabInit()
  {
  }

  public void Draw(CommandModule commandModule)
  {
    if (Object.op_Equality((Object) this.rectTransform, (Object) null))
      this.rectTransform = ((Component) this.graphBar).gameObject.GetComponent<RectTransform>();
    this.commandModule = commandModule;
    Rect rect = this.rectTransform.rect;
    this.totalWidth = ((Rect) ref rect).width;
    this.UpdateGraphDotPos(commandModule);
  }

  private void UpdateGraphDotPos(CommandModule rocket)
  {
    Rect rect = this.rectTransform.rect;
    this.totalWidth = ((Rect) ref rect).width;
    TransformExtensions.SetLocalPosition((Transform) ((Graphic) this.graphDot).rectTransform, new Vector3(Mathf.Clamp(Mathf.Lerp(0.0f, this.totalWidth, rocket.rocketStats.GetTotalMass() / this.maxMass), 0.0f, this.totalWidth), 0.0f, 0.0f));
    ((TMP_Text) this.graphDotText).text = "-" + Util.FormatWholeNumber(rocket.rocketStats.GetTotalThrust() - rocket.rocketStats.GetRocketMaxDistance()) + "km";
  }

  private void Update()
  {
    if (!this.mouseOver)
      return;
    if (Object.op_Equality((Object) this.rectTransform, (Object) null))
      this.rectTransform = ((Component) this.graphBar).gameObject.GetComponent<RectTransform>();
    Vector3 position = TransformExtensions.GetPosition((Transform) this.rectTransform);
    Rect rect = this.rectTransform.rect;
    Vector2 size = ((Rect) ref rect).size;
    float num1 = Mathf.Clamp((float) ((double) KInputManager.GetMousePos().x - (double) position.x + (double) size.x / 2.0), 0.0f, this.totalWidth);
    TransformExtensions.SetLocalPosition((Transform) ((Graphic) this.hoverMarker).rectTransform, new Vector3(num1, 0.0f, 0.0f));
    float num2 = Mathf.Lerp(0.0f, this.maxMass, num1 / this.totalWidth);
    float totalThrust = this.commandModule.rocketStats.GetTotalThrust();
    float rocketMaxDistance = this.commandModule.rocketStats.GetRocketMaxDistance();
    this.hoverTooltip.SetSimpleTooltip((string) STRINGS.UI.STARMAP.ROCKETWEIGHT.MASS + GameUtil.GetFormattedMass(num2, massFormat: GameUtil.MetricMassFormat.Kilogram) + "\n" + (string) STRINGS.UI.STARMAP.ROCKETWEIGHT.MASSPENALTY + Util.FormatWholeNumber(TUNING.ROCKETRY.CalculateMassWithPenalty(num2)) + (string) STRINGS.UI.UNITSUFFIXES.DISTANCE.KILOMETER + "\n\n" + (string) STRINGS.UI.STARMAP.ROCKETWEIGHT.CURRENTMASS + GameUtil.GetFormattedMass(this.commandModule.rocketStats.GetTotalMass(), massFormat: GameUtil.MetricMassFormat.Kilogram) + "\n" + (string) STRINGS.UI.STARMAP.ROCKETWEIGHT.CURRENTMASSPENALTY + Util.FormatWholeNumber(totalThrust - rocketMaxDistance) + (string) STRINGS.UI.UNITSUFFIXES.DISTANCE.KILOMETER);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    this.mouseOver = true;
    KMonoBehaviourExtensions.SetAlpha(this.hoverMarker, 1f);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    this.mouseOver = false;
    KMonoBehaviourExtensions.SetAlpha(this.hoverMarker, 0.0f);
  }
}
