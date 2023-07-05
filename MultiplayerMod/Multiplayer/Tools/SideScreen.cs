using System.Linq;
using System.Reflection;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Multiplayer.Tools;

public static class SideScreen<ScreenType> {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(SideScreen<ScreenType>));

    private const BindingFlags instanceFields = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public static bool TargetSelected<T>(T target, out ScreenType screen) where T : KMonoBehaviour {
        var instance = GetInstance();
        var selected = TargetSelected(instance, target);
        screen = instance;
        return selected;
    }

    private static ScreenType GetInstance() => DetailsScreen.Instance.sortedSideScreens
        .Select(it => it.Key.GetComponent<ScreenType>())
        .FirstOrDefault(it => it != null);

    private static bool TargetSelected<T>(ScreenType screen, T target) where T : KMonoBehaviour {
        if (screen == null)
            return false;

        var field = screen.GetType().GetField("target", instanceFields);
        if (field == null) {
            log.Warning("Side screen doesn't have 'target' field");
            return false;
        }

        var screenTarget = (T) field.GetValue(screen);
        return screenTarget == target;
    }

}
