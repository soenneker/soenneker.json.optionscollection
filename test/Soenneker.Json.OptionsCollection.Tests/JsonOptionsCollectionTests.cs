using System.Threading.Tasks;
using Soenneker.Tests.HostedUnit;
using System.Threading;

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

    [Test]
    public async ValueTask System_text_json_profiles_are_read_only(CancellationToken cancellationToken)
    {
        await Assert.That(JsonOptionsCollection.GeneralOptions.IsReadOnly).IsTrue();
        await Assert.That(JsonOptionsCollection.WebOptions.IsReadOnly).IsTrue();
        await Assert.That(JsonOptionsCollection.PrettyOptions.IsReadOnly).IsTrue();
        await Assert.That(JsonOptionsCollection.PrettySafeOptions.IsReadOnly).IsTrue();
    }
}
