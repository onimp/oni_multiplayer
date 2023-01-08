// Decompiled with JetBrains decompiler
// Type: Database.Emotes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

namespace Database
{
  public class Emotes : ResourceSet<Resource>
  {
    public Emotes.MinionEmotes Minion;
    public Emotes.CritterEmotes Critter;

    public Emotes(ResourceSet parent)
      : base(nameof (Emotes), parent)
    {
      this.Minion = new Emotes.MinionEmotes((ResourceSet) this);
      this.Critter = new Emotes.CritterEmotes((ResourceSet) this);
    }

    public void ResetProblematicReferences()
    {
      for (int index = 0; index < this.Minion.resources.Count; ++index)
      {
        Emote resource = this.Minion.resources[index];
        for (int stepIdx = 0; stepIdx < resource.StepCount; ++stepIdx)
          resource[stepIdx].UnregisterAllCallbacks();
      }
      for (int index = 0; index < this.Critter.resources.Count; ++index)
      {
        Emote resource = this.Critter.resources[index];
        for (int stepIdx = 0; stepIdx < resource.StepCount; ++stepIdx)
          resource[stepIdx].UnregisterAllCallbacks();
      }
    }

    public class MinionEmotes : ResourceSet<Emote>
    {
      private static EmoteStep[] DEFAULT_STEPS = new EmoteStep[1]
      {
        new EmoteStep() { anim = HashedString.op_Implicit("react") }
      };
      private static EmoteStep[] DEFAULT_IDLE_STEPS = new EmoteStep[3]
      {
        new EmoteStep()
        {
          anim = HashedString.op_Implicit("idle_pre")
        },
        new EmoteStep()
        {
          anim = HashedString.op_Implicit("idle_default")
        },
        new EmoteStep()
        {
          anim = HashedString.op_Implicit("idle_pst")
        }
      };
      public Emote ClapCheer;
      public Emote Cheer;
      public Emote ProductiveCheer;
      public Emote ResearchComplete;
      public Emote ThumbsUp;
      public Emote CloseCall_Fall;
      public Emote Cold;
      public Emote Cough;
      public Emote Cough_Small;
      public Emote FoodPoisoning;
      public Emote Hot;
      public Emote IritatedEyes;
      public Emote MorningStretch;
      public Emote Radiation_Glare;
      public Emote Radiation_Itch;
      public Emote Sick;
      public Emote Sneeze;
      public Emote Sneeze_Short;
      public Emote Concern;
      public Emote Cringe;
      public Emote Disappointed;
      public Emote Shock;
      public Emote Sing;
      public Emote FingerGuns;
      public Emote Wave;
      public Emote Wave_Shy;

      public MinionEmotes(ResourceSet parent)
        : base("Minion", parent)
      {
        this.InitializeCelebrations();
        this.InitializePhysicalStatus();
        this.InitializeEmotionalStatus();
        this.InitializeGreetings();
      }

