using System;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerMod.Game.Context;

// ReSharper disable Unity.IncorrectMonoBehaviourInstantiation
public class GameContextOverride : GameContext {

    public PriorityScreen PriorityScreen { get; set; }
    public ProductInfoScreen ProductInfoScreen { get; set; }
    public MaterialSelectionPanel MaterialSelectionPanel { get; set; }

    private static IntPtr NonZeroPtr = new(1);

    public GameContextOverride() {
        ToolMenu = CreateUnityObject<ToolMenu>();
        BuildMenu = CreateUnityObject<BuildMenu>();
        PlanScreen = CreateUnityObject<PlanScreen>();

        PriorityScreen = CreateUnityObject<PriorityScreen>();
        ProductInfoScreen = CreateUnityObject<ProductInfoScreen>();
        MaterialSelectionPanel = CreateUnityObject<MaterialSelectionPanel>();

        ToolMenu.priorityScreen = PriorityScreen;
        BuildMenu.productInfoScreen = ProductInfoScreen;
        PlanScreen.ProductInfoScreen = ProductInfoScreen;
        ProductInfoScreen.materialSelectionPanel = MaterialSelectionPanel;
        MaterialSelectionPanel.priorityScreen = PriorityScreen;

        InitializeDebugPaintElementScreen();
    }

    private void InitializeDebugPaintElementScreen() {
        DebugPaintElementScreen = CreateUnityObject<DebugPaintElementScreen>();
        DebugPaintElementScreen.paintElement = new Toggle();
        DebugPaintElementScreen.paintMass = new Toggle();
        DebugPaintElementScreen.paintTemperature = new Toggle();
        DebugPaintElementScreen.paintDiseaseCount = new Toggle();
        DebugPaintElementScreen.paintDisease = new Toggle();
        DebugPaintElementScreen.affectBuildings = new Toggle();
        DebugPaintElementScreen.affectCells = new Toggle();
    }

    private static T CreateUnityObject<T>() where T : MonoBehaviour, new() => new() {
        m_CachedPtr = NonZeroPtr // Support for != and == for Unity objects
    };

}
