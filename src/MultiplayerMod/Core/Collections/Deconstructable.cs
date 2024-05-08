namespace MultiplayerMod.Core.Collections;

public class Deconstructable<T1>(T1 v1) {

    public void Deconstruct(out T1 a1) {
        a1 = v1;
    }

}

public class Deconstructable<T1, T2>(T1 v1, T2 v2) {

    public void Deconstruct(out T1 a1, out T2 a2) {
        a1 = v1;
        a2 = v2;
    }

}