      public void InitializeCelebrations()
      {
        this.ClapCheer = new Emote((ResourceSet) this, "ClapCheer", new EmoteStep[3]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("clapcheer_pre")
          },
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("clapcheer_loop")
          },
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("clapcheer_pst")
          }
        }, "anim_clapcheer_kanim");
        this.Cheer = new Emote((ResourceSet) this, "Cheer", new EmoteStep[3]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("cheer_pre")
          },
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("cheer_loop")
          },
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("cheer_pst")
          }
        }, "anim_cheer_kanim");
        this.ProductiveCheer = new Emote((ResourceSet) this, "Productive Cheer", new EmoteStep[1]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("productive")
          }
        }, "anim_productive_kanim");
        this.ResearchComplete = new Emote((ResourceSet) this, "ResearchComplete", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_research_complete_kanim");
        this.ThumbsUp = new Emote((ResourceSet) this, "ThumbsUp", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_thumbsup_kanim");
      }

      private void InitializePhysicalStatus()
      {
        this.CloseCall_Fall = new Emote((ResourceSet) this, "Near Fall", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_floor_missing_kanim");
        this.Cold = new Emote((ResourceSet) this, "Cold", Emotes.MinionEmotes.DEFAULT_IDLE_STEPS, "andim_idle_cold_kanim");
        this.Cough = new Emote((ResourceSet) this, "Cough", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_slimelungcough_kanim");
        this.Cough_Small = new Emote((ResourceSet) this, "Small Cough", new EmoteStep[1]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("react_small")
          }
        }, "anim_slimelungcough_kanim");
        this.FoodPoisoning = new Emote((ResourceSet) this, "Food Poisoning", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_contaminated_food_kanim");
        this.Hot = new Emote((ResourceSet) this, "Hot", Emotes.MinionEmotes.DEFAULT_IDLE_STEPS, "anim_idle_hot_kanim");
        this.IritatedEyes = new Emote((ResourceSet) this, "Irritated Eyes", new EmoteStep[1]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("irritated_eyes")
          }
        }, "anim_irritated_eyes_kanim");
        this.MorningStretch = new Emote((ResourceSet) this, "Morning Stretch", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_morning_stretch_kanim");
        this.Radiation_Glare = new Emote((ResourceSet) this, "Radiation Glare", new EmoteStep[1]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("react_radiation_glare")
          }
        }, "anim_react_radiation_kanim");
        this.Radiation_Itch = new Emote((ResourceSet) this, "Radiation Itch", new EmoteStep[1]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("react_radiation_itch")
          }
        }, "anim_react_radiation_kanim");
        this.Sick = new Emote((ResourceSet) this, "Sick", Emotes.MinionEmotes.DEFAULT_IDLE_STEPS, "anim_idle_sick_kanim");
        this.Sneeze = new Emote((ResourceSet) this, "Sneeze", new EmoteStep[2]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("sneeze")
          },
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("sneeze_pst")
          }
        }, "anim_sneeze_kanim");
        this.Sneeze_Short = new Emote((ResourceSet) this, "Short Sneeze", new EmoteStep[2]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("sneeze_short")
          },
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("sneeze_short_pst")
          }
        }, "anim_sneeze_kanim");
      }

      private void InitializeEmotionalStatus()
      {
        this.Concern = new Emote((ResourceSet) this, "Concern", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_concern_kanim");
        this.Cringe = new Emote((ResourceSet) this, "Cringe", new EmoteStep[3]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("cringe_pre")
          },
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("cringe_loop")
          },
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("cringe_pst")
          }
        }, "anim_cringe_kanim");
        this.Disappointed = new Emote((ResourceSet) this, "Disappointed", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_disappointed_kanim");
        this.Shock = new Emote((ResourceSet) this, "Shock", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_shock_kanim");
        this.Sing = new Emote((ResourceSet) this, "Sing", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_singer_kanim");
      }

      private void InitializeGreetings()
      {
        this.FingerGuns = new Emote((ResourceSet) this, "Finger Guns", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_fingerguns_kanim");
        this.Wave = new Emote((ResourceSet) this, "Wave", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_wave_kanim");
        this.Wave_Shy = new Emote((ResourceSet) this, "Shy Wave", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_wave_shy_kanim");
      }
    }

    public class CritterEmotes : ResourceSet<Emote>
    {
      public Emote Hungry;
      public Emote Angry;
      public Emote Happy;
      public Emote Idle;
      public Emote Sad;

      public CritterEmotes(ResourceSet parent)
        : base("Critter", parent)
      {
        this.InitializePhysicalState();
        this.InitializeEmotionalState();
      }

      private void InitializePhysicalState() => this.Hungry = new Emote((ResourceSet) this, "Hungry", new EmoteStep[1]
      {
        new EmoteStep()
        {
          anim = HashedString.op_Implicit("react_hungry")
        }
      });

      private void InitializeEmotionalState()
      {
        this.Angry = new Emote((ResourceSet) this, "Angry", new EmoteStep[1]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("react_angry")
          }
        });
        this.Happy = new Emote((ResourceSet) this, "Happy", new EmoteStep[1]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("react_happy")
          }
        });
        this.Idle = new Emote((ResourceSet) this, "Idle", new EmoteStep[1]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("react_idle")
          }
        });
        this.Sad = new Emote((ResourceSet) this, "Sad", new EmoteStep[1]
        {
          new EmoteStep()
          {
            anim = HashedString.op_Implicit("react_sad")
          }
        });
      }
    }
  }
}
