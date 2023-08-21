namespace MultiplayerModTests.Environment.Unity;

public class UnityEvent {

    public static UnityEvent Awake = new("Awake");
    public static UnityEvent Update = new("Update");
    public static UnityEvent LateUpdate = new("LateUpdate");
    public static UnityEvent Enable = new("OnEnable");
    public static UnityEvent Start = new("Start");

    public string MethodName { get; }

    public UnityEvent(string methodName) {
        MethodName = methodName;
    }

    public override int GetHashCode() => MethodName.GetHashCode();
    public override bool Equals(object? other) => other is UnityEvent @event && @event.MethodName.Equals(MethodName);

}
