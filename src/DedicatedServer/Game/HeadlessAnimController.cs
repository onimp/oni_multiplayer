using System.Collections.Generic;
using UnityEngine;

namespace DedicatedServer.Game;

/// <summary>
/// No-op stub for KAnimControllerBase in headless mode.
///
/// StandardWorker.StartWork / InternalStopWork / Work all do:
///   GetComponent&lt;KAnimControllerBase&gt;().Offset += ...
///   GetComponent&lt;KAnimControllerBase&gt;().Play(...)
///   GetComponent&lt;KAnimControllerBase&gt;().IsStopped()
/// Without a real KAnimControllerBase component those calls NPE.
///
/// Adding this stub as a component makes GetComponent&lt;KAnimControllerBase&gt;()
/// return a non-null object.  All abstract members are no-ops; field initializers
/// in the base class (overrideAnimFiles, overrideAnims, …) ensure the concrete
/// base methods like AddAnimOverrides / RemoveAnimOverrides don't NPE.
/// </summary>
public class HeadlessAnimController : KAnimControllerBase {

    // All abstract members — implement as no-ops.

    public override KAnim.Anim GetAnim(int index)                                 => default;
    protected override void    SuspendUpdates(bool suspend)                        { }
    protected override void    OnStartQueuedAnim()                                 { }
    public override    void    SetDirty()                                          { }
    protected override void    RefreshVisibilityListener()                         { }
    protected override void    DeRegister()                                        { }
    protected override void    Register()                                          { }
    protected override void    OnAwake()                                           { }
    protected override void    OnStart()                                           { }
    protected override void    OnStop()                                            { }
    protected override void    Enable()                                            { }
    protected override void    Disable()                                           { }
    protected override void    UpdateFrame(float t)                                { }
    public override    Matrix2x3 GetTransformMatrix()                              => default;
    public override    Matrix2x3 GetSymbolLocalTransform(HashedString symbol, out bool symbolVisible) {
        symbolVisible = false;
        return default;
    }
    public override void UpdateAllHiddenSymbols()                                  { }
    public override void UpdateHiddenSymbol(KAnimHashedString specificSymbol)      { }
    public override void UpdateHiddenSymbolSet(HashSet<KAnimHashedString> symbols) { }
    public override void TriggerStop()                                             { }
}
