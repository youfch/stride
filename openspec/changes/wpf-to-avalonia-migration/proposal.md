## Why

The Stride Game Studio editor is currently built on WPF (.NET Framework), which is Windows-only and has reached platform stability with no new innovation. Migrating to **Avalonia 12** unlocks cross-platform support (Windows, Linux, macOS), taps into a rapidly evolving UI framework with 19x performance improvements over WPF, and future-proofs the editor for the next decade. This migration is critical for expanding Stride's reach beyond Windows and modernizing the development experience.

## What Changes

- Replace WPF UI framework with **Avalonia 12** across all 14 WPF editor projects
- Re-implement `Stride.Core.Presentation.Wpf` controls as Avalonia controls in `Stride.Core.Presentation.Avalonia`
- Replace **AvalonDock** (WPF) with **forked Dock.Avalonia** (wieslawsoltes/Dock customized for Stride)
- Migrate **PropertyGrid** (WPF-specific) to Avalonia DataGrid-based implementation
- Port **GameEngineHost** (HWND embedding via Win32 API) to cross-platform `HwndHost` + platform adapters (Win32/X11/macOS)
- Replace **RoslynPad** (WPF) with **forked RoslynPad.Avalonia** (roslynpad/RoslynPad customized for Stride)
- Replace WPF-specific rendering paths (`D3DImage`, `HwndHost`) with platform-agnostic equivalents
- Update build system: WPF project SDK → Avalonia project SDK (`Avalonia.Desktop`, `Avalonia.Diagnostics`)
- Maintain parallel WPF/Avalonia support during migration (Phase 1-2) for incremental rollout
- **Fork & Customize Strategy**: Fork critical third-party libraries (Dock.Avalonia, RoslynPad.Avalonia) for full control and customization

## Capabilities

### New Capabilities
- `avn-infrastructure`: Avalonia project scaffolding, build system, shared base services, DI integration, cross-platform targeting (Windows/Linux/macOS)
- `avn-core-controls`: Avalonia equivalents of all core WPF controls — `PropertyGrid`, `TreeView`, `EditableListBox`, `WatermarkTextBox`, `ColorPicker`, custom Themes/Styles
- `avn-docking`: Replace AvalonDock with **customized Dock.Avalonia** — multi-pane layout, floating windows, tool windows, layout persistence, context-sensitive tool visibility
- `avn-asset-editors`: Port all WPF asset editors — `UIEditorView`, `VisualScriptEditorView`, `CurveEditorView`, `ScriptEditorView`, `MaterialEditorView` — to Avalonia with identical UX
- `avn-game-studio`: Main Game Studio window shell — menu bar, toolbar, status bar, pane management, session management, startup wizard, settings dialogs
- `avn-3d-preview`: Cross-platform 3D preview rendering — `GameEngineHost` migration, HWND embedding abstraction (Win32/X11/macOS), input forwarding, DPI-aware resize
- `avn-roslynpad`: Code editor integration — replace WPF RoslynPad with **forked RoslynPad.Avalonia**, syntax highlighting, IntelliSense, diagnostics, code completion, file tree navigation
- `avn-editor-interop`: Seamless WPF/Avalonia interoperability layer — shared view models, data binding bridge, event forwarding, visual tree embedding during migration

### Modified Capabilities
*(No existing specs to modify — this is the first OpenSpec change for this repo.)*

## Impact

| Area | Impact |
|------|--------|
| **Build System** | WPF SDK (`<UseWPF>`) → Avalonia SDK (`Avalonia.Desktop`). Update `Directory.Build.props`, MSBuild SDK packages, CI/CD pipelines |
| **Project Structure** | 14 WPF projects need Avalonia counterparts or direct replacement. Folder layout: `sources/presentation/Stride.Core.Presentation.Avalonia/` |
| **Dependencies** | Remove: `AvalonDock`, `System.Windows.Interactivity`, WPF-specific packages. Add: `Avalonia 12.x`, `Avalonia.Desktop`, `Avalonia.Diagnostics`, **customized Dock.Avalonia**, **forked RoslynPad.Avalonia** |
| **NuGet Packages** | Publish `Stride.Core.Presentation.Avalonia` as a NuGet. Maintain backward-compatible WPF packages during transition |
| **CI/CD** | Update GitHub Actions workflows: Windows/Linux/macOS build targets, Avalonia headless for testing, Pixel-perfect screenshot diff tests |
| **Performance** | Expected 5-19x improvement in UI responsiveness (Avalonia 12 renderer). Reduced memory footprint from modern rendering pipeline |
| **Accessibility** | Avalonia's accessibility support (AutomationPeer) differs from WPF's. May need custom automation peers for custom controls |
| **Third-Party** | Impact on Stride Community Toolkit, plugin ecosystem, and any external tools referencing WPF internals |
| **Forked Repositories** | Maintain `Stridefork/Dock` and `Stridefork/RoslynPad` with quarterly upstream syncs |

## Timeline

| Phase | Duration | Timeline |
|-------|----------|----------|
| **Phase 0**: Spike Validation | 12-16 weeks | Year 1, Q1 |
| **Phase 1**: Infrastructure | 16-20 weeks | Year 1, Q2-Q4 |
| **Phase 2**: Core Components | 20-24 weeks | Year 2, Q1-Q2 |
| **Phase 3**: Asset Editors | 24-32 weeks | Year 2, Q3-Q4 |
| **Phase 4**: Game Studio | 24-32 weeks | Year 3, Q1-Q2 |
| **Phase 5**: Post-Migration | 12-16 weeks | Year 3, Q3-Q4 |
| **Buffer** | 15 weeks | — |
| **Total** | **108-140 weeks** | **2.1-2.7 years** |

**Approved Timeline**: 3 years (within scope with ~15 weeks buffer)

## Key Risks & Mitigations

| Risk | Rating | Mitigation |
|------|--------|------------|
| **macOS Metal rendering** | 🔴 HIGH | S.3 Spike in Phase 0 validates MSL pipeline; fallback: MoltenVK (+4-8 weeks) |
| **Dock.Avalonia feature gap** | 🟡 MEDIUM | Fork wieslawsoltes/Dock, S.1 Spike validates all scenarios |
| **Wayland constraint** | 🟡 MEDIUM | Phase 1-2 documented as Windows-only; Phase 3-4 cross-platform |
| **Plugin ecosystem breakage** | 🟢 LOW | Interop bridge + migration guide; WPF support for 2 release cycles |