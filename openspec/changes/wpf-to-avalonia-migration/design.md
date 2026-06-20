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

## Third-Party Library Customization Strategy

### Fork & Customize Approach

Given the 3-year timeline, we adopt a **fork-and-customize** strategy for critical third-party libraries. This reduces risk compared to building from scratch while maintaining full control over the codebase.

```
Customization Strategy Matrix
══════════════════════════════════════════════════════════════════

Phase 1-2 (Hybrid Mode)              Phase 3-4 (Pure Avalonia)
─────────────────────                 ─────────────────────
AvalonDock (WPF)                      →  Customized Dock.Avalonia
  │  fork Dirkster99/AvalonDock         │  fork wieslawsoltes/Dock
  │  Keep existing API compatibility    │  Adapt Stride layout config
  │  Reuse XmlLayoutSerializer          │  Implement XML-compatible serialization
  │                                     │
RoslynPad (WPF)                       →  RoslynPad.Avalonia
  │  Existing integration unchanged     │  fork roslynpad/RoslynPad
  │                                     │  Adapt Stride editor wrappers
  │                                     │  Reuse RoslynHost/RoslynWorkspace
  │                                     │
GameEngineHost                        →  GameEngineHost (Cross-Platform)
  │  WPF HwndHost                       │  IGameHost interface
  │  Win32 message forwarding           │  Win32/X11/Mac implementations
```

### Library-Specific Customization Plans

| Library | Source | License | Stars | Customization Path |
|---------|--------|---------|-------|-------------------|
| **AvalonDock** | `Dirkster99/AvalonDock` | MS-PL | 1.6k | Fork v5 branch, extend XmlLayoutSerializer for Stride compatibility |
| **Dock.Avalonia** | `wieslawsoltes/Dock` | MIT | 1.4k | Fork, adapt Dock.Serializer.Xml, implement Stride-specific behaviors |
| **RoslynPad** | `roslynpad/RoslynPad` | MIT | 2.8k | Fork, adapt ScriptTextEditor wrappers, reuse RoslynHost/RoslynWorkspace |

**Key Insight**: Stride team members (including xen2) are already top contributors to RoslynPad, making this customization path well-understood.

### Customization Workload Estimates

| Customization Task | Original Estimate | Fork & Customize | Savings |
|--------------------|-------------------|------------------|---------|
| AvalonDock → Dock.Avalonia | 6-8 weeks (rewrite) | 4-6 weeks (customize) | 2 weeks |
| RoslynPad → RoslynPad.Avalonia | 2-3 weeks (integration) | 1-2 weeks (fork) | 1 week |
| AvalonDock customization (Phase 1-2) | Not counted | 2-3 weeks (fork + adapt) | New task |

**Net Change**: Roughly flat, but significantly reduced technical risk.

---

## Risks / Trade-offs (Updated)

| Risk | Impact | Likelihood | Mitigation | Updated Rating (3-year timeline) |
|------|--------|-----------|------------|--------------------------------|
| **macOS Metal rendering maturity** | High — editor viewport may not render on macOS | Medium | Spike in Phase 0 to validate MSL pipeline; fallback: MoltenVK or software rendering | 🔴 **HIGH** (Blocker) |
| **AvalonDock → Dock feature gap** | High — missing layout persistence or floating windows | Medium | Fork wieslawsoltes/Dock, adapt XmlLayoutSerializer; Spike to validate all scenarios | 🟡 **MEDIUM** (Spike required) |
| **Wayland window embedding** | High — Phase 1-2 interop only works on Windows | High | Protocol-level constraint; Phase 1-2 documented as Windows-only, cross-platform in Phase 3-4 | 🟡 **MEDIUM** (Known constraint) |
| **PropertyGrid behavior differences** | Medium — subtle UX differences in inline editing | Low-medium | Comprehensive test harness; Pixel-diff automated tests | 🟢 **LOW** |
| **Plugin ecosystem breakage** | High — third-party plugins referencing WPF internals | High | Interop bridge + migration guide; WPF support for 2 release cycles | 🟢 **LOW** (mitigated) |
| **Input handling latency in 3D preview** | Medium — forwarding input through interop adds overhead | Low | Benchmark Phase 1; if >5ms, implement direct input hook | 🟢 **LOW** |
| **Accessibility regression** | Medium — screen readers, keyboard navigation may differ | Low | Early accessibility audit; custom AutomationPeers if needed | 🟢 **LOW** |
| **Build system complexity** | Low — conditional MSBuild targets can be confusing | Medium | Document build matrix clearly; CI validates both builds | 🟢 **LOW** |
| **Team knowledge transfer** | Medium — Avalonia expertise needs to be built | Medium | 3-year timeline allows training; hire/train Avalonia specialists | 🟢 **LOW** (time buffer) |
| **Third-party library maintenance** | Medium — forked libraries need ongoing maintenance | Low | Full control over forked repos; assign dedicated maintainer | 🟢 **LOW** |

