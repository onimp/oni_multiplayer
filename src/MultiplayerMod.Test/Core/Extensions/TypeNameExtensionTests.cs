using MultiplayerMod.Core.Extensions;
using NUnit.Framework;

// ReSharper disable UnusedTypeParameter

namespace MultiplayerMod.Test.Core.Extensions;

[TestFixture]
public class TypeNameExtensionTests {

    [Test]
    public void SimpleGenericClassName() {
        var name = typeof(SimpleGenericClass<int>).GetPrettyName();
        Assert.AreEqual(expected: "SimpleGenericClass<Int32>", actual: name);
    }

    [Test]
    public void SimpleGenericInnerClassName() {
        var name = typeof(SimpleGenericClass<int>.InnerClass<string, long>).GetPrettyName();
        Assert.AreEqual(expected: "SimpleGenericClass<Int32>.InnerClass<String, Int64>", actual: name);
    }

    [Test]
    public void SimpleGenericNestedClassName() {
        var name = typeof(SimpleGenericClass<int>.InnerClass<string, long>.NestedClass).GetPrettyName();
        Assert.AreEqual(expected: "SimpleGenericClass<Int32>.InnerClass<String, Int64>.NestedClass", actual: name);
    }

    [Test]
    public void SimpleGenericNestedClassNameWithNamespace() {
        var name = typeof(SimpleGenericClass<int>.InnerClass<string, long>.NestedClass)
            .GetPrettyName(TypeNameOptions.IncludeNamespace);
        Assert.AreEqual(
            expected: "MultiplayerMod.Test.Core.Extensions.SimpleGenericClass<System.Int32>.InnerClass<System.String, System.Int64>.NestedClass",
            actual: name
        );
    }

    [Test]
    public void SimpleGenericNestedArrayClassName() {
        var name = typeof(SimpleGenericClass<int[]>.InnerClass<string, long>.NestedClass[]).GetPrettyName();
        Assert.AreEqual(expected: "SimpleGenericClass<Int32[]>.InnerClass<String, Int64>.NestedClass[]", actual: name);
    }

    [Test]
    public void SimpleClassName() {
        var name = typeof(SimpleClass).GetPrettyName();
        Assert.AreEqual(expected: "SimpleClass", actual: name);
    }

    [Test]
    public void SimpleArrayClassName() {
        var name = typeof(SimpleClass[]).GetPrettyName();
        Assert.AreEqual(expected: "SimpleClass[]", actual: name);
    }

    [Test]
    public void SimpleInnerClassName() {
        var name = typeof(SimpleClass.InnerClass<string, long>).GetPrettyName();
        Assert.AreEqual(expected: "SimpleClass.InnerClass<String, Int64>", actual: name);
    }

    [Test]
    public void SimpleNestedClassName() {
        var name = typeof(SimpleClass.InnerClass<string, long>.NestedClass).GetPrettyName();
        Assert.AreEqual(expected: "SimpleClass.InnerClass<String, Int64>.NestedClass", actual: name);
    }

}

public class SimpleClass {
    public class InnerClass<InnerType1, InnerType2> {
        public class NestedClass { }
    }
}

public class SimpleGenericClass<Type> {
    public class InnerClass<InnerType1, InnerType2> {
        public class NestedClass { }
    }
}
