// Decompiled with JetBrains decompiler
// Type: IGroupProber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public interface IGroupProber
{
  void Occupy(object prober, short serial_no, IEnumerable<int> cells);

  void SetValidSerialNos(object prober, short previous_serial_no, short serial_no);

  bool ReleaseProber(object prober);
}
