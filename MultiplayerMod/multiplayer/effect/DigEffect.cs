namespace MultiplayerMod.multiplayer.effect
{
    public static class DigEffect
    {
        public static void Dig(int priority, int cell, int distFromOrigin)
        {
            var currentPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
            try
            {
                ToolMenu.Instance.PriorityScreen.SetScreenPriority(
                    new PrioritySetting(PriorityScreen.PriorityClass.basic, priority));
                InterfaceTool.ActiveConfig.DigAction.Uproot(cell);
                InterfaceTool.ActiveConfig.DigAction.Dig(cell, distFromOrigin);
            }
            finally
            {
                ToolMenu.Instance.PriorityScreen.SetScreenPriority(currentPriority);
            }
        }
    }
}