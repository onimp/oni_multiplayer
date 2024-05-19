using MultiplayerMod.Core.Extensions;
using NUnit.Framework;

// ReSharper disable UnusedTypeParameter

namespace MultiplayerMod.Test.Core.Extensions;

[TestFixture]
public class TypeSignatureExtensionTests {

    [Test]
    public void SimpleGenericClassSignature() {
        var signature = typeof(SimpleGenericClass<int>).GetSignature();
        Assert.AreEqual(expected: "SimpleGenericClass<int>", actual: signature);
    }

    [Test]
    public void NullableValueTypeClassSignature() {
        var signature = typeof(int?).GetSignature();
        Assert.AreEqual(expected: "int?", actual: signature);
    }

    [Test]
    public void SimpleGenericInnerClassSignature() {
        var signature = typeof(SimpleGenericClass<int>.InnerClass<string, long>).GetSignature();
        Assert.AreEqual(expected: "SimpleGenericClass<int>.InnerClass<string, long>", actual: signature);
    }

    [Test]
    public void SimpleGenericNestedClassSignature() {
        var signature = typeof(SimpleGenericClass<int>.InnerClass<string, long>.NestedClass).GetSignature();
        Assert.AreEqual(expected: "SimpleGenericClass<int>.InnerClass<string, long>.NestedClass", actual: signature);
    }

    [Test]
    public void SimpleGenericNestedClassSignatureWithNamespace() {
        var signature = typeof(SimpleGenericClass<int>.InnerClass<string, long>.NestedClass)
            .GetSignature(SignatureOptions.Namespace);
        Assert.AreEqual(
            expected:
            "MultiplayerMod.Test.Core.Extensions.SimpleGenericClass<int>.InnerClass<string, long>.NestedClass",
            actual: signature
        );
    }

    [Test]
    public void SimpleGenericNestedArrayClassSignature() {
        var signature = typeof(SimpleGenericClass<int[]>.InnerClass<string, long>.NestedClass[]).GetSignature();
        Assert.AreEqual(expected: "SimpleGenericClass<int[]>.InnerClass<string, long>.NestedClass[]", actual: signature);
    }

    [Test]
    public void SimpleClassSignature() {
        var signature = typeof(SimpleClass).GetSignature();
        Assert.AreEqual(expected: "SimpleClass", actual: signature);
    }

    [Test]
    public void SimpleArrayClassSignature() {
        var signature = typeof(SimpleClass[]).GetSignature();
        Assert.AreEqual(expected: "SimpleClass[]", actual: signature);
    }

    [Test]
    public void SimpleInnerClassSignature() {
        var signature = typeof(SimpleClass.InnerSimpleClass).GetSignature();
        Assert.AreEqual(expected: "SimpleClass.InnerSimpleClass", actual: signature);
    }

    [Test]
    public void SimpleInnerGenericClassSignature() {
        var signature = typeof(SimpleClass.InnerClass<string, long>).GetSignature();
        Assert.AreEqual(expected: "SimpleClass.InnerClass<string, long>", actual: signature);
    }

    [Test]
    public void SimpleNestedClassSignature() {
        var signature = typeof(SimpleClass.InnerClass<string, long>.NestedClass).GetSignature();
        Assert.AreEqual(expected: "SimpleClass.InnerClass<string, long>.NestedClass", actual: signature);
    }

    [Test]
    public void DerivedClassSignatureWithInheritanceChain() {
        var signature = typeof(DerivedClass).GetSignature(SignatureOptions.Inheritance);
        Assert.AreEqual(expected: "DerivedClass : SimpleClass", actual: signature);
    }

    [Test]
    public void DerivedGenericClassSignatureWithInheritanceChain() {
        var signature = typeof(DerivedGenericClass<string>).GetSignature(SignatureOptions.Inheritance);
        Assert.AreEqual(expected: "DerivedGenericClass<string> : SimpleGenericClass<string>", actual: signature);
    }

    [Test]
    public void GenericParameterInGenericTypeDefinition() {
        var signature = typeof(DerivedGenericClass<>).GetSignature();
        Assert.AreEqual(expected: "DerivedGenericClass<Type>", actual: signature);
    }

    [Test]
    public void SimpleMethodSignature() {
        var signature = typeof(DerivedGenericClass<>).GetSignature();
        Assert.AreEqual(expected: "DerivedGenericClass<Type>", actual: signature);
    }

}

public class SimpleClass {
    public class InnerSimpleClass { }

    public class InnerClass<InnerType1, InnerType2> {
        public class NestedClass { }
    }
}

public class SimpleGenericClass<Type> {
    public class InnerClass<InnerType1, InnerType2> {
        public class NestedClass { }
    }
}

public class DerivedClass : SimpleClass { }

public class DerivedGenericClass<Type> : SimpleGenericClass<Type> { }
