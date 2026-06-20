# WPF API Inventory Report

**Generated**: 2026-06-20
**Scope**: `sources/editor/` directory (10 editor projects)

## Summary

| WPF API Pattern | Occurrences | Files Affected | Migration Impact |
|-----------------|-------------|----------------|------------------|
| **DependencyProperty / DependencyPropertyKey** | 243 | 49 files | 🔴 High — Core WPF property system, needs Avalonia `StyledProperty` / `AvaloniaProperty` |
| **ControlTemplate** | 209 | 21 files | 🔴 High — XAML templates, needs Avalonia `.axaml` templates |
| **ICommand / ICommandSource** | 222 | 71 files | 🟡 Medium — Avalonia has `ICommand`, but `ICommandSource` behavior differs |
| **Adorner / AdornerLayer** | 366 | 28 files | 🔴 High — Avalonia has no direct adorner equivalent; needs custom overlay approach |
| **HwndHost / HwndSource** | 7 | 6 files | 🟡 Medium — Avalonia has `NativeControlHost` (successor to HwndHost) |
| **D3DImage** | 1 | 1 file | 🔴 High — WPF DirectX interop; Avalonia uses `NativeControlHost` + platform rendering |
| **AttachedProperty / IAttachedObject** | 8 | 2 files | 🟢 Low — Avalonia has `AttachedProperty`, direct mapping |
| **AutomationPeer / AutomationProperties** | 0 | 0 files | ✅ None — Stride editor doesn't use WPF accessibility APIs |
| **INotifyDataErrorInfo / IDataErrorInfo** | 0 | 0 files | ✅ None — Stride uses custom validation patterns |

## Detailed Findings

### 1. DependencyProperty (243 occurrences, 49 files)

**Top affected files:**
- `Stride.Assets.Presentation\AssetEditors\UIEditor\ViewModels\PanelViewModel.cs` — 22
- `Stride.Core.Assets.Editor\View\AssetViewUserControl.xaml.cs` — 15
- `Stride.Core.Assets.Editor\View\Behaviors\DragDrop\DragDropBehavior.cs` — 13
- `Stride.Assets.Presentation\AssetEditors\UIEditor\Views\ThicknessEditor.cs` — 13

**Migration approach:**
- `DependencyProperty` → `AvaloniaProperty` (use `AvaloniaProperty.Register`)
- `DependencyPropertyKey` → `AvaloniaPropertyKey` (use `AvaloniaProperty.RegisterReadOnly`)
- `FrameworkPropertyMetadata` → `AvaloniaPropertyMetadata`
- `PropertyMetadata` → `AvaloniaPropertyMetadata`
- `CoerceValueCallback` → `AvaloniaProperty.CoerceValue`
- `ValidateValueCallback` → `AvaloniaProperty.ValidateValue`

### 2. ControlTemplate (209 occurrences, 21 files)

**Top affected files:**
- `Stride.GameStudio\Theme.AvalonDock.xaml` — 62 (AvalonDock theme XAML)
- `Stride.Assets.Presentation\Themes\Generic.xaml` — 28 (custom control styles)
- `Stride.Core.Assets.Editor\View\DefaultPropertyTemplateProviders.xaml` — 22
- `Stride.Assets.Presentation\AssetEditors\ScriptEditor\Resources\ThemeScriptEditor.xaml` — 18

**Migration approach:**
- Convert `.xaml` → `.axaml` (Avalonia XAML)
- `ControlTemplate` → `ControlTemplate` (Avalonia syntax differs slightly)
- `TemplateBinding` → `TemplateBinding` (same, but Avalonia uses `TemplateBinding` in XAML)
- `Binding` → `Binding` (same, but Avalonia binding syntax has differences)
- `RelativeSource` → `RelativeSource` (Avalonia supports `FindAncestor`, `Self`, `TemplatedParent`)

### 3. ICommand / ICommandSource (222 occurrences, 71 files)

**Top affected files:**
- `Stride.Core.Assets.Editor\ViewModel\SessionViewModel.cs` — 21
- `Stride.Core.Assets.Editor\ViewModel\AssetCollectionViewModel.cs` — 19
- `Stride.Assets.Presentation\AssetEditors\SpriteEditor\ViewModels\SpriteSheetEditorViewModel.cs` — 14

