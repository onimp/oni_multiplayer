using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Collections;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Collections;

[TestFixture]
public class BidirectionalMapTests {

    [Test]
    public void MustIndexValues() {
        var map = new BidirectionalMap<int, Extension> {
            [1] = new("one"),
            [2] = new("two"),
            [3] = new("three"),
            [4] = new("four")
        };
        Assert.AreEqual(expected: "one", actual: map[1].Text);
        Assert.AreEqual(expected: "two", actual: map[2].Text);
        Assert.AreEqual(expected: "three", actual: map[3].Text);
        Assert.AreEqual(expected: "four", actual: map[4].Text);
        Assert.AreEqual(expected: 1, actual: map[new Extension("one")]);
        Assert.AreEqual(expected: 2, actual: map[new Extension("two")]);
        Assert.AreEqual(expected: 3, actual: map[new Extension("three")]);
        Assert.AreEqual(expected: 4, actual: map[new Extension("four")]);
    }

    [Test]
    public void MustEnumerateByKey() {
        var map = new BidirectionalMap<int, Extension> {
            [1] = new("one"),
            [2] = new("two"),
            [3] = new("three"),
            [4] = new("four")
        };
        var pairs = map.ToList();
        Assert.AreEqual(expected: new KeyValuePair<int, Extension>(1, new Extension("one")), actual: pairs[0]);
        Assert.AreEqual(expected: new KeyValuePair<int, Extension>(2, new Extension("two")), actual: pairs[1]);
        Assert.AreEqual(expected: new KeyValuePair<int, Extension>(3, new Extension("three")), actual: pairs[2]);
        Assert.AreEqual(expected: new KeyValuePair<int, Extension>(4, new Extension("four")), actual: pairs[3]);
    }

    [Test]
    public void MustEnumerateByValue() {
        var map = new BidirectionalMap<int, Extension> {
            [1] = new("one"),
            [2] = new("two"),
            [3] = new("three"),
            [4] = new("four")
        };
        var pairs = map.GetEnumerableByValue().ToList();
        Assert.AreEqual(expected: new KeyValuePair<Extension, int>(new Extension("one"), 1), actual: pairs[0]);
        Assert.AreEqual(expected: new KeyValuePair<Extension, int>(new Extension("two"), 2), actual: pairs[1]);
        Assert.AreEqual(expected: new KeyValuePair<Extension, int>(new Extension("three"), 3), actual: pairs[2]);
        Assert.AreEqual(expected: new KeyValuePair<Extension, int>(new Extension("four"), 4), actual: pairs[3]);
    }

    [Test]
    public void MustSetByValue() {
        var map = new BidirectionalMap<int, Extension> {
            [1] = new("one"),
            [2] = new("two"),
            [new Extension("three")] = 3,
            [new Extension("four")] = 4
        };

        Assert.AreEqual(expected: "one", actual: map[1].Text);
        Assert.AreEqual(expected: "two", actual: map[2].Text);
        Assert.AreEqual(expected: "three", actual: map[3].Text);
        Assert.AreEqual(expected: "four", actual: map[4].Text);
        Assert.AreEqual(expected: 1, actual: map[new Extension("one")]);
        Assert.AreEqual(expected: 2, actual: map[new Extension("two")]);
        Assert.AreEqual(expected: 3, actual: map[new Extension("three")]);
        Assert.AreEqual(expected: 4, actual: map[new Extension("four")]);
    }

    [Test]
    public void MustReturnNullKeyAndValue() {
        var map = new BidirectionalMap<int?, Extension>();
        Assert.IsFalse(map.TryGetValue(1, out var value));
        Assert.IsNull(value);
        Assert.IsFalse(map.TryGetKey(new Extension("one"), out var key));
        Assert.IsNull(key);
    }

    [Test]
    public void MustThrowKeyNotFoundExceptionForKeyOrValue() {
        var map = new BidirectionalMap<int, Extension>();
        Assert.Throws<KeyNotFoundException>(
            () => {
                var value = map[1];
            }
        );

        Assert.Throws<KeyNotFoundException>(
            () => {
                var key = map[new Extension("one")];
            }
        );
    }

    [Test]
    public void MustRemoveEntries() {
        var map = new BidirectionalMap<int?, Extension> {
            [1] = new("one"),
            [2] = new("two"),
            [3] = new("three"),
            [4] = new("four")
        };

        Assert.IsTrue(map.Remove(2));
        Assert.IsTrue(map.Remove(new Extension("four")));

        Assert.AreEqual(expected: "one", actual: map[1].Text);
        Assert.AreEqual(expected: "three", actual: map[3].Text);
        Assert.AreEqual(expected: 1, actual: map[new Extension("one")]);
        Assert.AreEqual(expected: 3, actual: map[new Extension("three")]);

        Assert.IsFalse(map.TryGetValue(2, out var value));
        Assert.IsNull(value);
        Assert.IsFalse(map.TryGetKey(new Extension("two"), out var key));
        Assert.IsNull(key);

        Assert.IsFalse(map.TryGetValue(4, out var value2));
        Assert.IsNull(value2);
        Assert.IsFalse(map.TryGetKey(new Extension("four"), out var key2));
        Assert.IsNull(key2);
    }

    [Test]
    public void MustNotFailOnMissingEntriesRemoval() {
        var map = new BidirectionalMap<int?, Extension>();

        Assert.IsFalse(map.Remove(0));
        Assert.IsFalse(map.Remove(new Extension("zero")));
    }

    [Test]
    public void MustClearEntries() {
        var map = new BidirectionalMap<int?, Extension> {
            [1] = new("one"),
            [2] = new("two"),
            [3] = new("three"),
            [4] = new("four")
        };

        map.Clear();
        Assert.AreEqual(expected: 0, actual: map.ToList().Count);
    }

    public record Extension(string Text);

}
