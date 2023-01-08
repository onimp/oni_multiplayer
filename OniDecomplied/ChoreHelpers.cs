// Decompiled with JetBrains decompiler
// Type: ChoreHelpers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class ChoreHelpers
{
  public static GameObject CreateLocator(string name, Vector3 pos)
  {
    GameObject locator = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(ApproachableLocator.ID)), (GameObject) null, (string) null);
    ((Object) locator).name = name;
    TransformExtensions.SetPosition(locator.transform, pos);
    locator.gameObject.SetActive(true);
    return locator;
  }

  public static GameObject CreateSleepLocator(Vector3 pos)
  {
    GameObject sleepLocator = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(SleepLocator.ID)), (GameObject) null, (string) null);
    ((Object) sleepLocator).name = "SLeepLocator";
    TransformExtensions.SetPosition(sleepLocator.transform, pos);
    sleepLocator.gameObject.SetActive(true);
    return sleepLocator;
  }

  public static void DestroyLocator(GameObject locator)
  {
    if (!Object.op_Inequality((Object) locator, (Object) null))
      return;
    TracesExtesions.DeleteObject(locator.gameObject);
  }
}
