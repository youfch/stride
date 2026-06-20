## 1. Phase 0: Analysis & Preparation

- [ ] 0.1 Inventory all WPF-specific API usage in the 10 editor projects (DependencyProperty, ControlTemplate, HwndHost, D3DImage, etc.)
- [ ] 0.2 Document ViewModel interfaces and ensure they have no WPF dependency
- [ ] 0.3 Identify third-party NuGet packages with WPF dependencies and research Avalonia equivalents
- [ ] 0.4 Set up Avalonia 12 spike project to validate cross-platform build (Windows/Linux/macOS)
- [ ] 0.5 Evaluate AvaloniaDock feature completeness against current AvalonDock usage
- [ ] 0.6 Validate RoslynPad.Avalonia feature parity
- [ ] 0.7 Test Stride rendering engine on Linux (Vulkan) and macOS (Metal) for editor viewport capability
- [ ] 0.8 Define plugin API surface that must remain stable during migration
- [ ] 0.9 Create timeline and milestone plan with risk buffers

## 2. Phase 1: Infrastructure & Foundation

- [ ] 1.1 Create `Stride.Core.Presentation.Avalonia` project with Avalonia SDK references
- [ ] 1.2 Port base classes: `WindowBase`, `UserControl`, `ViewModel` binding infrastructure
- [ ] 1.3 Port core converters and value converters from WPF `IValueConverter` to Avalonia `IValueConverter`
- [ ] 1.4 Create Avalonia resource dictionaries matching Stride editor styles/colors
- [ ] 1.5 Set up conditional MSBuild targets in `Directory.Build.props` for WPF/Avalonia dual-build
- [ ] 1.6 Update SDK props (`Stride.Build.Sdk.Editor`) to support Avalonia targets
- [ ] 1.7 Create `Stride.Core.Presentation.Interop` project for WPF↔Avalonia bridge
- [ ] 1.8 Implement `AvaloniaInWpfHost` (embeds Avalonia Window HWND in WPF HwndHost)
- [ ] 1.9 Implement `WpfInAvaloniaHost` (embeds WPF Window HWND in Avalonia NativeControlHost) — fallback
- [ ] 1.10 Create interop service for clipboard, file dialogs, dispatcher bridging
- [ ] 1.11 Set up Avalonia.Headless test project for UI unit tests
- [ ] 1.12 Add GitHub Actions workflows for Avalonia build (Windows/Linux/macOS)
- [ ] 1.13 PoC: Host a simple Avalonia panel (e.g., "About" dialog) inside WPF Game Studio
- [ ] 1.14 Verify dual-build: same solution produces both WPF and Avalonia targets

## 3. Phase 2: Core UI Components

- [ ] 2.1 Implement Avalonia `PropertyGrid` control (DataGrid-based)
- [ ] 2.2 Implement property editor types: StringEditor, NumericEditor, BooleanEditor, EnumEditor
- [ ] 2.3 Implement complex editors: ColorEditor, VectorEditor, QuaternionEditor, AssetReferenceEditor
- [ ] 2.4 Implement PropertyGrid search/filter and category grouping
- [ ] 2.5 Implement custom `IPropertyEditorProvider` registry for extensibility
- [ ] 2.6 Port `EditableListBox` control
- [ ] 2.7 Port `WatermarkTextBox` control
- [ ] 2.8 Port custom `ColorPicker` control
- [ ] 2.9 Port custom `TreeView` with lazy loading and drag-drop
- [ ] 2.10 Port any additional custom controls in `Stride.Core.Presentation.Wpf.Controls`
- [ ] 2.11 Integrate AvaloniaDock + Dock.Model as parallel docking library
- [ ] 2.12 Port layout persistence (save/restore) to AvaloniaDock format
- [ ] 2.13 Port AvalonDock pane templates/themes to AvaloniaDock templates
- [ ] 2.14 Implement context-sensitive tool pane visibility
- [ ] 2.15 Port all custom Stride editor themes and styles to Avalonia
- [ ] 2.16 Port common dialogs: OpenFileDialog, SaveFileDialog, FolderBrowserDialog
- [ ] 2.17 Port message dialogs (Info, Warning, Error, Yes/No/Cancel)
- [ ] 2.18 Create side-by-side test harness comparing WPF vs Avalonia controls
- [ ] 2.19 Create regression test suite for all core controls (interaction + visual)

## 4. Phase 3: Asset Editors

