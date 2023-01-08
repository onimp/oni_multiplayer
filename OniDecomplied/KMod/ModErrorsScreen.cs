// Decompiled with JetBrains decompiler
// Type: KMod.ModErrorsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KMod
{
  public class ModErrorsScreen : KScreen
  {
    [SerializeField]
    private KButton closeButtonTitle;
    [SerializeField]
    private KButton closeButton;
    [SerializeField]
    private GameObject entryPrefab;
    [SerializeField]
    private Transform entryParent;

    public static bool ShowErrors(List<Event> events)
    {
      if (Global.Instance.modManager.events.Count == 0)
        return false;
      ModErrorsScreen modErrorsScreen = Util.KInstantiateUI<ModErrorsScreen>(Global.Instance.modErrorsPrefab, GameObject.Find("Canvas"), false);
      modErrorsScreen.Initialize(events);
      ((Component) modErrorsScreen).gameObject.SetActive(true);
      return true;
    }

    private void Initialize(List<Event> events)
    {
      foreach (Event @event in events)
      {
        HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.entryPrefab, ((Component) this.entryParent).gameObject, true);
        LocText reference1 = hierarchyReferences.GetReference<LocText>("Title");
        LocText reference2 = hierarchyReferences.GetReference<LocText>("Description");
        KButton reference3 = hierarchyReferences.GetReference<KButton>("Details");
        string title;
        string title_tooltip;
        Event.GetUIStrings(@event.event_type, out title, out title_tooltip);
        ((TMP_Text) reference1).text = title;
        ((Component) reference1).GetComponent<ToolTip>().toolTip = title_tooltip;
        ((TMP_Text) reference2).text = @event.mod.title;
        ToolTip component = ((Component) reference2).GetComponent<ToolTip>();
        if (Object.op_Inequality((Object) component, (Object) null))
          component.toolTip = @event.mod.ToString();
        reference3.isInteractable = false;
        Mod mod = Global.Instance.modManager.FindMod(@event.mod);
        if (mod != null)
        {
          if (Object.op_Inequality((Object) component, (Object) null) && !string.IsNullOrEmpty(mod.description))
            component.toolTip = mod.description;
          if (mod.on_managed != null)
          {
            reference3.onClick += mod.on_managed;
            reference3.isInteractable = true;
          }
        }
      }
    }

    protected virtual void OnActivate()
    {
      base.OnActivate();
      this.closeButtonTitle.onClick += new System.Action(((KScreen) this).Deactivate);
      this.closeButton.onClick += new System.Action(((KScreen) this).Deactivate);
    }
  }
}
