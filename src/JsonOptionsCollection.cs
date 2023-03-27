using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Soenneker.Enums.JsonOptions;

namespace Soenneker.Json.OptionsCollection;

public static class JsonOptionsCollection
{
    public static JsonSerializerOptions GeneralOptions => _lazyGeneralOptions.Value;

    public static JsonSerializerOptions WebOptions => _lazyWebOptions.Value;

    public static JsonSerializerSettings Newtonsoft => _lazyNewtonsoft.Value;

    /// <summary> Do not use outside of local/test/generator situations! Unsafe </summary>
    public static JsonSerializerOptions PrettyOptions => _lazyPrettyOptions.Value;

    /// <summary>
    /// Pretty printed, safe
    /// </summary>
    public static JsonSerializerOptions PrettySafeOptions => _lazyPrettySafeOptions.Value;

    // TODO: Look into paired down serializer options
    private static readonly Lazy<JsonSerializerOptions> _lazyWebOptions = new(() =>
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    });

    private static readonly Lazy<JsonSerializerOptions> _lazyPrettyOptions = new(() =>
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    });

    private static readonly Lazy<JsonSerializerOptions> _lazyPrettySafeOptions = new(() =>
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    });

    private static readonly Lazy<JsonSerializerOptions> _lazyGeneralOptions = new(() =>
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        return options;
    });

    private static readonly Lazy<JsonSerializerSettings> _lazyNewtonsoft = new(() =>
    {
        var options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        return options;
    });

    public static JsonSerializerOptions GetOptionsFromType(JsonOptionType? optionType)
    {
        JsonSerializerOptions options;

        if (optionType == null)
        {
            options = WebOptions;
        }
        else
        {
            options = optionType.Name switch
            {
                nameof(JsonOptionType.General) => GeneralOptions,
                nameof(JsonOptionType.Pretty) => PrettyOptions,
                nameof(JsonOptionType.PrettySafe) => PrettySafeOptions,
                _ => WebOptions
            };
        }

        return options;
    }
}