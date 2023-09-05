using MultiplayerMod.Core.Collections;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Collections;

[TestFixture]
[Parallelizable]
public class LinkedHashSetTests {

    [Test]
    public void ItemRemovedCorrectly() {
        var set = new LinkedHashSet<string> { "First", "Second" };
        Assert.IsTrue(set.Remove("Second"));
        Assert.IsFalse(set.Remove("Second"));
    }

}