**Migration approach:**
- `ICommand` → Avalonia `ICommand` (same interface, `ICommand` is in `Avalonia.Input` namespace)
- `ICommandSource` → Avalonia `ICommandSource` (different implementation in `Avalonia.Interactivity`)
- `RelayCommand` → Avalonia `RelayCommand` (use `Avalonia.Controls.ApplicationModel.Command` or custom implementation)
- `CommandManager` → Avalonia `CommandManager` (Avalonia has limited `CommandManager` support)

### 4. Adorner / AdornerLayer (366 occurrences, 28 files)

**Top affected files:**
- `Stride.Assets.Presentation\AssetEditors\UIEditor\Game\UIEditorGameAdornerService.cs` — 85
- `Stride.Assets.Presentation\AssetEditors\UIEditor\Game\UIEditorGameAdornerService.Events.cs` — 43
- `Stride.Core.Assets.Editor\View\Behaviors\DragDrop\DragDropAdornerManager.cs` — 42
- `Stride.Core.Assets.Editor\View\Behaviors\TextBoxVectorPropertyValueValidationBehavior.cs` — 41

**Migration approach:**
- Avalonia does NOT have `AdornerLayer` or `Adorner` API
- **Alternative 1**: Use `AdornerLayer` equivalent via `AdornerDecorator` (third-party library)
- **Alternative 2**: Implement custom overlay using `Canvas` + `VisualTree` manipulation
- **Alternative 3**: Use `Popup` controls for visual feedback (drop indicators, sizing handles)
- **Alternative 4**: Use `AdornerDecorator` from `Avalonia.Adorners` community package

**Recommendation**: Implement custom overlay system using `Canvas` + `Popup` for UI Editor adorner functionality. This is the most maintainable approach and avoids third-party dependencies.

### 5. HwndHost / HwndSource (7 occurrences, 6 files)

**Affected files:**
- `Stride.Core.Assets.Editor\View\Behaviors\DragDrop\DragWindow.cs` — 2
- `Stride.Assets.Presentation\CurveEditor\Views\CurveEditorView.xaml.cs` — 1
- `Stride.Assets.Presentation\AssetEditors\VisualScriptEditor\Views\VisualScriptEditorView.xaml.cs` — 1
- `Stride.Assets.Presentation\AssetEditors\ScriptEditor\ScriptEditorView.xaml.cs` — 1
- `Stride.Assets.Presentation\AssetEditors\UIEditor\Views\UIEditorView.xaml.cs` — 1
- `Stride.Assets.Presentation\AssetEditors\GameEditor\Services\EditorGameController.cs` — 1

**Migration approach:**
- `HwndHost` → Avalonia `NativeControlHost` (built-in, same purpose)
- `HwndSource` → Avalonia `TopLevel` + `PlatformHandle` (use `TopLevel.GetTopLevel(this).PlatformHandle`)
- `HwndTarget` → Avalonia `VisualRoot` + `RenderRoot`

### 6. D3DImage (1 occurrence, 1 file)

**Affected file:**
- `Stride.GameStudio.AutoTesting\UITestHost.cs` — 1

**Migration approach:**
- `D3DImage` → Avalonia `NativeControlHost` + platform-specific rendering
- This is only used in auto-testing infrastructure, not in the main editor
- Can be replaced with Avalonia's rendering pipeline

### 7. AttachedProperty (8 occurrences, 2 files)

**Affected files:**
- `Stride.Core.Assets.Editor\Quantum\NodePresenters\Updaters\InlineMemberNodeUpdater.cs` — 6
- `Stride.Assets.Presentation\NodePresenters\Updaters\UIAssetNodeUpdater.cs` — 2

**Migration approach:**
- `AttachedProperty` → Avalonia `AttachedProperty` (direct mapping)
- `IAttachedObject` → Avalonia `IAttachedObject` (same interface)
- `AttachedProperty<T>` → `AvaloniaProperty.RegisterAttached<T>`

### 8. AutomationPeer (0 occurrences)

**No impact** — Stride editor does not use WPF accessibility APIs. Avalonia's accessibility support can be added incrementally.

