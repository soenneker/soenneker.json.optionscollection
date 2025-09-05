using System;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Soenneker.Enums.JsonOptions;

namespace Soenneker.Json.OptionsCollection;

public static class JsonOptionsCollection
{
    // Reuse converter instances across options (no duplicate allocations).
    private static readonly JsonStringEnumConverter _stjEnum = new();
    private static readonly StringEnumConverter _newtEnum = new();

    // On-demand init without Lazy<T> overhead:
    public static JsonSerializerOptions GeneralOptions => GeneralHolder.Value;

    public static JsonSerializerOptions WebOptions => WebHolder.Value;

    public static JsonSerializerSettings Newtonsoft => NewtonsoftHolder.Value;

    public static JsonSerializerOptions PrettyOptions => PrettyHolder.Value; // unsafe escaping

    public static JsonSerializerOptions PrettySafeOptions => PrettySafeHolder.Value; // safe escaping

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsonSerializerOptions GetOptionsFromType(JsonOptionType? optionType) =>
        optionType switch
        {
            null => WebOptions,
            JsonOptionType.General => GeneralOptions,
            JsonOptionType.Pretty => PrettyOptions,
            JsonOptionType.PrettySafe => PrettySafeOptions,
            _ => WebOptions
        };

    // ----------------- Holders (initialized only on first access) -----------------

    private static class GeneralHolder
    {
        internal static readonly JsonSerializerOptions Value = CreateFrozen(defaults: JsonSerializerDefaults.General, writeIndented: false, unsafeRelaxedEscaping: false,
            includeEnumConverter: false, skipComments: true);
    }

    private static class WebHolder
    {
        internal static readonly JsonSerializerOptions Value = CreateFrozen(defaults: JsonSerializerDefaults.Web, writeIndented: false, unsafeRelaxedEscaping: false,
            includeEnumConverter: true, skipComments: true);
    }

    private static class PrettyHolder
    {
        internal static readonly JsonSerializerOptions Value = CreateFrozen(defaults: JsonSerializerDefaults.General, writeIndented: true,
            unsafeRelaxedEscaping: true, // ⚠ unsafe, for local/dev/logging only
            includeEnumConverter: true, skipComments: false);
    }

    private static class PrettySafeHolder
    {
        internal static readonly JsonSerializerOptions Value = CreateFrozen(defaults: JsonSerializerDefaults.General, writeIndented: true, unsafeRelaxedEscaping: false,
            includeEnumConverter: true, skipComments: false);
    }

    private static class NewtonsoftHolder
    {
        internal static readonly JsonSerializerSettings Value = CreateNewtonsoft();
    }

    // ----------------- Builders -----------------

    private static JsonSerializerOptions CreateFrozen(JsonSerializerDefaults defaults, bool writeIndented, bool unsafeRelaxedEscaping, bool includeEnumConverter, bool skipComments)
    {
        var opts = new JsonSerializerOptions(defaults)
        {
            WriteIndented = writeIndented,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        if (unsafeRelaxedEscaping)
            opts.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

        if (skipComments)
            opts.ReadCommentHandling = JsonCommentHandling.Skip;

        if (includeEnumConverter)
            opts.Converters.Add(_stjEnum);

        // If you adopt source-gen contexts, insert them here:
        // opts.TypeInfoResolverChain.Insert(0, MyContext.Default);

        opts.MakeReadOnly(); // .NET 9: freeze to reduce internal checks & guard against mutation
        return opts;
    }

    private static JsonSerializerSettings CreateNewtonsoft()
    {
        var s = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            CheckAdditionalContent = false // small perf nudge in common serialize-only paths
        };

        s.Converters.Add(_newtEnum);
        return s;
    }
}