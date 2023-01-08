// Decompiled with JetBrains decompiler
// Type: ResourceRef`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;

[SerializationConfig]
public class ResourceRef<ResourceType> : ISaveLoadable where ResourceType : Resource
{
  [Serialize]
  private ResourceGuid guid;
  private ResourceType resource;

  public ResourceRef(ResourceType resource) => this.Set(resource);

  public ResourceRef()
  {
  }

  public ResourceType Get() => this.resource;

  public void Set(ResourceType resource) => this.resource = resource;

  [System.Runtime.Serialization.OnSerializing]
  private void OnSerializing()
  {
    if ((object) this.resource == null)
      this.guid = (ResourceGuid) null;
    else
      this.guid = ((Resource) (object) this.resource).Guid;
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (!ResourceGuid.op_Inequality(this.guid, (ResourceGuid) null))
      return;
    this.resource = Db.Get().GetResource<ResourceType>(this.guid);
    this.guid = (ResourceGuid) null;
  }
}