---

## Migration Plan (3-Year Timeline)

### Phase 0: Spike Validation (Year 1, Q1 — 12-16 weeks)

**Goal**: Validate all technical blockers before committing to full migration.

```
S.1  Spike: Dock.Avalonia Feature Parity (2-3 weeks)
     ├── Fork wieslawsoltes/Dock → Stridefork/Dock
     ├── Test layout serialization (XML → JSON/XML/YAML compatibility)
     ├── Validate 7 Stride docking panes restore correctly
     ├── Test floating windows, auto-hide, MDI tabs
     └── Decision: Proceed with Dock.Avalonia or fallback to Actipro Avalonia

S.2  Spike: RoslynPad.Avalonia Integration (1-2 weeks)
     ├── Fork roslynpad/RoslynPad → Stridefork/RoslynPad
     ├── Integrate RoslynPad.Avalonia.Editor in Stride context
     ├── Validate ScriptTextEditor/SimpleCodeTextEditor wrappers
     ├── Test syntax highlighting, IntelliSense, diagnostics
     └── Decision: Proceed with forked RoslynPad

S.3  Spike: macOS Metal Rendering (2-3 weeks)
     ├── Build Stride from source on Apple Silicon (M1/M2/M3)
     ├── Test editor 3D viewport rendering via Metal/MSL
     ├── Measure FPS, latency, visual fidelity
     ├── Test SPIR-V → MSL conversion pipeline
     └── Decision: Metal viable? If not, plan MoltenVK fallback (+4-8 weeks)

S.4  Spike: WPF↔Avalonia Interop PoC (2-3 weeks)
     ├── Implement AvaloniaInWpfHost (Avalonia Window in WPF HwndHost)
     ├── Test input forwarding (keyboard, mouse, IME)
     ├── Measure latency (target: <5ms additional)
     ├── Test layout sync between WPF and Avalonia
     └── Decision: Interop viable for Phase 1-2?

S.5  Spike: Linux Vulkan Rendering (1-2 weeks)
     ├── Build Stride on Ubuntu 22.04+
     ├── Test editor 3D viewport via Vulkan
     ├── Measure FPS, latency
     └── Decision: Vulkan viable for editor viewport?

S.6  Spike: PropertyGrid DataGrid PoC (1-2 weeks)
     ├── Create basic Avalonia PropertyGrid using DataGrid
     ├── Test property binding, editors, validation
     ├── Compare UX with WPF PropertyGrid
     └── Decision: DataGrid-based approach viable?

Milestone 1 Decision Gate (End of Q1):
├── ✅ All 6 Spikes pass → Proceed to Phase 1
├── ⚠️ 1-2 Spikes fail → Execute fallback plans, add 4-8 weeks
└── ❌ 3+ Spikes fail → Re-evaluate migration feasibility
```

### Phase 1: Infrastructure & Foundation (Year 1, Q2-Q4 — 16-20 weeks)

```
Q2: Fork Setup & Build System
├── 1.1 Create Stridefork/Dock repository, fork wieslawsoltes/Dock
├── 1.2 Create Stridefork/RoslynPad repository, fork roslynpad/RoslynPad
├── 1.3 Set up dual-build system (conditional MSBuild targets)
├── 1.4 Update SDK props (Stride.Build.Sdk.Editor) for Avalonia targets
├── 1.5 Create Stride.Core.Presentation.Avalonia project
└── 1.6 Create Stride.Core.Presentation.Interop project

Q3: Interop Layer & Base Controls
├── 1.7 Implement AvaloniaInWpfHost (embed Avalonia Window in WPF)
├── 1.8 Implement WpfInAvaloniaHost (fallback: embed WPF in Avalonia)
├── 1.9 Create interop service (clipboard, file dialogs, dispatcher)
├── 1.10 Port base classes: WindowBase, UserControl, ViewModel binding
├── 1.11 Port core converters (IValueConverter)
├── 1.12 Create Avalonia resource dictionaries (styles, colors)
└── 1.13 PoC: Host simple Avalonia panel ("About" dialog) in WPF Game Studio

Q4: CI/CD & Testing Infrastructure
├── 1.14 Set up Avalonia.Headless test project
├── 1.15 Add GitHub Actions workflows (Windows/Linux/macOS)
├── 1.16 Verify dual-build: same solution produces WPF + Avalonia
├── 1.17 Document build matrix and CI/CD pipeline
└── 1.18 Establish code review and merge guidelines for forked repos

Milestone 2 Decision Gate (End of Q4):
├── ✅ Dual-build system works → Proceed to Phase 2
├── ⚠️ Interop layer has defects → Fix before continuing
└── ❌ Build system not functional → Re-evaluate
```

