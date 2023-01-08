// Decompiled with JetBrains decompiler
// Type: DiagnosticCriterion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class DiagnosticCriterion
{
  private Func<ColonyDiagnostic.DiagnosticResult> evaluateAction;

  public string id { get; private set; }

  public string name { get; private set; }

  public DiagnosticCriterion(string name, Func<ColonyDiagnostic.DiagnosticResult> action)
  {
    this.name = name;
    this.evaluateAction = action;
  }

  public void SetID(string id) => this.id = id;

  public ColonyDiagnostic.DiagnosticResult Evaluate() => this.evaluateAction();
}
