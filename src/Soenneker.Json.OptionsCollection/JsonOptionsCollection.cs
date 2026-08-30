using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata; // DefaultJsonTypeInfoResolver, JsonTypeInfoResolver
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Soenneker.Enums.JsonOptions;

namespace Soenneker.Json.OptionsCollection;

/// <summary>
/// Provides reusable, read-only System.Text.Json profiles and isolated Newtonsoft.Json settings.
/// </summary>
public static class JsonOptionsCollection
{
    // Reuse singletons to avoid per-options allocations.
    private static readonly JsonStringEnumConverter _stjEnum = new();
    private static readonly StringEnumConverter _newtEnum = new();
    private static readonly DefaultJsonTypeInfoResolver _reflectionResolver = new(); // thread-safe

    /// <summary>
    /// Gets compact, general-purpose System.Text.Json options without string-enum conversion.
    /// </summary>
    public static JsonSerializerOptions GeneralOptions => GeneralHolder.Value;
    /// <summary>
    /// Gets compact System.Text.Json web defaults with string-enum conversion.
    /// </summary>
    public static JsonSerializerOptions WebOptions => WebHolder.Value;
    /// <summary>
    /// Creates Newtonsoft.Json settings with null omission and string-enum conversion.
    /// </summary>
    public static JsonSerializerSettings Newtonsoft => CreateNewtonsoft();
    /// <summary>
    /// Gets indented System.Text.Json options with relaxed JSON escaping.
    /// </summary>
    public static JsonSerializerOptions PrettyOptions => PrettyHolder.Value; // unsafe escaping
    /// <summary>
    /// Gets indented System.Text.Json options with the default safe encoder.
    /// </summary>
    public static JsonSerializerOptions PrettySafeOptions => PrettySafeHolder.Value; // safe escaping

    /// <summary>
    /// Gets options from type.
    /// </summary>
    /// <param name="optionType">Option Type for the get options from type operation.</param>
    /// <returns>The requested JSON Serializer Options.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsonSerializerOptions GetOptionsFromType(JsonOptionType? optionType)
    {
        if (optionType is null)
            return WebOptions;

        switch (optionType.Value)
        {
            case JsonOptionType.GeneralValue:
                return GeneralOptions;

            case JsonOptionType.PrettyValue:
                return PrettyOptions;
            case JsonOptionType.PrettySafeValue:
                return PrettySafeOptions;
            default:
                return WebOptions;
        }
    }
    // -------- Holders (initialize on first access) --------

    private static class GeneralHolder
    {
        internal static readonly JsonSerializerOptions Value = CreateFrozen(JsonSerializerDefaults.General, writeIndented: false, unsafeRelaxedEscaping: false,
            includeEnumConverter: false, skipComments: true);
    }

    private static class WebHolder
    {
        internal static readonly JsonSerializerOptions Value = CreateFrozen(JsonSerializerDefaults.Web, writeIndented: false, unsafeRelaxedEscaping: false,
            includeEnumConverter: true, skipComments: true);
    }

    private static class PrettyHolder
    {
        internal static readonly JsonSerializerOptions Value = CreateFrozen(JsonSerializerDefaults.General, writeIndented: true, unsafeRelaxedEscaping: true,
            includeEnumConverter: true, skipComments: false);
    }

    private static class PrettySafeHolder
    {
        internal static readonly JsonSerializerOptions Value = CreateFrozen(JsonSerializerDefaults.General, writeIndented: true, unsafeRelaxedEscaping: false,
            includeEnumConverter: true, skipComments: false);
    }

    // -------- Builders --------

    private static JsonSerializerOptions CreateFrozen(JsonSerializerDefaults defaults, bool writeIndented, bool unsafeRelaxedEscaping,
        bool includeEnumConverter, bool skipComments)
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

        EnsureResolver(opts);

        opts.MakeReadOnly();
        return opts;
    }

    private static void EnsureResolver(JsonSerializerOptions opts)
    {
        if (opts.TypeInfoResolver is null && opts.TypeInfoResolverChain.Count == 0)
        {
            opts.TypeInfoResolver = _reflectionResolver;
        }
    }

    private static JsonSerializerSettings CreateNewtonsoft()
    {
        var s = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            CheckAdditionalContent = false
        };
        s.Converters.Add(_newtEnum);
        return s;
    }
}