### Phase 2: Core UI Components (Year 2, Q1-Q2 — 20-24 weeks)

```
Q1: PropertyGrid & Basic Controls
├── 2.1 Implement Avalonia PropertyGrid (DataGrid-based)
├── 2.2 Implement property editor types (String, Numeric, Boolean, Enum)
├── 2.3 Implement complex editors (Color, Vector, Quaternion, AssetReference)
├── 2.4 Implement PropertyGrid search/filter and category grouping
├── 2.5 Implement IPropertyEditorProvider registry
├── 2.6 Port EditableListBox, WatermarkTextBox
├── 2.7 Port ColorPicker, TreeView (lazy loading, drag-drop)
└── 2.8 Port additional controls from Stride.Core.Presentation.Wpf.Controls

Q2: Docking & Themes
├── 2.9 Adapt Dock.Serializer.Xml for Stride layout compatibility
├── 2.10 Adapt Stride's AvalonDockHelper.IsVisible to Dock.Avalonia
├── 2.11 Integrate customized Dock.Avalonia
├── 2.12 Port layout persistence (save/restore)
├── 2.13 Port AvalonDock pane templates/themes
├── 2.14 Implement context-sensitive tool pane visibility
├── 2.15 Port all custom Stride editor themes and styles
├── 2.16 Port common dialogs (OpenFile, SaveFile, FolderBrowser)
├── 2.17 Port message dialogs (Info, Warning, Error, Yes/No/Cancel)
├── 2.18 Create side-by-side test harness (WPF vs Avalonia)
└── 2.19 Create regression test suite for all core controls

Milestone 3 Decision Gate (End of Year 2 Q2):
├── ✅ PropertyGrid + Dock feature parity ≥90% → Proceed to Phase 3
├── ⚠️ Feature gap <10% → Acceptable, iterate in parallel
└── ❌ Feature gap >20% → Add 4-8 weeks for gap closure
```

### Phase 3: Asset Editors (Year 2, Q3-Q4 — 24-32 weeks)

```
Q3: Code Editors & UI Editors
├── 3.1 Integrate forked RoslynPad.Avalonia
├── 3.2 Adapt Stride's ScriptTextEditor wrapper
├── 3.3 Adapt Stride's RoslynHost and RoslynWorkspace
├── 3.4 Port ScriptEditorView (syntax highlighting, IntelliSense)
├── 3.5 Port script file tree navigation panel
├── 3.6 Port UIEditorView (WYSIWYG canvas, element selection)
└── 3.7 Port VisualScriptEditorView (node graph, connections)

Q4: Specialized Editors
├── 3.8 Port CurveEditorView (animation curves, keyframes, tangents)
├── 3.9 Port MaterialEditorView (shader graph, preview)
├── 3.10 Port AssetPickerView (thumbnail gallery, search, filtering)
├── 3.11 Port SpriteEditor, SoundEditor (if applicable)
├── 3.12 Wire each Avalonia editor into WPF host via interop
├── 3.13 Validate each editor in interop host (focus, input, layout)
├── 3.14 Create automated rendering tests (screenshot comparison)
└── 3.15 Document editor migration status and known issues

Milestone 4 Decision Gate (End of Year 2 Q4):
├── ✅ All 8 Asset Editors migrated → Proceed to Phase 4
├── ⚠️ 1-2 Editors have defects → Acceptable, fix in parallel
└── ❌ >3 Editors have defects → Add 4-8 weeks for fixes
```

### Phase 4: Game Studio & 3D Preview (Year 3, Q1-Q2 — 24-32 weeks)

```
Q1: GameEngineHost Cross-Platform
├── 4.1 Define IGameHost interface for cross-platform HWND hosting
├── 4.2 Implement Win32GameHost (SetParent + Win32 message forwarding)
├── 4.3 Implement X11GameHost (XReparentWindow + X11 event monitoring)
├── 4.4 Implement MacGameHost (NSView embedding + NSEvent monitoring)
├── 4.5 Implement AvaloniaGameHost wrapping IGameHost in NativeControlHost
├── 4.6 Create GameContext abstraction for platform-specific context
├── 4.7 Port input forwarding (keyboard, mouse) to platform-agnostic events
├── 4.8 Port DPI-aware resize logic for 3D viewport
└── 4.9 Test 3D preview on all 3 platforms

Q2: Main Window & Shell
├── 4.10 Integrate 3D preview into Avalonia editor layout
├── 4.11 Wire preview asset selection and property change updates
├── 4.12 Port menu bar (File, Edit, View, Assets, Scene, Tools, Help)
├── 4.13 Port toolbar (icons, dropdowns, toggle states)
├── 4.14 Port status bar (progress, messages, clickable items)
├── 4.15 Port startup wizard / new project dialog
├── 4.16 Port settings/preferences dialog
├── 4.17 Port project settings dialog
├── 4.18 Port session management (recent projects, auto-save, crash recovery)
├── 4.19 Switch main entry point from WPF Application to Avalonia AppBuilder
├── 4.20 Validate full Game Studio on all 3 platforms
└── 4.21 Document known issues and workarounds

Milestone 5 Decision Gate (End of Year 3 Q2):
├── ✅ All 3 platforms runnable → Proceed to Post-Migration
├── ⚠️ 1 platform has defects → Fix before continuing
└── ❌ >1 platform unusable → Re-evaluate
```

