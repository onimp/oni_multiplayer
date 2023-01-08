// Decompiled with JetBrains decompiler
// Type: ClothingOutfitUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class ClothingOutfitUtility
{
  private static string outfitfile = "OutfitUserData.json";

  public static string GetName(this ClothingOutfitUtility.OutfitType self)
  {
    if (self == ClothingOutfitUtility.OutfitType.Clothing)
      return (string) UI.MINION_BROWSER_SCREEN.OUTFIT_TYPE_CLOTHING;
    DebugUtil.DevAssert(false, string.Format("Couldn't find name for outfit type: {0}", (object) self), (Object) null);
    return self.ToString();
  }

  public static bool SaveClothingOutfitData()
  {
    if (!Directory.Exists(Util.RootFolder()))
      Directory.CreateDirectory(Util.RootFolder());
    string str = System.IO.Path.Combine(Util.RootFolder(), Util.GetKleiItemUserDataFolderName());
    if (!Directory.Exists(str))
      Directory.CreateDirectory(str);
    string path = System.IO.Path.Combine(str, ClothingOutfitUtility.outfitfile);
    string s = JsonConvert.SerializeObject((object) CustomClothingOutfits.Instance.OutfitData);
    bool flag = false;
    try
    {
      using (FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
      {
        flag = true;
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        fileStream.Write(bytes, 0, bytes.Length);
      }
    }
    catch (Exception ex)
    {
      Debug.LogWarningFormat("SaveClothingOutfitData failed", Array.Empty<object>());
    }
    return flag;
  }

  public static void LoadClothingOutfitData(ClothingOutfits dbClothingOutfits)
  {
    string path = System.IO.Path.Combine(Util.RootFolder(), Util.GetKleiItemUserDataFolderName(), ClothingOutfitUtility.outfitfile);
    if (!File.Exists(path))
      return;
    using (FileStream fileStream = File.Open(path, FileMode.Open))
    {
      using (StreamReader streamReader = new StreamReader((Stream) fileStream, (Encoding) new UTF8Encoding(false, true)))
      {
        using (JsonReader jsonReader = (JsonReader) new JsonTextReader((TextReader) streamReader))
        {
          string str1 = (string) null;
          string str2 = "DuplicantOutfits";
          string str3 = "CustomOutfits";
          while (jsonReader.Read())
          {
            JsonToken tokenType1 = jsonReader.TokenType;
            if (tokenType1 == 4)
              str1 = jsonReader.Value.ToString();
            if (tokenType1 == 1 && str1 == str2)
            {
              ClothingOutfitUtility.OutfitType result = ClothingOutfitUtility.OutfitType.LENGTH;
              while (jsonReader.Read())
              {
                JsonToken tokenType2 = jsonReader.TokenType;
                if (tokenType2 != 13)
                {
                  if (tokenType2 == 4)
                  {
                    string key = jsonReader.Value.ToString();
                    while (jsonReader.Read())
                    {
                      JsonToken tokenType3 = jsonReader.TokenType;
                      if (tokenType3 != 13)
                      {
                        if (tokenType3 == 4)
                        {
                          Enum.TryParse<ClothingOutfitUtility.OutfitType>(jsonReader.Value.ToString(), out result);
                          while (jsonReader.Read())
                          {
                            if (jsonReader.TokenType == 9)
                            {
                              string str4 = jsonReader.Value.ToString();
                              if (result != ClothingOutfitUtility.OutfitType.LENGTH)
                              {
                                if (!CustomClothingOutfits.Instance.OutfitData.DuplicantOutfits.ContainsKey(key))
                                  CustomClothingOutfits.Instance.OutfitData.DuplicantOutfits.Add(key, new Dictionary<ClothingOutfitUtility.OutfitType, string>());
                                CustomClothingOutfits.Instance.OutfitData.DuplicantOutfits[key][result] = str4;
                                break;
                              }
                              break;
                            }
                          }
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
            }
            else if (str1 == str3)
            {
              string key = (string) null;
              while (jsonReader.Read())
              {
                JsonToken tokenType4 = jsonReader.TokenType;
                if (tokenType4 != 13)
                {
                  if (tokenType4 == 4)
                    key = jsonReader.Value.ToString();
                  if (tokenType4 == 2)
                  {
                    JArray jarray = JArray.Load(jsonReader);
                    if (jarray != null)
                    {
                      string[] strArray = new string[((JContainer) jarray).Count];
                      for (int index = 0; index < ((JContainer) jarray).Count; ++index)
                        strArray[index] = ((object) jarray[index]).ToString();
                      if (key != null)
                        CustomClothingOutfits.Instance.OutfitData.CustomOutfits[key] = strArray;
                    }
                  }
                }
                else
                  break;
              }
            }
          }
        }
      }
    }
    foreach (KeyValuePair<string, string[]> customOutfit in CustomClothingOutfits.Instance.OutfitData.CustomOutfits)
    {
      if (dbClothingOutfits.TryGet(customOutfit.Key) != null)
        Debug.LogError((object) ("User outfit data is trying to overwrite default " + customOutfit.Key));
    }
    foreach (KeyValuePair<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> duplicantOutfit in CustomClothingOutfits.Instance.OutfitData.DuplicantOutfits)
    {
      Personality fromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(duplicantOutfit.Key);
      if (Util.IsNullOrDestroyed((object) fromNameStringKey))
      {
        DebugUtil.DevAssert(false, "<Loadings Outfit Error> Couldn't find personality \"" + duplicantOutfit.Key + "\" to apply outfit preferences", (Object) null);
      }
      else
      {
        foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, string> keyValuePair in duplicantOutfit.Value)
          fromNameStringKey.SetOutfit(keyValuePair.Key, (Option<string>) keyValuePair.Value);
      }
    }
  }

  public enum OutfitType
  {
    Clothing,
    LENGTH,
  }
}
