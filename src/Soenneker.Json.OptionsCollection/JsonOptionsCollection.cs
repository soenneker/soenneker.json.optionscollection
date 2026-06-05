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
/// Represents the json options collection.
/// </summary>
public static class JsonOptionsCollection
{
    // Reuse singletons to avoid per-options allocations.
    private static readonly JsonStringEnumConverter _stjEnum = new();
    private static readonly StringEnumConverter _newtEnum = new();
    private static readonly DefaultJsonTypeInfoResolver _reflectionResolver = new(); // thread-safe

    /// <summary>
    /// Gets or sets general options.
    /// </summary>
    public static JsonSerializerOptions GeneralOptions => GeneralHolder.Value;
    /// <summary>
    /// Gets or sets web options.
    /// </summary>
    public static JsonSerializerOptions WebOptions => WebHolder.Value;
    /// <summary>
    /// Gets or sets newtonsoft.
    /// </summary>
    public static JsonSerializerSettings Newtonsoft => NewtonsoftHolder.Value;
    /// <summary>
    /// Gets or sets pretty options.
    /// </summary>
    public static JsonSerializerOptions PrettyOptions => PrettyHolder.Value; // unsafe escaping
    /// <summary>
    /// Gets or sets pretty safe options.
    /// </summary>
    public static JsonSerializerOptions PrettySafeOptions => PrettySafeHolder.Value; // safe escaping

    /// <summary>
    /// Gets options from type.
    /// </summary>
    /// <param name="optionType">The option type.</param>
    /// <returns>The result of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsonSerializerOptions GetOptionsFromType(JsonOptionType? optionType)
    {
        if (optionType == null)
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

    private static class NewtonsoftHolder
    {
        internal static readonly JsonSerializerSettings Value = CreateNewtonsoft();
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

        // --- IMPORTANT: .NET 9 requires an explicit resolver before freezing. ---
        // If you have a source-generated context, prefer:
        // opts.TypeInfoResolver = JsonTypeInfoResolver.Combine(MyContext.Default, ReflectionResolver);
        EnsureResolver(opts);

        opts.MakeReadOnly(); // safe on .NET 9 now that a resolver is set
        return opts;
    }

    private static void EnsureResolver(JsonSerializerOptions opts)
    {
        // If neither TypeInfoResolver nor the chain is set, bind the reflection resolver explicitly.
        if (opts.TypeInfoResolver is null && opts.TypeInfoResolverChain.Count == 0)
        {
            opts.TypeInfoResolver = _reflectionResolver;
        }
        // If you want to ALWAYS include reflection as a fallback even when a chain exists, you could:
        // else if (opts.TypeInfoResolver is null)
        //     opts.TypeInfoResolver = JsonTypeInfoResolver.Combine(opts.TypeInfoResolverChain.ToArray(), ReflectionResolver);
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