// Decompiled with JetBrains decompiler
// Type: SnapOn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SnapOn")]
public class SnapOn : KMonoBehaviour
{
  private KAnimControllerBase kanimController;
  public List<SnapOn.SnapPoint> snapPoints = new List<SnapOn.SnapPoint>();
  private Dictionary<string, SnapOn.OverrideEntry> overrideMap = new Dictionary<string, SnapOn.OverrideEntry>();

  protected virtual void OnPrefabInit() => this.kanimController = ((Component) this).GetComponent<KAnimControllerBase>();

  protected virtual void OnSpawn()
  {
    foreach (SnapOn.SnapPoint snapPoint in this.snapPoints)
    {
      if (snapPoint.automatic)
        this.DoAttachSnapOn(snapPoint);
    }
  }

  public void AttachSnapOnByName(string name)
  {
    foreach (SnapOn.SnapPoint snapPoint in this.snapPoints)
    {
      if (snapPoint.pointName == name)
      {
        HashedString context = ((Component) this).GetComponent<AnimEventHandler>().GetContext();
        if (!((HashedString) ref context).IsValid || !((HashedString) ref snapPoint.context).IsValid || HashedString.op_Equality(context, snapPoint.context))
          this.DoAttachSnapOn(snapPoint);
      }
    }
  }

  public void DetachSnapOnByName(string name)
  {
    foreach (SnapOn.SnapPoint snapPoint in this.snapPoints)
    {
      if (snapPoint.pointName == name)
      {
        HashedString context = ((Component) this).GetComponent<AnimEventHandler>().GetContext();
        if (!((HashedString) ref context).IsValid || !((HashedString) ref snapPoint.context).IsValid || HashedString.op_Equality(context, snapPoint.context))
        {
          ((Component) this).GetComponent<SymbolOverrideController>().RemoveSymbolOverride(snapPoint.overrideSymbol, 5);
          this.kanimController.SetSymbolVisiblity(KAnimHashedString.op_Implicit(snapPoint.overrideSymbol), false);
          break;
        }
      }
    }
  }

  private void DoAttachSnapOn(SnapOn.SnapPoint point)
  {
    SnapOn.OverrideEntry overrideEntry = (SnapOn.OverrideEntry) null;
    KAnimFile buildFile = point.buildFile;
    string symbol_name = "";
    if (this.overrideMap.TryGetValue(point.pointName, out overrideEntry))
    {
      buildFile = overrideEntry.buildFile;
      symbol_name = overrideEntry.symbolName;
    }
    KAnim.Build.Symbol symbol = SnapOn.GetSymbol(buildFile, symbol_name);
    ((Component) this).GetComponent<SymbolOverrideController>().AddSymbolOverride(point.overrideSymbol, symbol, 5);
    this.kanimController.SetSymbolVisiblity(KAnimHashedString.op_Implicit(point.overrideSymbol), true);
  }

  private static KAnim.Build.Symbol GetSymbol(KAnimFile anim_file, string symbol_name)
  {
    KAnim.Build.Symbol symbol1 = anim_file.GetData().build.symbols[0];
    KAnimHashedString kanimHashedString;
    // ISSUE: explicit constructor call
    ((KAnimHashedString) ref kanimHashedString).\u002Ector(symbol_name);
    foreach (KAnim.Build.Symbol symbol2 in anim_file.GetData().build.symbols)
    {
      if (KAnimHashedString.op_Equality(symbol2.hash, kanimHashedString))
      {
        symbol1 = symbol2;
        break;
      }
    }
    return symbol1;
  }

  public void AddOverride(string point_name, KAnimFile build_override, string symbol_name) => this.overrideMap[point_name] = new SnapOn.OverrideEntry()
  {
    buildFile = build_override,
    symbolName = symbol_name
  };

  public void RemoveOverride(string point_name) => this.overrideMap.Remove(point_name);

  [Serializable]
  public class SnapPoint
  {
    public string pointName;
    public bool automatic = true;
    public HashedString context;
    public KAnimFile buildFile;
    public HashedString overrideSymbol;
  }

  public class OverrideEntry
  {
    public KAnimFile buildFile;
    public string symbolName;
  }
}
