// Decompiled with JetBrains decompiler
// Type: Klei.SimSaveFileStructure
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Klei
{
  public class SimSaveFileStructure
  {
    public int WidthInCells;
    public int HeightInCells;
    public int x;
    public int y;
    public byte[] Sim;
    public WorldDetailSave worldDetail;

    public SimSaveFileStructure() => this.worldDetail = new WorldDetailSave();
  }
}
