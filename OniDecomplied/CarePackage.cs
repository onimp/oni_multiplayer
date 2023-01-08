// Decompiled with JetBrains decompiler
// Type: CarePackage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using UnityEngine;

public class CarePackage : StateMachineComponent<CarePackage.SMInstance>
{
  [Serialize]
  public CarePackageInfo info;
  private string facadeID;
  private Reactable reactable;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    if (this.info != null)
      this.SetAnimToInfo();
    this.reactable = this.CreateReactable();
  }

  public Reactable CreateReactable() => (Reactable) new EmoteReactable(((Component) this).gameObject, HashedString.op_Implicit("UpgradeFX"), Db.Get().ChoreTypes.Emote).SetEmote(Db.Get().Emotes.Minion.Cheer);

  protected override void OnCleanUp()
  {
    this.reactable.Cleanup();
    base.OnCleanUp();
  }

  public void SetInfo(CarePackageInfo info)
  {
    this.info = info;
    this.SetAnimToInfo();
  }

  public void SetFacade(string facadeID)
  {
    this.facadeID = facadeID;
    this.SetAnimToInfo();
  }

  private void SetAnimToInfo()
  {
    GameObject prefab1 = Util.KInstantiate(Assets.GetPrefab(TagExtensions.ToTag("Meter")), ((Component) this).gameObject, (string) null);
    GameObject prefab2 = Assets.GetPrefab(Tag.op_Implicit(this.info.id));
    KBatchedAnimController component1 = ((Component) this).GetComponent<KBatchedAnimController>();
    KBatchedAnimController component2 = prefab2.GetComponent<KBatchedAnimController>();
    SymbolOverrideController component3 = prefab2.GetComponent<SymbolOverrideController>();
    KBatchedAnimController component4 = prefab1.GetComponent<KBatchedAnimController>();
    TransformExtensions.SetLocalPosition(((Component) component4).transform, Vector3.forward);
    component4.AnimFiles = component2.AnimFiles;
    component4.isMovable = true;
    component4.animWidth = component2.animWidth;
    component4.animHeight = component2.animHeight;
    if (Object.op_Inequality((Object) component3, (Object) null))
    {
      SymbolOverrideController prefab3 = SymbolOverrideControllerUtil.AddToPrefab(prefab1);
      foreach (SymbolOverrideController.SymbolEntry getSymbolOverride in component3.GetSymbolOverrides)
        prefab3.AddSymbolOverride(getSymbolOverride.targetSymbol, getSymbolOverride.sourceSymbol);
    }
    component4.initialAnim = component2.initialAnim;
    component4.initialMode = (KAnim.PlayMode) 0;
    if (!string.IsNullOrEmpty(this.facadeID))
    {
      component4.SwapAnims(new KAnimFile[1]
      {
        Db.GetEquippableFacades().Get(this.facadeID).AnimFile
      });
      ((Component) this).GetComponentsInChildren<KBatchedAnimController>()[1].SetSymbolVisiblity(KAnimHashedString.op_Implicit("object"), false);
    }
    KBatchedAnimTracker component5 = prefab1.GetComponent<KBatchedAnimTracker>();
    component5.controller = component1;
    component5.symbol = new HashedString("snapTO_object");
    component5.offset = new Vector3(0.0f, 0.5f, 0.0f);
    prefab1.SetActive(true);
    component1.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTO_object"), false);
    KAnimLink kanimLink = new KAnimLink((KAnimControllerBase) component1, (KAnimControllerBase) component4);
  }

  private void SpawnContents()
  {
    if (this.info == null)
    {
      Debug.LogWarning((object) "CarePackage has no data to spawn from. Probably a save from before the CarePackage info data was serialized.");
    }
    else
    {
      GameObject gameObject = (GameObject) null;
      GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(this.info.id));
      Element element = ElementLoader.GetElement(TagExtensions.ToTag(this.info.id));
      Vector3 position = Vector3.op_Addition(this.transform.position, Vector3.op_Division(Vector3.up, 2f));
      if (element == null && Object.op_Inequality((Object) prefab, (Object) null))
      {
        for (int index = 0; (double) index < (double) this.info.quantity; ++index)
        {
          gameObject = Util.KInstantiate(prefab, position);
          if (Object.op_Inequality((Object) gameObject, (Object) null))
          {
            if (!Util.IsNullOrWhiteSpace(this.facadeID))
              EquippableFacade.AddFacadeToEquippable(gameObject.GetComponent<Equippable>(), this.facadeID);
            gameObject.SetActive(true);
          }
        }
      }
      else if (element != null)
      {
        float quantity = this.info.quantity;
        gameObject = element.substance.SpawnResource(position, quantity, element.defaultValues.temperature, byte.MaxValue, 0, forceTemperature: true);
      }
      else
        Debug.LogWarning((object) ("Can't find spawnable thing from tag " + this.info.id));
      if (!Object.op_Inequality((Object) gameObject, (Object) null))
        return;
      gameObject.SetActive(true);
    }
  }

  public class SMInstance : 
    GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.GameInstance
  {
    public List<Chore> activeUseChores;

    public SMInstance(CarePackage master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage>
  {
    public StateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.BoolParameter spawnedContents;
    public GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.State spawn;
    public GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.State open;
    public GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.State pst;
    public GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.State destroy;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.spawn;
      this.serializable = StateMachine.SerializeType.ParamsOnly;
      this.spawn.PlayAnim("portalbirth").OnAnimQueueComplete(this.open).ParamTransition<bool>((StateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.Parameter<bool>) this.spawnedContents, this.pst, GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.IsTrue);
      this.open.PlayAnim("portalbirth_pst").QueueAnim("object_idle_loop").Exit((StateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.State.Callback) (smi =>
      {
        smi.master.SpawnContents();
        this.spawnedContents.Set(true, smi);
      })).ScheduleGoTo(1f, (StateMachine.BaseState) this.pst);
      this.pst.PlayAnim("object_idle_pst").ScheduleGoTo(5f, (StateMachine.BaseState) this.destroy);
      this.destroy.Enter((StateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.State.Callback) (smi => Util.KDestroyGameObject(((Component) smi.master).gameObject)));
    }
  }
}
