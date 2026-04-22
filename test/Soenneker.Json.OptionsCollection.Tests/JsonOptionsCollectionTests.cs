using Soenneker.Tests.HostedUnit;

namespace Soenneker.Json.OptionsCollection.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class JsonOptionsCollectionTests : HostedUnitTest
{
    public JsonOptionsCollectionTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
