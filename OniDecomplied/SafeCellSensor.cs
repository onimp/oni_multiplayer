// Decompiled with JetBrains decompiler
// Type: SafeCellSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

public class SafeCellSensor : Sensor
{
  private MinionBrain brain;
  private Navigator navigator;
  private KPrefabID prefabid;
  private Traits traits;
  private int cell = Grid.InvalidCell;

  public SafeCellSensor(Sensors sensors)
    : base(sensors)
  {
    this.navigator = this.GetComponent<Navigator>();
    this.brain = this.GetComponent<MinionBrain>();
    this.prefabid = this.GetComponent<KPrefabID>();
    this.traits = this.GetComponent<Traits>();
  }

  public override void Update()
  {
    if (!this.prefabid.HasTag(GameTags.Idle))
    {
      this.cell = Grid.InvalidCell;
    }
    else
    {
      bool flag1 = this.HasSafeCell();
      this.RunSafeCellQuery(false);
      bool flag2 = this.HasSafeCell();
      if (flag2 == flag1)
        return;
      if (flag2)
        this.sensors.Trigger(982561777, (object) null);
      else
        this.sensors.Trigger(506919987, (object) null);
    }
  }

  public void RunSafeCellQuery(bool avoid_light)
  {
    MinionPathFinderAbilities currentAbilities = (MinionPathFinderAbilities) this.navigator.GetCurrentAbilities();
    currentAbilities.SetIdleNavMaskEnabled(true);
    SafeCellQuery query = PathFinderQueries.safeCellQuery.Reset(this.brain, avoid_light);
    this.navigator.RunQuery((PathFinderQuery) query);
    currentAbilities.SetIdleNavMaskEnabled(false);
    this.cell = query.GetResultCell();
    if (this.cell != Grid.PosToCell((KMonoBehaviour) this.navigator))
      return;
    this.cell = Grid.InvalidCell;
  }

  public int GetSensorCell() => this.cell;

  public int GetCellQuery()
  {
    if (this.cell == Grid.InvalidCell)
      this.RunSafeCellQuery(false);
    return this.cell;
  }

  public int GetSleepCellQuery()
  {
    if (this.cell == Grid.InvalidCell)
      this.RunSafeCellQuery(!this.traits.HasTrait("NightLight"));
    return this.cell;
  }

  public bool HasSafeCell() => this.cell != Grid.InvalidCell && this.cell != Grid.PosToCell((KMonoBehaviour) this.sensors);
}
