## Context

The Stride Game Studio editor consists of 10 WPF projects (~300K lines) built on .NET Framework/WPF. The editor includes complex UI components (PropertyGrid, docking, 3D preview, code editor, multiple asset editors) that depend on WPF-specific APIs. The codebase has two cross-platform layers (`Stride.Core.Presentation`, `Stride.Core.Quantum`) that are already platform-agnostic and can be reused directly.

**Current Architecture (Simplified):**
```
Stride.GameStudio (WPF)                  ← Main executable
  ├── Stride.Editor (WPF)                ← Editor engine, preview hosting
  ├── Stride.Assets.Presentation (WPF)   ← Asset editors (UI/VisualScript/Curve/Script)
  ├── Stride.Core.Presentation.Wpf (WPF) ← WPF controls (PropertyGrid, GameEngineHost, etc.)
  ├── Stride.Core.Presentation (netstd)  ← Shared ViewModel base (platform-agnostic)
  ├── Stride.Core.Quantum (netstd)       ← ViewModel sync layer (platform-agnostic)
  └── [3rd Party] AvalonDock, RoslynPad, etc.
```

**Avalonia 12 Key Advantages:**
- Cross-platform: Windows (DirectX/ANGLE), Linux (X11/Wayland/Vulkan), macOS (Metal)
- 19x rendering performance improvement over WPF (compositor-based, immediate mode)
- Same XAML-like markup with C# code-behind (familiar to WPF developers)
- Rich control library with DataGrid, TreeView, Menu, Toolbar, Dock support
- Strong community and growing ecosystem (RoslynPad.Avalonia, Dock.Avalonia, etc.)
- .NET 8+ native support, AOT compilation ready

## Goals / Non-Goals

**Goals:**
- Replace all WPF UI code with Avalonia 12 while preserving 100% of editor functionality
- Enable cross-platform support: Windows, Linux (Ubuntu 22.04+), macOS (13+)
- Maintain identical user experience — no UX regressions during or after migration
- Parallel WPF/Avalonia support during migration to allow incremental rollout and rollback
- Match or exceed WPF rendering performance (target: 5-19x improvement as per Avalonia 12 benchmarks)
- Reuse existing cross-platform layers (`Stride.Core.Presentation`, `Stride.Core.Quantum`) as-is
- Minimal API surface changes for plugin/extension developers

**Non-Goals:**
- Not a UI redesign — preserve existing layout, styling, and interaction patterns
- No changes to the Stride game engine runtime (rendering, physics, audio, etc.)
- No changes to asset format or serialization
- No changes to the build pipeline for game projects (only editor projects)
- No removal of WPF support until all plugins have migrated (soft deprecation)

## Architecture

### Migration Strategy: Hybrid Incremental

```
Phase 1-2: Parallel WPF + Avalonia
┌─────────────────────────────────────────────────────┐
│  WPF Host Application                                │
│  ┌──────────────────┐  ┌──────────────────────────┐  │
│  │ WPF Controls     │  │ Avalonia Interop Window  │  │
│  │ (unchanged)      │  │ ┌──────────────────────┐ │  │
│  │                  │  │ │ Avalonia Controls    │ │  │
│  │                  │  │ │ (new)                │ │  │
│  │                  │  │ └──────────────────────┘ │  │
│  └──────────────────┘  └──────────────────────────┘  │
│                    │ Data binding bridge              │
│                    ▼                                   │
│  Stride.Core.Quantum (shared ViewModel layer)          │
└─────────────────────────────────────────────────────┘

Phase 3-4: Pure Avalonia
┌─────────────────────────────────────────────────────┐
│  Avalonia Application                                │
│  ┌──────────────────────────────────────────────┐   │
│  │ Avalonia Controls (all ported)               │   │
│  └──────────────────────────────────────────────┘   │
│                    │                                  │
│                    ▼                                  │
│  Stride.Core.Quantum (shared ViewModel layer)         │
└─────────────────────────────────────────────────────┘
```

