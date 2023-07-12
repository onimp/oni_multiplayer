using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Screens.Events;

public static class SideScreenEvents {

    public static event Action<SideScreenFieldArgs> OnAction;
    public static event Action<SideScreenFieldArgs, int, float> OnSlider;

    // TODO check every type for all its content being synced and remove from here.
    private static readonly Type[] excludeTypesTodo = {
        typeof(TagFilterScreen), typeof(AccessControlSideScreen), typeof(ActiveRangeSideScreen),
        typeof(AlarmSideScreen), typeof(ArtableSelectionSideScreen), typeof(ArtifactAnalysisSideScreen),
        typeof(AssignableSideScreen), typeof(AutoPlumberSideScreen), typeof(AutomatableSideScreen),
        typeof(ButtonMenuSideScreen), typeof(CapacityControlSideScreen), typeof(CheckboxListGroupSideScreen),
        typeof(ClusterDestinationSideScreen), typeof(ClusterGridWorldSideScreen),
        typeof(ClusterLocationFilterSideScreen), typeof(CometDetectorSideScreen), typeof(CommandModuleSideScreen),
        typeof(ComplexFabricatorSideScreen), typeof(ConditionListSideScreen), typeof(ConfigureConsumerSideScreen),
        typeof(CounterSideScreen), typeof(CritterSensorSideScreen), typeof(DispenserSideScreen),
        typeof(DoorToggleSideScreen), typeof(FilterSideScreen),
        typeof(FlatTagFilterSideScreen), typeof(GeneShufflerSideScreen), typeof(GeneticAnalysisStationSideScreen),
        typeof(GeoTunerSideScreen), typeof(HabitatModuleSideScreen), typeof(HarvestModuleSideScreen),
        typeof(HighEnergyParticleDirectionSideScreen), typeof(IncubatorSideScreen),
        typeof(LaunchButtonSideScreen), typeof(LaunchPadSideScreen), typeof(LimitValveSideScreen),
        typeof(LogicBitSelectorSideScreen), typeof(LogicBroadcastChannelSideScreen), typeof(LureSideScreen),
        typeof(MinionTodoSideScreen), typeof(ModuleFlightUtilitySideScreen), typeof(MonumentSideScreen),
        typeof(NToggleSideScreen), typeof(PixelPackSideScreen), typeof(PlanterSideScreen),
        typeof(PlayerControlledToggleSideScreen), typeof(ProgressBarSideScreen), typeof(RailGunSideScreen),
        typeof(ReceptacleSideScreen), typeof(RequestCrewSideScreen),
        typeof(RocketModuleSideScreen), typeof(RocketRestrictionSideScreen), typeof(RoleStationSideScreen),
        typeof(SealedDoorSideScreen), typeof(SelfDestructButtonSideScreen), typeof(SingleCheckboxSideScreen),
        typeof(SuitLockerSideScreen), typeof(TelepadSideScreen),
        typeof(TelescopeSideScreen), typeof(TemperatureSwitchSideScreen), typeof(TemporalTearSideScreen),
        typeof(ThresholdSwitchSideScreen), typeof(TimeRangeSideScreen), typeof(TimerSideScreen),
        typeof(TreeFilterableSideScreen), typeof(ValveSideScreen), typeof(WarpPortalSideScreen),
    };

    private static readonly Type[] excludeTypes = {
        typeof(ResearchSideScreen)
    };

    [HarmonyPatch]
    // ReSharper disable once UnusedType.Local
    private static class SideScreenContentEvents {

        // ReSharper disable once UnusedMember.Local
        private static IEnumerable<MethodBase> TargetMethods() {
            return Assembly.GetAssembly(typeof(SideScreenContent))
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(SideScreenContent)))
                .Where(type => !(excludeTypes.Contains(type) || excludeTypesTodo.Contains(type)))
                .Distinct()
                .Select(type => type.GetMethod("SetTarget"))
                .ToList();
        }

        private static readonly Dictionary<SideScreenContent, GameObject> TargetObjects = new();
        private static readonly HashSet<SideScreenContent> SubsribedScreens = new();

        [HarmonyPostfix]
        // ReSharper disable once UnusedMember.Local
        private static void SetTarget(SideScreenContent __instance, GameObject __0) {
            var sideScreenContent = __instance;
            var targetObject = __0;
            TargetObjects[sideScreenContent] = targetObject;
            if (SubsribedScreens.Contains(__instance)) return;

            SubsribedScreens.Add(__instance);

            var fieldInfos = sideScreenContent.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos) {
                void InvokeEventIfNeeded() => PatchControl.RunIfEnabled(
                    () => OnAction?.Invoke(
                        new SideScreenFieldArgs(sideScreenContent, TargetObjects[sideScreenContent], fieldInfo)
                    )
                );

                switch (fieldInfo.GetValue(sideScreenContent)) {
                    case KToggle toggle:
                        toggle!.onClick += InvokeEventIfNeeded;
                        break;
                    case KButton button:
                        button.onClick += InvokeEventIfNeeded;
                        break;
                    case KInputField input:
                        input.onEndEdit += InvokeEventIfNeeded;
                        break;
                    case List<SliderSet> sliderSets:
                        subscribeToSliders(sideScreenContent, fieldInfo, sliderSets);
                        break;
                }
            }
        }

        private static void subscribeToSliders(
            SideScreenContent sideScreenContent,
            FieldInfo fieldInfo,
            List<SliderSet> sliderSets
        ) {
            for (var i = 0; i < sliderSets.Count; i++) {
                var sliderSet = sliderSets[i];
                var sliderIndex = i;

                void ValueSliderOnReleaseHandle() => PatchControl.RunIfEnabled(
                    () => OnSlider?.Invoke(
                        new SideScreenFieldArgs(sideScreenContent, TargetObjects[sideScreenContent], fieldInfo),
                        sliderIndex,
                        sliderSet.valueSlider.value
                    )
                );

                void NumberInputOnEndEdit() => PatchControl.RunIfEnabled(
                    () => OnSlider?.Invoke(
                        new SideScreenFieldArgs(sideScreenContent, TargetObjects[sideScreenContent], fieldInfo),
                        sliderIndex,
                        sliderSet.numberInput.currentValue
                    )
                );

                sliderSet.valueSlider.onReleaseHandle += ValueSliderOnReleaseHandle;
                sliderSet.numberInput.onEndEdit += NumberInputOnEndEdit;
            }
        }
    }

    public class SideScreenFieldArgs {
        public SideScreenContent SideScreenContent { get; }
        public GameObject Target { get; }
        public FieldInfo FieldInfo { get; }

        public SideScreenFieldArgs(SideScreenContent sideScreenContent, GameObject target, FieldInfo fieldInfo) {
            SideScreenContent = sideScreenContent;
            Target = target;
            FieldInfo = fieldInfo;
        }
    }
}