### Phase 5: Post-Migration Validation (Year 3, Q3-Q4 — 12-16 weeks)

```
Q3: Cleanup & Optimization
├── 5.1 Remove WPF interop bridge
├── 5.2 Archive or remove WPF projects from solution
├── 5.3 Performance tuning (renderer settings, UI virtualization)
├── 5.4 Memory optimization
├── 5.5 Update public API documentation
└── 5.6 Document migration guide for plugin developers

Q4: Validation & Release
├── 5.7 Full feature regression test (Windows)
├── 5.8 Full feature regression test (Linux, Ubuntu 22.04+)
├── 5.9 Full feature regression test (macOS, 13+)
├── 5.10 Performance benchmark comparison (WPF vs Avalonia)
├── 5.11 Accessibility audit (keyboard navigation, screen reader)
├── 5.12 Plugin compatibility validation
├── 5.13 Community beta testing (2 weeks)
├── 5.14 Address feedback and edge cases
├── 5.15 Finalize CI/CD: all 3 platforms build, test, package
└── 5.16 Release Stride Game Studio with Avalonia UI

Milestone 6: Release Gate (End of Year 3 Q4):
├── ✅ All tests pass → Release
└── ⚠️ Critical defects → Delay release, fix issues
```

---

## Timeline Summary

| Phase | Duration | Cumulative |
|-------|----------|------------|
| **Phase 0**: Spike Validation | 12-16 weeks | 3-4 months |
| **Phase 1**: Infrastructure | 16-20 weeks | 7-9 months |
| **Phase 2**: Core Components | 20-24 weeks | 12-15 months |
| **Phase 3**: Asset Editors | 24-32 weeks | 18-23 months |
| **Phase 4**: Game Studio | 24-32 weeks | 24-31 months |
| **Phase 5**: Post-Migration | 12-16 weeks | 27-35 months |
| **Buffer** | 15 weeks | — |
| **Total** | **108-140 weeks** | **2.1-2.7 years** |

**3-Year Timeline**: ✅ Within scope, with ~15 weeks buffer for unexpected issues.

---

## Open Questions (Updated)

1. **macOS Metal rendering**: Does Stride's MSL pipeline (from SPIR-V rewrite) support macOS editor viewport rendering at acceptable FPS on Apple Silicon? **S.3 Spike required.**
2. **Dock.Avalonia feature parity**: Does wieslawsoltes/Dock support all AvalonDock features used by Stride? Specifically test: layout serialization across sessions, floating window position restoration, auto-hide with docked tool windows, MDI tab tear-off. **S.1 Spike required.**
3. **Wayland constraint**: Phase 1-2 interop is Windows-only due to Wayland's lack of window embedding support. Phase 3-4 (Pure Avalonia) removes this constraint. Is this acceptable?
4. **Avalonia 12 breaking changes**: Avalonia 12 made significant API changes (`Window`→`TopLevel`, renderer changes, input system). Do these affect the migration plan? **S.4 Spike covers this.**
5. **Plugin API surface**: What is the public API surface that plugins consume? Should be documented before breaking changes. **S.6 Spike covers this.**
6. **Fork maintenance**: Who will maintain the forked repositories (Stridefork/Dock, Stridefork/RoslynPad)? Assign dedicated maintainer.
7. **Team composition**: 5-7 person team recommended (see Team Configuration section). When can team be assembled?
8. **CI/CD infrastructure**: What build agents/OS combinations are needed? Linux/macOS build agents for cross-platform validation? Any cost implications?
9. **Fallback for macOS Metal**: If S.3 Spike fails, is MoltenVK or software rendering acceptable? Estimated additional effort: 4-8 weeks.
10. **Actipro Avalonia fallback**: If Dock.Avalonia customization proves insufficient, is Actipro Avalonia (commercial, ~$1k/dev) acceptable as fallback?