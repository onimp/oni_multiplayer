namespace MultiplayerMod.Game.Tools.Events;

public static class BuildToolPriority {

    public static PrioritySetting Get() {
        var priority = new PrioritySetting(PriorityScreen.PriorityClass.basic, 0);

        // Reference: BaseUtilityBuildTool.BuildPath, BuildTool.PostProcessBuild
        if (BuildMenu.Instance != null)
            priority = BuildMenu.Instance.GetBuildingPriority();
        if (PlanScreen.Instance != null)
            priority = PlanScreen.Instance.GetBuildingPriority();

        return priority;
    }

}
