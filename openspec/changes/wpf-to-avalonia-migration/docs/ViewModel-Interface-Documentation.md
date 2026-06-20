# ViewModel Interface Documentation

**Generated**: 2026-06-20
**Scope**: `sources/editor/` and `sources/presentation/Stride.Core.Presentation/`

## Summary

The Stride editor ViewModel layer is **mostly platform-agnostic**, with one critical WPF dependency that must be abstracted:

| Component | Platform-Agnostic | WPF Dependency | Notes |
|-----------|-------------------|----------------|-------|
| `ViewModelBase` | ✅ Yes | None | Uses `INotifyPropertyChanged`, `INotifyPropertyChanging` (standard .NET) |
| `EditorViewModel` | ✅ Yes | None | Inherits `ViewModelBase`, no WPF types |
| `SessionViewModel` | ✅ Yes | None | Inherits `DirtiableEditableViewModel` → `ViewModelBase` |
| `ICommandBase` | ❌ No | `System.Windows.Input.ICommand` | **Critical** — Must be abstracted for Avalonia |
| `ICommand` implementations | ❌ No | `System.Windows.Input.ICommand` | 222 occurrences across 71 files |

## ViewModel Hierarchy

```
ViewModelBase (Stride.Core.Presentation.ViewModels)
    ├── INotifyPropertyChanging
    ├── INotifyPropertyChanged
    ├── IDestroyable
    │
    ├── DirtiableEditableViewModel (Stride.Core.Presentation.ViewModels)
    │       └── SessionViewModel (Stride.Core.Assets.Editor.ViewModel)
    │
    └── EditableViewModel (Stride.Core.Presentation.ViewModels)
            └── EditorViewModel (Stride.Core.Assets.Editor.ViewModel)
                    ├── AssetViewModel
                    ├── PackageViewModel
                    ├── ProjectViewModel
                    └── [all other ViewModels]
```

## Critical Issue: ICommandBase WPF Dependency

### Current Implementation

```csharp
// Stride.Core.Presentation\Commands\ICommandBase.cs
using System.Windows.Input;

public interface ICommandBase : ICommand
{
    bool IsEnabled { get; set; }
    void Execute();
}
```

### Problem

`ICommand` from `System.Windows.Input` is WPF-specific. Avalonia uses `Avalonia.Input.ICommand` which has a similar but not identical interface.

### Required Fix

**Option A: Create platform-agnostic ICommand interface**

```csharp
// Stride.Core.Presentation.Commands\ICommandBase.cs (NEW)
namespace Stride.Core.Presentation.Commands;

public interface ICommandBase
{
    bool IsEnabled { get; set; }
    void Execute();
    event EventHandler CanExecuteChanged;
}
```

**Option B: Use Avalonia's ICommand directly**

```csharp
// Stride.Core.Presentation.Commands\ICommandBase.cs (NEW)
using Avalonia.Input;

namespace Stride.Core.Presentation.Commands;

public interface ICommandBase : ICommand
{
    bool IsEnabled { get; set; }
    void Execute();
}
```

**Recommendation**: **Option A** — Create a platform-agnostic `ICommandBase` interface. This allows:
- WPF implementation: `WpfICommandBase : ICommandBase, System.Windows.Input.ICommand`
- Avalonia implementation: `AvaloniaICommandBase : ICommandBase, Avalonia.Input.ICommand`

### Affected Files (222 occurrences, 71 files)

**Top affected files:**
- `Stride.Core.Assets.Editor\ViewModel\SessionViewModel.cs` — 21
- `Stride.Core.Assets.Editor\ViewModel\AssetCollectionViewModel.cs` — 19
- `Stride.Assets.Presentation\AssetEditors\SpriteEditor\ViewModels\SpriteSheetEditorViewModel.cs` — 14

All these files use `ICommand` implementations (likely `AnonymousCommand`, `AnonymousTaskCommand`, etc.) that inherit from `ICommandBase`.

### Migration Plan for ICommand

1. **Create `Stride.Core.Presentation.Commands\ICommandBase.cs`** with platform-agnostic interface
2. **Create `WpfICommandAdapter`** — Adapts WPF `ICommand` to `ICommandBase`
3. **Create `AvaloniaICommandAdapter`** — Adapts Avalonia `ICommand` to `ICommandBase`
4. **Update all `AnonymousCommand`, `AnonymousTaskCommand`** to implement `ICommandBase` instead of WPF `ICommand`
5. **Update `ICommandSource` usage** — Avalonia has different `ICommandSource` API

