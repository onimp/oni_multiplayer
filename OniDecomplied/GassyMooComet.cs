// Decompiled with JetBrains decompiler
// Type: GassyMooComet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class GassyMooComet : Comet
{
  public Vector3 mooSpawnImpactOffset = new Vector3(-0.5f, 0.0f, 0.0f);

  public override void RandomizeVelocity()
  {
    bool flag = false;
    byte id = Grid.WorldIdx[Grid.PosToCell(((Component) this).gameObject.transform.position)];
    WorldContainer world = ClusterManager.Instance.GetWorld((int) id);
    if (Object.op_Equality((Object) world, (Object) null))
      return;
    int num1 = world.WorldOffset.x + world.Width / 2;
    if (Grid.PosToXY(((Component) this).gameObject.transform.position).x > num1)
      flag = true;
    float num2 = (float) ((flag ? -75.0 : (double) byte.MaxValue) * 3.1415927410125732 / 180.0);
    float num3 = Random.Range(this.spawnVelocity.x, this.spawnVelocity.y);
    this.velocity = new Vector2(-Mathf.Cos(num2) * num3, Mathf.Sin(num2) * num3);
    ((Component) this).GetComponent<KBatchedAnimController>().FlipX = flag;
  }

  protected override void SpawnCraterPrefabs()
  {
    KBatchedAnimController animController = ((Component) this).GetComponent<KBatchedAnimController>();
    animController.Play(HashedString.op_Implicit("landing"));
    animController.onAnimComplete += (KAnimControllerBase.KAnimEvent) (obj =>
    {
      if (this.craterPrefabs != null && this.craterPrefabs.Length != 0)
      {
        int cell = Grid.PosToCell((KMonoBehaviour) this);
        if (Grid.IsValidCell(Grid.CellAbove(cell)))
          cell = Grid.CellAbove(cell);
        GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(this.craterPrefabs[Random.Range(0, this.craterPrefabs.Length)])), Grid.CellToPos(cell));
        gameObject.transform.position = new Vector3(((Component) this).gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        Transform transform = gameObject.transform;
        transform.position = Vector3.op_Addition(transform.position, this.mooSpawnImpactOffset);
        gameObject.GetComponent<KBatchedAnimController>().FlipX = animController.FlipX;
        gameObject.SetActive(true);
      }
      Util.KDestroyGameObject(((Component) this).gameObject);
    });
  }
}
