# Dreamine.MVVM.Locators.Wpf

WPF integration layer for the Dreamine MVVM locator system.

This package extends `Dreamine.MVVM.Locators` with WPF-specific runtime helpers for:

- automatic View ↔ ViewModel binding
- View resolution from ViewModel instances
- region-based content navigation
- `ContentControl`-based navigation hosting

It is designed for WPF applications that want lightweight locator-based composition without introducing a heavy framework.

[➡️ 한국어 문서 보기](./README_ko.md)

---

## Purpose

`Dreamine.MVVM.Locators.Wpf` provides the WPF runtime bridge between:

- `Dreamine.MVVM.Locators`
- WPF `FrameworkElement`
- XAML attached properties
- region/content navigation patterns

This package is responsible for connecting Views and ViewModels at runtime in a WPF-friendly way.

---

## Key Components

### `ViewModelBinder`

WPF attached-property helper for automatic ViewModel wiring.

Responsibilities:

- exposes `AutoWireViewModel` attached property
- resolves ViewModel by using `ViewModelLocator.Resolve(view.GetType())`
- assigns resolved ViewModel to `DataContext`
- resolves View instances from ViewModel instances using naming conventions

Example:

```xml
<Window
    xmlns:locator="clr-namespace:Dreamine.MVVM.Locators.Wpf"
    locator:ViewModelBinder.AutoWireViewModel="True">
</Window>
```

---

### `RegionBinder`

Attached-property-based region registry for `ContentControl`.

Responsibilities:

- registers `ContentControl` instances by region name
- navigates by region key
- resolves View from ViewModel through `ViewModelBinder.ResolveView`

Example:

```xml
<ContentControl
    local:RegionBinder.RegionName="MainRegion" />
```

```csharp
RegionBinder.Navigate("MainRegion", viewModel);
```

---

### `ContentControlNavigator`

Simple `INavigator` implementation for `ContentControl`.

Responsibilities:

- resolves a View from a ViewModel
- assigns the resolved View to `ContentControl.Content`

Example:

```csharp
var navigator = new ContentControlNavigator(MainContentControl);
navigator.Navigate(viewModel);
```

---

## View Resolution Rules

`ViewModelBinder.ResolveView(object viewModel)` tries multiple naming patterns.

Examples of supported conversions include:

- `ViewModels` → `Views`
- `ViewModel` → `View`
- `ViewModel` → empty string
- `ViewModels` → `Pages`
- `PageModels` → `Pages`
- `PageModel` → `Page`

This allows the package to work with several WPF naming styles.

---

## Requirements

- .NET 8.0
- WPF enabled
- Reference to `Dreamine.MVVM.Locators`

---

## Installation

```bash
dotnet add package Dreamine.MVVM.Locators.Wpf
```

Or add it directly to your project file:

```xml
<PackageReference Include="Dreamine.MVVM.Locators.Wpf" Version="1.0.3" />
```

---

## Architecture Role

This package belongs to the WPF integration layer of the Dreamine MVVM stack.

```text
Dreamine.MVVM.Locators
        ↓
Dreamine.MVVM.Locators.Wpf
        ↓
WPF Views / Regions / Navigation
```

---

## When to Use

Use this package when:

- you are building a WPF application with Dreamine MVVM
- you want automatic ViewModel binding from XAML
- you want to resolve a View from a ViewModel dynamically
- you need lightweight region/content navigation without a large framework

Do not use this package outside WPF.

---

## License

MIT License
