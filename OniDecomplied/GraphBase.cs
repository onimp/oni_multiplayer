// Decompiled with JetBrains decompiler
// Type: GraphBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

[AddComponentMenu("KMonoBehaviour/scripts/GraphBase")]
public class GraphBase : KMonoBehaviour
{
  [Header("Axis")]
  public GraphAxis axis_x;
  public GraphAxis axis_y;
  [Header("References")]
  public GameObject prefab_guide_x;
  public GameObject prefab_guide_y;
  public GameObject prefab_guide_horizontal_label;
  public GameObject prefab_guide_vertical_label;
  public GameObject guides_x;
  public GameObject guides_y;
  public LocText label_title;
  public LocText label_x;
  public LocText label_y;
  public string graphName;
  protected List<GameObject> horizontalGuides = new List<GameObject>();
  protected List<GameObject> verticalGuides = new List<GameObject>();
  private const int points_per_guide_line = 2;

  public Vector2 GetRelativePosition(Vector2 absolute_point)
  {
    Vector2 zero = Vector2.zero;
    float num1 = Mathf.Max(1f, this.axis_x.max_value - this.axis_x.min_value);
    float num2 = absolute_point.x - this.axis_x.min_value;
    zero.x = num2 / num1;
    float num3 = Mathf.Max(1f, this.axis_y.max_value - this.axis_y.min_value);
    float num4 = absolute_point.y - this.axis_y.min_value;
    zero.y = num4 / num3;
    return zero;
  }

  public Vector2 GetRelativeSize(Vector2 absolute_size) => this.GetRelativePosition(absolute_size);

  public void ClearGuides()
  {
    this.ClearVerticalGuides();
    this.ClearHorizontalGuides();
  }

  public void ClearHorizontalGuides()
  {
    foreach (GameObject horizontalGuide in this.horizontalGuides)
    {
      if (Object.op_Inequality((Object) horizontalGuide, (Object) null))
        Object.DestroyImmediate((Object) horizontalGuide);
    }
    this.horizontalGuides.Clear();
  }

  public void ClearVerticalGuides()
  {
    foreach (GameObject verticalGuide in this.verticalGuides)
    {
      if (Object.op_Inequality((Object) verticalGuide, (Object) null))
        Object.DestroyImmediate((Object) verticalGuide);
    }
    this.verticalGuides.Clear();
  }

  public void RefreshGuides()
  {
    this.ClearGuides();
    this.RefreshHorizontalGuides();
    this.RefreshVerticalGuides();
  }

  public void RefreshHorizontalGuides()
  {
    if (!Object.op_Inequality((Object) this.prefab_guide_x, (Object) null))
      return;
    GameObject gameObject1 = Util.KInstantiateUI(this.prefab_guide_x, this.guides_x, true);
    ((Object) gameObject1).name = "guides_horizontal";
    Vector2[] vector2Array = new Vector2[2 * (int) ((double) this.axis_y.range / (double) this.axis_y.guide_frequency)];
    for (int index = 0; index < vector2Array.Length; index += 2)
    {
      Vector2 absolute_point1;
      // ISSUE: explicit constructor call
      ((Vector2) ref absolute_point1).\u002Ector(this.axis_x.min_value, (float) index * (this.axis_y.guide_frequency / 2f));
      vector2Array[index] = this.GetRelativePosition(absolute_point1);
      Vector2 absolute_point2;
      // ISSUE: explicit constructor call
      ((Vector2) ref absolute_point2).\u002Ector(this.axis_x.max_value, (float) index * (this.axis_y.guide_frequency / 2f));
      vector2Array[index + 1] = this.GetRelativePosition(absolute_point2);
      if (Object.op_Inequality((Object) this.prefab_guide_horizontal_label, (Object) null))
      {
        GameObject gameObject2 = Util.KInstantiateUI(this.prefab_guide_horizontal_label, gameObject1, true);
        ((TMP_Text) gameObject2.GetComponent<LocText>()).alignment = (TextAlignmentOptions) 4097;
        ((TMP_Text) gameObject2.GetComponent<LocText>()).text = ((int) this.axis_y.guide_frequency * (index / 2)).ToString();
        RectTransform rectTransform = Util.rectTransform(gameObject2);
        double num1 = (double) index;
        Rect rect1 = Util.rectTransform(((Component) this).gameObject).rect;
        double num2 = (double) ((Rect) ref rect1).height / (double) vector2Array.Length;
        Vector2 vector2_1 = new Vector2(8f, (float) (num1 * num2));
        Rect rect2 = Util.rectTransform(((Component) this).gameObject).rect;
        Vector2 vector2_2 = Vector2.op_Division(((Rect) ref rect2).size, 2f);
        Vector3 vector3 = Vector2.op_Implicit(Vector2.op_Subtraction(vector2_1, vector2_2));
        TransformExtensions.SetLocalPosition((Transform) rectTransform, vector3);
      }
    }
    gameObject1.GetComponent<UILineRenderer>().Points = vector2Array;
    this.horizontalGuides.Add(gameObject1);
  }

  public void RefreshVerticalGuides()
  {
    if (!Object.op_Inequality((Object) this.prefab_guide_y, (Object) null))
      return;
    GameObject gameObject1 = Util.KInstantiateUI(this.prefab_guide_y, this.guides_y, true);
    ((Object) gameObject1).name = "guides_vertical";
    Vector2[] vector2Array = new Vector2[2 * (int) ((double) this.axis_x.range / (double) this.axis_x.guide_frequency)];
    for (int index = 0; index < vector2Array.Length; index += 2)
    {
      Vector2 absolute_point1;
      // ISSUE: explicit constructor call
      ((Vector2) ref absolute_point1).\u002Ector((float) index * (this.axis_x.guide_frequency / 2f), this.axis_y.min_value);
      vector2Array[index] = this.GetRelativePosition(absolute_point1);
      Vector2 absolute_point2;
      // ISSUE: explicit constructor call
      ((Vector2) ref absolute_point2).\u002Ector((float) index * (this.axis_x.guide_frequency / 2f), this.axis_y.max_value);
      vector2Array[index + 1] = this.GetRelativePosition(absolute_point2);
      if (Object.op_Inequality((Object) this.prefab_guide_vertical_label, (Object) null))
      {
        GameObject gameObject2 = Util.KInstantiateUI(this.prefab_guide_vertical_label, gameObject1, true);
        ((TMP_Text) gameObject2.GetComponent<LocText>()).alignment = (TextAlignmentOptions) 1026;
        ((TMP_Text) gameObject2.GetComponent<LocText>()).text = ((int) this.axis_x.guide_frequency * (index / 2)).ToString();
        RectTransform rectTransform = Util.rectTransform(gameObject2);
        double num1 = (double) index;
        Rect rect1 = Util.rectTransform(((Component) this).gameObject).rect;
        double num2 = (double) ((Rect) ref rect1).width / (double) vector2Array.Length;
        Vector2 vector2_1 = new Vector2((float) (num1 * num2), 4f);
        Rect rect2 = Util.rectTransform(((Component) this).gameObject).rect;
        Vector2 vector2_2 = Vector2.op_Division(((Rect) ref rect2).size, 2f);
        Vector3 vector3 = Vector2.op_Implicit(Vector2.op_Subtraction(vector2_1, vector2_2));
        TransformExtensions.SetLocalPosition((Transform) rectTransform, vector3);
      }
    }
    gameObject1.GetComponent<UILineRenderer>().Points = vector2Array;
    this.verticalGuides.Add(gameObject1);
  }
}
