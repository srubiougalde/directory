using System.Threading.Tasks;
using DirectoryApi.Helpers;
using Xunit;

namespace DirectoryApi.Tests;

public class FeedHelperTests
{
    [Fact]
    public async Task ReadHeadings_Success_Test()
    {
        var htmlNodes = FeedHelper.ReadHeadings("https://www.lipsum.com/");

        Assert.NotNull(htmlNodes);
    }

    [Fact]
    public async Task ReadHeadings_Failure_Test()
    {
        var htmlNodes = FeedHelper.ReadHeadings("invalidinput");

        Assert.Null(htmlNodes);
    }
}