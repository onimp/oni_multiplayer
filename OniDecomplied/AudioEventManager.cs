// Decompiled with JetBrains decompiler
// Type: AudioEventManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AudioEventManager")]
public class AudioEventManager : KMonoBehaviour
{
  public const float NO_NOISE_EFFECTORS = 0.0f;
  public const float MIN_LOUDNESS_THRESHOLD = 1f;
  private static AudioEventManager instance;
  private List<Pair<float, NoiseSplat>> removeTime = new List<Pair<float, NoiseSplat>>();
  private Dictionary<int, List<Polluter>> freePool = new Dictionary<int, List<Polluter>>();
  private Dictionary<int, List<Polluter>> inusePool = new Dictionary<int, List<Polluter>>();
  private HashSet<NoiseSplat> splats = new HashSet<NoiseSplat>();
  private UniformGrid<NoiseSplat> spatialSplats = new UniformGrid<NoiseSplat>();
  private List<AudioEventManager.PolluterDisplay> polluters = new List<AudioEventManager.PolluterDisplay>();

  public static AudioEventManager Get()
  {
    if (Object.op_Equality((Object) AudioEventManager.instance, (Object) null))
    {
      if (App.IsExiting)
        return (AudioEventManager) null;
      GameObject gameObject = GameObject.Find("/AudioEventManager");
      if (Object.op_Equality((Object) gameObject, (Object) null))
      {
        gameObject = new GameObject();
        ((Object) gameObject).name = nameof (AudioEventManager);
      }
      AudioEventManager.instance = gameObject.GetComponent<AudioEventManager>();
      if (Object.op_Equality((Object) AudioEventManager.instance, (Object) null))
        AudioEventManager.instance = gameObject.AddComponent<AudioEventManager>();
    }
    return AudioEventManager.instance;
  }

  protected virtual void OnSpawn()
  {
    this.OnPrefabInit();
    this.spatialSplats.Reset(Grid.WidthInCells, Grid.HeightInCells, 16, 16);
  }

  public static float LoudnessToDB(float loudness) => (double) loudness <= 0.0 ? 0.0f : 10f * Mathf.Log10(loudness);

  public static float DBToLoudness(float src_db) => Mathf.Pow(10f, src_db / 10f);

  public float GetDecibelsAtCell(int cell) => Mathf.Round(AudioEventManager.LoudnessToDB(Grid.Loudness[cell]) * 2f) / 2f;

  public static string GetLoudestNoisePolluterAtCell(int cell)
  {
    float num = float.NegativeInfinity;
    string noisePolluterAtCell = (string) null;
    AudioEventManager audioEventManager = AudioEventManager.Get();
    Vector2I xy = Grid.CellToXY(cell);
    Vector2 vector2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2).\u002Ector((float) xy.x, (float) xy.y);
    foreach (NoiseSplat noiseSplat in audioEventManager.spatialSplats.GetAllIntersecting(vector2))
    {
      if ((double) noiseSplat.GetLoudness(cell) > (double) num)
        noisePolluterAtCell = noiseSplat.GetProvider().GetName();
    }
    return noisePolluterAtCell;
  }

  public void ClearNoiseSplat(NoiseSplat splat)
  {
    if (!this.splats.Contains(splat))
      return;
    this.splats.Remove(splat);
    this.spatialSplats.Remove(splat);
  }

  public void AddSplat(NoiseSplat splat)
  {
    this.splats.Add(splat);
    this.spatialSplats.Add(splat);
  }

  public NoiseSplat CreateNoiseSplat(Vector2 pos, int dB, int radius, string name, GameObject go)
  {
    Polluter polluter = this.GetPolluter(radius);
    polluter.SetAttributes(pos, dB, go, name);
    NoiseSplat new_splat = new NoiseSplat((IPolluter) polluter);
    polluter.SetSplat(new_splat);
    return new_splat;
  }

  public List<AudioEventManager.PolluterDisplay> GetPollutersForCell(int cell)
  {
    this.polluters.Clear();
    Vector2I xy = Grid.CellToXY(cell);
    Vector2 vector2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2).\u002Ector((float) xy.x, (float) xy.y);
    foreach (NoiseSplat noiseSplat in this.spatialSplats.GetAllIntersecting(vector2))
    {
      float loudness = noiseSplat.GetLoudness(cell);
      if ((double) loudness > 0.0)
        this.polluters.Add(new AudioEventManager.PolluterDisplay()
        {
          name = noiseSplat.GetName(),
          value = AudioEventManager.LoudnessToDB(loudness),
          provider = noiseSplat.GetProvider()
        });
    }
    return this.polluters;
  }

  private void RemoveExpiredSplats()
  {
    if (this.removeTime.Count > 1)
      this.removeTime.Sort((Comparison<Pair<float, NoiseSplat>>) ((a, b) => a.first.CompareTo(b.first)));
    int num = -1;
    for (int index = 0; index < this.removeTime.Count && (double) this.removeTime[index].first <= (double) Time.time; ++index)
    {
      NoiseSplat second = this.removeTime[index].second;
      if (second != null)
        this.FreePolluter(second.GetProvider() as Polluter);
      num = index;
    }
    for (int index = num; index >= 0; --index)
      this.removeTime.RemoveAt(index);
  }

  private void Update() => this.RemoveExpiredSplats();

  private Polluter GetPolluter(int radius)
  {
    if (!this.freePool.ContainsKey(radius))
      this.freePool.Add(radius, new List<Polluter>());
    Polluter polluter;
    if (this.freePool[radius].Count > 0)
    {
      polluter = this.freePool[radius][0];
      this.freePool[radius].RemoveAt(0);
    }
    else
      polluter = new Polluter(radius);
    if (!this.inusePool.ContainsKey(radius))
      this.inusePool.Add(radius, new List<Polluter>());
    this.inusePool[radius].Add(polluter);
    return polluter;
  }

  private void FreePolluter(Polluter pol)
  {
    if (pol == null)
      return;
    pol.Clear();
    Debug.Assert(this.inusePool[pol.radius].Contains(pol));
    this.inusePool[pol.radius].Remove(pol);
    this.freePool[pol.radius].Add(pol);
  }

  public void PlayTimedOnceOff(
    Vector2 pos,
    int dB,
    int radius,
    string name,
    GameObject go,
    float time = 1f)
  {
    if (dB <= 0 || radius <= 0 || (double) time <= 0.0)
      return;
    Polluter polluter = this.GetPolluter(radius);
    polluter.SetAttributes(pos, dB, go, name);
    this.AddTimedInstance(polluter, time);
  }

  private void AddTimedInstance(Polluter p, float time)
  {
    NoiseSplat new_splat = new NoiseSplat((IPolluter) p, time + Time.time);
    p.SetSplat(new_splat);
    this.removeTime.Add(new Pair<float, NoiseSplat>(time + Time.time, new_splat));
  }

  private static void SoundLog(long itemId, string message) => Debug.Log((object) (" [" + itemId.ToString() + "] \t" + message));

  public enum NoiseEffect
  {
    Peaceful = 0,
    Quiet = 36, // 0x00000024
    TossAndTurn = 45, // 0x0000002D
    WakeUp = 60, // 0x0000003C
    Passive = 80, // 0x00000050
    Active = 106, // 0x0000006A
  }

  public struct PolluterDisplay
  {
    public string name;
    public float value;
    public IPolluter provider;
  }
}
