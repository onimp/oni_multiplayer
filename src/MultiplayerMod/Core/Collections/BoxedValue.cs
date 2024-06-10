namespace MultiplayerMod.Core.Collections;

public class BoxedValue<T>(T value) {
    public T Value { get; set; } = value;
}
