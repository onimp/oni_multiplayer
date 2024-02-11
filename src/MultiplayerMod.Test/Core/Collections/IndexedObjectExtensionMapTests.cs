using System.Collections.Generic;
using MultiplayerMod.Core.Collections;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Collections;

[TestFixture]
public class IndexedObjectExtensionMapTests {

    [Test]
    public void MustIndexValuesByIdentity() {
        var objects = new List<Base> { new(0), new(0), new(0), new(0) };
        var map = new IndexedObjectExtensionMap<Base, Extension> {
            [objects[0]] = new("one"),
            [objects[1]] = new("two"),
            [objects[2]] = new("three"),
            [objects[3]] = new("four")
        };

        Assert.AreSame(expected: objects[0], actual: map[new Extension("one")]);
        Assert.AreSame(expected: objects[1], actual: map[new Extension("two")]);
        Assert.AreSame(expected: objects[2], actual: map[new Extension("three")]);
        Assert.AreSame(expected: objects[3], actual: map[new Extension("four")]);

        Assert.AreNotSame(expected: new Base(1), actual: map[new Extension("one")]);
        Assert.AreNotSame(expected: new Base(1), actual: map[new Extension("two")]);
        Assert.AreNotSame(expected: new Base(2), actual: map[new Extension("three")]);
        Assert.AreNotSame(expected: new Base(2), actual: map[new Extension("four")]);

        Assert.AreEqual(actual: map[objects[0]], expected: new Extension("one"));
        Assert.AreEqual(actual: map[objects[1]], expected: new Extension("two"));
        Assert.AreEqual(actual: map[objects[2]], expected: new Extension("three"));
        Assert.AreEqual(actual: map[objects[3]], expected: new Extension("four"));

        Assert.IsFalse(map.TryGetValue(new Base(0), out var value));
        Assert.IsNull(value);
    }

    public record Base(int Value);

    public record Extension(string Text);

}
