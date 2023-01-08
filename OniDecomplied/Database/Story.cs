// Decompiled with JetBrains decompiler
// Type: Database.Story
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using System;

namespace Database
{
  public class Story : Resource, IComparable<Story>
  {
    public const int MODDED_STORY = -1;
    public int kleiUseOnlyCoordinateOffset;
    public bool autoStart;
    public string keepsakePrefabId;
    public readonly string worldgenStoryTraitKey;
    private readonly int displayOrder;
    private readonly int updateNumber;
    private WorldTrait _cachedStoryTrait;

    public int HashId { get; private set; }

    public WorldTrait StoryTrait
    {
      get
      {
        if (this._cachedStoryTrait == null)
          this._cachedStoryTrait = SettingsCache.GetCachedStoryTrait(this.worldgenStoryTraitKey, false);
        return this._cachedStoryTrait;
      }
    }

    public Story(string id, string worldgenStoryTraitKey, int displayOrder)
    {
      this.Id = id;
      this.worldgenStoryTraitKey = worldgenStoryTraitKey;
      this.displayOrder = displayOrder;
      this.kleiUseOnlyCoordinateOffset = -1;
      this.updateNumber = -1;
      this.HashId = Hash.SDBMLower(id);
    }

    public Story(
      string id,
      string worldgenStoryTraitKey,
      int displayOrder,
      int kleiUseOnlyCoordinateOffset,
      int updateNumber)
    {
      this.Id = id;
      this.worldgenStoryTraitKey = worldgenStoryTraitKey;
      this.displayOrder = displayOrder;
      this.updateNumber = updateNumber;
      DebugUtil.Assert(kleiUseOnlyCoordinateOffset < 20, "More than 19 stories is unsupported!");
      this.kleiUseOnlyCoordinateOffset = kleiUseOnlyCoordinateOffset;
      this.HashId = Hash.SDBMLower(id);
    }

    public int CompareTo(Story other) => this.displayOrder.CompareTo(other.displayOrder);

    public bool IsNew() => this.updateNumber == LaunchInitializer.UpdateNumber();

    public Story AutoStart()
    {
      this.autoStart = true;
      return this;
    }

    public Story SetKeepsake(string prefabId)
    {
      this.keepsakePrefabId = prefabId;
      return this;
    }
  }
}
