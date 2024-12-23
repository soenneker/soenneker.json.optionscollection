using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Json.OptionsCollection.Tests;

[Collection("Collection")]
public class JsonOptionsCollectionTests : FixturedUnitTest
{
    public JsonOptionsCollectionTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}
