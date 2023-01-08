// Decompiled with JetBrains decompiler
// Type: RetireColonyUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public static class RetireColonyUtility
{
  private const int FILE_IO_RETRY_ATTEMPTS = 5;
  private static char[] invalidCharacters = "<>:\"\\/|?*.".ToCharArray();
  private static Encoding[] attempt_encodings = new Encoding[3]
  {
    (Encoding) new UTF8Encoding(false, true),
    (Encoding) new UnicodeEncoding(false, true, true),
    Encoding.ASCII
  };

  public static bool SaveColonySummaryData()
  {
    if (!Directory.Exists(Util.RootFolder()))
      Directory.CreateDirectory(Util.RootFolder());
    string str1 = System.IO.Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
    if (!Directory.Exists(str1))
      Directory.CreateDirectory(str1);
    string path2 = RetireColonyUtility.StripInvalidCharacters(SaveGame.Instance.BaseName);
    string str2 = System.IO.Path.Combine(str1, path2);
    if (!Directory.Exists(str2))
      Directory.CreateDirectory(str2);
    string path = System.IO.Path.Combine(str2, path2 + ".json");
    string s = JsonConvert.SerializeObject((object) RetireColonyUtility.GetCurrentColonyRetiredColonyData());
    if (DlcManager.IsExpansion1Active())
    {
      foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
      {
        if (worldContainer.IsDiscovered && !worldContainer.IsModuleInterior)
        {
          string name = ((Component) worldContainer).GetComponent<ClusterGridEntity>().Name;
          string str3 = System.IO.Path.Combine(str2, name);
          string str4 = System.IO.Path.Combine(str2, worldContainer.id.ToString("D5"));
          if (Directory.Exists(str3))
          {
            bool flag = Directory.GetFiles(str3).Length != 0;
            if (!Directory.Exists(str4))
              Directory.CreateDirectory(str4);
            foreach (string file in Directory.GetFiles(str3))
            {
              try
              {
                File.Copy(file, file.Replace(str3, str4), true);
                File.Delete(file);
              }
              catch (Exception ex)
              {
                flag = false;
                Debug.LogWarning((object) ("Error occurred trying to migrate screenshot: " + file));
                Debug.LogWarning((object) ex);
              }
            }
            if (flag)
              Directory.Delete(str3);
          }
        }
      }
    }
    bool flag1 = false;
    int num = 0;
    while (!flag1)
    {
      if (num < 5)
      {
        try
        {
          Thread.Sleep(num * 100);
          using (FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
          {
            flag1 = true;
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            fileStream.Write(bytes, 0, bytes.Length);
          }
        }
        catch (Exception ex)
        {
          Debug.LogWarningFormat("SaveColonySummaryData failed attempt {0}: {1}", new object[2]
          {
            (object) (num + 1),
            (object) ex.ToString()
          });
        }
        ++num;
      }
      else
        break;
    }
    return flag1;
  }

  public static RetiredColonyData GetCurrentColonyRetiredColonyData()
  {
    List<MinionAssignablesProxy> assignablesProxyList = new List<MinionAssignablesProxy>();
    for (int idx = 0; idx < Components.MinionAssignablesProxy.Count; ++idx)
    {
      if (Object.op_Inequality((Object) Components.MinionAssignablesProxy[idx], (Object) null))
        assignablesProxyList.Add(Components.MinionAssignablesProxy[idx]);
    }
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<string, ColonyAchievementStatus> achievement in ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().achievements)
    {
      if (achievement.Value.success)
        stringList.Add(achievement.Key);
    }
    BuildingComplete[] buildingCompletes = new BuildingComplete[Components.BuildingCompletes.Count];
    for (int idx = 0; idx < buildingCompletes.Length; ++idx)
      buildingCompletes[idx] = Components.BuildingCompletes[idx];
    string startWorld = (string) null;
    Dictionary<string, string> worldIdentities = new Dictionary<string, string>();
    foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
    {
      if (worldContainer.IsDiscovered && !worldContainer.IsModuleInterior)
      {
        worldIdentities.Add(worldContainer.id.ToString("D5"), worldContainer.worldName);
        if (worldContainer.IsStartWorld)
          startWorld = worldContainer.id.ToString("D5");
      }
    }
    return new RetiredColonyData(SaveGame.Instance.BaseName, GameClock.Instance.GetCycle(), System.DateTime.Now.ToShortDateString(), stringList.ToArray(), assignablesProxyList.ToArray(), buildingCompletes, startWorld, worldIdentities);
  }

  private static RetiredColonyData LoadRetiredColony(string file, bool skipStats, Encoding enc)
  {
    RetiredColonyData retiredColonyData = new RetiredColonyData();
    using (FileStream fileStream = File.Open(file, FileMode.Open))
    {
      using (StreamReader streamReader = new StreamReader((Stream) fileStream, enc))
      {
        using (JsonReader jsonReader = (JsonReader) new JsonTextReader((TextReader) streamReader))
        {
          string empty = string.Empty;
          List<string> stringList = new List<string>();
          List<Tuple<string, int>> tupleList1 = new List<Tuple<string, int>>();
          List<RetiredColonyData.RetiredDuplicantData> retiredDuplicantDataList = new List<RetiredColonyData.RetiredDuplicantData>();
          List<RetiredColonyData.RetiredColonyStatistic> retiredColonyStatisticList = new List<RetiredColonyData.RetiredColonyStatistic>();
          Dictionary<string, string> dictionary = new Dictionary<string, string>();
          while (jsonReader.Read())
          {
            JsonToken tokenType = jsonReader.TokenType;
            if (tokenType == 4)
              empty = jsonReader.Value.ToString();
            if (tokenType == 9 && empty == "colonyName")
              retiredColonyData.colonyName = jsonReader.Value.ToString();
            if (tokenType == 9 && empty == "date")
              retiredColonyData.date = jsonReader.Value.ToString();
            if (tokenType == 7 && empty == "cycleCount")
              retiredColonyData.cycleCount = int.Parse(jsonReader.Value.ToString());
            if (tokenType == 9 && empty == "achievements")
              stringList.Add(jsonReader.Value.ToString());
            if (tokenType == 1 && empty == "Duplicants")
            {
              string str1 = (string) null;
              RetiredColonyData.RetiredDuplicantData retiredDuplicantData = new RetiredColonyData.RetiredDuplicantData();
              retiredDuplicantData.accessories = new Dictionary<string, string>();
              while (jsonReader.Read())
              {
                tokenType = jsonReader.TokenType;
                if (tokenType != 13)
                {
                  if (tokenType == 4)
                    str1 = jsonReader.Value.ToString();
                  if (str1 == "name" && tokenType == 9)
                    retiredDuplicantData.name = jsonReader.Value.ToString();
                  if (str1 == "age" && tokenType == 7)
                    retiredDuplicantData.age = int.Parse(jsonReader.Value.ToString());
                  if (str1 == "skillPointsGained" && tokenType == 7)
                    retiredDuplicantData.skillPointsGained = int.Parse(jsonReader.Value.ToString());
                  if (str1 == "accessories")
                  {
                    string key = (string) null;
                    while (jsonReader.Read())
                    {
                      tokenType = jsonReader.TokenType;
                      if (tokenType != 13)
                      {
                        if (tokenType == 4)
                          key = jsonReader.Value.ToString();
                        if (key != null && jsonReader.Value != null && tokenType == 9)
                        {
                          string str2 = jsonReader.Value.ToString();
                          retiredDuplicantData.accessories.Add(key, str2);
                        }
                      }
                      else
                        break;
                    }
                  }
                }
                else
                  break;
              }
              retiredDuplicantDataList.Add(retiredDuplicantData);
            }
            if (tokenType == 1 && empty == "buildings")
            {
              string str3 = (string) null;
              string str4 = (string) null;
              int num = 0;
              while (jsonReader.Read())
              {
                tokenType = jsonReader.TokenType;
                if (tokenType != 13)
                {
                  if (tokenType == 4)
                    str3 = jsonReader.Value.ToString();
                  if (str3 == "first" && tokenType == 9)
                    str4 = jsonReader.Value.ToString();
                  if (str3 == "second" && tokenType == 7)
                    num = int.Parse(jsonReader.Value.ToString());
                }
                else
                  break;
              }
              Tuple<string, int> tuple = new Tuple<string, int>(str4, num);
              tupleList1.Add(tuple);
            }
            if (tokenType == 1 && empty == "Stats")
            {
              if (!skipStats)
              {
                string str5 = (string) null;
                RetiredColonyData.RetiredColonyStatistic retiredColonyStatistic = new RetiredColonyData.RetiredColonyStatistic();
                List<Tuple<float, float>> tupleList2 = new List<Tuple<float, float>>();
                while (jsonReader.Read())
                {
                  tokenType = jsonReader.TokenType;
                  if (tokenType != 13)
                  {
                    if (tokenType == 4)
                      str5 = jsonReader.Value.ToString();
                    if (str5 == "id" && tokenType == 9)
                      retiredColonyStatistic.id = jsonReader.Value.ToString();
                    if (str5 == "name" && tokenType == 9)
                      retiredColonyStatistic.name = jsonReader.Value.ToString();
                    if (str5 == "nameX" && tokenType == 9)
                      retiredColonyStatistic.nameX = jsonReader.Value.ToString();
                    if (str5 == "nameY" && tokenType == 9)
                      retiredColonyStatistic.nameY = jsonReader.Value.ToString();
                    if (str5 == "value" && tokenType == 1)
                    {
                      string str6 = (string) null;
                      float num1 = 0.0f;
                      float num2 = 0.0f;
                      while (jsonReader.Read())
                      {
                        tokenType = jsonReader.TokenType;
                        if (tokenType != 13)
                        {
                          if (tokenType == 4)
                            str6 = jsonReader.Value.ToString();
                          if (str6 == "first" && (tokenType == 8 || tokenType == 7))
                            num1 = float.Parse(jsonReader.Value.ToString());
                          if (str6 == "second" && (tokenType == 8 || tokenType == 7))
                            num2 = float.Parse(jsonReader.Value.ToString());
                        }
                        else
                          break;
                      }
                      Tuple<float, float> tuple = new Tuple<float, float>(num1, num2);
                      tupleList2.Add(tuple);
                    }
                  }
                  else
                    break;
                }
                retiredColonyStatistic.value = tupleList2.ToArray();
                retiredColonyStatisticList.Add(retiredColonyStatistic);
              }
              else
                break;
            }
            if (tokenType == 1 && empty == "worldIdentities")
            {
              string key = (string) null;
              while (jsonReader.Read())
              {
                tokenType = jsonReader.TokenType;
                if (tokenType != 13)
                {
                  if (tokenType == 4)
                    key = jsonReader.Value.ToString();
                  if (key != null && jsonReader.Value != null && tokenType == 9)
                  {
                    string str = jsonReader.Value.ToString();
                    dictionary.Add(key, str);
                  }
                }
                else
                  break;
              }
            }
            if (tokenType == 9 && empty == "startWorld")
              retiredColonyData.startWorld = jsonReader.Value.ToString();
          }
          retiredColonyData.Duplicants = retiredDuplicantDataList.ToArray();
          retiredColonyData.Stats = retiredColonyStatisticList.ToArray();
          retiredColonyData.achievements = stringList.ToArray();
          retiredColonyData.buildings = tupleList1;
          retiredColonyData.worldIdentities = dictionary;
        }
      }
    }
    return retiredColonyData;
  }

  public static RetiredColonyData[] LoadRetiredColonies(bool skipStats = false)
  {
    List<RetiredColonyData> retiredColonyDataList = new List<RetiredColonyData>();
    if (!Directory.Exists(Util.RootFolder()))
      Directory.CreateDirectory(Util.RootFolder());
    string path = System.IO.Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName());
    if (!Directory.Exists(path))
      Directory.CreateDirectory(path);
    foreach (string directory in Directory.GetDirectories(System.IO.Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName())))
    {
      foreach (string file in Directory.GetFiles(directory))
      {
        if (file.EndsWith(".json"))
        {
          for (int index = 0; index < RetireColonyUtility.attempt_encodings.Length; ++index)
          {
            Encoding attemptEncoding = RetireColonyUtility.attempt_encodings[index];
            try
            {
              RetiredColonyData retiredColonyData = RetireColonyUtility.LoadRetiredColony(file, skipStats, attemptEncoding);
              if (retiredColonyData != null)
              {
                if (retiredColonyData.colonyName == null)
                  throw new Exception("data.colonyName was null");
                retiredColonyDataList.Add(retiredColonyData);
                break;
              }
              break;
            }
            catch (Exception ex)
            {
              Debug.LogWarningFormat("LoadRetiredColonies failed load {0} [{1}]: {2}", new object[3]
              {
                (object) attemptEncoding,
                (object) file,
                (object) ex.ToString()
              });
            }
          }
        }
      }
    }
    return retiredColonyDataList.ToArray();
  }

  public static string[] LoadColonySlideshowFiles(string colonyName, string world_name)
  {
    string path2 = RetireColonyUtility.StripInvalidCharacters(colonyName);
    string str = System.IO.Path.Combine(System.IO.Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), path2);
    if (!Util.IsNullOrWhiteSpace(world_name))
      str = System.IO.Path.Combine(str, world_name);
    List<string> stringList = new List<string>();
    if (Directory.Exists(str))
    {
      foreach (string file in Directory.GetFiles(str))
      {
        if (file.EndsWith(".png"))
          stringList.Add(file);
      }
    }
    else
      Debug.LogWarningFormat("LoadColonySlideshow path does not exist or is not directory [{0}]", new object[1]
      {
        (object) str
      });
    return stringList.ToArray();
  }

  public static Sprite[] LoadColonySlideshow(string colonyName)
  {
    string path2 = RetireColonyUtility.StripInvalidCharacters(colonyName);
    string path = System.IO.Path.Combine(System.IO.Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), path2);
    List<Sprite> spriteList = new List<Sprite>();
    if (Directory.Exists(path))
    {
      foreach (string file in Directory.GetFiles(path))
      {
        if (file.EndsWith(".png"))
        {
          Texture2D texture2D = new Texture2D(512, 768);
          ((Texture) texture2D).filterMode = (FilterMode) 0;
          ImageConversion.LoadImage(texture2D, File.ReadAllBytes(file));
          spriteList.Add(Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2((float) ((Texture) texture2D).width, (float) ((Texture) texture2D).height)), new Vector2(0.5f, 0.5f), 100f, 0U, (SpriteMeshType) 0));
        }
      }
    }
    else
      Debug.LogWarningFormat("LoadColonySlideshow path does not exist or is not directory [{0}]", new object[1]
      {
        (object) path
      });
    return spriteList.ToArray();
  }

  public static Sprite LoadRetiredColonyPreview(string colonyName, string startName = null)
  {
    try
    {
      string path2 = RetireColonyUtility.StripInvalidCharacters(colonyName);
      string str = System.IO.Path.Combine(System.IO.Path.Combine(Util.RootFolder(), Util.GetRetiredColoniesFolderName()), path2);
      if (!Util.IsNullOrWhiteSpace(startName))
        str = System.IO.Path.Combine(str, startName);
      List<string> stringList = new List<string>();
      if (Directory.Exists(str))
      {
        foreach (string file in Directory.GetFiles(str))
        {
          if (file.EndsWith(".png"))
            stringList.Add(file);
        }
      }
      if (stringList.Count > 0)
      {
        Texture2D texture2D = new Texture2D(512, 768);
        string path = stringList[stringList.Count - 1];
        return !ImageConversion.LoadImage(texture2D, File.ReadAllBytes(path)) || ((Texture) texture2D).width > SystemInfo.maxTextureSize || ((Texture) texture2D).height > SystemInfo.maxTextureSize || ((Texture) texture2D).width == 0 || ((Texture) texture2D).height == 0 ? (Sprite) null : Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2((float) ((Texture) texture2D).width, (float) ((Texture) texture2D).height)), new Vector2(0.5f, 0.5f), 100f, 0U, (SpriteMeshType) 0);
      }
    }
    catch (Exception ex)
    {
      Debug.Log((object) ("Loading timelapse preview failed! reason: " + ex.Message));
    }
    return (Sprite) null;
  }

  public static Sprite LoadColonyPreview(
    string savePath,
    string colonyName,
    bool fallbackToTimelapse = false)
  {
    string path = System.IO.Path.ChangeExtension(savePath, ".png");
    if (File.Exists(path))
    {
      try
      {
        Texture2D texture2D = new Texture2D(512, 768);
        return !ImageConversion.LoadImage(texture2D, File.ReadAllBytes(path)) || ((Texture) texture2D).width > SystemInfo.maxTextureSize || ((Texture) texture2D).height > SystemInfo.maxTextureSize || ((Texture) texture2D).width == 0 || ((Texture) texture2D).height == 0 ? (Sprite) null : Sprite.Create(texture2D, new Rect(Vector2.zero, new Vector2((float) ((Texture) texture2D).width, (float) ((Texture) texture2D).height)), new Vector2(0.5f, 0.5f), 100f, 0U, (SpriteMeshType) 0);
      }
      catch (Exception ex)
      {
        Debug.Log((object) ("failed to load preview image!? " + ex?.ToString()));
      }
    }
    if (!fallbackToTimelapse)
      return (Sprite) null;
    try
    {
      return RetireColonyUtility.LoadRetiredColonyPreview(colonyName);
    }
    catch (Exception ex)
    {
      Debug.Log((object) string.Format("failed to load fallback timelapse image!? {0}", (object) ex));
    }
    return (Sprite) null;
  }

  public static string StripInvalidCharacters(string source)
  {
    foreach (char invalidCharacter in RetireColonyUtility.invalidCharacters)
      source = source.Replace(invalidCharacter, '_');
    source = source.Trim();
    return source;
  }
}
