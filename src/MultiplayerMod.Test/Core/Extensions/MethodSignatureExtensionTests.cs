using MultiplayerMod.Core.Extensions;
using NUnit.Framework;

// ReSharper disable UnusedTypeParameter

namespace MultiplayerMod.Test.Core.Extensions;

[TestFixture]
public class MethodSignatureExtensionTests {

    [Test]
    public void SimpleMethodSignature() {
        var signature = typeof(MethodSignatureClass<>).GetMethod(nameof(MethodSignatureClass<object>.SimpleMethod))!
            .GetSignature();
        Assert.AreEqual(expected: "MethodSignatureClass<T>:SimpleMethod()", actual: signature);
    }

    [Test]
    public void SimpleMethodWithParametersSignature() {
        var signature = typeof(MethodSignatureClass<>)
            .GetMethod(nameof(MethodSignatureClass<object>.SimpleMethodWithParameters))!
            .GetSignature();
        Assert.AreEqual(
            expected: "MethodSignatureClass<T>:SimpleMethodWithParameters(int, int)",
            actual: signature
        );
    }

    [Test]
    public void SimpleMethodWithOutParametersSignature() {
        var signature = typeof(MethodSignatureClass<>)
            .GetMethod(nameof(MethodSignatureClass<object>.SimpleMethodWithOutParameters))!
            .GetSignature();
        Assert.AreEqual(
            expected: "MethodSignatureClass<T>:SimpleMethodWithOutParameters(int&, int&)",
            actual: signature
        );
    }

    [Test]
    public void SimpleMethodWithRefParametersSignature() {
        var signature = typeof(MethodSignatureClass<>)
            .GetMethod(nameof(MethodSignatureClass<object>.SimpleMethodWithRefParameters))!
            .GetSignature();
        Assert.AreEqual(
            expected: "MethodSignatureClass<T>:SimpleMethodWithRefParameters(int&, int&)",
            actual: signature
        );
    }

    [Test]
    public void SimpleMethodWithParametersAndNamesSignature() {
        var signature = typeof(MethodSignatureClass<>)
            .GetMethod(nameof(MethodSignatureClass<object>.SimpleMethodWithParameters))!
            .GetSignature(SignatureOptions.IncludeParametersName);
        Assert.AreEqual(
            expected: "MethodSignatureClass<T>:SimpleMethodWithParameters(int a, int b)",
            actual: signature
        );
    }

    [Test]
    public void SimpleMethodWithGenericParametersSignature() {
        var signature = typeof(MethodSignatureClass<>)
            .GetMethod(nameof(MethodSignatureClass<object>.SimpleMethodWithGenericParameters))!
            .GetSignature();
        Assert.AreEqual(
            expected: "MethodSignatureClass<T>:SimpleMethodWithGenericParameters(int, T)",
            actual: signature
        );
    }

    [Test]
    public void SimpleMethodWithGenericImplementedParametersSignature() {
        var signature = typeof(MethodSignatureClass<int>)
            .GetMethod(nameof(MethodSignatureClass<object>.SimpleMethodWithGenericParameters))!
            .GetSignature();
        Assert.AreEqual(
            expected: "MethodSignatureClass<int>:SimpleMethodWithGenericParameters(int, int)",
            actual: signature
        );
    }

    [Test]
    public void GenericMethodWithParametersSignature() {
        var signature = typeof(MethodSignatureClass<>)
            .GetMethod(nameof(MethodSignatureClass<object>.GenericMethodWithParameters))!
            .GetSignature();
        Assert.AreEqual(
            expected: "MethodSignatureClass<T>:GenericMethodWithParameters<V>(int, V)",
            actual: signature
        );
    }

    [Test]
    public void GenericMethodWithGenericParametersSignature() {
        var signature = typeof(MethodSignatureClass<>)
            .GetMethod(nameof(MethodSignatureClass<object>.GenericMethodWithGenericParameters))!
            .GetSignature();
        Assert.AreEqual(
            expected: "MethodSignatureClass<T>:GenericMethodWithGenericParameters<V>(T, V)",
            actual: signature
        );
    }

    [Test]
    public void GenericMethodWithGenericImplementedParametersSignature() {
        var signature = typeof(MethodSignatureClass<bool[]>)
            .GetMethod(nameof(MethodSignatureClass<object>.GenericMethodWithGenericParameters))!
            .GetSignature();
        Assert.AreEqual(
            expected: "MethodSignatureClass<bool[]>:GenericMethodWithGenericParameters<V>(bool[], V)",
            actual: signature
        );
    }

}

public class MethodSignatureClass<T> {

    public void SimpleMethod() { }

    public void SimpleMethodWithOutParameters(out int a, out int b) {
        a = 1;
        b = 2;
    }

    public void SimpleMethodWithRefParameters(ref int a, ref int b) { }

    public void SimpleMethodWithParameters(int a, int b) { }

    public void SimpleMethodWithGenericParameters(int a, T b) { }

    public void GenericMethodWithParameters<V>(int a, V v) { }

    public void GenericMethodWithGenericParameters<V>(T a, V v) { }

}
