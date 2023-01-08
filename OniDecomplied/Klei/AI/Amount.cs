// Decompiled with JetBrains decompiler
// Type: Klei.AI.Amount
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
  [DebuggerDisplay("{Id}")]
  public class Amount : Resource
  {
    public string description;
    public bool showMax;
    public Units units;
    public float visualDeltaThreshold;
    public Attribute minAttribute;
    public Attribute maxAttribute;
    public Attribute deltaAttribute;
    public bool showInUI;
    public string uiSprite;
    public string thoughtSprite;
    public IAmountDisplayer displayer;

    public Amount(
      string id,
      string name,
      string description,
      Attribute min_attribute,
      Attribute max_attribute,
      Attribute delta_attribute,
      bool show_max,
      Units units,
      float visual_delta_threshold,
      bool show_in_ui,
      string uiSprite = null,
      string thoughtSprite = null)
    {
      this.Id = id;
      this.Name = name;
      this.description = description;
      this.minAttribute = min_attribute;
      this.maxAttribute = max_attribute;
      this.deltaAttribute = delta_attribute;
      this.showMax = show_max;
      this.units = units;
      this.visualDeltaThreshold = visual_delta_threshold;
      this.showInUI = show_in_ui;
      this.uiSprite = uiSprite;
      this.thoughtSprite = thoughtSprite;
    }

    public void SetDisplayer(IAmountDisplayer displayer)
    {
      this.displayer = displayer;
      this.minAttribute.SetFormatter(displayer.Formatter);
      this.maxAttribute.SetFormatter(displayer.Formatter);
      this.deltaAttribute.SetFormatter(displayer.Formatter);
    }

    public AmountInstance Lookup(Component cmp) => this.Lookup(cmp.gameObject);

    public AmountInstance Lookup(GameObject go) => go.GetAmounts()?.Get(this);

    public void Copy(GameObject to, GameObject from) => this.Lookup(to).value = this.Lookup(from).value;

    public string GetValueString(AmountInstance instance) => this.displayer.GetValueString(this, instance);

    public string GetDescription(AmountInstance instance) => this.displayer.GetDescription(this, instance);

    public string GetTooltip(AmountInstance instance) => this.displayer.GetTooltip(this, instance);
  }
}