### 9. Data Error Info (0 occurrences)

**No impact** — Stride uses custom validation patterns (`IValidatable`, `ValidationResult`). Avalonia's `INotifyDataErrorInfo` can be integrated if needed.

## Additional WPF-Specific Patterns Found

### Behaviors (Behavior<T>, Interaction)
- Many files use WPF `Behavior<T>` and `Interaction` patterns
- Avalonia has `Avalonia.Interactivity` with similar but not identical API
- Migration: Use `Avalonia.Interactivity.Behavior` or implement custom behavior system

### DataTemplates / DataTemplateSelector
- Multiple `DataTemplateSelector` implementations
- Avalonia has `DataTemplateSelector` but with different API
- Migration: Convert to Avalonia `DataTemplateSelector` or use `TemplateSelector`

### Style / StyleSelector
- Custom styles defined in XAML
- Avalonia has `Style` and `StyleSelector` with similar API
- Migration: Direct mapping with XAML syntax changes

### Converters (IValueConverter)
- Multiple value converters defined
- Avalonia has `IValueConverter` with same interface
- Migration: Direct mapping, XAML syntax changes only

### Triggers (EventTrigger, DataTrigger)
- Triggers defined in styles
- Avalonia has `Trigger` and `EventTrigger` with similar API
- Migration: Direct mapping with XAML syntax changes

## Files with Highest WPF Dependency

| File | WPF API Count | Priority |
|------|---------------|----------|
| `UIEditorGameAdornerService.cs` | 85 | 🔴 Critical |
| `Theme.AvalonDock.xaml` | 62 | 🔴 Critical |
| `DragDropAdornerManager.cs` | 42 | 🔴 Critical |
| `TextBoxVectorPropertyValueValidationBehavior.cs` | 41 | 🔴 Critical |
| `TextBoxPropertyValueValidationBehavior.cs` | 18 | 🔴 High |
| `Generic.xaml` | 28 | 🔴 High |
| `DefaultPropertyTemplateProviders.xaml` | 22 | 🔴 High |
| `SessionViewModel.cs` | 21 | 🟡 Medium |

## Migration Priority Assessment

### Phase 1 (Infrastructure) — Must Complete First
1. **AttachedProperty** — Foundation for control binding
2. **ICommand** — Command infrastructure
3. **DataTemplates / Converters** — Basic UI infrastructure

### Phase 2 (Core Components) — High Priority
1. **DependencyProperty** — Core property system
2. **ControlTemplate** — Theme and styling
3. **Adorner** — UI Editor visual feedback (requires custom implementation)

### Phase 3 (Asset Editors) — Medium Priority
1. **HwndHost** — 3D preview hosting
2. **D3DImage** — Auto-testing (can defer)

### Phase 4 (Game Studio) — Lower Priority
1. **AutomationPeer** — Not used, can add later
2. **Data Error Info** — Not used, custom validation

## Recommendations

1. **Create `Stride.Core.Presentation.Avalonia` project** with base classes for:
   - `AvaloniaProperty` wrappers
   - `AvaloniaCommand` implementations
   - `AvaloniaBehavior` system

2. **Implement custom adorner overlay system** for UI Editor:
   - Use `Canvas` + `Popup` for sizing handles
   - Use `Canvas` + `Popup` for drop indicators
   - Use `Canvas` + `Popup` for validation feedback

3. **Use Avalonia's `NativeControlHost`** for HWND embedding (replaces `HwndHost`)

4. **Convert XAML themes to Avalonia AXAML** with syntax updates

5. **Test behavior-by-behavior** — Each behavior migration should be validated independently

## Open Questions

1. Should we use a third-party adorner library (`Avalonia.Adorners`) or implement custom?
2. Does Avalonia's `NativeControlHost` support all `HwndHost` scenarios (multiple HWNDs, DPI scaling, input forwarding)?
3. Are there any WPF-specific behaviors that don't have Avalonia equivalents?
4. Should we create a `Stride.Core.Presentation.Avalonia` NuGet package for plugin developers?

---

**Next Task**: 0.2 — Document ViewModel interfaces and ensure they have no WPF dependency
