// Decompiled with JetBrains decompiler
// Type: GameComps
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Reflection;

public class GameComps : KComponents
{
  public static GravityComponents Gravities;
  public static FallerComponents Fallers;
  public static InfraredVisualizerComponents InfraredVisualizers;
  public static ElementSplitterComponents ElementSplitters;
  public static OreSizeVisualizerComponents OreSizeVisualizers;
  public static StructureTemperatureComponents StructureTemperatures;
  public static DiseaseContainers DiseaseContainers;
  public static RequiresFoundation RequiresFoundations;
  public static WhiteBoard WhiteBoards;
  private static Dictionary<System.Type, IKComponentManager> kcomponentManagers = new Dictionary<System.Type, IKComponentManager>();

  public GameComps()
  {
    foreach (FieldInfo field in typeof (GameComps).GetFields())
    {
      object instance = Activator.CreateInstance(field.FieldType);
      field.SetValue((object) null, instance);
      this.Add<IComponentManager>(instance as IComponentManager);
      if (instance is IKComponentManager)
      {
        IKComponentManager inst = instance as IKComponentManager;
        GameComps.AddKComponentManager(field.FieldType, inst);
      }
    }
  }

  public void Clear()
  {
    foreach (FieldInfo field in typeof (GameComps).GetFields())
    {
      if (field.GetValue((object) null) is IComponentManager icomponentManager)
        icomponentManager.Clear();
    }
  }

  public static void AddKComponentManager(System.Type kcomponent, IKComponentManager inst) => GameComps.kcomponentManagers[kcomponent] = inst;

  public static IKComponentManager GetKComponentManager(System.Type kcomponent_type) => GameComps.kcomponentManagers[kcomponent_type];
}