- [ ] 3.1 Integrate RoslynPad.Avalonia NuGet package
- [ ] 3.2 Port `ScriptEditorView` — code editor with syntax highlighting and IntelliSense
- [ ] 3.3 Port script file tree navigation panel
- [ ] 3.4 Port `UIEditorView` — WYSIWYG canvas with element selection and property binding
- [ ] 3.5 Port `VisualScriptEditorView` — node graph canvas with connections and library panel
- [ ] 3.6 Port `CurveEditorView` — animation curve editor with keyframes and tangents
- [ ] 3.7 Port `MaterialEditorView` — shader graph editor with preview
- [ ] 3.8 Port asset picker dialog (thumbnail gallery with search and filtering)
- [ ] 3.9 Port any other asset-specific editors (SpriteEditor, SoundEditor, etc.)
- [ ] 3.10 Wire each Avalonia editor into the WPF host via interop bridge
- [ ] 3.11 Validate each editor in the interop host (focus, input, layout behavior)
- [ ] 3.12 Create automated rendering tests for each editor (screenshot comparison)

## 5. Phase 4: Game Studio & 3D Preview

- [ ] 4.1 Define `IGameHost` interface for cross-platform HWND hosting
- [ ] 4.2 Implement `Win32GameHost` — SetParent + Win32 message forwarding
- [ ] 4.3 Implement `X11GameHost` — XReparentWindow + X11 event monitoring
- [ ] 4.4 Implement `MacGameHost` — NSView embedding + NSEvent monitoring
- [ ] 4.5 Implement `AvaloniaGameHost` wrapping `IGameHost` in NativeControlHost
- [ ] 4.6 Create `GameContext` abstraction for different platforms (context creation)
- [ ] 4.7 Port input forwarding system (keyboard, mouse) to platform-agnostic events
- [ ] 4.8 Port DPI-aware resize logic for the 3D viewport
- [ ] 4.9 Integrate 3D preview into Avalonia editor layout
- [ ] 4.10 Wire preview asset selection and property change updates
- [ ] 4.11 Port menu bar (File, Edit, View, Assets, Scene, Tools, Help)
- [ ] 4.12 Port toolbar (icons, dropdowns, toggle states)
- [ ] 4.13 Port status bar (progress, messages, clickable items)
- [ ] 4.14 Port startup wizard / new project dialog
- [ ] 4.15 Port settings/preferences dialog
- [ ] 4.16 Port project settings dialog
- [ ] 4.17 Port session management (recent projects, auto-save, crash recovery)
- [ ] 4.18 Switch main entry point from WPF `Application` to Avalonia `AppBuilder`
- [ ] 4.19 Remove WPF interop bridge and WPF host dependencies
- [ ] 4.20 Clean up WPF projects (archive or remove from solution)
- [ ] 4.21 Finalize CI/CD: all 3 platforms build, test, and package

## 6. Post-Migration: Validation & Polish

- [ ] 5.1 Full feature regression test on Windows
- [ ] 5.2 Full feature regression test on Linux (Ubuntu 22.04+)
- [ ] 5.3 Full feature regression test on macOS (13+)
- [ ] 5.4 Performance benchmark comparison (WPF vs Avalonia)
- [ ] 5.5 Accessibility audit (keyboard navigation, screen reader)
- [ ] 5.6 Plugin compatibility validation
- [ ] 5.7 Document migration guide for plugin developers
- [ ] 5.8 Update public API documentation
- [ ] 5.9 Performance tuning (renderer settings, UI virtualization)
- [ ] 5.10 Community beta testing period (2 weeks)
- [ ] 5.11 Address feedback and edge cases

## 7. Spike Tasks (Pre-Phase Mitigation)

- [ ] S.1 **Spike: AvaloniaDock feature audit** — Compare AvalonDock vs AvaloniaDock features. Test layout serialization, floating windows, auto-hide, MDI tabs
- [ ] S.2 **Spike: RoslynPad.Avalonia API** — Validate RoslynPad.Avalonia supports code lens, inline diagnostics, completion commit behaviors, custom classifications
- [ ] S.3 **Spike: Stride cross-platform rendering** — Test Stride editor viewport on Linux (Vulkan) and macOS (Metal). Measure FPS, latency, feature gaps
- [ ] S.4 **Spike: Avalonia 12 breaking changes** — Evaluate `Window`→`TopLevel`, renderer changes (`Compositor`), input system changes that affect migration
- [ ] S.5 **Spike: Interop performance** — Measure input latency through WPF↔Avalonia interop bridge. Target: <5ms additional latency
- [ ] S.6 **Spike: Plugin API surface** — Identify all public API surfaces that plugins consume from WPF projects. Document before any breaking changes