### Project Structure

```
sources/presentation/
├── Stride.Core.Presentation/             ← SHARED (netstandard2.0) — existing, unchanged
├── Stride.Core.Presentation.Wpf/         ← existing WPF, frozen during migration
│
├── Stride.Core.Presentation.Avalonia/    ← NEW (net8.0) — Avalonia controls
│   ├── Controls/
│   │   ├── PropertyGrid/
│   │   ├── GameEngineHost/               ← Cross-platform HWND hosting
│   │   ├── TreeView/
│   │   └── ...
│   ├── Themes/
│   │   ├── Styles/                       ← Avalonia styles (analogous to WPF themes)
│   │   └── Controls/                     ← Custom control themes
│   └── Converters/
│
├── Stride.Core.Presentation.Interop/     ← NEW (net8.0) — WPF↔Avalonia bridge
│   ├── WpfAvaloniaHost.cs               ← Hosts Avalonia in WPF (Window).
│   └── AvaloniaWpfHost.cs               ← Hosts WPF in Avalonia (via HwndHost).

sources/editor/
├── Stride.Editor.Avalonia/               ← NEW — Avalonia-specific editor services
│   ├── Preview/
│   │   └── AvaloniaGameEngineHost.cs     ← Avalonia-compatible GameEngineHost
│   └── Services/
│
├── Stride.Editor.Wpf/                    ← existing, frozen
│
└── Stride.Assets.Presentation.Avalonia/  ← NEW — Avalonia asset editors
    ├── AssetEditors/
    │   ├── GameEditor/
    │   ├── UIEditor/
    │   ├── VisualScriptEditor/
    │   ├── CurveEditor/
    │   └── ScriptEditor/
    └── Views/
```

### Executable Entry Point

```
Current: Stride.GameStudio.exe (WPF Application)
 Future: Stride.GameStudio.exe (Avalonia Application)

Phase 1-2: Dual-build mode via conditional compilation
  #if AVALONIA
      AppBuilder.Configure<App>().UsePlatformDetect().Start(...)
  #else
      new App().Run();
  #endif

Phase 3-4: Pure Avalonia Application
  AppBuilder.Configure<App>().UsePlatformDetect().Start(...)
```

## Decisions

### D1: Migration Strategy — Hybrid Incremental (not Big Bang or Parallel Fork)

**Chosen**: Hybrid incremental — build Avalonia components alongside WPF, replace one capability at a time.

| Approach | Pros | Cons |
|----------|------|------|
| **Big Bang** | Single switchover | Massive risk, impossible to debug, 12+ months without shipping |
| **Parallel Fork** | Clean slate per platform | 2x maintenance cost, feature divergence |
| **Hybrid Incremental** ✅ | Low risk, ship during migration, test each component | Interop complexity, temporary dual-build cost |

**Key Insight**: The shared ViewModel layer (`Stride.Core.Quantum`) is already platform-agnostic. We only replace the View layer. This makes incremental migration practical — swap one View at a time while keeping ViewModels unchanged.

### D2: WPF/Avalonia Interop — `Window`-Level Dual-Hosting (not `HwndHost`-Level)

**Chosen**: Host Avalonia in a separate `Window` embedded into the WPF main window via interop.

| Approach | Detail | Viability |
|----------|--------|-----------|
| **HwndHost embedding** | Embed WPF in Avalonia or vice versa at HWND level | ❌ Fragile, broken input/IME/accessibility |
| **Window-level hosting** ✅ | Run Avalonia in its own window, embedded in WPF parent | ✅ Reliable, clean boundaries |
| **Shared process + named pipes** | Separate processes communicate via IPC | ❌ Over-engineered for incremental migration |

**Decision**: Use WPF as the host during migration. Avalonia components are rendered in an embedded Avalonia `Window` whose HWND is parented to a WPF `HwndHost`. The interop layer forwards input events and syncs layout.

