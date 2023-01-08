// Decompiled with JetBrains decompiler
// Type: Database.StickerBombs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class StickerBombs : ResourceSet<DbStickerBomb>
  {
    public static StickerBombs.Info[] Infos = new StickerBombs.Info[20]
    {
      new StickerBombs.Info("a", (string) STICKERNAMES.STICKER_A, "sticker_a_kanim", "a"),
      new StickerBombs.Info("b", (string) STICKERNAMES.STICKER_B, "sticker_b_kanim", "b"),
      new StickerBombs.Info("c", (string) STICKERNAMES.STICKER_C, "sticker_c_kanim", "c"),
      new StickerBombs.Info("d", (string) STICKERNAMES.STICKER_D, "sticker_d_kanim", "d"),
      new StickerBombs.Info("e", (string) STICKERNAMES.STICKER_E, "sticker_e_kanim", "e"),
      new StickerBombs.Info("f", (string) STICKERNAMES.STICKER_F, "sticker_f_kanim", "f"),
      new StickerBombs.Info("g", (string) STICKERNAMES.STICKER_G, "sticker_g_kanim", "g"),
      new StickerBombs.Info("h", (string) STICKERNAMES.STICKER_H, "sticker_h_kanim", "h"),
      new StickerBombs.Info("rocket", (string) STICKERNAMES.STICKER_ROCKET, "sticker_rocket_kanim", "rocket"),
      new StickerBombs.Info("paperplane", (string) STICKERNAMES.STICKER_PAPERPLANE, "sticker_paperplane_kanim", "paperplane"),
      new StickerBombs.Info("plant", (string) STICKERNAMES.STICKER_PLANT, "sticker_plant_kanim", "plant"),
      new StickerBombs.Info("plantpot", (string) STICKERNAMES.STICKER_PLANTPOT, "sticker_plantpot_kanim", "plantpot"),
      new StickerBombs.Info("mushroom", (string) STICKERNAMES.STICKER_MUSHROOM, "sticker_mushroom_kanim", "mushroom"),
      new StickerBombs.Info("mermaid", (string) STICKERNAMES.STICKER_MERMAID, "sticker_mermaid_kanim", "mermaid"),
      new StickerBombs.Info("spacepet", (string) STICKERNAMES.STICKER_SPACEPET, "sticker_spacepet_kanim", "spacepet"),
      new StickerBombs.Info("spacepet2", (string) STICKERNAMES.STICKER_SPACEPET2, "sticker_spacepet2_kanim", "spacepet2"),
      new StickerBombs.Info("spacepet3", (string) STICKERNAMES.STICKER_SPACEPET3, "sticker_spacepet3_kanim", "spacepet3"),
      new StickerBombs.Info("spacepet4", (string) STICKERNAMES.STICKER_SPACEPET4, "sticker_spacepet4_kanim", "spacepet4"),
      new StickerBombs.Info("spacepet5", (string) STICKERNAMES.STICKER_SPACEPET5, "sticker_spacepet5_kanim", "spacepet5"),
      new StickerBombs.Info("unicorn", (string) STICKERNAMES.STICKER_UNICORN, "sticker_unicorn_kanim", "unicorn")
    };

    public StickerBombs(ResourceSet parent)
      : base(nameof (StickerBombs), parent)
    {
      foreach (StickerBombs.Info info in StickerBombs.Infos)
        this.Add(info.id, info.stickerName, info.animfilename, info.sticker);
    }

    private DbStickerBomb Add(
      string id,
      string stickerType,
      string animfilename,
      string symbolName)
    {
      DbStickerBomb dbStickerBomb = new DbStickerBomb(id, stickerType, animfilename, symbolName);
      this.resources.Add(dbStickerBomb);
      return dbStickerBomb;
    }

    public DbStickerBomb GetRandomSticker() => Util.GetRandom<DbStickerBomb>(this.resources);

    public struct Info
    {
      public string id;
      public string stickerName;
      public string animfilename;
      public string sticker;

      public Info(string id, string stickerName, string animfilename, string sticker)
      {
        this.id = id;
        this.stickerName = stickerName;
        this.animfilename = animfilename;
        this.sticker = sticker;
      }
    }
  }
}
