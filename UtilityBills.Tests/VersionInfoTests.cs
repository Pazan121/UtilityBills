using UtilityBills.Core;
using Xunit;

namespace UtilityBills.Tests;

public class VersionInfoTests
{
    [Fact]
    public void Current_ReturnsVersionFromDirectoryBuildProps()
    {
        var version = VersionInfo.Current();

        Assert.Equal("0.8.0", version);
    }
}
