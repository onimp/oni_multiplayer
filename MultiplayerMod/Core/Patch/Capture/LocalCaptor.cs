namespace MultiplayerMod.Core.Patch.Capture;

public class LocalCaptor {

    private readonly LocalCaptureContainer container = new();

    private LocalCaptor(System.Action action) {
        LocalVariableCaptureSupport.ContainerReference.Value = container;
        action();
        LocalVariableCaptureSupport.ContainerReference.Value = null;
    }

    public static T Capture<T>(System.Action action) where T : ILocalCapture {
        var captures = new LocalCaptor(action).container.Captures;
        if (captures.Count == 0 || captures[0].GetType() != typeof(T))
            throw new CaptureUnavailableException($"Capture of type {typeof(T).FullName} is unavailable");
        return (T) captures[0];
    }

}