## ViewModel Classes Inventory

### Core Editor ViewModels (Stride.Core.Assets.Editor.ViewModel)

| ViewModel | Base Class | Purpose |
|-----------|------------|---------|
| `SessionViewModel` | `DirtiableEditableViewModel` | Main session state, packages, assets |
| `EditorViewModel` | `ViewModelBase` | Editor service provider, status, MRU |
| `AssetViewModel` | `ViewModelBase` | Asset representation |
| `PackageViewModel` | `ViewModelBase` | Package representation |
| `ProjectViewModel` | `ViewModelBase` | Project representation |
| `AssetCollectionViewModel` | `ViewModelBase` | Asset collection state |
| `BackgroundTaskDescription` | `ViewModelBase` | Background task info |
| `EditorDialogService` | `ViewModelBase` | Dialog service |
| `StatusViewModel` | `ViewModelBase` | Status bar state |
| `TagsViewModel` | `ViewModelBase` | Asset tags |
| `AssetLogViewModel` | `ViewModelBase` | Asset log |
| `ActionHistoryViewModel` | `ViewModelBase` | Undo/redo history |

### Asset Editor ViewModels (Stride.Assets.Presentation)

| ViewModel | Base Class | Purpose |
|-----------|------------|---------|
| `UIEditorBaseViewModel` | `ViewModelBase` | UI editor base |
| `UIElementViewModel` | `ViewModelBase` | UI element in editor |
| `PanelViewModel` | `ViewModelBase` | UI panel element |
| `SpriteSheetEditorViewModel` | `ViewModelBase` | Sprite sheet editor |
| `VisualScriptEditorViewModel` | `ViewModelBase` | Visual script editor |
| `CurveEditorViewModel` | `ViewModelBase` | Curve editor |
| `GraphicsCompositorEditorViewModel` | `ViewModelBase` | Graphics compositor editor |
| `EntityHierarchyEditorViewModel` | `ViewModelBase` | Entity hierarchy editor |
| `GameEditorViewModel` | `ViewModelBase` | Game editor |

### Preview ViewModels (Stride.Assets.Presentation.Preview)

| ViewModel | Base Class | Purpose |
|-----------|------------|---------|
| `TexturePreviewViewModel` | `ViewModelBase` | Texture preview |
| `ModelPreviewViewModel` | `ViewModelBase` | 3D model preview |
| `AnimationPreviewViewModel` | `ViewModelBase` | Animation preview |
| `SoundPreviewViewModel` | `ViewModelBase` | Sound preview |
| `SpriteFontPreviewViewModel` | `ViewModelBase` | Sprite font preview |

## Platform-Agnostic Interfaces

The following interfaces are platform-agnostic and can be used directly in Avalonia:

| Interface | Namespace | Avalonia Equivalent |
|-----------|-----------|---------------------|
| `INotifyPropertyChanged` | `System.ComponentModel` | ✅ Same |
| `INotifyPropertyChanging` | `System.ComponentModel` | ✅ Same |
| `INotifyDataErrorInfo` | `System.ComponentModel` | Avalonia has `INotifyDataErrorInfo` |
| `IDataErrorInfo` | `System.ComponentModel` | Avalonia has `IDataErrorInfo` |
| `ICommand` | `System.Windows.Input` | ❌ Avalonia: `Avalonia.Input.ICommand` |
| `IValueConverter` | `System.Windows.Data` | Avalonia: `Avalonia.Data.Converters.IValueConverter` |

## Recommendation

1. **Abstract `ICommand`** — Create platform-agnostic `ICommandBase` interface in `Stride.Core.Presentation.Commands`
2. **Create `ICommandAdapter`** — WPF and Avalonia adapters for `ICommand`
3. **Update `AnonymousCommand`** — Implement `ICommandBase` instead of WPF `ICommand`
4. **Test ViewModel binding** — Ensure `PropertyChanged` events work correctly with Avalonia binding

## Open Questions

1. Does Avalonia's `ICommand` interface have all the methods needed for `AnonymousCommand` implementation?
2. Are there any WPF-specific `ICommandSource` usages that need Avalonia equivalents?
3. Should `IValueConverter` be abstracted similarly to `ICommand`?

---

**Next Task**: 0.3 — Identify third-party NuGet packages with WPF dependencies and research Avalonia equivalents
