// Decompiled with JetBrains decompiler
// Type: Global
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KMod;
using KSerialization;
using Newtonsoft.Json;
using ProcGenGame;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.U2D;

public class Global : MonoBehaviour
{
  public SpriteAtlas[] forcedAtlasInitializationList;
  public GameObject modErrorsPrefab;
  public GameObject globalCanvas;
  private static GameInputManager mInputManager;
  private DevToolManager DevTools = new DevToolManager();
  public Manager modManager;
  private bool gotKleiUserID;
  public Thread mainThread;
  private static string saveFolderTestResult = "unknown";
  private bool updated_with_initialized_distribution_platform;
  public static readonly string LanguageModKey = "LanguageMod";
  public static readonly string LanguageCodeKey = "LanguageCode";

  public static Global Instance { get; private set; }

  public static BindingEntry[] GenerateDefaultBindings(bool hotKeyBuildMenuPermitted = true)
  {
    List<BindingEntry> bindingEntryList = new List<BindingEntry>();
    bindingEntryList.Add(new BindingEntry((string) null, (GamepadButton) 7, (KKeyCode) 27, (Modifier) 0, (Action) 1, false, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 119, (Modifier) 0, (Action) 134, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 115, (Modifier) 0, (Action) 135, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 97, (Modifier) 0, (Action) 136, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 100, (Modifier) 0, (Action) 137, true, false));
    bindingEntryList.Add(new BindingEntry("Tool", (GamepadButton) 16, (KKeyCode) 111, (Modifier) 0, (Action) 217, true, false));
    bindingEntryList.Add(new BindingEntry("Management", (GamepadButton) 16, (KKeyCode) 108, (Modifier) 0, (Action) 108, true, false));
    bindingEntryList.Add(new BindingEntry("Management", (GamepadButton) 16, (KKeyCode) 102, (Modifier) 0, (Action) 109, true, false));
    bindingEntryList.Add(new BindingEntry("Management", (GamepadButton) 16, (KKeyCode) 118, (Modifier) 0, (Action) 110, true, false));
    bindingEntryList.Add(new BindingEntry("Management", (GamepadButton) 16, (KKeyCode) 114, (Modifier) 0, (Action) 112, true, false));
    bindingEntryList.Add(new BindingEntry("Management", (GamepadButton) 16, (KKeyCode) 101, (Modifier) 0, (Action) 114, true, false));
    bindingEntryList.Add(new BindingEntry("Management", (GamepadButton) 16, (KKeyCode) 117, (Modifier) 0, (Action) 115, true, false));
    bindingEntryList.Add(new BindingEntry("Management", (GamepadButton) 16, (KKeyCode) 106, (Modifier) 0, (Action) 116, true, false));
    bindingEntryList.Add(new BindingEntry("Management", (GamepadButton) 16, (KKeyCode) 46, (Modifier) 0, (Action) 113, true, false));
    bindingEntryList.Add(new BindingEntry("Management", (GamepadButton) 16, (KKeyCode) 122, (Modifier) 0, (Action) 117, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 103, (Modifier) 0, (Action) 144, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 109, (Modifier) 0, (Action) 150, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 107, (Modifier) 0, (Action) 151, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 105, (Modifier) 0, (Action) 152, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 116, (Modifier) 0, (Action) 145, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 110, (Modifier) 0, (Action) 146, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 121, (Modifier) 0, (Action) 147, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 277, (Modifier) 0, (Action) 148, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 112, (Modifier) 0, (Action) 154, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 115, (Modifier) 1, (Action) 168, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 99, (Modifier) 0, (Action) 143, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 120, (Modifier) 0, (Action) 142, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 9, (Modifier) 0, (Action) 12, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 104, (Modifier) 0, (Action) 138, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 0, (KKeyCode) 323, (Modifier) 0, (Action) 3, false, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 0, (KKeyCode) 323, (Modifier) 4, (Action) 4, false, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 1, (KKeyCode) 324, (Modifier) 0, (Action) 5, false, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 325, (Modifier) 0, (Action) 6, false, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 49, (Modifier) 0, (Action) 36, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 50, (Modifier) 0, (Action) 37, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 51, (Modifier) 0, (Action) 38, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 52, (Modifier) 0, (Action) 39, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 53, (Modifier) 0, (Action) 40, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 54, (Modifier) 0, (Action) 41, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 55, (Modifier) 0, (Action) 42, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 56, (Modifier) 0, (Action) 43, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 57, (Modifier) 0, (Action) 44, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 48, (Modifier) 0, (Action) 45, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 45, (Modifier) 0, (Action) 46, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 61, (Modifier) 0, (Action) 47, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 45, (Modifier) 4, (Action) 48, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 61, (Modifier) 4, (Action) 49, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 98, (Modifier) 0, (Action) 50, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 11, (KKeyCode) 1002, (Modifier) 0, (Action) 7, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 10, (KKeyCode) 1001, (Modifier) 0, (Action) 8, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 282, (Modifier) 0, (Action) 119, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 283, (Modifier) 0, (Action) 120, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 284, (Modifier) 0, (Action) 121, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 285, (Modifier) 0, (Action) 122, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 286, (Modifier) 0, (Action) 123, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 287, (Modifier) 0, (Action) 124, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 288, (Modifier) 0, (Action) 125, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 289, (Modifier) 0, (Action) 126, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 290, (Modifier) 0, (Action) (int) sbyte.MaxValue, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 291, (Modifier) 0, (Action) 128, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 292, (Modifier) 0, (Action) 129, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 282, (Modifier) 4, (Action) 130, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 283, (Modifier) 4, (Action) 131, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 284, (Modifier) 4, (Action) 132, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 285, (Modifier) 4, (Action) 133, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 270, (Modifier) 0, (Action) 9, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 269, (Modifier) 0, (Action) 10, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 6, (KKeyCode) 32, (Modifier) 0, (Action) 11, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 49, (Modifier) 2, (Action) 15, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 50, (Modifier) 2, (Action) 16, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 51, (Modifier) 2, (Action) 17, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 52, (Modifier) 2, (Action) 18, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 53, (Modifier) 2, (Action) 19, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 54, (Modifier) 2, (Action) 20, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 55, (Modifier) 2, (Action) 21, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 56, (Modifier) 2, (Action) 22, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 57, (Modifier) 2, (Action) 23, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 48, (Modifier) 2, (Action) 24, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 49, (Modifier) 4, (Action) 25, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 50, (Modifier) 4, (Action) 26, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 51, (Modifier) 4, (Action) 27, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 52, (Modifier) 4, (Action) 28, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 53, (Modifier) 4, (Action) 29, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 54, (Modifier) 4, (Action) 30, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 55, (Modifier) 4, (Action) 31, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 56, (Modifier) 4, (Action) 32, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 57, (Modifier) 4, (Action) 33, true, false));
    bindingEntryList.Add(new BindingEntry("Navigation", (GamepadButton) 16, (KKeyCode) 48, (Modifier) 4, (Action) 34, true, false));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 99, (Modifier) 0, (Action) 244, true, true));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 97, (Modifier) 0, (Action) 245, true, true));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 100, (Modifier) 0, (Action) 246, true, true));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 119, (Modifier) 0, (Action) 247, true, true));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 115, (Modifier) 0, (Action) 248, true, true));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 105, (Modifier) 0, (Action) 249, true, true));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 111, (Modifier) 0, (Action) 250, true, true));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 122, (Modifier) 0, (Action) 254, true, true));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 122, (Modifier) 4, (Action) 253, true, true));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 112, (Modifier) 0, (Action) 257, true, true));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 116, (Modifier) 0, (Action) (int) byte.MaxValue, true, true));
    bindingEntryList.Add(new BindingEntry("CinematicCamera", (GamepadButton) 16, (KKeyCode) 101, (Modifier) 0, (Action) 256, true, true));
    bindingEntryList.Add(new BindingEntry("Building", (GamepadButton) 16, (KKeyCode) 47, (Modifier) 0, (Action) 167, true, false));
    bindingEntryList.Add(new BindingEntry("Building", (GamepadButton) 16, (KKeyCode) 13, (Modifier) 0, (Action) 166, true, false));
    bindingEntryList.Add(new BindingEntry("Building", (GamepadButton) 16, (KKeyCode) 92, (Modifier) 0, (Action) 139, true, false));
    bindingEntryList.Add(new BindingEntry("Building", (GamepadButton) 16, (KKeyCode) 91, (Modifier) 0, (Action) 140, true, false));
    bindingEntryList.Add(new BindingEntry("Building", (GamepadButton) 16, (KKeyCode) 93, (Modifier) 0, (Action) 141, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 308, (Modifier) 1, (Action) 13, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 307, (Modifier) 1, (Action) 13, true, false));
    bindingEntryList.Add(new BindingEntry("Tool", (GamepadButton) 16, (KKeyCode) 304, (Modifier) 4, (Action) 14, true, false));
    bindingEntryList.Add(new BindingEntry("Tool", (GamepadButton) 16, (KKeyCode) 303, (Modifier) 4, (Action) 14, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 116, (Modifier) 2, (Action) 219, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 117, (Modifier) 2, (Action) 177, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 282, (Modifier) 1, (Action) 188, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 284, (Modifier) 1, (Action) 189, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 288, (Modifier) 1, (Action) 190, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 291, (Modifier) 1, (Action) 195, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 291, (Modifier) 4, (Action) 199, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 293, (Modifier) 4, (Action) 201, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 110, (Modifier) 1, (Action) 191, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 113, (Modifier) 2, (Action) 202, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 115, (Modifier) 2, (Action) 204, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 109, (Modifier) 2, (Action) 205, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 102, (Modifier) 2, (Action) 192, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 8, (Modifier) 0, (Action) 173, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 8, (Modifier) 2, (Action) 180, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 113, (Modifier) 1, (Action) 203, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 283, (Modifier) 1, (Action) 268, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 283, (Modifier) 2, (Action) 174, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 284, (Modifier) 2, (Action) 196, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 285, (Modifier) 2, (Action) 179, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 286, (Modifier) 2, (Action) 178, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 287, (Modifier) 2, (Action) 187, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 289, (Modifier) 2, (Action) 181, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 290, (Modifier) 2, (Action) 182, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 116, (Modifier) 1, (Action) 206, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 112, (Modifier) 1, (Action) 207, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 99, (Modifier) 2, (Action) 186, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 122, (Modifier) 1, (Action) 208, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 61, (Modifier) 1, (Action) 209, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 45, (Modifier) 1, (Action) 210, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 120, (Modifier) 1, (Action) 211, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 99, (Modifier) 1, (Action) 212, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 96, (Modifier) 0, (Action) 215, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 96, (Modifier) 1, (Action) 216, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 282, (Modifier) 2, (Action) 224, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 293, (Modifier) 2, (Action) 183, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 293, (Modifier) 6, (Action) 184, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 291, (Modifier) 2, (Action) 221, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 291, (Modifier) 3, (Action) 222, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 292, (Modifier) 2, (Action) 223, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 288, (Modifier) 3, (Action) 225, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 57, (Modifier) 1, (Action) 226, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 49, (Modifier) 1, (Action) 169, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 50, (Modifier) 1, (Action) 170, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 51, (Modifier) 1, (Action) 171, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 52, (Modifier) 1, (Action) 172, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 53, (Modifier) 1, (Action) 227, true, false));
    bindingEntryList.Add(new BindingEntry("Debug", (GamepadButton) 16, (KKeyCode) 48, (Modifier) 1, (Action) 185, true, false));
    bindingEntryList.Add(new BindingEntry("Root", (GamepadButton) 16, (KKeyCode) 13, (Modifier) 0, (Action) 228, false, false));
    bindingEntryList.Add(new BindingEntry("Analog", (GamepadButton) 16, (KKeyCode) 0, (Modifier) 0, (Action) 273, false, false));
    bindingEntryList.Add(new BindingEntry("Analog", (GamepadButton) 16, (KKeyCode) 0, (Modifier) 0, (Action) 274, false, false));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 97, (Modifier) 0, (Action) 82, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 98, (Modifier) 0, (Action) 83, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 99, (Modifier) 0, (Action) 84, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 100, (Modifier) 0, (Action) 85, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 101, (Modifier) 0, (Action) 86, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 102, (Modifier) 0, (Action) 87, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 103, (Modifier) 0, (Action) 88, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 104, (Modifier) 0, (Action) 89, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 105, (Modifier) 0, (Action) 90, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 106, (Modifier) 0, (Action) 91, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 107, (Modifier) 0, (Action) 92, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 108, (Modifier) 0, (Action) 93, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 109, (Modifier) 0, (Action) 94, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 110, (Modifier) 0, (Action) 95, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 111, (Modifier) 0, (Action) 96, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 112, (Modifier) 0, (Action) 97, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 113, (Modifier) 0, (Action) 98, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 114, (Modifier) 0, (Action) 99, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 115, (Modifier) 0, (Action) 100, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 116, (Modifier) 0, (Action) 101, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 117, (Modifier) 0, (Action) 102, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 118, (Modifier) 0, (Action) 103, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 119, (Modifier) 0, (Action) 104, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 120, (Modifier) 0, (Action) 105, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 121, (Modifier) 0, (Action) 106, false, true));
    bindingEntryList.Add(new BindingEntry("BuildingsMenu", (GamepadButton) 16, (KKeyCode) 122, (Modifier) 0, (Action) 107, false, true));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 98, (Modifier) 4, (Action) 229, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 110, (Modifier) 4, (Action) 230, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 102, (Modifier) 4, (Action) 231, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 107, (Modifier) 4, (Action) 233, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 104, (Modifier) 4, (Action) 234, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 106, (Modifier) 4, (Action) 243, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 99, (Modifier) 4, (Action) 235, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 120, (Modifier) 4, (Action) 236, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 101, (Modifier) 4, (Action) 237, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 115, (Modifier) 4, (Action) 238, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 114, (Modifier) 4, (Action) 239, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 122, (Modifier) 4, (Action) 241, true, false));
    bindingEntryList.Add(new BindingEntry("Sandbox", (GamepadButton) 16, (KKeyCode) 323, (Modifier) 2, (Action) 242, true, false));
    bindingEntryList.Add(new BindingEntry("SwitchActiveWorld", (GamepadButton) 16, (KKeyCode) 49, (Modifier) 16, (Action) 258, true, false, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    bindingEntryList.Add(new BindingEntry("SwitchActiveWorld", (GamepadButton) 16, (KKeyCode) 50, (Modifier) 16, (Action) 259, true, false, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    bindingEntryList.Add(new BindingEntry("SwitchActiveWorld", (GamepadButton) 16, (KKeyCode) 51, (Modifier) 16, (Action) 260, true, false, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    bindingEntryList.Add(new BindingEntry("SwitchActiveWorld", (GamepadButton) 16, (KKeyCode) 52, (Modifier) 16, (Action) 261, true, false, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    bindingEntryList.Add(new BindingEntry("SwitchActiveWorld", (GamepadButton) 16, (KKeyCode) 53, (Modifier) 16, (Action) 262, true, false, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    bindingEntryList.Add(new BindingEntry("SwitchActiveWorld", (GamepadButton) 16, (KKeyCode) 54, (Modifier) 16, (Action) 263, true, false, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    bindingEntryList.Add(new BindingEntry("SwitchActiveWorld", (GamepadButton) 16, (KKeyCode) 55, (Modifier) 16, (Action) 264, true, false, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    bindingEntryList.Add(new BindingEntry("SwitchActiveWorld", (GamepadButton) 16, (KKeyCode) 56, (Modifier) 16, (Action) 265, true, false, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    bindingEntryList.Add(new BindingEntry("SwitchActiveWorld", (GamepadButton) 16, (KKeyCode) 57, (Modifier) 16, (Action) 266, true, false, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    bindingEntryList.Add(new BindingEntry("SwitchActiveWorld", (GamepadButton) 16, (KKeyCode) 48, (Modifier) 16, (Action) 267, true, false, DlcManager.AVAILABLE_EXPANSION1_ONLY));
    List<BindingEntry> bindings = bindingEntryList;
    IList<BuildMenu.DisplayInfo> data = (IList<BuildMenu.DisplayInfo>) BuildMenu.OrderedBuildings.data;
    if (BuildMenu.UseHotkeyBuildMenu() & hotKeyBuildMenuPermitted)
    {
      foreach (BuildMenu.DisplayInfo display_info in (IEnumerable<BuildMenu.DisplayInfo>) data)
        Global.AddBindings(HashedString.Invalid, display_info, bindings);
    }
    return bindings.ToArray();
  }

  private static void AddBindings(
    HashedString parent_category,
    BuildMenu.DisplayInfo display_info,
    List<BindingEntry> bindings)
  {
    if (display_info.data == null)
      return;
    System.Type type = display_info.data.GetType();
    if (typeof (IList<BuildMenu.DisplayInfo>).IsAssignableFrom(type))
    {
      foreach (BuildMenu.DisplayInfo display_info1 in (IEnumerable<BuildMenu.DisplayInfo>) display_info.data)
        Global.AddBindings(display_info.category, display_info1, bindings);
    }
    else
    {
      if (!typeof (IList<BuildMenu.BuildingInfo>).IsAssignableFrom(type))
        return;
      string str = new CultureInfo("en-US", false).TextInfo.ToTitleCase(HashCache.Get().Get(parent_category)) + " Menu";
      BindingEntry bindingEntry;
      // ISSUE: explicit constructor call
      ((BindingEntry) ref bindingEntry).\u002Ector(str, (GamepadButton) 16, display_info.keyCode, (Modifier) 0, display_info.hotkey, true, true);
      bindings.Add(bindingEntry);
    }
  }

  private void Awake()
  {
    KCrashReporter crash_reporter = ((Component) this).GetComponent<KCrashReporter>();
    if (Object.op_Inequality((Object) crash_reporter, (Object) null) & SceneInitializerLoader.ReportDeferredError == null)
      SceneInitializerLoader.ReportDeferredError = (SceneInitializerLoader.DeferredErrorDelegate) (deferred_error => crash_reporter.ShowDialog(deferred_error.msg, deferred_error.stack_trace));
    this.globalCanvas = GameObject.Find("Canvas");
    Object.DontDestroyOnLoad((Object) this.globalCanvas.gameObject);
    this.OutputSystemInfo();
    Debug.Assert(Object.op_Equality((Object) Global.Instance, (Object) null));
    Global.Instance = this;
    Debug.Log((object) ("Initializing at " + System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")));
    Debug.Log((object) ("Save path: " + Util.RootFolder()));
    MyCmp.Init();
    MySmi.Init();
    DevToolManager.Instance.Init();
    if (this.forcedAtlasInitializationList != null)
    {
      foreach (SpriteAtlas atlasInitialization in this.forcedAtlasInitializationList)
      {
        Sprite[] spriteArray = new Sprite[atlasInitialization.spriteCount];
        atlasInitialization.GetSprites(spriteArray);
        foreach (Sprite sprite in spriteArray)
        {
          Texture2D texture = sprite.texture;
          if (Object.op_Inequality((Object) texture, (Object) null))
          {
            ((Texture) texture).filterMode = (FilterMode) 1;
            ((Texture) texture).anisoLevel = 4;
            ((Texture) texture).mipMapBias = 0.0f;
          }
        }
      }
    }
    FileSystem.Initialize();
    Singleton<StateMachineUpdater>.CreateInstance();
    Singleton<StateMachineManager>.CreateInstance();
    Localization.RegisterForTranslation(typeof (UI));
    this.modManager = new Manager();
    this.modManager.LoadModDBAndInitialize();
    this.modManager.Load(Content.DLL);
    this.modManager.Load(Content.Strings);
    Manager.Initialize();
    Global.InitializeGlobalInput();
    Global.InitializeGlobalSound();
    Global.InitializeGlobalAnimation();
    Localization.Initialize();
    this.modManager.Load(Content.Translation);
    this.modManager.distribution_platforms.Add((IDistributionPlatform) new Local("Local", KMod.Label.DistributionPlatform.Local, false));
    this.modManager.distribution_platforms.Add((IDistributionPlatform) new Local("Dev", KMod.Label.DistributionPlatform.Dev, true));
    this.mainThread = Thread.CurrentThread;
    KCrashReporter.onCrashReported += new Action<string>(this.OnCrashReported);
    KProfiler.main_thread = Thread.CurrentThread;
    this.RestoreLegacyMetricsSetting();
    this.TestDataLocations();
    DistributionPlatform.onExitRequest += new System.Action(this.OnExitRequest);
    DistributionPlatform.onDlcAuthenticationFailed += new System.Action(this.OnDlcAuthenticationFailed);
    if (DistributionPlatform.Initialized)
    {
      if (!KPrivacyPrefs.instance.disableDataCollection)
      {
        Debug.Log((object) ("Logged into " + DistributionPlatform.Inst.Name + " with ID:" + ((object) DistributionPlatform.Inst.LocalUser.Id)?.ToString() + ", NAME:" + DistributionPlatform.Inst.LocalUser.Name));
        // ISSUE: method pointer
        ThreadedHttps<KleiAccount>.Instance.AuthenticateUser(new KleiAccount.GetUserIDdelegate((object) this, __methodptr(OnGetUserIdKey)), false);
      }
    }
    else
    {
      Debug.LogWarning((object) ("Can't init " + DistributionPlatform.Inst.Name + " distribution platform..."));
      this.OnGetUserIdKey();
    }
    ThreadedHttps<KleiItems>.Instance.LoadInventoryCache();
    this.modManager.Load(Content.LayerableFiles);
    WorldGen.LoadSettings(true);
    GlobalResources.Instance();
  }

  private static void InitializeGlobalInput()
  {
    if (Game.IsQuitting())
      return;
    Global.mInputManager = new GameInputManager(Global.GenerateDefaultBindings());
  }

  private static void InitializeGlobalSound()
  {
    Audio.Get();
    Singleton<SoundEventVolumeCache>.CreateInstance();
  }

  private static void InitializeGlobalAnimation()
  {
    KAnimBatchManager.CreateInstance();
    Singleton<AnimEventManager>.CreateInstance();
    Singleton<KBatchedAnimUpdater>.CreateInstance();
  }

  private void OnExitRequest()
  {
    bool flag = true;
    if (Object.op_Inequality((Object) Game.Instance, (Object) null))
    {
      string filename = SaveLoader.GetActiveSaveFilePath();
      if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
      {
        flag = false;
        KScreen component = KScreenManager.AddChild(this.globalCanvas, ((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject).GetComponent<KScreen>();
        component.Activate();
        ((Component) component).GetComponent<ConfirmDialogScreen>().PopupConfirmDialog(string.Format((string) UI.FRONTEND.RAILFORCEQUIT.SAVE_EXIT, (object) System.IO.Path.GetFileNameWithoutExtension(filename)), (System.Action) (() =>
        {
          SaveLoader.Instance.Save(filename);
          App.Quit();
        }), (System.Action) (() => App.Quit()));
      }
    }
    if (!flag)
      return;
    KScreen component1 = KScreenManager.AddChild(this.globalCanvas, ((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject).GetComponent<KScreen>();
    component1.Activate();
    ((Component) component1).GetComponent<ConfirmDialogScreen>().PopupConfirmDialog((string) UI.FRONTEND.RAILFORCEQUIT.WARN_EXIT, (System.Action) (() => App.Quit()), (System.Action) null);
  }

  private void OnDlcAuthenticationFailed()
  {
    KScreen component1 = KScreenManager.AddChild(this.globalCanvas, ((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject).GetComponent<KScreen>();
    component1.Activate();
    ConfirmDialogScreen component2 = ((Component) component1).GetComponent<ConfirmDialogScreen>();
    component2.deactivateOnCancelAction = false;
    component2.PopupConfirmDialog((string) UI.FRONTEND.RAILFORCEQUIT.DLC_NOT_PURCHASED, (System.Action) (() => App.Quit()), (System.Action) null);
  }

  private void RestoreLegacyMetricsSetting()
  {
    if (KPlayerPrefs.GetInt("ENABLE_METRICS", 1) != 0)
      return;
    KPlayerPrefs.DeleteKey("ENABLE_METRICS");
    KPlayerPrefs.Save();
    KPrivacyPrefs.instance.disableDataCollection = true;
    KPrivacyPrefs.Save();
  }

  private void TestDataLocations()
  {
    if (Application.platform != 2 && Application.platform != 7)
      return;
    try
    {
      string str1 = Util.RootFolder();
      string str2 = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Klei"), Util.GetTitleFolderName());
      Debug.Log((object) ("Test Data Location / docs / " + str1));
      Debug.Log((object) ("Test Data Location / local / " + str2));
      if (!System.IO.Directory.Exists(str2))
        System.IO.Directory.CreateDirectory(str2);
      if (!System.IO.Directory.Exists(str1))
        System.IO.Directory.CreateDirectory(str1);
      string[] strArray = new string[2]
      {
        System.IO.Path.Combine(str1, "test"),
        System.IO.Path.Combine(str2, "test")
      };
      bool[] flagArray1 = new bool[2];
      bool[] flagArray2 = new bool[2];
      bool[] flagArray3 = new bool[2];
      for (int index = 0; index < strArray.Length; ++index)
      {
        try
        {
          using (FileStream fileStream = File.Open(strArray[index], FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
          {
            byte[] bytes = Encoding.UTF8.GetBytes("test");
            fileStream.Write(bytes, 0, bytes.Length);
            flagArray1[index] = true;
          }
        }
        catch (Exception ex)
        {
          flagArray1[index] = false;
          KCrashReporter.Assert(false, "Test Data Locations / failed to write " + strArray[index] + ": " + ex.Message);
        }
        try
        {
          using (FileStream fileStream = File.Open(strArray[index], FileMode.Open, FileAccess.Read))
          {
            Encoding utF8 = Encoding.UTF8;
            byte[] numArray = new byte[fileStream.Length];
            if ((long) fileStream.Read(numArray, 0, numArray.Length) == fileStream.Length)
            {
              string str3 = utF8.GetString(numArray);
              if (str3 == "test")
              {
                flagArray2[index] = true;
              }
              else
              {
                flagArray2[index] = false;
                KCrashReporter.Assert(false, "Test Data Locations / failed to validate contents " + strArray[index] + ", got: `" + str3 + "`");
              }
            }
          }
        }
        catch (Exception ex)
        {
          flagArray2[index] = false;
          KCrashReporter.Assert(false, "Test Data Locations / failed to read " + strArray[index] + ": " + ex.Message);
        }
        try
        {
          File.Delete(strArray[index]);
          flagArray3[index] = true;
        }
        catch (Exception ex)
        {
          flagArray3[index] = false;
          KCrashReporter.Assert(false, "Test Data Locations / failed to remove " + strArray[index] + ": " + ex.Message);
        }
      }
      for (int index = 0; index < strArray.Length; ++index)
        Debug.Log((object) ("Test Data Locations / " + strArray[index] + " / write " + flagArray1[index].ToString() + " / read " + flagArray2[index].ToString() + " / removed " + flagArray3[index].ToString()));
      bool flag1 = flagArray1[0] && flagArray2[0];
      bool flag2 = flagArray1[1] && flagArray2[1];
      if (flag1 & flag2)
        Global.saveFolderTestResult = "both";
      else if (flag1 && !flag2)
        Global.saveFolderTestResult = "docs_only";
      else if (!flag1 & flag2)
        Global.saveFolderTestResult = "local_only";
      else
        Global.saveFolderTestResult = "neither";
    }
    catch (Exception ex)
    {
      KCrashReporter.Assert(false, "Test Data Locations / failed: " + ex.Message);
    }
  }

  public static GameInputManager GetInputManager()
  {
    if (Global.mInputManager == null)
      Global.InitializeGlobalInput();
    return Global.mInputManager;
  }

  private void OnApplicationFocus(bool focus)
  {
    if (Global.mInputManager == null)
      return;
    ((KInputManager) Global.mInputManager).OnApplicationFocus(focus);
  }

  private void OnGetUserIdKey() => this.gotKleiUserID = true;

  private void Update()
  {
    ImGuiRenderer instance = ImGuiRenderer.GetInstance();
    if (Object.op_Implicit((Object) instance))
    {
      this.DevTools.UpdateShouldShowTools();
      ((Component) ((Component) instance).gameObject.transform.parent).gameObject.SetActive(this.DevTools.Show);
      if (this.DevTools.Show)
        instance.NewFrame();
      this.DevTools.UpdateTools();
    }
    ((KInputManager) Global.mInputManager).Update();
    if (Singleton<AnimEventManager>.Instance != null)
      Singleton<AnimEventManager>.Instance.Update();
    if (DistributionPlatform.Initialized && !this.updated_with_initialized_distribution_platform)
    {
      this.updated_with_initialized_distribution_platform = true;
      SteamUGCService.Initialize();
      Steam steam = new Steam();
      SteamUGCService.Instance.AddClient((SteamUGCService.IClient) steam);
      this.modManager.distribution_platforms.Add((IDistributionPlatform) steam);
      SteamAchievementService.Initialize();
    }
    if (this.gotKleiUserID)
    {
      this.gotKleiUserID = false;
      ThreadedHttps<KleiMetrics>.Instance.SetCallBacks(new System.Action(this.SetONIStaticSessionVariables), new Action<Dictionary<string, object>>(this.SetONIDynamicSessionVariables));
      ThreadedHttps<KleiMetrics>.Instance.StartSession();
      KleiItems.AddRequestInventoryRefresh();
    }
    ThreadedHttps<KleiMetrics>.Instance.SetLastUserAction(KInputManager.lastUserActionTicks);
    Localization.VerifyTranslationModSubscription(this.globalCanvas);
    if (!DistributionPlatform.Initialized)
      return;
    ThreadedHttps<KleiItems>.Instance.Update();
  }

  private void SetONIStaticSessionVariables()
  {
    ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Branch", (object) "release");
    ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("Build", (object) 535842U);
    ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable("SaveFolderWriteTest", (object) Global.saveFolderTestResult);
    if (KPlayerPrefs.HasKey(UnitConfigurationScreen.MassUnitKey))
      ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.MassUnitKey, (object) ((GameUtil.MassUnit) KPlayerPrefs.GetInt(UnitConfigurationScreen.MassUnitKey)).ToString());
    if (KPlayerPrefs.HasKey(UnitConfigurationScreen.TemperatureUnitKey))
      ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(UnitConfigurationScreen.TemperatureUnitKey, (object) ((GameUtil.TemperatureUnit) KPlayerPrefs.GetInt(UnitConfigurationScreen.TemperatureUnitKey)).ToString());
    int selectedLanguageType = (int) Localization.GetSelectedLanguageType();
    ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(Global.LanguageCodeKey, (object) Localization.GetCurrentLanguageCode());
    if (selectedLanguageType != 2)
      return;
    ThreadedHttps<KleiMetrics>.Instance.SetStaticSessionVariable(Global.LanguageModKey, (object) LanguageOptionsScreen.GetSavedLanguageMod());
  }

  private void SetONIDynamicSessionVariables(Dictionary<string, object> data)
  {
    if (!Object.op_Inequality((Object) Game.Instance, (Object) null) || !Object.op_Inequality((Object) GameClock.Instance, (Object) null))
      return;
    data.Add("GameTimeSeconds", (object) (uint) GameClock.Instance.GetTime());
    data.Add("WasDebugEverUsed", (object) Game.Instance.debugWasUsed);
    data.Add("IsSandboxEnabled", (object) SaveGame.Instance.sandboxEnabled);
  }

  private void LateUpdate()
  {
    StreamedTextures.UpdateRequests();
    Singleton<KBatchedAnimUpdater>.Instance.LateUpdate();
    if (!this.DevTools.Show)
      return;
    ImGuiRenderer.GetInstance()?.EndFrame();
  }

  private void OnDestroy()
  {
    if (this.modManager != null)
      this.modManager.Shutdown();
    Global.Instance = (Global) null;
    if (Singleton<AnimEventManager>.Instance != null)
      Singleton<AnimEventManager>.Instance.FreeResources();
    Singleton<KBatchedAnimUpdater>.DestroyInstance();
  }

  private void OnApplicationQuit()
  {
    KGlobalAnimParser.DestroyInstance();
    ThreadedHttps<KleiMetrics>.Instance.EndSession(false);
  }

  private void OnCrashReported(string json_response)
  {
    if (Thread.CurrentThread != this.mainThread)
      return;
    if (!string.IsNullOrEmpty(json_response))
      Debug.Log((object) ("devhash: " + JsonConvert.DeserializeObject<Dictionary<string, string>>(json_response)["CrashHash"]));
    else
      Debug.Log((object) "Empty json response");
  }

  private void OutputSystemInfo()
  {
    try
    {
      Console.WriteLine("SYSTEM INFO:");
      foreach (KeyValuePair<string, object> hardwareStat in KleiMetrics.GetHardwareStats())
      {
        try
        {
          Console.WriteLine(string.Format("    {0}={1}", (object) hardwareStat.Key.ToString(), (object) hardwareStat.Value.ToString()));
        }
        catch
        {
        }
      }
      Console.WriteLine(string.Format("    {0}={1}", (object) "System Language", (object) Application.systemLanguage.ToString()));
    }
    catch
    {
    }
  }
}
