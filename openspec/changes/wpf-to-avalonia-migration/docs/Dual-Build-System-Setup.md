# Dual-Build System Setup

**Date**: 2026-06-20
**Phase**: 1.3 - Infrastructure & Foundation

## Overview

Stride now supports dual-build mode, allowing both WPF and Avalonia UI frameworks to coexist in the same solution.

## Configuration

### UI Framework Selection

Set the `$(UIFramework)` property to switch between frameworks:

```xml
<!-- WPF (default) -->
<PropertyGroup>
  <UIFramework>Wpf</UIFramework>
</PropertyGroup>

<!-- Avalonia -->
<PropertyGroup>
  <UIFramework>Avalonia</UIFramework>
</PropertyGroup>
```

### Default Behavior

- **Default**: `UIFramework=Wpf` (backward compatible)
- Existing WPF projects continue to work without modification
- Avalonia projects explicitly set `UIFramework=Avalonia`

## Modified Files

| File | Change |
|------|--------|
| `sources/sdk/Stride.Build.Sdk.Editor/Sdk/Stride.Editor.Frameworks.props` | Added `$(UIFramework)` property and default |
| `sources/sdk/Stride.Build.Sdk.Editor/Sdk/Stride.Editor.Frameworks.targets` | New file with conditional logic |
| `sources/presentation/Stride.Core.Presentation.Wpf/Stride.Core.Presentation.Wpf.csproj` | Added conditional items |
| `sources/presentation/Stride.Core.Presentation.Avalonia/Stride.Core.Presentation.Avalonia.csproj` | Set `UIFramework=Avalonia` |

## Framework Constants

| Property | WPF | Avalonia |
|----------|-----|----------|
| `StrideEditorTargetFramework` | `net10.0-windows` | `net10.0` |
| `UseWPF` | `true` | `false` |
| `AvaloniaVersion` | (unused) | `12.0.3` |

## NuGet Packages (Avalonia)

| Package | Version |
|---------|---------|
| `Avalonia` | 12.0.3 |
| `Avalonia.Desktop` | 12.0.3 |
| `Avalonia.Themes.Fluent` | 12.0.3 |
| `Avalonia.Fonts.Inter` | 12.0.3 |
| `Avalonia.Controls.DataGrid` | 12.0.0 |
| `Dock.Avalonia` | 12.0.0 |
| `Dock.Model.Avalonia` | 12.0.0 |
| `Dock.Avalonia.Themes.Fluent` | 12.0.0 |
| `RoslynPad.Editor.Avalonia` | 5.0.0 |
| `RoslynPad.Roslyn.Avalonia` | 5.0.0 |
| `RoslynPad.Themes` | 5.0.0 |

## Build Verification

| Project | Configuration | Result |
|---------|---------------|--------|
| `Stride.Core.Presentation.Wpf` | Release | ✅ 0 errors, 120 warnings |
| `Stride.Core.Presentation.Avalonia` | Release | ✅ 0 errors, 106 warnings |

## Next Steps

1. **Phase 1.2** - Create Interop project for WPF ↔ Avalonia bridging
2. **Phase 2** - Begin migrating core UI controls

## Usage in CI/CD

```yaml
# GitHub Actions example
- name: Build WPF
  run: dotnet build Stride.Core.Presentation.Wpf.csproj -c Release

- name: Build Avalonia
  run: dotnet build Stride.Core.Presentation.Avalonia.csproj -c Release
```

## Notes

- Both projects can be built in the same solution
- No conflicts between WPF and Avalonia references
- Conditional compilation allows gradual migration
- All existing WPF projects remain unchanged
