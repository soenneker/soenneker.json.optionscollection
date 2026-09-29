using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Soenneker.Enums.JsonOptions;

namespace Soenneker.Json.OptionsCollection;

/// <summary>Provides reusable reflection-based JSON profiles and explicit generated-metadata profiles for AOT.</summary>
public static class JsonOptionsCollection
{
    private static readonly object _sync = new();
    private static JsonSerializerOptions? _general;
    private static JsonSerializerOptions? _web;
    private static JsonSerializerOptions? _pretty;
    private static JsonSerializerOptions? _prettySafe;

    /// <summary>Gets the read-only General reflection-based JSON profile.</summary>
    public static JsonSerializerOptions GeneralOptions
    {
        [RequiresUnreferencedCode("Reflection-based profiles require preserved model members. Use CreateOptions with a generated JSON context in trimmed applications.")]
        [RequiresDynamicCode("Reflection-based profiles and runtime enum converters require dynamic code. Use CreateOptions with a generated JSON context for AOT.")]
        get => GetReflectionProfile(ref _general, JsonSerializerDefaults.General, false, false, false, true);
    }

    /// <summary>Gets the read-only Web reflection-based JSON profile.</summary>
    public static JsonSerializerOptions WebOptions
    {
        [RequiresUnreferencedCode("Reflection-based profiles require preserved model members. Use CreateOptions with a generated JSON context in trimmed applications.")]
        [RequiresDynamicCode("Reflection-based profiles and runtime enum converters require dynamic code. Use CreateOptions with a generated JSON context for AOT.")]
        get => GetReflectionProfile(ref _web, JsonSerializerDefaults.Web, false, false, true, true);
    }

    /// <summary>Gets the read-only Pretty reflection-based JSON profile.</summary>
    public static JsonSerializerOptions PrettyOptions
    {
        [RequiresUnreferencedCode("Reflection-based profiles require preserved model members. Use CreateOptions with a generated JSON context in trimmed applications.")]
        [RequiresDynamicCode("Reflection-based profiles and runtime enum converters require dynamic code. Use CreateOptions with a generated JSON context for AOT.")]
        get => GetReflectionProfile(ref _pretty, JsonSerializerDefaults.General, true, true, true, false);
    }

    /// <summary>Gets the read-only PrettySafe reflection-based JSON profile.</summary>
    public static JsonSerializerOptions PrettySafeOptions
    {
        [RequiresUnreferencedCode("Reflection-based profiles require preserved model members. Use CreateOptions with a generated JSON context in trimmed applications.")]
        [RequiresDynamicCode("Reflection-based profiles and runtime enum converters require dynamic code. Use CreateOptions with a generated JSON context for AOT.")]
        get => GetReflectionProfile(ref _prettySafe, JsonSerializerDefaults.General, true, false, true, false);
    }

    /// <summary>Gets a read-only reflection-based profile.</summary>
    /// <param name="optionType">The profile; null selects web defaults.</param>
    /// <returns>The cached profile.</returns>
    [RequiresUnreferencedCode("Reflection-based profiles require preserved model members. Use CreateOptions with a generated JSON context in trimmed applications.")]
        [RequiresDynamicCode("Reflection-based profiles and runtime enum converters require dynamic code. Use CreateOptions with a generated JSON context for AOT.")]
    public static JsonSerializerOptions GetOptionsFromType(JsonOptionType? optionType) => optionType?.Value switch
    {
        JsonOptionType.GeneralValue => GeneralOptions,
        JsonOptionType.PrettyValue => PrettyOptions,
        JsonOptionType.PrettySafeValue => PrettySafeOptions,
        _ => WebOptions
    };

    /// <summary>Creates a mutable JSON profile using only explicitly supplied metadata.</summary>
    /// <param name="resolver">A generated JSON context or another reflection-free metadata resolver.</param>
    /// <param name="optionType">The profile; null selects web defaults.</param>
    /// <returns>The configured options. Configure string-enum conversion in the generated context for AOT.</returns>
    public static JsonSerializerOptions CreateOptions(IJsonTypeInfoResolver resolver, JsonOptionType? optionType = null)
    {
        ArgumentNullException.ThrowIfNull(resolver);
        JsonSerializerOptions options = optionType?.Value switch
        {
            JsonOptionType.GeneralValue => CreateProfile(JsonSerializerDefaults.General, false, false, true),
            JsonOptionType.PrettyValue => CreateProfile(JsonSerializerDefaults.General, true, true, false),
            JsonOptionType.PrettySafeValue => CreateProfile(JsonSerializerDefaults.General, true, false, false),
            _ => CreateProfile(JsonSerializerDefaults.Web, false, false, true)
        };
        options.TypeInfoResolver = resolver;
        return options;
    }

    [RequiresUnreferencedCode("Reflection-based profiles require preserved model members. Use CreateOptions with a generated JSON context in trimmed applications.")]
        [RequiresDynamicCode("Reflection-based profiles and runtime enum converters require dynamic code. Use CreateOptions with a generated JSON context for AOT.")]
    private static JsonSerializerOptions GetReflectionProfile(ref JsonSerializerOptions? cached, JsonSerializerDefaults defaults,
        bool indented, bool relaxed, bool enums, bool comments)
    {
        lock (_sync)
        {
            if (cached is not null)
                return cached;
            JsonSerializerOptions options = CreateProfile(defaults, indented, relaxed, comments);
            if (enums)
                options.Converters.Add(new JsonStringEnumConverter());
            options.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
            options.MakeReadOnly();
            return cached = options;
        }
    }

    private static JsonSerializerOptions CreateProfile(JsonSerializerDefaults defaults, bool indented, bool relaxed, bool comments)
    {
        var options = new JsonSerializerOptions(defaults)
        {
            WriteIndented = indented,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        if (relaxed) options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        if (comments) options.ReadCommentHandling = JsonCommentHandling.Skip;
        return options;
    }
}
