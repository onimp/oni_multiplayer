// Decompiled with JetBrains decompiler
// Type: CodeWriter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using System.IO;

public class CodeWriter
{
  private List<string> Lines = new List<string>();
  private string Path;
  private int Indent;

  public CodeWriter(string path) => this.Path = path;

  public void Comment(string text) => this.Lines.Add("// " + text);

  public void BeginPartialClass(string class_name, string parent_name = null)
  {
    string text = "public partial class " + class_name;
    if (parent_name != null)
      text = text + " : " + parent_name;
    this.Line(text);
    this.Line("{");
    ++this.Indent;
  }

  public void BeginClass(string class_name, string parent_name = null)
  {
    string text = "public class " + class_name;
    if (parent_name != null)
      text = text + " : " + parent_name;
    this.Line(text);
    this.Line("{");
    ++this.Indent;
  }

  public void EndClass()
  {
    --this.Indent;
    this.Line("}");
  }

  public void BeginNameSpace(string name)
  {
    this.Line("namespace " + name);
    this.Line("{");
    ++this.Indent;
  }

  public void EndNameSpace()
  {
    --this.Indent;
    this.Line("}");
  }

  public void BeginArrayStructureInitialization(string name)
  {
    this.Line("new " + name);
    this.Line("{");
    ++this.Indent;
  }

  public void EndArrayStructureInitialization(bool last_item)
  {
    --this.Indent;
    if (!last_item)
      this.Line("},");
    else
      this.Line("}");
  }

  public void BeginArraArrayInitialization(string array_type, string array_name)
  {
    this.Line(array_name + " = new " + array_type + "[]");
    this.Line("{");
    ++this.Indent;
  }

  public void EndArrayArrayInitialization(bool last_item)
  {
    --this.Indent;
    if (last_item)
      this.Line("}");
    else
      this.Line("},");
  }

  public void BeginConstructor(string name)
  {
    this.Line("public " + name + "()");
    this.Line("{");
    ++this.Indent;
  }

  public void EndConstructor()
  {
    --this.Indent;
    this.Line("}");
  }

  public void BeginArrayAssignment(string array_type, string array_name)
  {
    this.Line(array_name + " = new " + array_type + "[]");
    this.Line("{");
    ++this.Indent;
  }

  public void EndArrayAssignment()
  {
    --this.Indent;
    this.Line("};");
  }

  public void FieldAssignment(string field_name, string value) => this.Line(field_name + " = " + value + ";");

  public void BeginStructureDelegateFieldInitializer(string name)
  {
    this.Line(name + "=delegate()");
    this.Line("{");
    ++this.Indent;
  }

  public void EndStructureDelegateFieldInitializer()
  {
    --this.Indent;
    this.Line("},");
  }

  public void BeginIf(string condition)
  {
    this.Line("if(" + condition + ")");
    this.Line("{");
    ++this.Indent;
  }

  public void BeginElseIf(string condition)
  {
    --this.Indent;
    this.Line("}");
    this.Line("else if(" + condition + ")");
    this.Line("{");
    ++this.Indent;
  }

  public void EndIf()
  {
    --this.Indent;
    this.Line("}");
  }

  public void BeginFunctionDeclaration(string name, string parameter, string return_type)
  {
    this.Line("public " + return_type + " " + name + "(" + parameter + ")");
    this.Line("{");
    ++this.Indent;
  }

  public void BeginFunctionDeclaration(string name, string return_type)
  {
    this.Line("public " + return_type + " " + name + "()");
    this.Line("{");
    ++this.Indent;
  }

  public void EndFunctionDeclaration()
  {
    --this.Indent;
    this.Line("}");
  }

  private void InternalNamedParameter(string name, string value, bool last_parameter)
  {
    string str = "";
    if (!last_parameter)
      str = ",";
    this.Line(name + ":" + value + str);
  }

  public void NamedParameterBool(string name, bool value, bool last_parameter = false) => this.InternalNamedParameter(name, value.ToString().ToLower(), last_parameter);

  public void NamedParameterInt(string name, int value, bool last_parameter = false) => this.InternalNamedParameter(name, value.ToString(), last_parameter);

  public void NamedParameterFloat(string name, float value, bool last_parameter = false) => this.InternalNamedParameter(name, value.ToString() + "f", last_parameter);

  public void NamedParameterString(string name, string value, bool last_parameter = false) => this.InternalNamedParameter(name, value, last_parameter);

  public void BeginFunctionCall(string name)
  {
    this.Line(name);
    this.Line("(");
    ++this.Indent;
  }

  public void EndFunctionCall()
  {
    --this.Indent;
    this.Line(");");
  }

  public void FunctionCall(string function_name, params string[] parameters)
  {
    string str = function_name + "(";
    for (int index = 0; index < parameters.Length; ++index)
    {
      str += parameters[index];
      if (index != parameters.Length - 1)
        str += ", ";
    }
    this.Line(str + ");");
  }

  public void StructureFieldInitializer(string field, string value) => this.Line(field + " = " + value + ",");

  public void StructureArrayFieldInitializer(
    string field,
    string field_type,
    params string[] values)
  {
    string str = field + " = new " + field_type + "[]{ ";
    for (int index = 0; index < values.Length; ++index)
    {
      str += values[index];
      if (index < values.Length - 1)
        str += ", ";
    }
    this.Line(str + " },");
  }

  public void Line(string text = "")
  {
    for (int index = 0; index < this.Indent; ++index)
      text = "\t" + text;
    this.Lines.Add(text);
  }

  public void Flush() => File.WriteAllLines(this.Path, this.Lines.ToArray());
}
