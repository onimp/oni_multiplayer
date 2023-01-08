// Decompiled with JetBrains decompiler
// Type: OldNoteEntriesV5
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

public class OldNoteEntriesV5
{
  public List<OldNoteEntriesV5.NoteStorageBlock> storageBlocks = new List<OldNoteEntriesV5.NoteStorageBlock>();

  public void Deserialize(BinaryReader reader)
  {
    int num = reader.ReadInt32();
    for (int index = 0; index < num; ++index)
    {
      OldNoteEntriesV5.NoteStorageBlock noteStorageBlock = new OldNoteEntriesV5.NoteStorageBlock();
      noteStorageBlock.Deserialize(reader);
      this.storageBlocks.Add(noteStorageBlock);
    }
  }

  [StructLayout(LayoutKind.Explicit)]
  public struct NoteEntry
  {
    [FieldOffset(0)]
    public int reportEntryId;
    [FieldOffset(4)]
    public int noteHash;
    [FieldOffset(8)]
    public float value;
  }

  [StructLayout(LayoutKind.Explicit)]
  public struct NoteEntryArray
  {
    [FieldOffset(0)]
    public byte[] bytes;
    [FieldOffset(0)]
    public OldNoteEntriesV5.NoteEntry[] structs;

    public int StructSizeInBytes => Marshal.SizeOf(typeof (OldNoteEntriesV5.NoteEntry));
  }

  public struct NoteStorageBlock
  {
    public int entryCount;
    public OldNoteEntriesV5.NoteEntryArray entries;

    public void Deserialize(BinaryReader reader)
    {
      this.entryCount = reader.ReadInt32();
      this.entries.bytes = reader.ReadBytes(this.entries.StructSizeInBytes * this.entryCount);
    }
  }
}
