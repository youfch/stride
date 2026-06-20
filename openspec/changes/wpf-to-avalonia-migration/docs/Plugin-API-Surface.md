# Plugin API Surface Analysis — WPF to Avalonia Migration

> **Purpose**: Identify all public API surfaces that plugins consume from WPF projects,
> classify WPF-specific vs. platform-agnostic dependencies, and provide migration
> guidance for plugin developers during the Stride Editor's migration to Avalonia.

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Project Inventory](#2-project-inventory)
3. [Layer 0: Platform-Agnostic Core](#3-layer-0-platform-agnostic-core)
4. [Layer 1: ViewModel / Service Interfaces](#4-layer-1-viewmodel--service-interfaces)
5. [Layer 2: Editor Infrastructure (Platform-Agnostic)](#5-layer-2-editor-infrastructure-platform-agnostic)
6. [Layer 3: WPF-Specific UI Layer](#6-layer-3-wpf-specific-ui-layer)
7. [Plugin Extension Points](#7-plugin-extension-points)
8. [Breaking Change Analysis](#8-breaking-change-analysis)
9. [Migration Guide for Plugin Developers](#9-migration-guide-for-plugin-developers)
10. [Backward Compatibility Strategies](#10-backward-compatibility-strategies)
11. [Appendix: Complete API Surface Inventory](#11-appendix-complete-api-surface-inventory)

---

## 1. Executive Summary

The Stride Editor plugin system spans **5 major projects** across **3 architectural layers**:

| Layer | Project | WPF Deps? | Migration Impact |
|-------|---------|-----------|------------------|
| **Layer 0** — ViewModel/Service Interfaces | `Stride.Core.Presentation` | **No** | Minimal — stays stable |
| **Layer 1** — Editor Infrastructure | `Stride.Core.Assets.Editor` | Partial | Some renames, stable interfaces |
| **Layer 2** — Asset-Specific Editor | `Stride.Assets.Presentation` | Partial | Service interfaces stable; views change |
| **Layer 3** — WPF UI Controls | `Stride.Core.Presentation.Wpf` | **Yes — 100%** | Full replacement needed |
| **Layer 3** — Editor Host | `Stride.Editor` | **Yes** | Replacement needed |

**Key finding**: ~60% of the plugin-accessible API surface is **platform-agnostic** (interfaces, ViewModels, services) and can remain unchanged. ~40% is **WPF-specific** (controls, behaviors, converters, XAML) and requires migration.

---

## 2. Project Inventory

### 2.1 Stride.Core.Presentation (Layer 0)

| Attribute | Value |
|-----------|-------|
| **Path** | `sources/presentation/Stride.Core.Presentation/` |
| **TFM** | `$(StrideXplatEditorTargetFramework)` |
| **WPF** | ❌ No dependencies |
| **References** | `Stride.Core.Design`, `Stride.Core.Translation` |
| **Role** | Base ViewModel library, service interfaces, commands, dirtiables |
| **Migration** | ❄️ Stable — no changes expected |

### 2.2 Stride.Core.Presentation.Wpf (Layer 3)

| Attribute | Value |
|-----------|-------|
| **Path** | `sources/presentation/Stride.Core.Presentation.Wpf/` |
| **TFM** | `$(StrideEditorTargetFramework)` (WPF) |
| **WPF** | ✅ Enables `UseWPF=true`, depends on `Microsoft.Xaml.Behaviors.Wpf` |
| **References** | `Stride.Core.Presentation`, `Stride.Core.Translation.Presentation` |
| **Role** | All WPF controls, behaviors, converters, windows, services |
| **Migration** | 🔴 Full replacement — entire project maps to Avalonia equivalents |

### 2.3 Stride.Core.Assets.Editor (Layer 1 + 2)

| Attribute | Value |
|-----------|-------|
| **Path** | `sources/editor/Stride.Core.Assets.Editor/` |
| **WPF** | ⚠️ Partial — View layer is WPF, ViewModel/Services are not |
| **Role** | Plugin base classes, editor services, session/asset view models |
| **Migration** | 🟡 Service interfaces stable; view references need updating |

### 2.4 Stride.Assets.Presentation (Layer 2)

| Attribute | Value |
|-----------|-------|
| **Path** | `sources/editor/Stride.Assets.Presentation/` |
| **WPF** | ⚠️ Partial — asset editors, previews, views are WPF |
| **Role** | Asset-specific editors, node presenters, curve editor, templates |
| **Migration** | 🟡 Service interfaces stable; individual editors migrated per-asset |

### 2.5 Stride.Editor (Layer 2 + 3)

| Attribute | Value |
|-----------|-------|
| **Path** | `sources/editor/Stride.Editor/` |
| **WPF** | ⚠️ Partial — preview views are WPF, editor game services are not |
| **Role** | Plugin base class (`StrideAssetsPlugin`), preview system, editor game |
| **Migration** | 🟡 Plugin API stable; preview view infrastructure changes |

---

## 3. Layer 0: Platform-Agnostic Core

> **Migration Status**: ❄️ **No changes needed** — these types are platform-agnostic
> and will remain stable during and after migration.

### 3.1 ViewModel Base Classes

| Type | Base | Key Members | Notes |
|------|------|-------------|-------|
| `ViewModelBase` | `abstract` | `ServiceProvider`, `Destroy()`, `SetValue<T>(...)`, `DependentProperties`, `PropertyChanging/Changed` | Foundation of all ViewModels. `INotifyPropertyChanging`, `INotifyPropertyChanged`, `IDestroyable` |
| `DispatcherViewModel` | `ViewModelBase` | `IDispatcherService` integration | Thread-safe property changes via dispatcher |
| `EditableViewModel` | `DispatcherViewModel` | `IsEditing`, `BeginEdit()`, `EndEdit()`, `CancelEdit()` | Edit session support |
| `DirtiableEditableViewModel` | `EditableViewModel` | `IDirtiable` integration | Tracks dirty state for undo/redo |

### 3.2 Service Interfaces

| Interface | Purpose | Key Members |
|-----------|---------|-------------|
| `IDialogService` | Abstract dialog invocations | `MessageBoxAsync()`, `OpenFilePickerAsync()`, `OpenFolderPickerAsync()`, `SaveFilePickerAsync()`, `CheckedMessageBoxAsync()`, `HasMainWindow`, `Exit()` |
| `IDispatcherService` | Thread dispatching | `Invoke(Action)`, `Invoke<T>(Func<T>)`, `InvokeAsync(...)`, `CheckAccess()`, `EnsureAccess()` |
| `IUndoRedoService` | Undo/redo management | `CreateTransaction()`, `PushOperation()`, `Undo()`, `Redo()`, `NotifySave()`, `CanUndo/Redo` |
| `IViewModelServiceProvider` | Service container | `RegisterService()`, `UnregisterService()`, `TryGet<T>()`, `Get<T>()` |

### 3.3 Command Types

| Type | Base | Notes |
|------|------|-------|
| `ICommandBase` | `ICommand` | `IsEnabled`, `Execute()` (parameterless) |
| `CommandBase` | `DispatcherViewModel`, `ICommandBase` | Abstract command base |
| `AnonymousCommand` | `CommandBase` | Lambda-based command |
| `DisabledCommand` | `CommandBase` | Always-disabled command |

### 3.4 Dirtiable System

| Type | Notes |
|------|-------|
| `IDirtiable` | Interface for objects that track dirty state |
| `IDirtyingOperation` | Operation marker for undo/redo |
| `DirtiableManager` | Manages dirtiable object state |
| `PropertyChangeOperation` | Tracks property changes for undo |
| `CollectionChangeOperation` | Tracks collection changes for undo |
| `AnonymousDirtyingOperation` | Lambda-based dirty operation |

### 3.5 Collection Types

| Type | Notes |
|------|-------|
| `IObservableCollection` | Generic observable collection interface |
| `IObservableList` | Observable list interface |
| `IReadOnlyObservableCollection` | Read-only observable collection |
| `IReadOnlyObservableList` | Read-only observable list |
| `AutoUpdatingSortedObservableCollection` | Self-sorting observable collection |
| `SortedObservableCollection` | Manually-sorted observable collection |

### 3.6 Core Utilities

| Type | Notes |
|------|-------|
| `AnonymousComparer` | Lambda-based comparer |
| `NaturalStringComparer` | Natural sort comparer |
| `DialogButtonInfo` | Dialog button configuration |
| `FilePickerFilter` | File picker filter struct |
| `CheckedMessageBoxResult` | Checked message box result |
| `MessageBoxButton` | Button configuration enum |
| `MessageBoxImage` | Image/icon enum |
| `MessageBoxResult` | Result enum |

### 3.7 Extension Methods

| Class | Key Methods |
|-------|-------------|
| `ClassFieldExtensions` | Field reflection helpers |
| `ObjectExtensions` | Object utility extensions |
| `StringExtensions` | String utility extensions |
| `TypeExtensions` | Type reflection helpers |

---

## 4. Layer 1: ViewModel / Service Interfaces

> **Migration Status**: 🟡 **Minimal changes** — interfaces stay stable, but some
> WPF-specific service interfaces (extensions of base interfaces) need Avalonia
> reimplementations.

### 4.1 Editor Service Interfaces (Stride.Core.Assets.Editor.Services)

| Interface | Extends | Key Members | WPF? |
|-----------|---------|-------------|------|
| `IAssetsPluginService` | — | `Plugins`, `GetAssetViewModelType()`, `GetEditorViewType()`, `RegisterSession()` | ❌ |
| `IAssetEditorsManager` | — | `OpenAssetEditorWindow()`, `CloseAssetEditorWindow()`, `OpenCurveEditorWindow()` | ❌ |
| `IAssetPreviewService` | `IDisposable` | `SetAssetToPreview()`, `GetCurrentPreviewView()`, `PreviewAssetUpdated` | ❌ |
| `IBuildService` | — | `AssetBuilt` event | ❌ |
| `IAssetCreationView` | — | Asset creation dialog interface | ❌ |
| `IAssetPickerDialog` | `IModalDialog` | Asset picking dialog | ✅ |
| `IAssetDependencyManager` | — | Dependency analysis | ❌ |

**Editor dialog chain** (WPF interfaces — need Avalonia equivalents):

```
IDialogService (Layer 0, stable)
  └── IDialogService2 (Wpf project, adds BlockingMessageBox)
       └── IEditorDialogService (Assets.Editor, adds ShowNotificationWindow, CreateAddAssetDialog, CreateAssetPickerDialog...)
            └── IStrideDialogService (GameStudio, adds CreateCredentialsDialog, ShowAboutPage)
```

### 4.2 Editor ViewModel Types (Stride.Core.Assets.Editor.ViewModel)

| Type | Base | Notes |
|------|------|-------|
| `SessionViewModel` | `ViewModelBase` | Top-level session VM. Properties: `CurrentProject`, `LocalPackages`, `ServiceProvider` |
| `EditorViewModel` | `ViewModelBase` | Base editor VM |
| `AssetViewModel` | `DirtiableEditableViewModel` | Base asset VM |
| `AssetEditorViewModel` | `EditorViewModel` | Base editor for a single asset |
| `IAssetEditorViewModel` | — | Interface for asset editors |
| `PackageViewModel` | `AssetViewModel` | Package (project) VM |
| `DirectoryBaseViewModel` | `SessionObjectViewModel` | Directory in session tree |
| `DirectoryViewModel` | `DirectoryBaseViewModel` | Concrete directory |
| `ProjectViewModel` | `DirectoryBaseViewModel` | Project root |
| `AssetCollectionViewModel` | `SessionObjectViewModel` | Collection of assets |
| `SessionObjectViewModel` | `DirtiableEditableViewModel` | Base for session tree items |
| `MountPointViewModel` | `DirectoryBaseViewModel` | Mount point |
| `AssetMountPointViewModel` | `MountPointViewModel` | Asset mount point |
| `CategoryViewModel` | `SessionObjectViewModel` | Category grouping |
| `ContentReferenceViewModel` | `ViewModelBase` | Content reference wrapper |
| `ThumbnailsViewModel` | `ViewModelBase` | Thumbnail management |
| `LogsViewModel` | `ViewModelBase` | Log viewer VM |

### 4.3 Editor EditorGame Services (Stride.Editor)

| Type | Base | Notes |
|------|------|-------|
| `IEditorGameService` | `IAsyncDisposable` | Editor game service contract |
| `EditorGameServiceBase` | `IEditorGameService` | Convenience base class |
| `EditorGameServiceRegistry` | — | Service container for editor game |
| `EditorServiceGame` | `EmbeddedGame` | Game host for editor |
| `IEditorGameViewModelService` | — | Marker interface for VM→game services |
| `IEditorContentLoader` | `IDisposable` | Runtime asset loading/unloading |

### 4.4 Editor Service Implementations — WPF Refs

These service implementations (in `Stride.Editor`, `Stride.GameStudio`) depend on WPF:

| Service | WPF Dependency | Migration |
|---------|---------------|-----------|
| `GameStudioBuilderService` | None (engine-side) | ❄️ No impact |
| `GameStudioPreviewService` | None | ❄️ No impact |
| `GameStudioThumbnailService` | None | ❄️ No impact |
| `GameSettingsProviderService` | None | ❄️ No impact |
| `PluginService` | `ITemplateProvider` (WPF type) | 🔴 Needs IPlatformTemplateProvider |
| `StrideDialogService` | WPF dialogs | 🔴 Reimplement for Avalonia |
| `StrideEditorPlugin` | `ResourceDictionary` | 🔴 Remove XAML resource path |

---

## 5. Layer 2: Editor Infrastructure (Platform-Agnostic)

> **Migration Status**: 🟡 **Service interfaces stable** but implementations need to
> be ported. Asset-specific editor Views need Avalonia equivalents.

### 5.1 Plugin Base Class Hierarchy

```
AssetsPlugin (Layer 0, abstract)
  └── AssetsEditorPlugin (abstract, adds editor view registration)
       └── StrideAssetsPlugin (Stride.Editor, abstract, adds resource dicts, preview factories)
            └── StrideEditorPlugin (GameStudio, concrete, internal)
            └── StrideDefaultAssetsPlugin (Asserts.Presentation, concrete)
```

**`AssetsPlugin` — Key virtual methods:**

```csharp
public abstract class AssetsPlugin {
    public abstract void InitializePlugin(ILogger logger);
    public abstract void InitializeSession(SessionViewModel session);
    public abstract void RegisterPrimitiveTypes(ICollection<Type> primitiveTypes);
    public void RegisterAssetViewModelTypes(IDictionary<Type, Type> types); // scans assembly
    protected internal virtual void SessionLoaded(SessionViewModel session);
    protected internal virtual void SessionDisposed(SessionViewModel session);
}
```

**`AssetsEditorPlugin` — Additional virtual methods:**

```csharp
public abstract class AssetsEditorPlugin : AssetsPlugin {
    public abstract void RegisterAssetPreviewViewModelTypes(IDictionary<Type, Type> types);
    public abstract void RegisterAssetPreviewViewTypes(IDictionary<Type, Type> types);
    public abstract void RegisterEnumImages(IDictionary<object, object> enumImages);
    public abstract void RegisterCopyProcessors(ICollection<ICopyProcessor> processors, SessionViewModel session);
    public abstract void RegisterPasteProcessors(ICollection<IPasteProcessor> processors, SessionViewModel session);
    public abstract void RegisterPostPasteProcessors(ICollection<IAssetPostPasteProcessor> processors, SessionViewModel session);
    public abstract void RegisterTemplateProviders(ICollection<ITemplateProvider> providers);
}
```

**`StrideAssetsPlugin` — Additional virtual methods:**

```csharp
public abstract class StrideAssetsPlugin : AssetsEditorPlugin {
    protected abstract void Initialize(ILogger logger);
    protected virtual void RegisterResourceDictionary(ResourceDictionary dictionary); // WPF-specific!
}
```

### 5.2 Preview System (Stride.Editor.Preview)

| Type | Base | Notes |
|------|------|-------|
| `IAssetPreview` | — | `Initialize()`, `Update()`, `Dispose()`, `RenderingMode` |
| `AssetPreview` | `IAssetPreview` | Abstract base with `CreatePreviewScene()`, `GetGraphicsCompositor()` |
| `AssetPreview<T>` | `AssetPreview` | Typed preview (`T : Asset`) |
| `IAssetPreviewViewModel` | — | `AttachPreview(IAssetPreview)` |
| `IPreviewView` | — | `InitializeView(IPreviewBuilder, IAssetPreview)` — **WPF-specific** |
| `IPreviewBuilder` | — | `Compile(AssetItem)`, `GetStrideView()` |

### 5.3 Annotations (Attribute-Based Registration)

| Attribute | Targets | Purpose |
|-----------|---------|---------|
| `[AssetViewModelAttribute(Type)]` | `AssetViewModel` subclass | Maps asset type → ViewModel |
| `[AssetEditorViewModelAttribute(Type)]` | `IAssetEditorViewModel` impl | Maps VM → editor VM |
| `[AssetEditorViewAttribute(Type)]` | `IEditorView` impl | Maps editor VM → view |
| `[AssetPreviewAttribute<TAsset>]` | `IAssetPreview` impl | Maps asset → preview |
| `[AssetPreviewViewModelAttribute<TPreview>]` | `IAssetPreviewViewModel` impl | Maps preview → preview VM |
| `[AssetPreviewViewAttribute<TPreview>]` | `IPreviewView` impl | Maps preview → preview view |

### 5.4 Template Providers (Stride.Core.Presentation.View)

| Type | Base | WPF? |
|------|------|------|
| `ITemplateProvider` | `IComparable<ITemplateProvider>` | ✅ `DataTemplate Template` |
| `TemplateProviderBase` | `DependencyObject`, `ITemplateProvider` | ✅ `DependencyObject`, `DataTemplate` |
| `TemplateProviderSelector` | `DataTemplateSelector` | ✅ WPF template selector |
| `OverrideRule` | `enum` | ✅ Used by WPF template resolution |

### 5.5 Quantum Integration (Stride.Core.Presentation.Quantum)

| Type | Notes |
|------|-------|
| `GraphViewModelService` | Quantum graph service |
| `GraphNodeBinding` | Binds graph node to ViewModel |
| `MemberGraphNodeBinding` | Member-specific binding |
| `ObjectGraphNodeBinding` | Object-level binding |
| `IPropertyProviderViewModel` | Interface for property providers |
| `CombineMode` | Merge strategy enum |

---

## 6. Layer 3: WPF-Specific UI Layer

> **Migration Status**: 🔴 **Full replacement needed** — all types below are WPF-specific
> and have no direct equivalent in Avalonia.

### 6.1 Controls (47 total)

| Control | Base | WPF Dependencies | Avalonia Equivalent |
|---------|------|-----------------|-------------------|
| `PropertyView` | `ItemsControl` | `DependencyProperty`, `RoutedEvent` | `ItemsControl` |
| `PropertyViewItem` | `ExpandableItemsControl` | `DependencyProperty`, `RoutedEvent` | `HeaderedItemsControl` |
| `TreeView` | `ItemsControl` | `DependencyProperty`, `RoutedEvent` | `TreeView` |
| `TreeViewItem` | `ExpandableItemsControl` | `DependencyProperty` | `TreeViewItem` |
| `ExpandableItemsControl` | `HeaderedItemsControl` | `DependencyProperty`, `RoutedEvent` | `Expander` |
| `NumericTextBox` | `TextBoxBase` | `DependencyProperty`, `RoutedEvent` | `NumericUpDown` |
| `TextBox` | `TextBoxBase` | `DependencyProperty` | `TextBox` |
| `TextBoxBase` | `TextBox` | `DependencyProperty`, `RoutedCommand` | `TextBox` |
| `FilteringComboBox` | `Selector` | `DependencyProperty`, `RoutedEvent` | `ComboBox` + search |
| `ColorPicker` | `Control` | `DependencyProperty` | `ColorPicker` |
| `GameEngineHost` | `FrameworkElement` | `IWin32Window`, `IKeyboardInputSink` | `NativeControlHost` |
| `MarkdownTextBlock` | `Control` | `DependencyProperty` | Markdown control |
| `ScaleBar` | `FrameworkElement` | `DependencyProperty`, `RoutedEvent` | Custom drawing |
| `TextLogViewer` | `Control` | `DependencyProperty` | Custom control |
| `ModalWindow` | `Window` | WPF Window | `Window` |
| `KeyValueGrid` | `Grid` | Attached DP | `Grid` |
| `VirtualizingTilePanel` | `VirtualizingPanel` | `IScrollInfo` | Virtualizing panel |
| `VirtualizingTreePanel` | `VirtualizingPanel` | `IScrollInfo` | Virtualizing panel |
| `VectorEditorBase` | `Control` | `DependencyProperty` | Custom control |
| `VectorEditorBase<T>` | `VectorEditorBase` | `DependencyProperty` | Custom control |
| `VectorEditor<T>` | `VectorEditorBase<T>` | `DependencyProperty` | Custom control |
| Other editors (7 int/float/rect editors) | Various | `DependencyProperty` | Custom controls |
| `AngleEditor`, `DateTimeEditor` | Various | `DependencyProperty` | Custom controls |
| `MatrixEditor` | Various | `DependencyProperty` | Custom control |
| `RotationEditor` | Various | `DependencyProperty` | Custom control |
| `SearchComboBox` | Various | `DependencyProperty` | Custom control |
| `TagControl` | Various | `DependencyProperty` | Custom control |
| `TextBlockFormatting` | Various | `DependencyProperty` | Custom control |
| `TimeSpanEditor` | Various | `DependencyProperty` | Custom control |
| `Trimming` | Various | `DependencyProperty` | Custom control |
| `UnitSystem` | Various | `DependencyProperty` | Custom control |

### 6.2 Behaviors (37 total)

All 37 behaviors inherit from `Microsoft.Xaml.Behaviors.Behavior<T>` or `TriggerAction<T>`.
Full list: `ActivateOnCollectionChangedBehavior`, `BindableSelectedItemsBehavior`, 
`BindCurrentToolTipStringBehavior`, `ButtonCloseWindowBehavior`, `ChangeCursorOnSliderThumbBehavior`,
`CharInputBehavior`, `CloseWindowBehavior`, `CommandBindingBehavior`, `ContainTextAdornerBehavior`,
`DeferredBehaviorBase`, `DoubleClickCloseWindowBehavior`, `DragOverAutoScrollBehavior`,
`DropCommandParameters`, `HyperlinkCloseWindowBehavior`, `ItemsControlCollectionViewBehavior`,
`ListBoxBindableSelectedItemsBehavior`, `MenuItemCloseWindowBehavior`, `MouseMoveCaptureBehaviorBase`,
`MultiOverrideCursorBehavior`, `NumericTextBoxDragBehavior`, `NumericTextBoxTransactionalRepeatButtonsBehavior`,
`OnEventBehavior`, `OnEventCommandBehavior`, `OnEventSetPropertyBehavior`, `OnFocusBindingInterruptionBehavior`,
`OnMouseEventBehavior`, `OnPropertyChangedCommandBehavior`, `OverrideCursorBehavior`, `ResizeBehavior`,
`SelectionRectangleBehavior`, `SetFocusOnLoadBehavior`, `SliderDragFromTrackBehavior`,
`TextBoxCloseWindowBehavior`, `TextBoxKeyUpCommandBehavior`, `TilePanelNavigationBehavior`,
`ToggleButtonPopupBehavior`.

**Avalonia supports**: `Avalonia.Interactivity` behaviors via `Interaction.Behaviors`.
Many behaviors can be ported 1:1, but window-close behaviors, Win32 interop, and cursor
behaviors need rethinking.

### 6.3 Value Converters (71 total)

All implement `IValueConverter` or `IMultiValueConverter` from WPF.

**Category breakdown:**
- **Boolean logic** (11): `InvertBool`, `BoolToParam`, `IntToBool`, `NumericToBool`, 
  `ObjectToBool`, `EmptyStringToBool`, `AndMultiConverter`, `OrMultiConverter`, 
  `XOrMultiConverter`, `IsEqualToParam`, `StringEquals`
- **Math/Number** (12): `CompareNum`, `MaxNum`, `MinNum`, `SumNum`, `SumSize`, 
  `SumThickness`, `Multiply`, `MultiplyMultiConverter`, `SumMultiConverter`, 
  `ToDouble`, `NumericToSize`, `NumericToThickness`
- **String/Text** (7): `FormatString`, `JoinStrings`, `StringConcat`, `ToLower`, 
  `TrimString`, `Take`, `CamelCaseTextConverter`
- **Type reflection** (7): `ObjectToType`, `ObjectToTypeName`, `ObjectToFullTypeName`, 
  `TypeToTypeName`, `TypeToNamespace`, `MatchType`, `UnderlyingType`
- **Stride-specific** (7): `UFileToString`, `UFileToFileName`, `UFileToFileNameWithExt`, 
  `UFileToUri`, `UDirectoryToString`, `ColorConverter`, `DegreeAngleSingle`
- **Visibility** (2): `VisibleOrCollapsed`, `VisibleOrHidden`
- **Chaining** (3): `Chained`, `MultiChained`, `MultiBindingToTuple`
- **Other** (22): `AllEqualMultiConverter`, `CountEnumerable`, `DateTimeToString`, 
  `EnumToDisplayName`, `EnumValues`, `ExtendedOrSingle`, `ItemToIndex`, 
  `NotSupportedTypeToTypeName`, `NullToUnset`, `StaticResourceConverter`, 
  `TextToMarkdownFlowDocumentConverter`, `ThicknessMultiConverter`, 
  `ValueToUnset`, `VectorEditingModeToBoolean`, `Yield`, `CharToString`, 
  `CharToUnicode`, `FormatString`, `ConverterHelper`

**Avalonia supports**: `IValueConverter` natively with identical API. All converters
can be ported 1:1 with minimal changes (namespace/assembly updates).

### 6.4 WPF Services

| Interface/Class | Notes |
|----------------|-------|
| `IDialogService2` | Extends `IDialogService` with blocking variants |
| `IModalDialog` | Modal dialog interface |
| `IModalDialogInternal` | Internal modal dialog contract |
| `IFileModalDialog` | File dialog interface |
| `IFileOpenModalDialog` | Open file dialog |
| `IFileSaveModalDialog` | Save file dialog |
| `IFolderOpenModalDialog` | Folder picker dialog |
| `DispatcherService` | WPF dispatcher implementation |
| `DialogResult` | Enum: `None`, `Ok`, `Cancel` |
| `FileDialogFilter` | Filter struct |
| `WindowManager` | Singleton window manager |
| `IAsyncClosableWindow` | Async window close contract |

### 6.5 WPF Window System

| Type | Base | Notes |
|------|------|-------|
| `ModalWindow` | `Window` | Abstract modal dialog base |
| `PopupModalWindow` | `Window` | Popup modal variant |
| `MessageDialogBase` | `ModalWindow` | Base for message boxes |
| `MessageBox` | `MessageDialogBase` | MessageBox implementation |
| `CheckedMessageBox` | `MessageDialogBase` | Checked message box |
| `WindowManager` | `IDisposable` | Window lifecycle management |
| `NativeHelper` | `static` | P/Invoke to user32.dll |
| `SafeClipboard` | `static` | Thread-safe clipboard |
| `ClipboardMonitor` | `static` | Clipboard change monitoring |

### 6.6 XAML Themes and Resources

| File | Content |
|------|---------|
| `Themes/generic.xaml` | Default theme for all custom controls |
| `Themes/ThemeSelector.xaml` | Theme selection |
| `Resources/Images/*.png` | Embedded icon resources |
| `Resources/Cursors/*.cur` | Custom cursors |
| `Stride.Core.Presentation.Wpf\View\DefaultPropertyTemplateProviders.xaml` | Default template registrations in XAML |

---

## 7. Plugin Extension Points

### 7.1 Complete Plugin API Surface

**To create a plugin, a developer must:**

```csharp
// 1. Create plugin class
public class MyPlugin : StrideAssetsPlugin  // or AssetsPlugin / AssetsEditorPlugin
{
    protected override void Initialize(ILogger logger) { }
    public override void InitializeSession(SessionViewModel session) { }
}

// 2. Optionally register through assembly attributes
[assembly: AssetViewModelAttribute(typeof(MyAsset), typeof(MyAssetViewModel))]
[assembly: AssetEditorViewModelAttribute(typeof(MyAssetViewModel), typeof(MyAssetEditorViewModel))]
[assembly: AssetEditorViewAttribute(typeof(MyAssetEditorViewModel), typeof(MyAssetEditorView))]

// 3. Register preview
[AssetPreviewAttribute<MyAsset>]
public class MyAssetPreview : AssetPreview<MyAsset> { }

// 4. Implement editor game service
public class MyEditorService : EditorGameServiceBase { ... }
```

### 7.2 Extension Points by Layer

| Layer | Extension Point | Mechanism | Migration Impact |
|-------|----------------|-----------|-----------------|
| 0 | `AssetsPlugin` subclass | Inheritance | ❄️ Stable |
| 0 | `IAssetPreview` implementation | Interface | ❄️ Stable |
| 0 | `IEditorGameService` implementation | Interface | ❄️ Stable |
| 0 | Asset ViewModel registration | `[AssetViewModel]` attribute | ❄️ Stable |
| 0 | Copy/Paste processors | Interface registration | ❄️ Stable |
| 0 | Primitive type registration | `RegisterPrimitiveTypes()` | ❄️ Stable |
| 1 | `AssetsEditorPlugin` subclass | Inheritance | ❄️ Stable |
| 1 | Editor VM registration | `[AssetEditorViewModel]` attribute | ❄️ Stable |
| 1 | Editor View registration | `[AssetEditorView]` attribute | 🟡 Returns `Type`, not instance |
| 1 | Preview VM registration | `[AssetPreviewViewModel]` attribute | ❄️ Stable |
| 1 | Preview View registration | `[AssetPreviewView]` attribute | 🟡 View type changes |
| 1 | Enum images | `RegisterEnumImages()` | 🟡 Image source format changes |
| 1 | Template providers | `RegisterTemplateProviders()` | 🔴 `ITemplateProvider` is WPF |
| 2 | `StrideAssetsPlugin` subclass | Inheritance | ❄️ Stable |
| 2 | Resource dicts | `RegisterResourceDictionary()` | 🔴 XAML/WPF only |
| 2 | Preview factories | `AssetPreviewFactory` delegate | ❄️ Stable |
| 3 | Custom controls | WPF control inheritance | 🔴 Full rewrite |
| 3 | Behaviors | Behavior<T> inheritance | 🟡 Port to Avalonia behaviors |
| 3 | Converters | IValueConverter | 🟡 Trivial port |
| 3 | Dialogs | IModalDialog | 🔴 Avalonia dialog system |
| 3 | Templates | DataTemplate/XAML | 🔴 Avalonia templates |

---

## 8. Breaking Change Analysis

### 8.1 High-Impact Breaking Changes (Plugin Code MUST Change)

| Breaking Change | Affected Projects | Count | Migration |
|----------------|-------------------|-------|-----------|
| `ITemplateProvider` with `DataTemplate` | All plugins with custom editors | **High** | Create `IAvaloniaTemplateProvider` or use `IDataTemplate` |
| `TemplateProviderSelector` (WPF) | All plugins with template registration | **High** | Replace with Avalonia `DataTemplate` system |
| `IModalDialog` / `ModalWindow` | All plugins with custom dialogs | **High** | Use Avalonia `Window` + async show |
| `IDialogService2.BlockingMessageBox()` | Dialogs | **Medium** | Remove blocking variants; async only |
| `IModalDialogInternal` | Internal dialog handling | **Medium** | Replace with Avalonia pattern |
| `WindowManager` | Window lifecycle | **Medium** | Use Avalonia `Window` lifetime |
| `IDispatcherService.Invoke()` (sync) | UI thread access | **Medium** | Use `Dispatcher.UIThread.InvokeAsync()` |
| `DispatcherService` (WPF impl) | Dispatcher | **Medium** | New Avalonia `DispatcherService` |
| `ResourceDictionary` in plugins | XAML resources | **High** | Use Avalonia `ResourceDictionary` |
| `RegisterResourceDictionary(ResourceDictionary)` | `StrideAssetsPlugin` subclasses | **High** | Remove; use Avalonia merging |
| `GameEngineHost` (IWin32Window) | Preview / game embedding | **High** | Use `NativeControlHost` |
| `IKeyboardInputSink` | Keyboard routing | **High** | Avalonia `TopLevel` input |
| `EmbeddedGameForm` (WinForms) | Game embedding | **High** | Remove WinForms dependency |
| `NativeHelper` (P/Invoke) | Window management | **Medium** | Use Avalonia platform API |
| Clipboard interop | Clipboard | **Low** | Use `TopLevel.Clipboard` |

### 8.2 Medium-Impact Changes (Plugin Code May Need Changes)

| Breaking Change | Affected Projects | Migration |
|----------------|-------------------|-----------|
| `DataTemplate` → `IDataTemplate` | Template providers | Change type reference |
| `Style` (WPF) → `Style` (Avalonia) | All XAML | Most properties compatible |
| `IValueConverter` → `IValueConverter` | Converters | Identical interface |
| `IMultiValueConverter` → `IMultiValueConverter` | Multi-converters | Identical interface |
| `DependencyProperty` → `StyledProperty` | Control authors | API is similar |
| `RoutedEvent` → `RoutedEvent` | Events | Avalonia has routed events |
| `INotifyPropertyChanged` | All VMs | Same .NET interface |
| `ICommand` | All commands | Same .NET interface |

### 8.3 Low-Impact Changes (Transparent)

| Change | Notes |
|--------|-------|
| Namespace changes `…Wpf` → `…Avalonia` | Search-and-replace |
| Assembly name changes | New NuGet package references |
| XAML namespace changes | xmlns URI updates |
| Resource key format | Different key types |

### 8.4 Changes Requiring No Plugin Action

| API | Reason |
|-----|--------|
| `ViewModelBase` hierarchy | Pure C#, no UI dependency |
| `IViewModelServiceProvider` | Platform-agnostic |
| `IDialogService` (base) | Interface unchanged; new impl |
| `IUndoRedoService` | Platform-agnostic |
| `IDispatcherService` (interface) | Interface unchanged; new impl |
| `ICommandBase`, `CommandBase` | No WPF dependency |
| Asset ViewModel types | No UI dependency |
| `IAssetsPluginService` | Platform-agnostic |
| `IAssetEditorsManager` | Platform-agnostic |
| `IBuildService` | Platform-agnostic |
| `IEditorGameService` | Engine-side only |
| `EditorGameServiceBase` | Engine-side only |
| `IEditorContentLoader` | Engine-side only |
| `IAssetPreview` | Platform-agnostic |
| `AssetPreview` base class | Platform-agnostic |
| `IPreviewBuilder` | Platform-agnostic (except `GetStrideView()`) |
| `IThumbnailCompiler` | Platform-agnostic |
| All annotation attributes | Platform-agnostic |
| Quantum integration types | Platform-agnostic |
| Dirtiable system | Platform-agnostic |
| Collection types | Platform-agnostic |
| `FilePickerFilter` | Data-only struct |

---

## 9. Migration Guide for Plugin Developers

### Phase 1: Audit Your Plugin Dependencies

1. **Check your project references**
   - Replace `Stride.Core.Presentation.Wpf` → `Stride.Core.Presentation.Avalonia`
   - Remove `Microsoft.Xaml.Behaviors.Wpf` → Add Avalonia `Interactivity`
   - Remove `.xaml` pages from csproj; use Avalonia `.axaml`

2. **Identify WPF-specific imports**
   ```csharp
   // 🔴 WPF-specific — needs replacement
   using System.Windows;
   using System.Windows.Controls;
   using System.Windows.Data;
   using System.Windows.Input;  // only ICommand safe
   using System.Windows.Media;
   
   // 🟢 Platform-agnostic — can stay
   using Stride.Core.Presentation.ViewModels;
   using Stride.Core.Presentation.Services;
   using Stride.Core.Assets.Editor.Services;
   using Stride.Core.Assets.Editor.ViewModel;
   ```

### Phase 2: Replace View Layer

3. **Convert controls**
   - `ITemplateProvider` → Use Avalonia `DataTemplate` / `IDataTemplate`
   - `TemplateProviderBase` → `TargetType`-based templates
   - Custom WPF controls → Custom Avalonia controls
   - Behaviors → Port to Avalonia `Behavior<T>`
   - Converters → Keep `IValueConverter` (same API)

4. **Replace dialogs**
   - `ModalWindow` → Avalonia `Window`
   - `IModalDialog.ShowModal()` → `Window.ShowDialog()`
   - Remove `IDialogService2.BlockingMessageBox()` calls
   - `IEditorDialogService.ShowNotificationWindow()` → Use Avalonia notification

5. **Update XAML**
   - Convert `.xaml` → `.axaml`
   - `DependencyProperty` → `StyledProperty`
   - `{Binding}` → `{Binding}` (syntax similar)
   - `{StaticResource}` → `{StaticResource}`
   - Validate all template keys and resource references

### Phase 3: Update Plugin Registration

6. **Remove WPF-specific registration**
   ```csharp
   // REMOVE
   protected override void RegisterResourceDictionary(ResourceDictionary dictionary) { }

   // REMOVE XAML resource references
   ```

7. **Migrate template providers**
   ```csharp
   // BEFORE (WPF)
   public override void RegisterTemplateProviders(ICollection<ITemplateProvider> providers) {
       providers.Add(new MyTemplateProvider());
   }
   
   // AFTER (Avalonia)
   // Templates registered via XAML with DataTemplate.ContentType
   ```

8. **Update enum images**
   ```csharp
   // BEFORE
   public override void RegisterEnumImages(IDictionary<object, object> enumImages) {
       enumImages.Add(MyEnum.Value, new BitmapImage(new Uri(...)));
   }
   // AFTER — use Avalonia IImage or drawing
   ```

### Phase 4: Rebuild and Test

9. **Validate compilation** — fix any remaining WPF references
10. **Test editor integration** — asset editors, previews, property grid
11. **Test game integration** — editor game services, content loading
12. **Test undo/redo** — verify dirtiable/transaction system

---

## 10. Backward Compatibility Strategies

### Strategy A: Abstraction Layer (Recommended)

Introduce an abstraction layer between plugin code and UI framework:

```
Plugin Code
    ↓ uses
Abstraction Layer (Stride.Core.Presentation.Abstractions)
    ├── IPlatformTemplateProvider (no WPF dependency)
    ├── IPlatformDialogService (unified async dialog API)
    ├── IPlatformDispatcher (UI thread dispatch)
    ├── IPlatformWindow (window lifecycle)
    └── IPlatformClipboard (clipboard access)
    ↓ implemented by
WPF Implementation (Stride.Core.Presentation.Wpf — deprecated)
Avalonia Implementation (Stride.Core.Presentation.Avalonia — new)
```

**Benefits**:
- Plugins compiled once, work on both WPF and Avalonia during transition
- Clean migration path — swap implementation assemblies
- No conditional compilation in plugin code

**Cost**: ~10-15 new interfaces, ~2-3 weeks dev time

### Strategy B: Compatibility Shim Layer

Provide WPF-compatible API wrappers in Avalonia:

```
Plugin Code (uses old WPF APIs)
    ↓
Compatibility Shim (Stride.Core.Presentation.Compat)
    ├── IDataTemplate → wraps Avalonia DataTemplate
    ├── DispatcherService → wraps Avalonia Dispatcher
    ├── ModalWindow → wraps Avalonia Window
    └── etc.
```

**Benefits**:
- Minimal plugin changes
- Plugins can migrate gradually

**Cost**: Shim complexity grows with each WPF API; long-term maintenance burden

### Strategy C: Major Version Break

Update all plugin interfaces at once. Provide a migration document and migration tool.

**Benefits**:
- Clean break, no legacy code
- Encourages modernization

**Cost**: All plugins must update simultaneously; community friction

### Strategy D: Conditional Compilation

```csharp
#if AVALONIA
    // Avalonia-specific code
#else
    // WPF-specific code
#endif
```

**Benefits**: Single plugin assembly for both platforms
**Cost**: Messy code; not recommended for large plugins

### Recommendation: Strategy A + Strategy C

- **Short term** (during migration): Provide abstraction layer so plugins can work
  on both WPF (legacy) and Avalonia (new) during the transition period.
- **Long term** (post-migration): Remove WPF support, promote a clean API break.
  Provide migration tools and documentation.

---

## 11. Appendix: Complete API Surface Inventory

### 11.1 Platform-Agnostic APIs (Stride.Core.Presentation)

```
Services/
├── IDialogService.cs             — interface (stable)
├── IDispatcherService.cs          — interface (stable)
├── IUndoRedoService.cs            — interface (stable)
├── FilePickerFilter.cs            — struct (stable)
├── CheckedMessageBoxResult.cs     — class (stable)
├── MessageBoxButton.cs            — enum (stable)
├── MessageBoxImage.cs             — enum (stable)
├── MessageBoxResult.cs            — enum (stable)
├── NullDispatcherService.cs       — class (null object, stable)
├── UndoRedoService.cs             — class (stable)
├── DummyTransaction.cs            — class (stable)

ViewModels/
├── ViewModelBase.cs               — abstract class (stable)
├── DispatcherViewModel.cs         — abstract class (stable)
├── EditableViewModel.cs           — abstract class (stable)
├── DirtiableEditableViewModel.cs   — abstract class (stable)
├── IViewModelServiceProvider.cs    — interface (stable)
├── ViewModelServiceProvider.cs     — class (stable)
├── NullServiceProvider.cs          — class (stable)
├── ServiceRegistrationEventArgs.cs — class (stable)

Commands/
├── ICommandBase.cs                 — interface (stable)
├── CommandBase.cs                  — abstract class (stable)
├── AnonymousCommand.cs             — class (stable)
├── DisabledCommand.cs              — class (stable)

Dirtiables/
├── IDirtiable.cs                   — interface (stable)
├── IDirtyingOperation.cs           — interface (stable)
├── DirtiableManager.cs             — class (stable)
├── DirtiableSnapshot.cs            — class (stable)
├── DirtyingOperation.cs            — abstract class (stable)
├── AnonymousDirtyingOperation.cs   — class (stable)
├── CollectionChangeOperation.cs    — class (stable)
├── EmptyDirtyingOperation.cs       — class (stable)
├── PropertyChangeOperation.cs      — class (stable)

Collections/
├── IObservableCollection.cs        — interface (stable)
├── IObservableList.cs              — interface (stable)
├── IReadOnlyObservableCollection.cs— interface (stable)
├── IReadOnlyObservableList.cs      — interface (stable)
├── AutoUpdatingSortedObservableCollection.cs — class (stable)
├── SortedObservableCollection.cs   — class (stable)

Windows/
├── DialogButtonInfo.cs             — class (stable)

Core/
├── AnonymousComparer.cs            — class (stable)
├── NaturalStringComparer.cs        — class (stable)
├── Utils.cs                        — static class (stable)

Extensions/
├── ClassFieldExtensions.cs         — static class (stable)
├── ObjectExtensions.cs             — static class (stable)
├── StringExtensions.cs             — static class (stable)
├── TypeExtensions.cs               — static class (stable)
```

### 11.2 WPF-Specific APIs (Stride.Core.Presentation.Wpf)

```
Controls/ (47 files)
├── AngleEditor.cs
├── ColorPicker.cs
├── DateTimeEditor.cs
├── ExpandableItemsControl.cs
├── FilteringComboBox.cs
├── GameEngineHost.cs
├── Int2Editor.cs, Int3Editor.cs, Int4Editor.cs
├── KeyValueGrid.cs
├── MarkdownTextBlock.cs
├── MatrixEditor.cs
├── ModalWindow.cs
├── NumericTextBox.cs
├── PopupModalWindow.cs
├── PropertyView.cs, PropertyViewItem.cs
├── RectangleEditor.cs, RectangleFEditor.cs
├── RotationEditor.cs
├── ScaleBar.cs
├── SearchComboBox.cs
├── TagControl.cs
├── TextBlockFormatting.cs
├── TextBox.cs, TextBoxBase.cs
├── TextLogViewer.cs
├── TimeSpanEditor.cs
├── TreeView.cs, TreeViewItem.cs
├── Trimming.cs, TrimmingSource.cs
├── UnitSystem.cs
├── Vector2Editor.cs, Vector3Editor.cs, Vector4Editor.cs
├── VectorEditor.cs, VectorEditorBase.cs
├── VirtualizingTilePanel.cs, VirtualizingTreePanel.cs
├── CanvasView/ (subdirectory)

Behaviors/ (37 files)
├── ActivateOnCollectionChangedBehavior.cs
├── BehaviorProperties.cs
├── BindableSelectedItemsBehavior.cs
├── ... (35 more)

ValueConverters/ (71 files)
├── AllEqualMultiConverter.cs
├── ... (70 more)

Services/
├── IDialogService2.cs
├── IModalDialog.cs, IModalDialogInternal.cs
├── IFileModalDialog.cs, IFileOpenModalDialog.cs, IFileSaveModalDialog.cs
├── IFolderOpenModalDialog.cs
├── FileDialogFilter.cs
├── DialogResult.cs

View/
├── ITemplateProvider.cs
├── TemplateProviderBase.cs
├── TemplateProviderSelector.cs
├── DefaultTemplateProvider.cs, DefaultTemplateProviderComparer.cs
├── TemplateProviderComparerBase.cs
├── DispatcherService.cs

Windows/
├── WindowManager.cs
├── WindowInfo.cs
├── IAsyncClosableWindow.cs
├── MessageBox.cs, CheckedMessageBox.cs, MessageDialogBase.cs
├── DialogHelper.cs, HwndHelper.cs

Core/
├── BindingProxy.cs
├── DependencyPropertyWatcher.cs
├── FocusManager.cs
├── CancelRoutedEvent.cs, ValidationRoutedEvent.cs
├── AnonymousEventHandler.cs, AutoUnsubscribeHandler.cs
├── TimeoutDispatcherTimer.cs

Extensions/
├── BindingExtensions.cs
├── DependencyObjectExtensions.cs
├── DrawingContextExtensions.cs
├── ImageExtensions.cs, ItemsControlExtensions.cs
├── MathExtensions.cs
├── SystemColorExtensions.cs, VisualExtensions.cs
├── WindowHelper.cs

Commands/
├── SystemCommand.cs, SystemCommands.cs, UtilityCommands.cs

Drawing/
├── IDrawingContext, IDrawingModel, IDrawingView
├── CanvasRenderer, VisualHost, HslColor

Interop/
├── NativeHelper.cs, SafeClipboard.cs, ClipboardMonitor.cs

Interactivity/
├── BehaviorCollection.cs, Interaction.cs

ViewModel/
├── LoggerViewModel.cs
```

### 11.3 Editor Infrastructure APIs

```
Stride.Core.Assets.Editor/
Services/
├── AssetsPlugin.cs              — abstract (extension point)
├── AssetsEditorPlugin.cs        — abstract (extension point)
├── IAssetsPluginService.cs      — interface
├── IAssetEditorsManager.cs      — interface
├── IAssetPreviewService.cs      — interface
├── IBuildService.cs             — interface
├── IEditorDialogService.cs      — interface (extends IDialogService2)
├── IEditorView.cs               — interface
├── IAssetCreationView.cs        — interface
├── IAssetPickerDialog.cs        — interface
├── IFixReferencesDialog.cs      — interface
├── IItemTemplateDialog.cs       — interface
├── INewProjectDialog.cs         — interface
├── IThumbnailService.cs         — interface
├── EditorDialogHelper.cs        — static
├── SelectionService.cs          — class
├── CodeEditorOpenerService.cs   — class
├── VisualStudioService.cs       — class
├── CopyPaste/                   — copy/paste processors
├── AssetBuilderService.cs       — class
├── UserDocumentationService.cs  — class

ViewModel/ (55 files)
├── SessionViewModel.cs          — class
├── AssetViewModel.cs            — class
├── AssetEditorViewModel.cs      — class
├── EditorViewModel.cs           — class
├── IAssetEditorViewModel.cs     — interface
├── PackageViewModel.cs          — class
├── ... (50 more)

Annotations/
├── AssetViewModelAttribute.cs           — attribute
├── AssetEditorViewModelAttribute.cs     — attribute
├── AssetEditorViewAttribute.cs          — attribute

Components/
├── Properties/                   — property provider interfaces
├── Transactions/                 — transaction support
├── Status/                       — status bar
├── TemplateDescriptions/         — template descriptions
├── AddAssets/                    — add asset dialogs
├── FixAssetReferences/           — fix references
├── FixReferences/                — fix references
├── DebugTools/                   — debug tools
├── CopyPaste/                    — copy/paste

Quantum/
├── AssetContentValueChangeOperation.cs
├── ContentValueChangeOperation.cs
├── NodePresenters/               — node presentation for property grid
├── ViewModels/                   — quantum view models

View/ (28 files)
├── EditorDialogService.cs        — WPF implementation
├── AssetPickerWindow.xaml        — WPF window
├── NotificationWindow.xaml       — WPF window
├── Controls/                     — WPF controls
├── Behaviors/                    — WPF behaviors
├── ValueConverters/              — WPF converters
├── TemplateProviders/            — WPF template providers
├── DebugTools/                   — WPF debug UI

Settings/
├── EditorSettings.cs             — settings keys
├── ThemesSettings.cs             — theme settings
├── InternalSettings.cs           — internal settings
├── DebugTestSettings.cs          — debug settings
├── SettingsCommand.cs            — settings command
├── ViewModels/                   — settings view models

Stride.Editor/
├── StrideAssetsPlugin.cs         — abstract (extension point)
├── Module.cs                     — module initializer
EditorGame/ (Game/)
├── IEditorGameService.cs         — interface
├── EditorGameServiceBase.cs      — abstract class
├── EditorGameServiceRegistry.cs  — class
├── EditorServiceGame.cs          — abstract class
EditorGame/ (ViewModels/)
├── IEditorGameViewModelService.cs— interface
EditorGame/ (ContentLoader/)
├── IEditorContentLoader.cs       — interface
Preview/
├── IAssetPreview.cs              — interface
├── AssetPreview.cs               — abstract class
├── AssetPreview<T>.cs            — abstract class
├── IAssetPreviewViewModel.cs     — interface
├── IPreviewView.cs               — interface (WPF — IPreviewView)
├── IPreviewBuilder.cs            — interface (WPF — GetStrideView)
├── View/                         — WPF preview views
├── ViewModel/                    — preview view models
Build/
├── GameStudioBuilderService.cs   — class
├── GameStudioDatabase.cs         — class
├── IGameSettingsAccessor.cs      — interface
├── GameSettingsProviderService.cs— class
Thumbnails/
├── IThumbnailCompiler.cs         — interface
├── ThumbnailCompilerBase.cs      — abstract class
├── CustomAssetThumbnailCompiler.cs — class
Engine/
├── EmbeddedGame.cs               — class
├── EmbeddedGameForm.cs           — WinForms host (WPF)
├── EntityExtensions.cs           — extension methods (platform-agnostic)

Stride.Assets.Presentation/
├── StrideDefaultAssetsPlugin.cs  — concrete plugin
├── Module.cs                     — module initializer
AssetEditors/                    — asset-specific editors (WPF views)
CurveEditor/                     — curve editor (WPF views)
NodePresenters/                  — node presenters (platform-agnostic)
Preview/                         — preview renderers
SceneEditor/                     — scene editor
View/                            — asset editor views (WPF)
ViewModel/                       — asset editor view models
TemplateProviders/               — template providers (WPF)
ValueConverters/                 — value converters (WPF)
Quantum/                         — quantum node presenters
AssemblyReloading/               — assembly reloading

Stride.GameStudio/
Plugin/
├── StrideEditorPlugin.cs        — internal plugin
Services/
├── IStrideDialogService.cs      — interface
├── PluginService.cs             — plugin registration service
├── StrideDialogService.cs       — WPF implementation
├── ICredentialsDialog.cs        — interface
View/
├── GameStudioWindow.xaml        — main WPF window
├── GameStudioWindow.xaml.cs     — main WPF window code-behind
ViewModels/
├── GameStudioViewModel.cs       — main VM
├── EditionPanelViewModel.cs     — editor panel VM
├── PreviewViewModel.cs          — preview VM
```

### 11.4 Dependency Graph

```
Plugin Assembly
    ↓ references
Stride.Assets.Presentation  (asset-specific editors)
    ↓ references
Stride.Editor               (preview system, editor game)
    ↓ references
Stride.Core.Assets.Editor   (plugin base, session, services)
    ↓ references
Stride.Core.Presentation.Wpf  (WPF UI controls)    ← REPLACE
    ↓ references
Stride.Core.Presentation    (ViewModels, services)  ← KEEP
    ↓ references
Stride.Core.Design          (foundation types)      ← KEEP
```

---

*Document generated from codebase analysis on 2026-06-20.*
*Projects analyzed: Stride.Core.Presentation, Stride.Core.Presentation.Wpf,
 Stride.Core.Assets.Editor, Stride.Assets.Presentation, Stride.Editor, Stride.GameStudio.*
