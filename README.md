[![](https://img.shields.io/nuget/v/Soenneker.Json.OptionsCollection.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Json.OptionsCollection/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.json.optionscollection/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.json.optionscollection/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Json.OptionsCollection.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Json.OptionsCollection/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.json.optionscollection/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.json.optionscollection/actions/workflows/codeql.yml)

# Soenneker.Json.OptionsCollection

Represents the json options collection.

## Install

```bash
dotnet add package Soenneker.Json.OptionsCollection
```

## Quick start

```csharp
using Soenneker.Json.OptionsCollection;

var result = JsonOptionsCollection.GetOptionsFromType(/* supply optionType */ default!);
```

Gets options from type.

## What you get

- `JsonOptionsCollection` — Represents the json options collection.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `JsonOptionsCollection.GeneralOptions` | Gets or sets general options. | Gets or sets general options. |
| `JsonOptionsCollection.WebOptions` | Gets or sets web options. | Gets or sets web options. |
| `JsonOptionsCollection.Newtonsoft` | Gets or sets newtonsoft. | Gets or sets newtonsoft. |
| `JsonOptionsCollection.PrettyOptions` | Gets or sets pretty options. | Gets or sets pretty options. |
| `JsonOptionsCollection.PrettySafeOptions` | Gets or sets pretty safe options. | Gets or sets pretty safe options. |
