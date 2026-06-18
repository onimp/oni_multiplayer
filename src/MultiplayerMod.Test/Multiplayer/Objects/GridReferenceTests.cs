using MultiplayerMod.Multiplayer.Objects.Reference;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Objects;

[TestFixture]
public class GridReferenceTests {

    [Test]
    public void OldReferencesWithoutWorldIdRemainCompatible() {
        Assert.AreEqual(new GridReference(42, 1), new GridReference(42, 1, 7));
    }

    [Test]
    public void DifferentWorldIdsAreNotEqualWhenBothAreKnown() {
        Assert.AreNotEqual(new GridReference(42, 1, 7), new GridReference(42, 1, 8));
    }

}
