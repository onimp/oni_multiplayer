using System;
using UnityEngine;

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
    }

    private static T CreateUnityObject<T>() where T : MonoBehaviour, new() => new() {
        m_CachedPtr = NonZeroPtr // Support for != and == for Unity objects
    };

}
