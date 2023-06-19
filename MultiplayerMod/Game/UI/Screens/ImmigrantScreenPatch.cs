using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Core.Scheduling;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Screens;

[HarmonyPatch(typeof(ImmigrantScreen))]
public static class ImmigrantScreenPatch {

    private const int delayMS = 1;
    private const int maxWaitMS = 50;

    public static List<ITelepadDeliverable> Deliverables { get; set; }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ImmigrantScreen.Initialize))]
    private static void Initialize(ImmigrantScreen __instance) => PatchControl.RunIfEnabled(
        () => new TaskFactory(Container.Get<UnityTaskScheduler>()).StartNew(
            async () => {
                if (Deliverables == null) return;

                // Wait until default initialize is complete
                await WaitForAllDeliverablesReady(__instance);
                // Create correct containers.
                InitializeContainers(ImmigrantScreen.instance);
                // Wait until those containers are initialized with random data.
                await WaitForAllDeliverablesReady(__instance);

                SetDeliverablesData(__instance);
            }
        )
    );

    public static async Task<List<ITelepadDeliverable>> WaitForAllDeliverablesReady(ImmigrantScreen instance) {
        var currentDelay = 0;
        while (currentDelay < maxWaitMS) {
            var readyDeliverables = instance.containers?.Select(
                a => a switch {
                    CharacterContainer container => (ITelepadDeliverable) container.stats,
                    CarePackageContainer packageContainer => packageContainer.carePackageInstanceData,
                    _ => null
                }
            ).Where(a => a != null).ToList();
            if (readyDeliverables != null && readyDeliverables.Count == instance.containers.Count)
                return readyDeliverables;

            await Task.Delay(delayMS);
            currentDelay += delayMS;
        }
        return null;
    }

    private static void InitializeContainers(CharacterSelectionController screen) {
        screen.OnReplacedEvent = null;
        screen.containers?.ForEach(cc => Object.Destroy(cc.GetGameObject()));
        screen.containers?.Clear();
        screen.containers = new List<ITelepadDeliverableContainer>();

        screen.numberOfCarePackageOptions = 0;
        screen.numberOfDuplicantOptions = 0;
        screen.selectedDeliverables = new List<ITelepadDeliverable>();

        foreach (var deliverable in Deliverables) {
            if (deliverable is MinionStartingStats) {
                var characterContainer = Util.KInstantiateUI<CharacterContainer>(
                    screen.containerPrefab.gameObject,
                    screen.containerParent
                );
                screen.containers.Add(characterContainer);
                characterContainer.SetController(screen);
                characterContainer.SetReshufflingState(false);
                screen.numberOfDuplicantOptions++;
            } else {
                var packageContainer = Util.KInstantiateUI<CarePackageContainer>(
                    screen.carePackageContainerPrefab.gameObject,
                    screen.containerParent
                );
                screen.containers.Add(packageContainer);
                packageContainer.SetController(screen);
                screen.numberOfCarePackageOptions++;
            }
        }
    }

    private static void SetDeliverablesData(CharacterSelectionController screen) {
        var minionStats = Deliverables.OfType<MinionStartingStats>().ToArray();
        var packageData =
            Deliverables.OfType<CarePackageContainer.CarePackageInstanceData>().ToArray();
        for (var i = 0; i < minionStats.Length; i++) {
            SetCharacterStats(screen.containers.OfType<CharacterContainer>().ToArray()[i], minionStats[i]);
        }
        for (var i = 0; i < packageData.Length; i++) {
            SetPackageData(screen.containers.OfType<CarePackageContainer>().ToArray()[i], packageData[i]);
        }
    }

    private static void SetCharacterStats(CharacterContainer characterContainer, MinionStartingStats stats) {
        // Based on CharacterContainer.GenerateCharacter
        characterContainer.stats = stats;
        characterContainer.SetAnimator();
        characterContainer.SetInfoText();
    }

    private static void SetPackageData(
        CarePackageContainer packageContainer,
        CarePackageContainer.CarePackageInstanceData data
    ) {
        // Based on CharacterContainer.GenerateCharacter
        packageContainer.carePackageInstanceData = data;
        packageContainer.info = data.info;
        packageContainer.ClearEntryIcons();
        packageContainer.SetAnimator();
        packageContainer.SetInfoText();
    }
}
