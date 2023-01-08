// Decompiled with JetBrains decompiler
// Type: Database.MonumentParts
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace Database
{
  public class MonumentParts : ResourceSet<MonumentPartResource>
  {
    public static MonumentParts.Info[] Infos_Vanilla = new MonumentParts.Info[40]
    {
      new MonumentParts.Info("bottom_option_a", "monument_base_a_kanim", "option_a", "straight_legs", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_b", "monument_base_b_kanim", "option_b", "wide_stance", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_c", "monument_base_c_kanim", "option_c", "hmmm_legs", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_d", "monument_base_d_kanim", "option_d", "sitting_stool", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_e", "monument_base_e_kanim", "option_e", "wide_stance2", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_f", "monument_base_f_kanim", "option_f", "posing1", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_g", "monument_base_g_kanim", "option_g", "knee_kick", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_h", "monument_base_h_kanim", "option_h", "step_on_hatches", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_i", "monument_base_i_kanim", "option_i", "sit_on_tools", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_j", "monument_base_j_kanim", "option_j", "water_pacu", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_k", "monument_base_k_kanim", "option_k", "sit_on_eggs", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("mid_option_a", "monument_mid_a_kanim", "option_a", "thumbs_up", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_b", "monument_mid_b_kanim", "option_b", "wrench", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_c", "monument_mid_c_kanim", "option_c", "hmmm", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_d", "monument_mid_d_kanim", "option_d", "hips_hands", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_e", "monument_mid_e_kanim", "option_e", "hold_face", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_f", "monument_mid_f_kanim", "option_f", "finger_gun", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_g", "monument_mid_g_kanim", "option_g", "model_pose", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_h", "monument_mid_h_kanim", "option_h", "punch", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_i", "monument_mid_i_kanim", "option_i", "holding_hatch", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_j", "monument_mid_j_kanim", "option_j", "model_pose2", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_k", "monument_mid_k_kanim", "option_k", "balancing", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_l", "monument_mid_l_kanim", "option_l", "holding_babies", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("top_option_a", "monument_upper_a_kanim", "option_a", "leira", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_b", "monument_upper_b_kanim", "option_b", "mae", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_c", "monument_upper_c_kanim", "option_c", "puft", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_d", "monument_upper_d_kanim", "option_d", "nikola", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_e", "monument_upper_e_kanim", "option_e", "burt", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_f", "monument_upper_f_kanim", "option_f", "rowan", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_g", "monument_upper_g_kanim", "option_g", "nisbet", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_h", "monument_upper_h_kanim", "option_h", "joshua", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_i", "monument_upper_i_kanim", "option_i", "ren", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_j", "monument_upper_j_kanim", "option_j", "hatch", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_k", "monument_upper_k_kanim", "option_k", "drecko", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_l", "monument_upper_l_kanim", "option_l", "driller", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_m", "monument_upper_m_kanim", "option_m", "gassymoo", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_n", "monument_upper_n_kanim", "option_n", "glom", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_o", "monument_upper_o_kanim", "option_o", "lightbug", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_p", "monument_upper_p_kanim", "option_p", "slickster", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_q", "monument_upper_q_kanim", "option_q", "pacu", MonumentPartResource.Part.Top)
    };
    public static MonumentParts.Info[] Infos_Expansion1 = new MonumentParts.Info[21]
    {
      new MonumentParts.Info("bottom_option_l", "monument_base_l_kanim", "option_l", "rocketnosecone", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_m", "monument_base_m_kanim", "option_m", "rocketsugarengine", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_n", "monument_base_n_kanim", "option_n", "rocketnCO2", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_o", "monument_base_o_kanim", "option_o", "rocketpetro", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_p", "monument_base_p_kanim", "option_p", "rocketnoseconesmall", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_q", "monument_base_q_kanim", "option_q", "rocketradengine", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_r", "monument_base_r_kanim", "option_r", "sweepyoff", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_s", "monument_base_s_kanim", "option_s", "sweepypeek", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("bottom_option_t", "monument_base_t_kanim", "option_t", "sweepy", MonumentPartResource.Part.Bottom),
      new MonumentParts.Info("mid_option_m", "monument_mid_m_kanim", "option_m", "rocket", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_n", "monument_mid_n_kanim", "option_n", "holding_baby_worm", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("mid_option_o", "monument_mid_o_kanim", "option_o", "holding_baby_blarva_critter", MonumentPartResource.Part.Middle),
      new MonumentParts.Info("top_option_r", "monument_upper_r_kanim", "option_r", "bee", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_s", "monument_upper_s_kanim", "option_s", "critter", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_t", "monument_upper_t_kanim", "option_t", "caterpillar", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_u", "monument_upper_u_kanim", "option_u", "worm", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_v", "monument_upper_v_kanim", "option_v", "scout_bot", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_w", "monument_upper_w_kanim", "option_w", "MiMa", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_x", "monument_upper_x_kanim", "option_x", "Stinky", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_y", "monument_upper_y_kanim", "option_y", "Harold", MonumentPartResource.Part.Top),
      new MonumentParts.Info("top_option_z", "monument_upper_z_kanim", "option_z", "Nails", MonumentPartResource.Part.Top)
    };

    public MonumentParts(ResourceSet parent)
      : base(nameof (MonumentParts), parent)
    {
      this.Initialize();
      foreach (MonumentParts.Info info in MonumentParts.Infos_Vanilla)
        this.Add(info.id, info.animFilename, info.state, info.symbolName, info.part);
      if (!DlcManager.IsExpansion1Active())
        return;
      foreach (MonumentParts.Info info in MonumentParts.Infos_Expansion1)
        this.Add(info.id, info.animFilename, info.state, info.symbolName, info.part);
    }

    public void Add(
      string id,
      string animFilename,
      string state,
      string symbolName,
      MonumentPartResource.Part part)
    {
      this.resources.Add(new MonumentPartResource(id, animFilename, state, symbolName, part));
    }

    public List<MonumentPartResource> GetParts(MonumentPartResource.Part part) => this.resources.FindAll((Predicate<MonumentPartResource>) (mpr => mpr.part == part));

    public struct Info
    {
      public string id;
      public string animFilename;
      public string state;
      public string symbolName;
      public MonumentPartResource.Part part;

      public Info(
        string id,
        string animFilename,
        string state,
        string symbolName,
        MonumentPartResource.Part part)
      {
        this.id = id;
        this.animFilename = animFilename;
        this.state = state;
        this.symbolName = symbolName;
        this.part = part;
      }
    }
  }
}
