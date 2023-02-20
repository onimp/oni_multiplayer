using System;
using System.Linq;
using MultiplayerMod.patch;
using Object = UnityEngine.Object;

namespace MultiplayerMod.multiplayer.effect
{

    public static class GoToStateEffect
    {
        public static void GoToState(object[] payload)
        {
            var instanceId = (int)payload[0];
            var stateName = (string)payload[1];
            var instanceType = (Type)payload[2];
            var serverStackTrace = (string)payload[3];
            var prefabID = Object.FindObjectsOfType<KPrefabID>().FirstOrDefault(a => a.InstanceID == instanceId);
            if (prefabID == null)
            {
                Debug.LogWarning($"Multiplayer: KPrefabID not found {instanceId}. Server stack trace {serverStackTrace}");
                return;
            }

            var stateMachineController = prefabID.GetComponent<StateMachineController>();
            if (stateMachineController == null)
            {
                Debug.LogWarning($"Multiplayer: stateMachineController not found {instanceId}. Server stack trace {serverStackTrace}");
                return;
            }
            var stateMachineInstance = stateMachineController.GetSMI(instanceType);
            if (stateMachineInstance == null)
            {
                Debug.LogWarning($"Multiplayer: stateMachineInstance not found {instanceId} {instanceType}. Server stack trace {serverStackTrace}");
                return;
            }
            Debug.Log("Multiplayer: stateMachine OK");
            StateMachinePatch.DisablePatch = true;
            stateMachineInstance.GoTo(stateName);
            StateMachinePatch.DisablePatch = false;
        }
    }

}
