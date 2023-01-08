// Decompiled with JetBrains decompiler
// Type: SymbolOverrideControllerUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class SymbolOverrideControllerUtil
{
  public static SymbolOverrideController AddToPrefab(GameObject prefab)
  {
    SymbolOverrideController prefab1 = prefab.AddComponent<SymbolOverrideController>();
    KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
    DebugUtil.Assert(Object.op_Inequality((Object) component, (Object) null), "SymbolOverrideController must be added after a KBatchedAnimController component.");
    component.usingNewSymbolOverrideSystem = true;
    return prefab1;
  }

  public static void AddBuildOverride(
    this SymbolOverrideController symbol_override_controller,
    KAnimFileData anim_file_data,
    int priority = 0)
  {
    for (int index = 0; index < anim_file_data.build.symbols.Length; ++index)
    {
      KAnim.Build.Symbol symbol = anim_file_data.build.symbols[index];
      symbol_override_controller.AddSymbolOverride(new HashedString(((KAnimHashedString) ref symbol.hash).HashValue), symbol, priority);
    }
  }

  public static void RemoveBuildOverride(
    this SymbolOverrideController symbol_override_controller,
    KAnimFileData anim_file_data,
    int priority = 0)
  {
    for (int index = 0; index < anim_file_data.build.symbols.Length; ++index)
    {
      KAnim.Build.Symbol symbol = anim_file_data.build.symbols[index];
      symbol_override_controller.RemoveSymbolOverride(new HashedString(((KAnimHashedString) ref symbol.hash).HashValue), priority);
    }
  }

  public static void TryRemoveBuildOverride(
    this SymbolOverrideController symbol_override_controller,
    KAnimFileData anim_file_data,
    int priority = 0)
  {
    for (int index = 0; index < anim_file_data.build.symbols.Length; ++index)
    {
      KAnim.Build.Symbol symbol = anim_file_data.build.symbols[index];
      symbol_override_controller.TryRemoveSymbolOverride(new HashedString(((KAnimHashedString) ref symbol.hash).HashValue), priority);
    }
  }

  public static bool TryRemoveSymbolOverride(
    this SymbolOverrideController symbol_override_controller,
    HashedString target_symbol,
    int priority = 0)
  {
    return symbol_override_controller.GetSymbolOverrideIdx(target_symbol, priority) >= 0 && symbol_override_controller.RemoveSymbolOverride(target_symbol, priority);
  }

  public static void ApplySymbolOverridesByAffix(
    this SymbolOverrideController symbol_override_controller,
    KAnimFile anim_file,
    string prefix = null,
    string postfix = null,
    int priority = 0)
  {
    for (int index = 0; index < anim_file.GetData().build.symbols.Length; ++index)
    {
      KAnim.Build.Symbol symbol = anim_file.GetData().build.symbols[index];
      string str1 = HashCache.Get().Get(symbol.hash);
      if (prefix != null && postfix != null)
      {
        if (str1.StartsWith(prefix) && str1.EndsWith(postfix))
        {
          string str2 = str1.Substring(prefix.Length, str1.Length - prefix.Length);
          string str3 = str2.Substring(0, str2.Length - postfix.Length);
          symbol_override_controller.AddSymbolOverride(HashedString.op_Implicit(str3), symbol, priority);
        }
      }
      else if (prefix != null && str1.StartsWith(prefix))
        symbol_override_controller.AddSymbolOverride(HashedString.op_Implicit(str1.Substring(prefix.Length, str1.Length - prefix.Length)), symbol, priority);
      else if (postfix != null && str1.EndsWith(postfix))
        symbol_override_controller.AddSymbolOverride(HashedString.op_Implicit(str1.Substring(0, str1.Length - postfix.Length)), symbol, priority);
    }
  }
}
