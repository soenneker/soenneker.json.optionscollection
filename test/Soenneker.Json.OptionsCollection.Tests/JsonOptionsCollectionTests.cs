using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

    [Test]
    public async Task System_text_json_profiles_are_read_only()
    {
        await Assert.That(JsonOptionsCollection.GeneralOptions.IsReadOnly).IsTrue();
        await Assert.That(JsonOptionsCollection.WebOptions.IsReadOnly).IsTrue();
        await Assert.That(JsonOptionsCollection.PrettyOptions.IsReadOnly).IsTrue();
        await Assert.That(JsonOptionsCollection.PrettySafeOptions.IsReadOnly).IsTrue();
    }

    [Test]
    public async Task Newtonsoft_settings_are_isolated_per_caller()
    {
        JsonSerializerSettings first = JsonOptionsCollection.Newtonsoft;
        JsonSerializerSettings second = JsonOptionsCollection.Newtonsoft;

        first.NullValueHandling = NullValueHandling.Include;

        await Assert.That(ReferenceEquals(first, second)).IsFalse();
        await Assert.That(second.NullValueHandling).IsEqualTo(NullValueHandling.Ignore);
    }
}
