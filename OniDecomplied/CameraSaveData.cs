// Decompiled with JetBrains decompiler
// Type: CameraSaveData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class CameraSaveData
{
  public static bool valid;
  public static Vector3 position;
  public static Vector3 localScale;
  public static Quaternion rotation;
  public static float orthographicsSize;

  public static void Load(FastReader reader)
  {
    CameraSaveData.position = Util.ReadVector3((IReader) reader);
    CameraSaveData.localScale = Util.ReadVector3((IReader) reader);
    CameraSaveData.rotation = Util.ReadQuaternion((IReader) reader);
    CameraSaveData.orthographicsSize = reader.ReadSingle();
    CameraSaveData.valid = true;
  }
}
