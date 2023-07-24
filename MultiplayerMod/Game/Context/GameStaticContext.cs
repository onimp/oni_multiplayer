using System;
using System.Diagnostics.CodeAnalysis;
using MultiplayerMod.Core.Unity;
using UnityEngine.UI;

namespace MultiplayerMod.Game.Context;

public class GameStaticContext {

    public ToolMenu ToolMenu { get; }
    public BuildMenu BuildMenu { get; }
    public PlanScreen PlanScreen { get; }
    public DebugPaintElementScreen DebugPaintElementScreen { get; }

    public PriorityScreen? PriorityScreen { get; private init; }
    public ProductInfoScreen? ProductInfoScreen { get; init; }
    public MaterialSelectionPanel? MaterialSelectionPanel { get; init; }

    private GameStaticContext(
        ToolMenu toolMenu,
        BuildMenu buildMenu,
        PlanScreen planScreen,
        DebugPaintElementScreen debugPaintElementScreen
    ) {
        ToolMenu = toolMenu;
        BuildMenu = buildMenu;
        PlanScreen = planScreen;
        DebugPaintElementScreen = debugPaintElementScreen;
    }

    private static GameStaticContext? originalContext;
    private static readonly GameStaticContext overridden = Create();

    public static GameStaticContext Current => originalContext != null ? overridden : GetCurrent();

    public static void Override() {
        if (originalContext != null)
            throw new Exception("Unable to override already overridden static context");

        originalContext = GetCurrent();
        overridden.Switch();
    }

    public static void Restore() {
        if (originalContext == null)
            throw new Exception("Unable to restore non-overridden static context");

        originalContext.Switch();
        originalContext = null;
    }

    private static GameStaticContext GetCurrent() => new(
        ToolMenu.Instance,
        BuildMenu.Instance,
        PlanScreen.Instance,
        DebugPaintElementScreen.Instance
    );

    [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
    private static GameStaticContext Create() {
        var context = new GameStaticContext(
            UnityObject.CreateStub<ToolMenu>(),
            UnityObject.CreateStub<BuildMenu>(),
            UnityObject.CreateStub<PlanScreen>(),
            UnityObject.CreateStub<DebugPaintElementScreen>()
        ) {
            PriorityScreen = UnityObject.CreateStub<PriorityScreen>(),
            ProductInfoScreen = UnityObject.CreateStub<ProductInfoScreen>(),
            MaterialSelectionPanel = UnityObject.CreateStub<MaterialSelectionPanel>()
        };
        context.ToolMenu.priorityScreen = context.PriorityScreen;
        context.BuildMenu.productInfoScreen = context.ProductInfoScreen;
        context.PlanScreen.ProductInfoScreen = context.ProductInfoScreen;
        context.ProductInfoScreen.materialSelectionPanel = context.MaterialSelectionPanel;
        context.MaterialSelectionPanel.priorityScreen = context.PriorityScreen;

        context.DebugPaintElementScreen.paintElement = new Toggle();
        context.DebugPaintElementScreen.paintMass = new Toggle();
        context.DebugPaintElementScreen.paintTemperature = new Toggle();
        context.DebugPaintElementScreen.paintDiseaseCount = new Toggle();
        context.DebugPaintElementScreen.paintDisease = new Toggle();
        context.DebugPaintElementScreen.affectBuildings = new Toggle();
        context.DebugPaintElementScreen.affectCells = new Toggle();

        return context;
    }

    private void Switch() {
        ToolMenu.Instance = ToolMenu;
        BuildMenu.Instance = BuildMenu;
        PlanScreen.Instance = PlanScreen;
        DebugPaintElementScreen.Instance = DebugPaintElementScreen;
    }

}
