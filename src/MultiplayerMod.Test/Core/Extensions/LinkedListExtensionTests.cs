using System.Collections.Generic;
using MultiplayerMod.Core.Extensions;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Extensions;

[TestFixture]
public class LinkedListExtensionTests {

    [Test]
    public void MustRemoveFirstNode() {
        var list = CreateList();
        list.ForEach(
            (value, node) => {
                if (value == 1)
                    list.Remove(node);
            }
        );
        Assert.AreEqual(expected: new[] { 2, 3 }, actual: list);
    }

    [Test]
    public void MustRemoveNode() {
        var list = CreateList();
        list.ForEach(
            (value, node) => {
                if (value == 2)
                    list.Remove(node);
            }
        );
        Assert.AreEqual(expected: new[] { 1, 3 }, actual: list);
    }

    [Test]
    public void MustRemoveLastNode() {
        var list = CreateList();
        list.ForEach(
            (value, node) => {
                if (value == 3)
                    list.Remove(node);
            }
        );
        Assert.AreEqual(expected: new[] { 1, 2 }, actual: list);
    }

    private LinkedList<int> CreateList() {
        var list = new LinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);
        return list;
    }

}