### D3: Docking Solution — Dock.Avalonia (over AvaloniaDock and WPF Dock)

**Alternatives considered:**
- **AvaloniaDock** (by wieslawsoltes): Mature, MIT license, active maintenance, MDI/outline support ✅ **Chosen**
- **Dock.Model** (by wieslawsoltes): MVVM-oriented, works with AvaloniaDock
- **WPF AvalonDock via interop**: Keep WPF dock and embed it in Avalonia ❌ Defeats migration purpose
- **Custom docking**: Build from scratch ❌ 6+ months of effort

**Decision**: Use [AvaloniaDock](https://github.com/wieslawsoltes/AvaloniaDock) + [Dock.Model](https://github.com/wieslawsoltes/Dock.Model) for MVVM-driven docking. Map AvalonDock concepts:
```
AvalonDock          →  AvaloniaDock
DockingManager       →  DockView / DockFactory
LayoutItem           →  IDockWindow / IDockDocument
AnchorablePane       →  ToolDock
DocumentPane         →  DocumentDock
LayoutSerializer     →  DockFactory.SaveLayout/LoadLayout
```

### D4: PropertyGrid — Rebuild in Avalonia (not reuse via interop)

**Chosen**: Build a new `PropertyGrid` control for Avalonia using `DataGrid` + custom editors.

**Rationale**: The WPF PropertyGrid is deeply tied to WPF-specific patterns (`DependencyProperty`, `ControlTemplate`, WPF `GridView`). Embedding it via interop would be fragile and perform poorly. A clean Avalonia implementation is more maintainable and can leverage Avalonia's superior DataGrid virtualization.

**Mapping:**
```
WPF PropertyGrid              →  Avalonia PropertyGrid (custom control)
PropertyView.xaml              →  PropertyGrid.axaml
PropertyCellEditor             →  DataGridTemplateColumn editors
PropertyGridSearchBox          →  TextBox + ICollectionView Filter
CategoryGroup                  →  Expander templates in DataGrid
InlineEditor                   →  Flyout-based inline editing
```

### D5: 3D Preview Rendering — Cross-Platform HWND Embedding Abstraction

**Chosen**: Abstract `GameEngineHost` into platform-specific implementations behind a common interface.

```
IGameHost (interface)
├── Win32GameHost     — SetParent + HWND embedding (Windows)
├── X11GameHost       — XReparentWindow (Linux)
└── MacGameHost       — CAMetalLayer / NSView embedding (macOS)
```

**Avalonia integration**: `AvaloniaGameHost` wraps the platform implementation and integrates with Avalonia's `NativeControlHost` (formerly `HwndHost` in older Avalonia). The native window handle is provided to the rendering engine via `GameContext`.

```
AvaloniaNativeControlHost
  └── PlatformHostWindow (IntPtr)
        └── Stride Render Window
              ├── DirectX 11 (Windows)
              ├── Vulkan (Linux)
              └── Metal (macOS)
```

**Input forwarding**: Replace Win32 message pump (`WndProc` → `ForwardMessage`) with platform-agnostic input events:
```
Stride Input Service
  ← IGameHost.InputEvents (abstracted mouse/keyboard events)
      ← Platform-specific input sources:
          ├── Win32: Hook into WndProc
          ├── X11: XNextEvent / XCheckTypedEvent
          └── macOS: NSEvent monitoring
```

### D6: RoslynPad Code Editor — RoslynPad.Avalonia NuGet Package

**Chosen**: Use the official [RoslynPad.Avalonia](https://github.com/aelij/RoslynPad) package (maintained by @aelij).

**Integration**: The Avalonia version of RoslynPad provides `RoslynPad.Avalonia.Editor` control. Same API surface as WPF version — minimal ViewModel changes needed. The `ScriptEditorView` can be directly replaced:

```
WPF: RoslynPad.Editor.RoslynCodeEditor (WPF)
    ├── Syntax highlighting
    ├── IntelliSense
    ├── Diagnostics
    └── Code completion

Avalonia: RoslynPad.Avalonia.Editor (Avalonia)
    ├── Same features via abstracted editor API
    └── Also supports AvaloniaEdit (alternative)
```

### D7: Build System — SDK-Style Projects with Conditional Framework Targeting

**Chosen**: Use MSBuild conditions to toggle WPF/Avalonia at the project level.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- WPF target (default for backward compatibility) -->
    <TargetFrameworks Condition="!$(TargetFramework.Contains('avalonia'))">net8.0-windows</TargetFrameworks>
    <!-- Avalonia target (opt-in via -p:UseAvalonia=true) -->
    <TargetFrameworks Condition="$(UseAvalonia)">net8.0</TargetFrameworks>
  </PropertyGroup>

  <ItemGroup Condition="!$(UseAvalonia)">
    <PackageReference Include="AvalonDock" Version="..." />
    <PackageReference Include="RoslynPad" Version="..." />
  </ItemGroup>

  <ItemGroup Condition="$(UseAvalonia)">
    <PackageReference Include="Avalonia" Version="12.0.0" />
    <PackageReference Include="Avalonia.Desktop" Version="12.0.0" />
    <PackageReference Include="AvaloniaDock" Version="..." />
    <PackageReference Include="RoslynPad.Avalonia" Version="..." />
  </ItemGroup>
</Project>
```

### D8: Stride.Core.Presentation.Avalonia — Shared Control Library NuGet

**Chosen**: Package shared Avalonia controls as a NuGet for external plugin/extension consumption.

```xml
<!-- Stride.Core.Presentation.Avalonia.csproj -->
<Project>
  <ItemGroup>
    <PackageReference Include="Avalonia" Version="12.0.0" />
    <PackageReference Include="Avalonia.Desktop" Version="12.0.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Stride.Core.Presentation\Stride.Core.Presentation.csproj" />
  </ItemGroup>
</Project>
```

## Data Flow: WPF ↔ Avalonia Interop Layer

During Phase 1-2 (parallel WPF/Avalonia):

```
┌──────────────────────────────────────────────────┐
│  WPF Main Window (Stride.GameStudio)              │
│                                                    │
│  ┌──────────────────────────────────────────────┐ │
│  │  Tool Pane (AvalonDock)                      │ │
│  │  ┌──────────────────────────────────────────┐│ │
│  │  │  HwndHost → Avalonia Window HWND         ││ │
│  │  │  ┌────────────────────────────────────┐  ││ │
│  │  │  │  Avalonia UI (any control)          │  ││ │
│  │  │  │  ┌──────────────────────────────┐  │  ││ │
│  │  │  │  │  PropertyGrid / Editor / etc. │  │  ││ │
│  │  │  │  └──────────────────────────────┘  │  ││ │
│  │  │  └────────────────────────────────────┘  ││ │
│  │  └──────────────────────────────────────────┘│ │
│  └──────────────────────────────────────────────┘ │
│                                                    │
│  Data Binding: AvaloniaWindow.Resources["DataContext"]
│     ← WpfInteropBridge.SyncViewModel(viewModel)    │
└──────────────────────────────────────────────────────┘
```

## Risks / Trade-offs

| Risk | Impact | Likelihood | Mitigation |
|------|--------|-----------|------------|
| **AvaloniaDock not feature-matching AvalonDock** | High — missing layout persistence or floating windows could block migration | Medium | Evaluate early (Phase 1 spike). Fallback: Custom layout wrapper or embed AvalonDock via interop as temporary measure |
| **3D preview performance on Linux/macOS** | Medium — game rendering via Vulkan/Metal instead of DirectX | Medium | Use Stride's existing Vulkan backend for Linux, Metal for macOS. Validate in Phase 2 spike |
| **PropertyGrid behavior differences** | Medium — subtle UX differences in inline editing, validation, keyboard navigation | Low-medium | Comprehensive test harness comparing WPF vs Avalonia PropertyGrid side-by-side. Pixel-diff automated tests |
| **Plugin ecosystem breakage** | High — third-party plugins referencing WPF internals will break | High | Provide `Stride.Core.Presentation.Interop` bridge. Document migration guide for plugin authors. Soft deprecation timeline (WPF support for 2 release cycles) |
| **Input handling latency in 3D preview** | Medium — forwarding input through interop layer adds overhead | Low | Benchmark Phase 1. If latency > 5ms, implement direct input hook (bypass interop) |
| **Accessibility regression** | Medium — screen readers, keyboard navigation may differ | Low | Early accessibility audit. Avalonia's AutomationPeer is less mature than WPF's — may need custom peers |
| **Build system complexity** | Low — conditional MSBuild targets can be confusing | Medium | Document build matrix clearly. CI validates both WPF and Avalonia builds |
| **Migration fatigue** | Medium — 4+ phase migration over 6-9 months | Medium | Ship value in each phase. Phase 1 (infrastructure) and Phase 2 (core controls) deliver tangible improvements |

## Migration Plan (High-Level)

```
Phase 1: Infrastructure (8-10 weeks)
├── Create Stride.Core.Presentation.Avalonia project
├── Port base controls (Window, UserControl, Styles, Converters)
├── Set up dual-build system (conditional MSBuild targets)
├── Implement WPF↔Avalonia interop bridge (HwndHost hosting)
└── PoC: Host a simple Avalonia panel inside the WPF Game Studio

Phase 2: Core Components (8-10 weeks)
├── Port PropertyGrid to Avalonia (DataGrid-based)
├── Integrate AvaloniaDock (replace AvalonDock)
├── Port TreeView, EditableListBox, ColorPicker, WatermarkTextBox
├── Port custom Themes/Styles
└── Port common dialogs (OpenFile, SaveFile, FolderBrowser)

Phase 3: Asset Editors (8-10 weeks)
├── Port ScriptEditorView (RoslynPad.Avalonia)
├── Port UIEditorView
├── Port VisualScriptEditorView
├── Port CurveEditorView
├── Port MaterialEditorView
└── Port AssetPickerView (thumbnail gallery, search)

Phase 4: Game Studio & Preview (8-12 weeks)
├── Port GameEngineHost (cross-platform HWND abstraction)
├── Port 3D preview rendering pipeline
├── Integrate 3D preview with Avalonia host
├── Port main Game Studio window shell
├── Port menu bar, toolbar, status bar
├── Port session management and startup wizard
├── Port settings dialogs
├── Remove WPF interop bridge
├── Remove WPF projects (cleanup)
└── Finalize CI/CD pipelines

Total Estimated Timeline: 32-42 weeks (8-10 months)
```

## Open Questions

1. **AvaloniaDock maturity**: Does AvaloniaDock support all AvalonDock features used in Stride (layout serialization, floating windows, auto-hide, MDI tabs)? Need a spike in Phase 1 to validate.
2. **Avalonia 12 breaking changes**: Avalonia 12 made significant API changes. Need to evaluate: `Window` to `TopLevel`, renderer changes, input system changes. Do these affect the migration plan?
3. **Stride rendering engine cross-platform**: Does Stride's rendering engine support Vulkan (Linux) and Metal (macOS) at a level suitable for the editor 3D viewport? Need to test with current Stride builds.
4. **RoslynPad.Avalonia features**: Does RoslynPad.Avalonia support all RoslynPad features used (code lens, inline diagnostics, completion commit behaviors)? Need to verify.
5. **Plugin API compatibility**: What is the public API surface that plugins consume? Should be documented before breaking changes.
6. **NuGet package strategy**: Should Stride.Core.Presentation.Avalonia be a NuGet package? What's the versioning strategy during migration?
7. **CI/CD matrix**: What build agents/OS combinations are needed? Linux/macOS build agents for cross-platform validation? Any cost implications?