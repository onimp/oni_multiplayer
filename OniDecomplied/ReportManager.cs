// Decompiled with JetBrains decompiler
// Type: ReportManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ReportManager")]
public class ReportManager : KMonoBehaviour
{
  [MyCmpAdd]
  private Notifier notifier;
  private ReportManager.NoteStorage noteStorage;
  public Dictionary<ReportManager.ReportType, ReportManager.ReportGroup> ReportGroups = new Dictionary<ReportManager.ReportType, ReportManager.ReportGroup>()
  {
    {
      ReportManager.ReportType.DuplicantHeader,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) null, true, 1, (string) UI.ENDOFDAYREPORT.DUPLICANT_DETAILS_HEADER, "", "", is_header: true)
    },
    {
      ReportManager.ReportType.CaloriesCreated,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) (v => GameUtil.GetFormattedCalories(v)), true, 1, (string) UI.ENDOFDAYREPORT.CALORIES_CREATED.NAME, (string) UI.ENDOFDAYREPORT.CALORIES_CREATED.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.CALORIES_CREATED.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.StressDelta,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) (v => GameUtil.GetFormattedPercent(v)), true, 1, (string) UI.ENDOFDAYREPORT.STRESS_DELTA.NAME, (string) UI.ENDOFDAYREPORT.STRESS_DELTA.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.STRESS_DELTA.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.DiseaseAdded,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) null, false, 1, (string) UI.ENDOFDAYREPORT.DISEASE_ADDED.NAME, (string) UI.ENDOFDAYREPORT.DISEASE_ADDED.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.DISEASE_ADDED.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.DiseaseStatus,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) (v => GameUtil.GetFormattedDiseaseAmount((int) v)), true, 1, (string) UI.ENDOFDAYREPORT.DISEASE_STATUS.NAME, (string) UI.ENDOFDAYREPORT.DISEASE_STATUS.TOOLTIP, (string) UI.ENDOFDAYREPORT.DISEASE_STATUS.TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.LevelUp,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) null, false, 1, (string) UI.ENDOFDAYREPORT.LEVEL_UP.NAME, (string) UI.ENDOFDAYREPORT.LEVEL_UP.TOOLTIP, (string) UI.ENDOFDAYREPORT.NONE, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.ToiletIncident,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) null, false, 1, (string) UI.ENDOFDAYREPORT.TOILET_INCIDENT.NAME, (string) UI.ENDOFDAYREPORT.TOILET_INCIDENT.TOOLTIP, (string) UI.ENDOFDAYREPORT.TOILET_INCIDENT.TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.ChoreStatus,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) null, true, 1, (string) UI.ENDOFDAYREPORT.CHORE_STATUS.NAME, (string) UI.ENDOFDAYREPORT.CHORE_STATUS.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.CHORE_STATUS.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.DomesticatedCritters,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) null, true, 1, (string) UI.ENDOFDAYREPORT.NUMBER_OF_DOMESTICATED_CRITTERS.NAME, (string) UI.ENDOFDAYREPORT.NUMBER_OF_DOMESTICATED_CRITTERS.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.NUMBER_OF_DOMESTICATED_CRITTERS.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.WildCritters,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) null, true, 1, (string) UI.ENDOFDAYREPORT.NUMBER_OF_WILD_CRITTERS.NAME, (string) UI.ENDOFDAYREPORT.NUMBER_OF_WILD_CRITTERS.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.NUMBER_OF_WILD_CRITTERS.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.RocketsInFlight,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) null, true, 1, (string) UI.ENDOFDAYREPORT.ROCKETS_IN_FLIGHT.NAME, (string) UI.ENDOFDAYREPORT.ROCKETS_IN_FLIGHT.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.ROCKETS_IN_FLIGHT.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.TimeSpentHeader,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) null, true, 2, (string) UI.ENDOFDAYREPORT.TIME_DETAILS_HEADER, "", "", is_header: true)
    },
    {
      ReportManager.ReportType.WorkTime,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) (v => GameUtil.GetFormattedPercent((float) ((double) v / 600.0 * 100.0))), true, 2, (string) UI.ENDOFDAYREPORT.WORK_TIME.NAME, (string) UI.ENDOFDAYREPORT.WORK_TIME.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.NONE, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending, group_format_fn: ((ReportManager.GroupFormattingFn) ((v, num_entries) => GameUtil.GetFormattedPercent((float) ((double) v / 600.0 * 100.0) / num_entries))))
    },
    {
      ReportManager.ReportType.TravelTime,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) (v => GameUtil.GetFormattedPercent((float) ((double) v / 600.0 * 100.0))), true, 2, (string) UI.ENDOFDAYREPORT.TRAVEL_TIME.NAME, (string) UI.ENDOFDAYREPORT.TRAVEL_TIME.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.NONE, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending, group_format_fn: ((ReportManager.GroupFormattingFn) ((v, num_entries) => GameUtil.GetFormattedPercent((float) ((double) v / 600.0 * 100.0) / num_entries))))
    },
    {
      ReportManager.ReportType.PersonalTime,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) (v => GameUtil.GetFormattedPercent((float) ((double) v / 600.0 * 100.0))), true, 2, (string) UI.ENDOFDAYREPORT.PERSONAL_TIME.NAME, (string) UI.ENDOFDAYREPORT.PERSONAL_TIME.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.NONE, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending, group_format_fn: ((ReportManager.GroupFormattingFn) ((v, num_entries) => GameUtil.GetFormattedPercent((float) ((double) v / 600.0 * 100.0) / num_entries))))
    },
    {
      ReportManager.ReportType.IdleTime,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) (v => GameUtil.GetFormattedPercent((float) ((double) v / 600.0 * 100.0))), true, 2, (string) UI.ENDOFDAYREPORT.IDLE_TIME.NAME, (string) UI.ENDOFDAYREPORT.IDLE_TIME.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.NONE, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending, group_format_fn: ((ReportManager.GroupFormattingFn) ((v, num_entries) => GameUtil.GetFormattedPercent((float) ((double) v / 600.0 * 100.0) / num_entries))))
    },
    {
      ReportManager.ReportType.BaseHeader,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) null, true, 3, (string) UI.ENDOFDAYREPORT.BASE_DETAILS_HEADER, "", "", is_header: true)
    },
    {
      ReportManager.ReportType.OxygenCreated,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) (v => GameUtil.GetFormattedMass(v)), true, 3, (string) UI.ENDOFDAYREPORT.OXYGEN_CREATED.NAME, (string) UI.ENDOFDAYREPORT.OXYGEN_CREATED.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.OXYGEN_CREATED.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.EnergyCreated,
      new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedRoundedJoules), true, 3, (string) UI.ENDOFDAYREPORT.ENERGY_USAGE.NAME, (string) UI.ENDOFDAYREPORT.ENERGY_USAGE.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.ENERGY_USAGE.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.EnergyWasted,
      new ReportManager.ReportGroup(new ReportManager.FormattingFn(GameUtil.GetFormattedRoundedJoules), true, 3, (string) UI.ENDOFDAYREPORT.ENERGY_WASTED.NAME, (string) UI.ENDOFDAYREPORT.NONE, (string) UI.ENDOFDAYREPORT.ENERGY_WASTED.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.ContaminatedOxygenToilet,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) (v => GameUtil.GetFormattedMass(v)), false, 3, (string) UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.NAME, (string) UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    },
    {
      ReportManager.ReportType.ContaminatedOxygenSublimation,
      new ReportManager.ReportGroup((ReportManager.FormattingFn) (v => GameUtil.GetFormattedMass(v)), false, 3, (string) UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.NAME, (string) UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.POSITIVE_TOOLTIP, (string) UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.NEGATIVE_TOOLTIP, ReportManager.ReportEntry.Order.Descending, ReportManager.ReportEntry.Order.Descending)
    }
  };
  [Serialize]
  private List<ReportManager.DailyReport> dailyReports = new List<ReportManager.DailyReport>();
  [Serialize]
  private ReportManager.DailyReport todaysReport;
  [Serialize]
  private byte[] noteStorageBytes;

  public List<ReportManager.DailyReport> reports => this.dailyReports;

  public static void DestroyInstance() => ReportManager.Instance = (ReportManager) null;

  public static ReportManager Instance { get; private set; }

  public ReportManager.DailyReport TodaysReport => this.todaysReport;

  public ReportManager.DailyReport YesterdaysReport => this.dailyReports.Count <= 1 ? (ReportManager.DailyReport) null : this.dailyReports[this.dailyReports.Count - 1];

  protected virtual void OnPrefabInit()
  {
    ReportManager.Instance = this;
    this.Subscribe(((Component) Game.Instance).gameObject, -1917495436, new Action<object>(this.OnSaveGameReady));
    this.noteStorage = new ReportManager.NoteStorage();
  }

  protected virtual void OnCleanUp() => ReportManager.Instance = (ReportManager) null;

  [KSerialization.CustomSerialize]
  private void CustomSerialize(BinaryWriter writer)
  {
    writer.Write(0);
    this.noteStorage.Serialize(writer);
  }

  [KSerialization.CustomDeserialize]
  private void CustomDeserialize(IReader reader)
  {
    if (this.noteStorageBytes != null)
      return;
    Debug.Assert(reader.ReadInt32() == 0);
    BinaryReader reader1 = new BinaryReader((Stream) new MemoryStream(reader.RawBytes()));
    reader1.BaseStream.Position = (long) reader.Position;
    this.noteStorage.Deserialize(reader1);
    reader.SkipBytes((int) reader1.BaseStream.Position - reader.Position);
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (this.noteStorageBytes == null)
      return;
    this.noteStorage.Deserialize(new BinaryReader((Stream) new MemoryStream(this.noteStorageBytes)));
    this.noteStorageBytes = (byte[]) null;
  }

  private void OnSaveGameReady(object data)
  {
    this.Subscribe(((Component) GameClock.Instance).gameObject, -722330267, new Action<object>(this.OnNightTime));
    if (this.todaysReport != null)
      return;
    this.todaysReport = new ReportManager.DailyReport(this);
    this.todaysReport.day = GameUtil.GetCurrentCycle();
  }

  public void ReportValue(
    ReportManager.ReportType reportType,
    float value,
    string note = null,
    string context = null)
  {
    this.TodaysReport.AddData(reportType, value, note, context);
  }

  private void OnNightTime(object data)
  {
    this.dailyReports.Add(this.todaysReport);
    int day = this.todaysReport.day;
    ManagementMenuNotification menuNotification = new ManagementMenuNotification((Action) 114, NotificationValence.Good, (string) null, string.Format((string) UI.ENDOFDAYREPORT.NOTIFICATION_TITLE, (object) day), NotificationType.Good, (Func<List<Notification>, object, string>) ((n, d) => string.Format((string) UI.ENDOFDAYREPORT.NOTIFICATION_TOOLTIP, (object) day)), custom_click_callback: ((Notification.ClickCallback) (d => ManagementMenu.Instance.OpenReports(day))));
    if (Object.op_Equality((Object) this.notifier, (Object) null))
      Debug.LogError((object) "Cant notify, null notifier");
    else
      this.notifier.Add((Notification) menuNotification);
    this.todaysReport = new ReportManager.DailyReport(this);
    this.todaysReport.day = GameUtil.GetCurrentCycle() + 1;
  }

  public ReportManager.DailyReport FindReport(int day)
  {
    foreach (ReportManager.DailyReport dailyReport in this.dailyReports)
    {
      if (dailyReport.day == day)
        return dailyReport;
    }
    return this.todaysReport.day == day ? this.todaysReport : (ReportManager.DailyReport) null;
  }

  public delegate string FormattingFn(float v);

  public delegate string GroupFormattingFn(float v, float numEntries);

  public enum ReportType
  {
    DuplicantHeader,
    CaloriesCreated,
    StressDelta,
    LevelUp,
    DiseaseStatus,
    DiseaseAdded,
    ToiletIncident,
    ChoreStatus,
    TimeSpentHeader,
    TimeSpent,
    WorkTime,
    TravelTime,
    PersonalTime,
    IdleTime,
    BaseHeader,
    ContaminatedOxygenFlatulence,
    ContaminatedOxygenToilet,
    ContaminatedOxygenSublimation,
    OxygenCreated,
    EnergyCreated,
    EnergyWasted,
    DomesticatedCritters,
    WildCritters,
    RocketsInFlight,
  }

  public struct ReportGroup
  {
    public ReportManager.FormattingFn formatfn;
    public ReportManager.GroupFormattingFn groupFormatfn;
    public string stringKey;
    public string positiveTooltip;
    public string negativeTooltip;
    public bool reportIfZero;
    public int group;
    public bool isHeader;
    public ReportManager.ReportEntry.Order posNoteOrder;
    public ReportManager.ReportEntry.Order negNoteOrder;

    public ReportGroup(
      ReportManager.FormattingFn formatfn,
      bool reportIfZero,
      int group,
      string stringKey,
      string positiveTooltip,
      string negativeTooltip,
      ReportManager.ReportEntry.Order pos_note_order = ReportManager.ReportEntry.Order.Unordered,
      ReportManager.ReportEntry.Order neg_note_order = ReportManager.ReportEntry.Order.Unordered,
      bool is_header = false,
      ReportManager.GroupFormattingFn group_format_fn = null)
    {
      this.formatfn = formatfn != null ? formatfn : (ReportManager.FormattingFn) (v => v.ToString());
      this.groupFormatfn = group_format_fn;
      this.stringKey = stringKey;
      this.positiveTooltip = positiveTooltip;
      this.negativeTooltip = negativeTooltip;
      this.reportIfZero = reportIfZero;
      this.group = group;
      this.posNoteOrder = pos_note_order;
      this.negNoteOrder = neg_note_order;
      this.isHeader = is_header;
    }
  }

  [SerializationConfig]
  public class ReportEntry
  {
    [Serialize]
    public int noteStorageId;
    [Serialize]
    public int gameHash = -1;
    [Serialize]
    public ReportManager.ReportType reportType;
    [Serialize]
    public string context;
    [Serialize]
    public float accumulate;
    [Serialize]
    public float accPositive;
    [Serialize]
    public float accNegative;
    [Serialize]
    public ArrayRef<ReportManager.ReportEntry> contextEntries;
    public bool isChild;

    public ReportEntry(
      ReportManager.ReportType reportType,
      int note_storage_id,
      string context,
      bool is_child = false)
    {
      this.reportType = reportType;
      this.context = context;
      this.isChild = is_child;
      this.accumulate = 0.0f;
      this.accPositive = 0.0f;
      this.accNegative = 0.0f;
      this.noteStorageId = note_storage_id;
    }

    public float Positive => this.accPositive;

    public float Negative => this.accNegative;

    public float Net => this.accPositive + this.accNegative;

    [OnDeserializing]
    private void OnDeserialize() => this.contextEntries.Clear();

    public void IterateNotes(Action<ReportManager.ReportEntry.Note> callback) => ReportManager.Instance.noteStorage.IterateNotes(this.noteStorageId, callback);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized()
    {
      if (this.gameHash == -1)
        return;
      this.reportType = (ReportManager.ReportType) this.gameHash;
      this.gameHash = -1;
    }

    public void AddData(
      ReportManager.NoteStorage note_storage,
      float value,
      string note = null,
      string dataContext = null)
    {
      this.AddActualData(note_storage, value, note);
      if (dataContext == null)
        return;
      ReportManager.ReportEntry reportEntry = (ReportManager.ReportEntry) null;
      for (int index = 0; index < this.contextEntries.Count; ++index)
      {
        if (this.contextEntries[index].context == dataContext)
        {
          reportEntry = this.contextEntries[index];
          break;
        }
      }
      if (reportEntry == null)
      {
        reportEntry = new ReportManager.ReportEntry(this.reportType, note_storage.GetNewNoteId(), dataContext, true);
        this.contextEntries.Add(reportEntry);
      }
      reportEntry.AddActualData(note_storage, value, note);
    }

    private void AddActualData(ReportManager.NoteStorage note_storage, float value, string note = null)
    {
      this.accumulate += value;
      if ((double) value > 0.0)
        this.accPositive += value;
      else
        this.accNegative += value;
      if (note == null)
        return;
      note_storage.Add(this.noteStorageId, value, note);
    }

    public bool HasContextEntries() => this.contextEntries.Count > 0;

    public struct Note
    {
      public float value;
      public string note;

      public Note(float value, string note)
      {
        this.value = value;
        this.note = note;
      }
    }

    public enum Order
    {
      Unordered,
      Ascending,
      Descending,
    }
  }

  public class DailyReport
  {
    [Serialize]
    public int day;
    [Serialize]
    public List<ReportManager.ReportEntry> reportEntries = new List<ReportManager.ReportEntry>();

    public DailyReport(ReportManager manager)
    {
      foreach (KeyValuePair<ReportManager.ReportType, ReportManager.ReportGroup> reportGroup in manager.ReportGroups)
        this.reportEntries.Add(new ReportManager.ReportEntry(reportGroup.Key, this.noteStorage.GetNewNoteId(), (string) null));
    }

    private ReportManager.NoteStorage noteStorage => ReportManager.Instance.noteStorage;

    public ReportManager.ReportEntry GetEntry(ReportManager.ReportType reportType)
    {
      for (int index = 0; index < this.reportEntries.Count; ++index)
      {
        ReportManager.ReportEntry reportEntry = this.reportEntries[index];
        if (reportEntry.reportType == reportType)
          return reportEntry;
      }
      ReportManager.ReportEntry entry = new ReportManager.ReportEntry(reportType, this.noteStorage.GetNewNoteId(), (string) null);
      this.reportEntries.Add(entry);
      return entry;
    }

    public void AddData(
      ReportManager.ReportType reportType,
      float value,
      string note = null,
      string context = null)
    {
      this.GetEntry(reportType).AddData(this.noteStorage, value, note, context);
    }
  }

  public class NoteStorage
  {
    public const int SERIALIZATION_VERSION = 6;
    private int nextNoteId;
    private ReportManager.NoteStorage.NoteEntries noteEntries;
    private ReportManager.NoteStorage.StringTable stringTable;

    public NoteStorage()
    {
      this.noteEntries = new ReportManager.NoteStorage.NoteEntries();
      this.stringTable = new ReportManager.NoteStorage.StringTable();
    }

    public void Add(int report_entry_id, float value, string note)
    {
      int note_id = this.stringTable.AddString(note);
      this.noteEntries.Add(report_entry_id, value, note_id);
    }

    public int GetNewNoteId() => ++this.nextNoteId;

    public void IterateNotes(int report_entry_id, Action<ReportManager.ReportEntry.Note> callback) => this.noteEntries.IterateNotes(this.stringTable, report_entry_id, callback);

    public void Serialize(BinaryWriter writer)
    {
      writer.Write(6);
      writer.Write(this.nextNoteId);
      this.stringTable.Serialize(writer);
      this.noteEntries.Serialize(writer);
    }

    public void Deserialize(BinaryReader reader)
    {
      int version = reader.ReadInt32();
      if (version < 5)
        return;
      this.nextNoteId = reader.ReadInt32();
      this.stringTable.Deserialize(reader, version);
      this.noteEntries.Deserialize(reader, version);
    }

    private class StringTable
    {
      private Dictionary<int, string> strings = new Dictionary<int, string>();

      public int AddString(string str, int version = 6)
      {
        int key = Hash.SDBMLower(str);
        this.strings[key] = str;
        return key;
      }

      public string GetStringByHash(int hash)
      {
        string stringByHash = "";
        this.strings.TryGetValue(hash, out stringByHash);
        return stringByHash;
      }

      public void Serialize(BinaryWriter writer)
      {
        writer.Write(this.strings.Count);
        foreach (KeyValuePair<int, string> keyValuePair in this.strings)
          writer.Write(keyValuePair.Value);
      }

      public void Deserialize(BinaryReader reader, int version)
      {
        int num = reader.ReadInt32();
        for (int index = 0; index < num; ++index)
          this.AddString(reader.ReadString(), version);
      }
    }

    private class NoteEntries
    {
      private static ReportManager.NoteStorage.NoteEntries.NoteEntryKeyComparer sKeyComparer = new ReportManager.NoteStorage.NoteEntries.NoteEntryKeyComparer();
      private Dictionary<int, Dictionary<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float>> entries = new Dictionary<int, Dictionary<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float>>();

      public void Add(int report_entry_id, float value, int note_id)
      {
        Dictionary<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float> dictionary1;
        if (!this.entries.TryGetValue(report_entry_id, out dictionary1))
        {
          dictionary1 = new Dictionary<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float>((IEqualityComparer<ReportManager.NoteStorage.NoteEntries.NoteEntryKey>) ReportManager.NoteStorage.NoteEntries.sKeyComparer);
          this.entries[report_entry_id] = dictionary1;
        }
        ReportManager.NoteStorage.NoteEntries.NoteEntryKey key1 = new ReportManager.NoteStorage.NoteEntries.NoteEntryKey();
        key1.noteHash = note_id;
        key1.isPositive = (double) value > 0.0;
        ReportManager.NoteStorage.NoteEntries.NoteEntryKey key2 = key1;
        if (dictionary1.ContainsKey(key2))
        {
          Dictionary<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float> dictionary2 = dictionary1;
          key1 = key2;
          dictionary2[key1] += value;
        }
        else
          dictionary1[key2] = value;
      }

      public void Serialize(BinaryWriter writer)
      {
        writer.Write(this.entries.Count);
        foreach (KeyValuePair<int, Dictionary<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float>> entry in this.entries)
        {
          writer.Write(entry.Key);
          writer.Write(entry.Value.Count);
          foreach (KeyValuePair<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float> keyValuePair in entry.Value)
          {
            writer.Write(keyValuePair.Key.noteHash);
            writer.Write(keyValuePair.Key.isPositive);
            IOHelper.WriteSingleFast(writer, keyValuePair.Value);
          }
        }
      }

      public void Deserialize(BinaryReader reader, int version)
      {
        if (version < 6)
        {
          OldNoteEntriesV5 oldNoteEntriesV5 = new OldNoteEntriesV5();
          oldNoteEntriesV5.Deserialize(reader);
          foreach (OldNoteEntriesV5.NoteStorageBlock storageBlock in oldNoteEntriesV5.storageBlocks)
          {
            for (int index = 0; index < storageBlock.entryCount; ++index)
            {
              OldNoteEntriesV5.NoteEntry noteEntry = storageBlock.entries.structs[index];
              this.Add(noteEntry.reportEntryId, noteEntry.value, noteEntry.noteHash);
            }
          }
        }
        else
        {
          int capacity1 = reader.ReadInt32();
          this.entries = new Dictionary<int, Dictionary<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float>>(capacity1);
          for (int index1 = 0; index1 < capacity1; ++index1)
          {
            int key1 = reader.ReadInt32();
            int capacity2 = reader.ReadInt32();
            Dictionary<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float> dictionary = new Dictionary<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float>(capacity2, (IEqualityComparer<ReportManager.NoteStorage.NoteEntries.NoteEntryKey>) ReportManager.NoteStorage.NoteEntries.sKeyComparer);
            this.entries[key1] = dictionary;
            for (int index2 = 0; index2 < capacity2; ++index2)
            {
              ReportManager.NoteStorage.NoteEntries.NoteEntryKey key2 = new ReportManager.NoteStorage.NoteEntries.NoteEntryKey()
              {
                noteHash = reader.ReadInt32(),
                isPositive = reader.ReadBoolean()
              };
              dictionary[key2] = reader.ReadSingle();
            }
          }
        }
      }

      public void IterateNotes(
        ReportManager.NoteStorage.StringTable string_table,
        int report_entry_id,
        Action<ReportManager.ReportEntry.Note> callback)
      {
        Dictionary<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float> dictionary;
        if (!this.entries.TryGetValue(report_entry_id, out dictionary))
          return;
        foreach (KeyValuePair<ReportManager.NoteStorage.NoteEntries.NoteEntryKey, float> keyValuePair in dictionary)
        {
          string stringByHash = string_table.GetStringByHash(keyValuePair.Key.noteHash);
          ReportManager.ReportEntry.Note note = new ReportManager.ReportEntry.Note(keyValuePair.Value, stringByHash);
          callback(note);
        }
      }

      public struct NoteEntryKey
      {
        public int noteHash;
        public bool isPositive;
      }

      public class NoteEntryKeyComparer : 
        IEqualityComparer<ReportManager.NoteStorage.NoteEntries.NoteEntryKey>
      {
        public bool Equals(
          ReportManager.NoteStorage.NoteEntries.NoteEntryKey a,
          ReportManager.NoteStorage.NoteEntries.NoteEntryKey b)
        {
          return a.noteHash == b.noteHash && a.isPositive == b.isPositive;
        }

        public int GetHashCode(
          ReportManager.NoteStorage.NoteEntries.NoteEntryKey a)
        {
          return a.noteHash * (a.isPositive ? 1 : -1);
        }
      }
    }
  }
}
