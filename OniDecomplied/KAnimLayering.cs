// Decompiled with JetBrains decompiler
// Type: KAnimLayering
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class KAnimLayering
{
  private bool isForeground;
  private KAnimControllerBase controller;
  private KAnimControllerBase foregroundController;
  private KAnimLink link;
  private Grid.SceneLayer layer = Grid.SceneLayer.BuildingFront;
  public static readonly KAnimHashedString UI = new KAnimHashedString("ui");

  public KAnimLayering(KAnimControllerBase controller, Grid.SceneLayer layer)
  {
    this.controller = controller;
    this.layer = layer;
  }

  public void SetLayer(Grid.SceneLayer layer)
  {
    this.layer = layer;
    if (!Object.op_Inequality((Object) this.foregroundController, (Object) null))
      return;
    Vector3 vector3;
    // ISSUE: explicit constructor call
    ((Vector3) ref vector3).\u002Ector(0.0f, 0.0f, (float) ((double) Grid.GetLayerZ(layer) - (double) TransformExtensions.GetPosition(((Component) this.controller).gameObject.transform).z - 0.10000000149011612));
    TransformExtensions.SetLocalPosition(((Component) this.foregroundController).transform, vector3);
  }

  public void SetIsForeground(bool is_foreground) => this.isForeground = is_foreground;

  public bool GetIsForeground() => this.isForeground;

  public KAnimLink GetLink() => this.link;

  private static bool IsAnimLayered(KAnimFile[] anims)
  {
    for (int index = 0; index < anims.Length; ++index)
    {
      KAnimFile anim = anims[index];
      if (!Object.op_Equality((Object) anim, (Object) null))
      {
        KAnimFileData data = anim.GetData();
        if (data.build != null)
        {
          foreach (KAnim.Build.Symbol symbol in data.build.symbols)
          {
            if ((symbol.flags & 8) != 0)
              return true;
          }
        }
      }
    }
    return false;
  }

  private void HideSymbolsInternal()
  {
    foreach (KAnimFile animFile in this.controller.AnimFiles)
    {
      if (!Object.op_Equality((Object) animFile, (Object) null))
      {
        KAnimFileData data = animFile.GetData();
        if (data.build != null)
        {
          KAnim.Build.Symbol[] symbols = data.build.symbols;
          for (int index = 0; index < symbols.Length; ++index)
          {
            if ((symbols[index].flags & 8) != 0 != this.isForeground && !KAnimHashedString.op_Equality(symbols[index].hash, KAnimLayering.UI))
              this.controller.SetSymbolVisiblity(symbols[index].hash, false);
          }
        }
      }
    }
  }

  public void HideSymbols()
  {
    if (Object.op_Equality((Object) EntityPrefabs.Instance, (Object) null) || this.isForeground)
      return;
    KAnimFile[] animFiles = this.controller.AnimFiles;
    bool flag = KAnimLayering.IsAnimLayered(animFiles);
    if (flag && Object.op_Equality((Object) this.foregroundController, (Object) null) && this.layer != Grid.SceneLayer.NoLayer)
    {
      GameObject gameObject = Util.KInstantiate(EntityPrefabs.Instance.ForegroundLayer, ((Component) this.controller).gameObject, (string) null);
      ((Object) gameObject).name = ((Object) this.controller).name + "_fg";
      this.foregroundController = gameObject.GetComponent<KAnimControllerBase>();
      this.foregroundController.AnimFiles = animFiles;
      this.foregroundController.GetLayering().SetIsForeground(true);
      this.foregroundController.initialAnim = this.controller.initialAnim;
      this.link = new KAnimLink(this.controller, this.foregroundController);
      this.Dirty();
      KAnimSynchronizer synchronizer = this.controller.GetSynchronizer();
      synchronizer.Add(this.foregroundController);
      synchronizer.Sync(this.foregroundController);
      Vector3 vector3;
      // ISSUE: explicit constructor call
      ((Vector3) ref vector3).\u002Ector(0.0f, 0.0f, (float) ((double) Grid.GetLayerZ(this.layer) - (double) TransformExtensions.GetPosition(((Component) this.controller).gameObject.transform).z - 0.10000000149011612));
      TransformExtensions.SetLocalPosition(gameObject.transform, vector3);
      gameObject.SetActive(true);
    }
    else if (!flag && Object.op_Inequality((Object) this.foregroundController, (Object) null))
    {
      this.controller.GetSynchronizer().Remove(this.foregroundController);
      TracesExtesions.DeleteObject(((Component) this.foregroundController).gameObject);
      this.link = (KAnimLink) null;
    }
    if (!Object.op_Inequality((Object) this.foregroundController, (Object) null))
      return;
    this.HideSymbolsInternal();
    this.foregroundController.GetLayering()?.HideSymbolsInternal();
  }

  public void Dirty()
  {
    if (Object.op_Equality((Object) this.foregroundController, (Object) null))
      return;
    this.foregroundController.Offset = this.controller.Offset;
    this.foregroundController.Pivot = this.controller.Pivot;
    this.foregroundController.Rotation = this.controller.Rotation;
    this.foregroundController.FlipX = this.controller.FlipX;
    this.foregroundController.FlipY = this.controller.FlipY;
  }
}
