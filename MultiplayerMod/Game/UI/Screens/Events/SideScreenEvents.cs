using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Screens.Events;

public static class SideScreenEvents {

    public static event Action<SideScreenContent, GameObject, FieldInfo> OnAction;

    // TODO check every type for all its content being synced and remove from here.
    private static readonly Type[] excludeTypes = {
        typeof(TagFilterScreen), typeof(AccessControlSideScreen), typeof(ActiveRangeSideScreen),
        typeof(AlarmSideScreen), typeof(ArtableSelectionSideScreen), typeof(ArtifactAnalysisSideScreen),
        typeof(AssignableSideScreen), typeof(AutoPlumberSideScreen), typeof(AutomatableSideScreen),
        typeof(ButtonMenuSideScreen), typeof(CapacityControlSideScreen), typeof(CheckboxListGroupSideScreen),
        typeof(ClusterDestinationSideScreen), typeof(ClusterGridWorldSideScreen),
        typeof(ClusterLocationFilterSideScreen), typeof(CometDetectorSideScreen), typeof(CommandModuleSideScreen),
        typeof(ComplexFabricatorSideScreen), typeof(ConditionListSideScreen), typeof(ConfigureConsumerSideScreen),
        typeof(CounterSideScreen), typeof(CritterSensorSideScreen), typeof(DispenserSideScreen),
        typeof(DoorToggleSideScreen), typeof(DualSliderSideScreen), typeof(FilterSideScreen),
        typeof(FlatTagFilterSideScreen), typeof(GeneShufflerSideScreen), typeof(GeneticAnalysisStationSideScreen),
        typeof(GeoTunerSideScreen), typeof(HabitatModuleSideScreen), typeof(HarvestModuleSideScreen),
        typeof(HighEnergyParticleDirectionSideScreen), typeof(IncubatorSideScreen), typeof(IntSliderSideScreen),
        typeof(LaunchButtonSideScreen), typeof(LaunchPadSideScreen), typeof(LimitValveSideScreen),
        typeof(LogicBitSelectorSideScreen), typeof(LogicBroadcastChannelSideScreen), typeof(LureSideScreen),
        typeof(MinionTodoSideScreen), typeof(ModuleFlightUtilitySideScreen), typeof(MonumentSideScreen),
        typeof(NToggleSideScreen), typeof(PixelPackSideScreen), typeof(PlanterSideScreen),
        typeof(PlayerControlledToggleSideScreen), typeof(ProgressBarSideScreen), typeof(RailGunSideScreen),
        typeof(ReceptacleSideScreen), typeof(RequestCrewSideScreen), typeof(ResearchSideScreen),
        typeof(RocketModuleSideScreen), typeof(RocketRestrictionSideScreen), typeof(RoleStationSideScreen),
        typeof(SealedDoorSideScreen), typeof(SelfDestructButtonSideScreen), typeof(SingleCheckboxSideScreen),
        typeof(SingleSliderSideScreen), typeof(SuitLockerSideScreen), typeof(TelepadSideScreen),
        typeof(TelescopeSideScreen), typeof(TemperatureSwitchSideScreen), typeof(TemporalTearSideScreen),
        typeof(ThresholdSwitchSideScreen), typeof(TimeRangeSideScreen), typeof(TimerSideScreen),
        typeof(TreeFilterableSideScreen), typeof(ValveSideScreen), typeof(WarpPortalSideScreen),
    };

    [HarmonyPatch]
    // ReSharper disable once UnusedType.Local
    private static class SideScreenContentEvents {

        private static IEnumerable<MethodBase> TargetMethods() {
            return Assembly.GetAssembly(typeof(SideScreenContent))
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(SideScreenContent)))
                .Where(type => !excludeTypes.Contains(type))
                .Distinct()
                .Select(type => type.GetMethod("SetTarget"))
                .ToList();
        }

        [HarmonyPostfix]
        // ReSharper disable once UnusedMember.Local
        private static void SetTarget(SideScreenContent __instance, GameObject __0) {
            var sideScreenContent = __instance;
            var target = __0;
            var fieldInfos = sideScreenContent.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos) {
                var fieldType = fieldInfo.GetType();
                var fieldValue = fieldInfo.GetValue(sideScreenContent);

                void InvokeEventIfNeeded() =>
                    PatchControl.RunIfEnabled(() => OnAction?.Invoke(sideScreenContent, target, fieldInfo));

                switch (fieldValue) {
                    case KToggle toggle:
                        toggle!.onClick += InvokeEventIfNeeded;
                        break;
                    case KButton button:
                        button.onClick += InvokeEventIfNeeded;
                        break;
                    case KSlider slider:
                        slider.onDrag += InvokeEventIfNeeded;
                        break;
                    case KInputField input:
                        input.onEndEdit += InvokeEventIfNeeded;
                        break;
                }
            }
        }
    }
}
