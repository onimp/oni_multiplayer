// Decompiled with JetBrains decompiler
// Type: Database.Thoughts
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class Thoughts : ResourceSet<Thought>
  {
    public Thought Starving;
    public Thought Hot;
    public Thought Cold;
    public Thought BreakBladder;
    public Thought FullBladder;
    public Thought Happy;
    public Thought Unhappy;
    public Thought PoorDecor;
    public Thought PoorFoodQuality;
    public Thought GoodFoodQuality;
    public Thought Sleepy;
    public Thought Suffocating;
    public Thought Angry;
    public Thought Raging;
    public Thought GotInfected;
    public Thought PutridOdour;
    public Thought Noisy;
    public Thought NewRole;
    public Thought Chatty;
    public Thought Encourage;
    public Thought CatchyTune;
    public Thought Dreaming;

    public Thoughts(ResourceSet parent)
      : base(nameof (Thoughts), parent)
    {
      this.GotInfected = new Thought(nameof (GotInfected), (ResourceSet) this, "crew_state_sick", (string) null, "crew_state_sick", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.GOTINFECTED.TOOLTIP);
      this.Starving = new Thought(nameof (Starving), (ResourceSet) this, "crew_state_hungry", (string) null, "crew_state_hungry", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.STARVING.TOOLTIP);
      this.Hot = new Thought(nameof (Hot), (ResourceSet) this, "crew_state_temp_up", (string) null, "crew_state_temp_up", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.HOT.TOOLTIP);
      this.Cold = new Thought(nameof (Cold), (ResourceSet) this, "crew_state_temp_down", (string) null, "crew_state_temp_down", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.COLD.TOOLTIP);
      this.BreakBladder = new Thought(nameof (BreakBladder), (ResourceSet) this, "crew_state_full_bladder", (string) null, "crew_state_full_bladder", "bubble_conversation", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.BREAKBLADDER.TOOLTIP);
      this.FullBladder = new Thought(nameof (FullBladder), (ResourceSet) this, "crew_state_full_bladder", (string) null, "crew_state_full_bladder", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.FULLBLADDER.TOOLTIP);
      this.PoorDecor = new Thought(nameof (PoorDecor), (ResourceSet) this, "crew_state_decor", (string) null, "crew_state_decor", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.POORDECOR.TOOLTIP);
      this.PoorFoodQuality = new Thought(nameof (PoorFoodQuality), (ResourceSet) this, "crew_state_yuck", (string) null, "crew_state_yuck", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.POOR_FOOD_QUALITY.TOOLTIP);
      this.GoodFoodQuality = new Thought(nameof (GoodFoodQuality), (ResourceSet) this, "crew_state_happy", (string) null, "crew_state_happy", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.GOOD_FOOD_QUALITY.TOOLTIP);
      this.Happy = new Thought(nameof (Happy), (ResourceSet) this, "crew_state_happy", (string) null, "crew_state_happy", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.HAPPY.TOOLTIP);
      this.Unhappy = new Thought(nameof (Unhappy), (ResourceSet) this, "crew_state_unhappy", (string) null, "crew_state_unhappy", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.UNHAPPY.TOOLTIP);
      this.Sleepy = new Thought(nameof (Sleepy), (ResourceSet) this, "crew_state_sleepy", (string) null, "crew_state_sleepy", "bubble_conversation", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.SLEEPY.TOOLTIP);
      this.Suffocating = new Thought(nameof (Suffocating), (ResourceSet) this, "crew_state_cantbreathe", (string) null, "crew_state_cantbreathe", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.SUFFOCATING.TOOLTIP);
      this.Angry = new Thought(nameof (Angry), (ResourceSet) this, "crew_state_angry", (string) null, "crew_state_angry", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.ANGRY.TOOLTIP);
      this.Raging = new Thought("Enraged", (ResourceSet) this, "crew_state_enraged", (string) null, "crew_state_enraged", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.RAGING.TOOLTIP);
      this.PutridOdour = new Thought(nameof (PutridOdour), (ResourceSet) this, "crew_state_smelled_putrid_odour", (string) null, "crew_state_smelled_putrid_odour", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.PUTRIDODOUR.TOOLTIP, true);
      this.Noisy = new Thought(nameof (Noisy), (ResourceSet) this, "crew_state_noisey", (string) null, "crew_state_noisey", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.NOISY.TOOLTIP, true);
      this.NewRole = new Thought(nameof (NewRole), (ResourceSet) this, "crew_state_role", (string) null, "crew_state_role", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.NEWROLE.TOOLTIP);
      this.Encourage = new Thought(nameof (Encourage), (ResourceSet) this, "crew_state_encourage", (string) null, "crew_state_happy", "bubble_conversation", SpeechMonitor.PREFIX_HAPPY, DUPLICANTS.THOUGHTS.ENCOURAGE.TOOLTIP);
      this.Chatty = new Thought(nameof (Chatty), (ResourceSet) this, "crew_state_chatty", (string) null, "conversation_short", "bubble_conversation", SpeechMonitor.PREFIX_HAPPY, DUPLICANTS.THOUGHTS.CHATTY.TOOLTIP);
      this.CatchyTune = new Thought(nameof (CatchyTune), (ResourceSet) this, "crew_state_music", (string) null, "crew_state_music", "bubble_conversation", SpeechMonitor.PREFIX_SINGER, DUPLICANTS.THOUGHTS.CATCHYTUNE.TOOLTIP, true);
      this.Dreaming = new Thought(nameof (Dreaming), (ResourceSet) this, "pajamas", (string) null, (string) null, "bubble_dream", (string) null, DUPLICANTS.THOUGHTS.DREAMY.TOOLTIP);
      for (int index = ((ResourceSet) this).Count - 1; index >= 0; --index)
        this.resources[index].priority = 100 * (((ResourceSet) this).Count - index);
    }
  }
}
