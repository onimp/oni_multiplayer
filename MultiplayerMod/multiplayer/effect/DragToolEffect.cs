using UnityEngine;

namespace MultiplayerMod.multiplayer.effect
{
    public abstract class DragToolEffect
    {
        public void DragTool(PrioritySetting prioritySetting)
        {
            var gameObject = OriginalMethod();

            var component = gameObject.GetComponent<Prioritizable>();
            component.SetMasterPriority(prioritySetting);
        }

        protected abstract GameObject OriginalMethod();
    }
}