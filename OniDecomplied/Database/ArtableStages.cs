// Decompiled with JetBrains decompiler
// Type: Database.ArtableStages
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;

namespace Database
{
  public class ArtableStages : ResourceSet<ArtableStage>
  {
    public static ArtableStages.Info[] Infos = new ArtableStages.Info[57]
    {
      new ArtableStages.Info("Canvas_Bad", (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_A.NAME, (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_A.DESC, PermitRarity.Universal, "painting_art_a_kanim", "art_a", 5, false, "LookingUgly", "Canvas", "canvas"),
      new ArtableStages.Info("Canvas_Average", (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_B.NAME, (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_B.DESC, PermitRarity.Universal, "painting_art_b_kanim", "art_b", 10, false, "LookingOkay", "Canvas", "canvas"),
      new ArtableStages.Info("Canvas_Good", (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_C.NAME, (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_C.DESC, PermitRarity.Universal, "painting_art_c_kanim", "art_c", 15, true, "LookingGreat", "Canvas", "canvas"),
      new ArtableStages.Info("Canvas_Good2", (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_D.NAME, (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_D.DESC, PermitRarity.Universal, "painting_art_d_kanim", "art_d", 15, true, "LookingGreat", "Canvas", "canvas"),
      new ArtableStages.Info("Canvas_Good3", (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_E.NAME, (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_E.DESC, PermitRarity.Universal, "painting_art_e_kanim", "art_e", 15, true, "LookingGreat", "Canvas", "canvas"),
      new ArtableStages.Info("Canvas_Good4", (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_F.NAME, (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_F.DESC, PermitRarity.Universal, "painting_art_f_kanim", "art_f", 15, true, "LookingGreat", "Canvas", "canvas"),
      new ArtableStages.Info("Canvas_Good5", (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_G.NAME, (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_G.DESC, PermitRarity.Universal, "painting_art_g_kanim", "art_g", 15, true, "LookingGreat", "Canvas", "canvas"),
      new ArtableStages.Info("Canvas_Good6", (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_H.NAME, (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_H.DESC, PermitRarity.Universal, "painting_art_h_kanim", "art_h", 15, true, "LookingGreat", "Canvas", "canvas"),
      new ArtableStages.Info("CanvasTall_Bad", (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_A.NAME, (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_A.DESC, PermitRarity.Universal, "painting_tall_art_a_kanim", "art_a", 5, false, "LookingUgly", "CanvasTall", "canvas"),
      new ArtableStages.Info("CanvasTall_Average", (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_B.NAME, (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_B.DESC, PermitRarity.Universal, "painting_tall_art_b_kanim", "art_b", 10, false, "LookingOkay", "CanvasTall", "canvas"),
      new ArtableStages.Info("CanvasTall_Good", (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_C.NAME, (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_C.DESC, PermitRarity.Universal, "painting_tall_art_c_kanim", "art_c", 15, true, "LookingGreat", "CanvasTall", "canvas"),
      new ArtableStages.Info("CanvasTall_Good2", (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_D.NAME, (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_D.DESC, PermitRarity.Universal, "painting_tall_art_d_kanim", "art_d", 15, true, "LookingGreat", "CanvasTall", "canvas"),
      new ArtableStages.Info("CanvasTall_Good3", (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_E.NAME, (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_E.DESC, PermitRarity.Universal, "painting_tall_art_e_kanim", "art_e", 15, true, "LookingGreat", "CanvasTall", "canvas"),
      new ArtableStages.Info("CanvasTall_Good4", (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_F.NAME, (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_F.DESC, PermitRarity.Universal, "painting_tall_art_f_kanim", "art_f", 15, true, "LookingGreat", "CanvasTall", "canvas"),
      new ArtableStages.Info("CanvasWide_Bad", (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_A.NAME, (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_A.DESC, PermitRarity.Universal, "painting_wide_art_a_kanim", "art_a", 5, false, "LookingUgly", "CanvasWide", "canvas"),
      new ArtableStages.Info("CanvasWide_Average", (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_B.NAME, (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_B.DESC, PermitRarity.Universal, "painting_wide_art_b_kanim", "art_b", 10, false, "LookingOkay", "CanvasWide", "canvas"),
      new ArtableStages.Info("CanvasWide_Good", (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_C.NAME, (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_C.DESC, PermitRarity.Universal, "painting_wide_art_c_kanim", "art_c", 15, true, "LookingGreat", "CanvasWide", "canvas"),
      new ArtableStages.Info("CanvasWide_Good2", (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_D.NAME, (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_D.DESC, PermitRarity.Universal, "painting_wide_art_d_kanim", "art_d", 15, true, "LookingGreat", "CanvasWide", "canvas"),
      new ArtableStages.Info("CanvasWide_Good3", (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_E.NAME, (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_E.DESC, PermitRarity.Universal, "painting_wide_art_e_kanim", "art_e", 15, true, "LookingGreat", "CanvasWide", "canvas"),
      new ArtableStages.Info("CanvasWide_Good4", (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_F.NAME, (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_F.DESC, PermitRarity.Universal, "painting_wide_art_f_kanim", "art_f", 15, true, "LookingGreat", "CanvasWide", "canvas"),
      new ArtableStages.Info("Sculpture_Bad", (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_CRAP_1.NAME, (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_CRAP_1.DESC, PermitRarity.Universal, "sculpture_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "Sculpture"),
      new ArtableStages.Info("Sculpture_Average", (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_GOOD_1.NAME, (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_GOOD_1.DESC, PermitRarity.Universal, "sculpture_good_1_kanim", "good_1", 10, false, "LookingOkay", "Sculpture"),
      new ArtableStages.Info("Sculpture_Good1", (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_1.NAME, (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "Sculpture"),
      new ArtableStages.Info("Sculpture_Good2", (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_2.NAME, (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "Sculpture"),
      new ArtableStages.Info("Sculpture_Good3", (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_3.NAME, (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "Sculpture"),
      new ArtableStages.Info("SmallSculpture_Bad", (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_CRAP.NAME, (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_CRAP.DESC, PermitRarity.Universal, "sculpture_1x2_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "SmallSculpture"),
      new ArtableStages.Info("SmallSculpture_Average", (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_GOOD.NAME, (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_GOOD.DESC, PermitRarity.Universal, "sculpture_1x2_good_1_kanim", "good_1", 10, false, "LookingOkay", "SmallSculpture"),
      new ArtableStages.Info("SmallSculpture_Good", (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_1.NAME, (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_1x2_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "SmallSculpture"),
      new ArtableStages.Info("SmallSculpture_Good2", (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_2.NAME, (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_1x2_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "SmallSculpture"),
      new ArtableStages.Info("SmallSculpture_Good3", (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_3.NAME, (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_1x2_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "SmallSculpture"),
      new ArtableStages.Info("IceSculpture_Bad", (string) BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_CRAP.NAME, (string) BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_CRAP.DESC, PermitRarity.Universal, "icesculpture_crap_kanim", "crap", 5, false, "LookingUgly", "IceSculpture"),
      new ArtableStages.Info("IceSculpture_Average", (string) BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_1.NAME, (string) BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_1.DESC, PermitRarity.Universal, "icesculpture_idle_kanim", "idle", 10, false, "LookingOkay", "IceSculpture", "good"),
      new ArtableStages.Info("MarbleSculpture_Bad", (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_CRAP_1.NAME, (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_CRAP_1.DESC, PermitRarity.Universal, "sculpture_marble_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "MarbleSculpture"),
      new ArtableStages.Info("MarbleSculpture_Average", (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_GOOD_1.NAME, (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_GOOD_1.DESC, PermitRarity.Universal, "sculpture_marble_good_1_kanim", "good_1", 10, false, "LookingOkay", "MarbleSculpture"),
      new ArtableStages.Info("MarbleSculpture_Good1", (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_1.NAME, (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_marble_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "MarbleSculpture"),
      new ArtableStages.Info("MarbleSculpture_Good2", (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_2.NAME, (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_marble_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "MarbleSculpture"),
      new ArtableStages.Info("MarbleSculpture_Good3", (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_3.NAME, (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_marble_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "MarbleSculpture"),
      new ArtableStages.Info("MetalSculpture_Bad", (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_CRAP_1.NAME, (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_CRAP_1.DESC, PermitRarity.Universal, "sculpture_metal_crap_1_kanim", "crap_1", 5, false, "LookingUgly", "MetalSculpture"),
      new ArtableStages.Info("MetalSculpture_Average", (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_GOOD_1.NAME, (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_GOOD_1.DESC, PermitRarity.Universal, "sculpture_metal_good_1_kanim", "good_1", 10, false, "LookingOkay", "MetalSculpture"),
      new ArtableStages.Info("MetalSculpture_Good1", (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_1.NAME, (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_1.DESC, PermitRarity.Universal, "sculpture_metal_amazing_1_kanim", "amazing_1", 15, true, "LookingGreat", "MetalSculpture"),
      new ArtableStages.Info("MetalSculpture_Good2", (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_2.NAME, (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_2.DESC, PermitRarity.Universal, "sculpture_metal_amazing_2_kanim", "amazing_2", 15, true, "LookingGreat", "MetalSculpture"),
      new ArtableStages.Info("MetalSculpture_Good3", (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_3.NAME, (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_3.DESC, PermitRarity.Universal, "sculpture_metal_amazing_3_kanim", "amazing_3", 15, true, "LookingGreat", "MetalSculpture"),
      new ArtableStages.Info("Canvas_Good7", (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_I.NAME, (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_I.DESC, PermitRarity.Decent, "painting_art_i_kanim", "art_i", 15, true, "LookingGreat", "Canvas", "canvas"),
      new ArtableStages.Info("Canvas_Good8", (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_J.NAME, (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_J.DESC, PermitRarity.Decent, "painting_art_j_kanim", "art_j", 15, true, "LookingGreat", "Canvas", "canvas"),
      new ArtableStages.Info("Canvas_Good9", (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_K.NAME, (string) BUILDINGS.PREFABS.CANVAS.FACADES.ART_K.DESC, PermitRarity.Decent, "painting_art_k_kanim", "art_k", 15, true, "LookingGreat", "Canvas", "canvas"),
      new ArtableStages.Info("CanvasTall_Good5", (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_G.NAME, (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_G.DESC, PermitRarity.Decent, "painting_tall_art_g_kanim", "art_g", 15, true, "LookingGreat", "CanvasTall", "canvas"),
      new ArtableStages.Info("CanvasTall_Good6", (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_H.NAME, (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_H.DESC, PermitRarity.Decent, "painting_tall_art_h_kanim", "art_h", 15, true, "LookingGreat", "CanvasTall", "canvas"),
      new ArtableStages.Info("CanvasTall_Good7", (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_I.NAME, (string) BUILDINGS.PREFABS.CANVASTALL.FACADES.ART_TALL_I.DESC, PermitRarity.Decent, "painting_tall_art_i_kanim", "art_i", 15, true, "LookingGreat", "CanvasTall", "canvas"),
      new ArtableStages.Info("CanvasWide_Good5", (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_G.NAME, (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_G.DESC, PermitRarity.Decent, "painting_wide_art_g_kanim", "art_g", 15, true, "LookingGreat", "CanvasWide", "canvas"),
      new ArtableStages.Info("CanvasWide_Good6", (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_H.NAME, (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_H.DESC, PermitRarity.Decent, "painting_wide_art_h_kanim", "art_h", 15, true, "LookingGreat", "CanvasWide", "canvas"),
      new ArtableStages.Info("CanvasWide_Good7", (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_I.NAME, (string) BUILDINGS.PREFABS.CANVASWIDE.FACADES.ART_WIDE_I.DESC, PermitRarity.Decent, "painting_wide_art_i_kanim", "art_i", 15, true, "LookingGreat", "CanvasWide", "canvas"),
      new ArtableStages.Info("Sculpture_Good4", (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_4.NAME, (string) BUILDINGS.PREFABS.SCULPTURE.FACADES.SCULPTURE_AMAZING_4.DESC, PermitRarity.Decent, "sculpture_amazing_4_kanim", "amazing_4", 15, true, "LookingGreat", "Sculpture"),
      new ArtableStages.Info("SmallSculpture_Good4", (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_4.NAME, (string) BUILDINGS.PREFABS.SMALLSCULPTURE.FACADES.SCULPTURE_1x2_AMAZING_4.DESC, PermitRarity.Decent, "sculpture_1x2_amazing_4_kanim", "amazing_4", 15, true, "LookingGreat", "SmallSculpture"),
      new ArtableStages.Info("MetalSculpture_Good4", (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_4.NAME, (string) BUILDINGS.PREFABS.METALSCULPTURE.FACADES.SCULPTURE_METAL_AMAZING_4.DESC, PermitRarity.Decent, "sculpture_metal_amazing_4_kanim", "amazing_4", 15, true, "LookingGreat", "MetalSculpture"),
      new ArtableStages.Info("MarbleSculpture_Good4", (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_4.NAME, (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_4.DESC, PermitRarity.Decent, "sculpture_marble_amazing_4_kanim", "amazing_4", 15, true, "LookingGreat", "MarbleSculpture"),
      new ArtableStages.Info("MarbleSculpture_Good5", (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_5.NAME, (string) BUILDINGS.PREFABS.MARBLESCULPTURE.FACADES.SCULPTURE_MARBLE_AMAZING_5.DESC, PermitRarity.Decent, "sculpture_marble_amazing_5_kanim", "amazing_5", 15, true, "LookingGreat", "MarbleSculpture"),
      new ArtableStages.Info("IceSculpture_Average2", (string) BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_2.NAME, (string) BUILDINGS.PREFABS.ICESCULPTURE.FACADES.ICESCULPTURE_AMAZING_2.DESC, PermitRarity.Decent, "icesculpture_idle_2_kanim", "idle_2", 10, false, "LookingOkay", "IceSculpture")
    };

    public ArtableStage Add(
      string id,
      string name,
      string desc,
      PermitRarity rarity,
      string animFile,
      string anim,
      int decor_value,
      bool cheer_on_complete,
      string status_id,
      string prefabId,
      string symbolname = "")
    {
      ArtableStatusItem status_item = Db.Get().ArtableStatuses.Get(status_id);
      ArtableStage artableStage = new ArtableStage(id, name, desc, rarity, animFile, anim, decor_value, cheer_on_complete, status_item, prefabId, symbolname);
      this.resources.Add(artableStage);
      return artableStage;
    }

    public ArtableStages(ResourceSet parent)
      : base(nameof (ArtableStages), parent)
    {
      foreach (ArtableStages.Info info in ArtableStages.Infos)
        this.Add(info.id, info.name, info.desc, info.rarity, info.animFile, info.anim, info.decor_value, info.cheer_on_complete, info.status_id, info.prefabId, info.symbolname);
    }

    public List<ArtableStage> GetPrefabStages(Tag prefab_id) => this.resources.FindAll((Predicate<ArtableStage>) (stage => Tag.op_Equality(Tag.op_Implicit(stage.prefabId), prefab_id)));

    public ArtableStage DefaultPrefabStage(Tag prefab_id) => this.GetPrefabStages(prefab_id).Find((Predicate<ArtableStage>) (stage => stage.statusItem == Db.Get().ArtableStatuses.AwaitingArting));

    public struct Info
    {
      public string id;
      public string name;
      public string desc;
      public PermitRarity rarity;
      public string animFile;
      public string anim;
      public int decor_value;
      public bool cheer_on_complete;
      public string status_id;
      public string prefabId;
      public string symbolname;

      public Info(
        string id,
        string name,
        string desc,
        PermitRarity rarity,
        string animFile,
        string anim,
        int decor_value,
        bool cheer_on_complete,
        string status_id,
        string prefabId,
        string symbolname = "")
      {
        this.id = id;
        this.name = name;
        this.desc = desc;
        this.rarity = rarity;
        this.animFile = animFile;
        this.anim = anim;
        this.decor_value = decor_value;
        this.cheer_on_complete = cheer_on_complete;
        this.status_id = status_id;
        this.prefabId = prefabId;
        this.symbolname = symbolname;
      }
    }
  }
}
