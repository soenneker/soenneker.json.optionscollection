[![](https://img.shields.io/nuget/v/Soenneker.Json.OptionsCollection.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Json.OptionsCollection/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.json.optionscollection/build-and-test.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.json.optionscollection/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.json.optionscollection/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.json.optionscollection/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Json.OptionsCollection.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Json.OptionsCollection/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.json.optionscollection/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.json.optionscollection/actions/workflows/codeql.yml)

# Soenneker.Json.OptionsCollection

Common serializer profiles for `System.Text.Json` and Newtonsoft.Json.

## Install

```bash
dotnet add package Soenneker.Json.OptionsCollection
```

## System.Text.Json profiles

```csharp
using System.Text.Json;
using Soenneker.Json.OptionsCollection;

string apiJson = JsonSerializer.Serialize(
    response,
    JsonOptionsCollection.WebOptions);

string readableJson = JsonSerializer.Serialize(
    response,
    JsonOptionsCollection.PrettySafeOptions);
```

| Profile | Naming/defaults | Indented | Enum strings | Comment reads | Encoder |
| --- | --- | ---: | ---: | --- | --- |
| `GeneralOptions` | General | No | No | Skip | Default |
| `WebOptions` | Web | No | Yes | Skip | Default |
| `PrettyOptions` | General | Yes | Yes | Disallow | Relaxed |
| `PrettySafeOptions` | General | Yes | Yes | Disallow | Default |

All four profiles omit null properties and are shared, read-only instances. Clone one before customization:

```csharp
var custom = new JsonSerializerOptions(
    JsonOptionsCollection.WebOptions)
{
    WriteIndented = true
};
```

`PrettyOptions` uses `UnsafeRelaxedJsonEscaping`, which leaves characters such as `<`, `>`, and `&` less escaped. Do not embed its output directly into HTML or a script context. Use `PrettySafeOptions` when output crosses a trust boundary.

## Select a profile by `JsonOptionType`

```csharp
JsonSerializerOptions options =
    JsonOptionsCollection.GetOptionsFromType(optionType);
```

`GeneralValue`, `PrettyValue`, and `PrettySafeValue` select their matching profiles. A null or unrecognized value falls back to `WebOptions`.

## Newtonsoft.Json

```csharp
using Newtonsoft.Json;

JsonSerializerSettings settings = JsonOptionsCollection.Newtonsoft;
settings.DateParseHandling = DateParseHandling.None;

string json = JsonConvert.SerializeObject(value, settings);
```

Each access returns a new settings object, so caller customization cannot alter another serializer's behavior. The defaults omit null properties, write enums as strings, and leave `CheckAdditionalContent` disabled.
