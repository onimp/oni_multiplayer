// Decompiled with JetBrains decompiler
// Type: DevToolMenuFontSize
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;

public class DevToolMenuFontSize
{
  public const string SETTINGS_KEY_FONT_SIZE_CATEGORY = "Imgui_font_size_category";
  private DevToolMenuFontSize.FontSizeCategory fontSizeCategory;

  public bool initialized { private set; get; }

  public void RefreshFontSize() => this.SetFontSizeCategory((DevToolMenuFontSize.FontSizeCategory) KPlayerPrefs.GetInt("Imgui_font_size_category", 2));

  public void InitializeIfNeeded()
  {
    if (this.initialized)
      return;
    this.initialized = true;
    this.RefreshFontSize();
  }

  public void DrawMenu()
  {
    if (!ImGui.BeginMenu("Settings"))
      return;
    bool flag1 = this.fontSizeCategory == DevToolMenuFontSize.FontSizeCategory.Fabric;
    bool flag2 = this.fontSizeCategory == DevToolMenuFontSize.FontSizeCategory.Small;
    bool flag3 = this.fontSizeCategory == DevToolMenuFontSize.FontSizeCategory.Regular;
    bool flag4 = this.fontSizeCategory == DevToolMenuFontSize.FontSizeCategory.Large;
    if (ImGui.BeginMenu("Size"))
    {
      if (ImGui.Checkbox("Original Font", ref flag1) && this.fontSizeCategory != DevToolMenuFontSize.FontSizeCategory.Fabric)
        this.SetFontSizeCategory(DevToolMenuFontSize.FontSizeCategory.Fabric);
      if (ImGui.Checkbox("Small Text", ref flag2) && this.fontSizeCategory != DevToolMenuFontSize.FontSizeCategory.Small)
        this.SetFontSizeCategory(DevToolMenuFontSize.FontSizeCategory.Small);
      if (ImGui.Checkbox("Regular Text", ref flag3) && this.fontSizeCategory != DevToolMenuFontSize.FontSizeCategory.Regular)
        this.SetFontSizeCategory(DevToolMenuFontSize.FontSizeCategory.Regular);
      if (ImGui.Checkbox("Large Text", ref flag4) && this.fontSizeCategory != DevToolMenuFontSize.FontSizeCategory.Large)
        this.SetFontSizeCategory(DevToolMenuFontSize.FontSizeCategory.Large);
      ImGui.EndMenu();
    }
    ImGui.EndMenu();
  }

  public unsafe void SetFontSizeCategory(DevToolMenuFontSize.FontSizeCategory size)
  {
    this.fontSizeCategory = size;
    KPlayerPrefs.SetInt("Imgui_font_size_category", (int) size);
    ImGuiIOPtr io = ImGui.GetIO();
    int num1 = (int) size;
    int num2 = num1;
    ImFontAtlasPtr fonts1 = ((ImGuiIOPtr) ref io).Fonts;
    int size1 = ((ImFontAtlasPtr) ref fonts1).Fonts.Size;
    if (num2 >= size1)
      return;
    ImFontAtlasPtr fonts2 = ((ImGuiIOPtr) ref io).Fonts;
    ImFontPtr imFontPtr = ((ImFontAtlasPtr) ref fonts2).Fonts[num1];
    ((ImGuiIOPtr) ref io).NativePtr->FontDefault = ImFontPtr.op_Implicit(imFontPtr);
  }

  public enum FontSizeCategory
  {
    Fabric,
    Small,
    Regular,
    Large,
  }
}
