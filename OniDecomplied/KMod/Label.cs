// Decompiled with JetBrains decompiler
// Type: KMod.Label
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace KMod
{
  [JsonObject]
  [DebuggerDisplay("{title}")]
  public struct Label
  {
    public Label.DistributionPlatform distribution_platform;
    public string id;
    public string title;
    public long version;

    [JsonIgnore]
    private string distribution_platform_name => this.distribution_platform.ToString();

    [JsonIgnore]
    public string install_path => FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), this.distribution_platform_name, this.id));

    [JsonIgnore]
    public string defaultStaticID => this.id + "." + this.distribution_platform.ToString();

    public override string ToString() => this.title;

    public bool Match(Label rhs) => this.id == rhs.id && this.distribution_platform == rhs.distribution_platform;

    public enum DistributionPlatform
    {
      Local,
      Steam,
      Epic,
      Rail,
      Dev,
    }
  }
}
