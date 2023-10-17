using System.IO;
using MultiplayerMod.Core.Paths;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Paths;

[TestFixture]
public class SecurePathTests {

    [Test]
    public void RootIsAccessible() {
        Assert.DoesNotThrow(() => SecurePath.Combine("root"));
    }

    [Test]
    public void PathsUnderRootAreAccessible() {
        Assert.DoesNotThrow(() => SecurePath.Combine("root", "foo.txt"));
        Assert.DoesNotThrow(() => SecurePath.Combine("root", ".", "foo.txt"));
        Assert.DoesNotThrow(() => SecurePath.Combine("root", "dir", "foo.txt"));
    }

    [Test]
    public void AccessDeniedForPathsOutsideRoot() {
        Assert.Throws<AccessDeniedException>(() => SecurePath.Combine("root", "..", "foo.txt"));
    }

    [Test]
    public void PathsAreSeparated() {
        Assert.IsTrue(SecurePath.Combine("root", "a").EndsWith($"{Path.DirectorySeparatorChar}a"));
    }

}
