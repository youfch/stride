## 0. Phase 0: Spike Validation (Year 1, Q1 — 12-16 weeks)

**Phase 0 Status**: ✅ **COMPLETE** (All 9 spike/validation tasks finished)

### S.1 Spike: Dock.Avalonia Feature Parity (2-3 weeks)
- [x] S.1.1 Fork `wieslawsoltes/Dock` → create `Stridefork/Dock` repository
- [x] S.1.2 Set up local development environment for Dock.Avalonia
- [x] S.1.3 Test layout serialization (JSON, XML, YAML, Protobuf formats)
- [x] S.1.4 Map Stride's current AvalonDock layout configuration to Dock.Avalonia
- [x] S.1.5 Test floating windows (detach/attach, position restoration)
- [x] S.1.6 Test auto-hide functionality with docked tool windows
- [x] S.1.7 Test MDI tabs and tear-off behavior
- [x] S.1.8 Test layout persistence across sessions (save/restore)
- [x] S.1.9 Evaluate Actipro Avalonia as fallback (commercial option)
- [x] S.1.10 **Decision Gate**: Proceed with Dock.Avalonia or fallback to Actipro?

### S.2 Spike: RoslynPad.Avalonia Integration (1-2 weeks)
- [x] S.2.1 Fork `roslynpad/RoslynPad` → create `Stridefork/RoslynPad` repository
- [x] S.2.2 Set up local development environment for RoslynPad.Avalonia
- [x] S.2.3 Integrate `RoslynPad.Avalonia.Editor` in Stride context
- [x] S.2.4 Adapt Stride's `ScriptTextEditor` wrapper for Avalonia
- [x] S.2.5 Adapt Stride's `SimpleCodeTextEditor` wrapper for Avalonia
- [x] S.2.6 Validate syntax highlighting (ExpressionDark theme compatibility)
- [x] S.2.7 Validate IntelliSense (completion, signature help)
- [x] S.2.8 Validate diagnostics (real-time, inline)
- [x] S.2.9 Validate code fixes and quick actions
- [x] S.2.10 **Decision Gate**: Proceed with forked RoslynPad?

### S.3 Spike: macOS Metal Rendering (2-3 weeks)
- [x] S.3.1 Set up macOS development environment (Apple Silicon Mac)
- [x] S.3.2 Build Stride from source targeting macOS
- [x] S.3.3 Test editor 3D viewport rendering via Metal/MSL
- [x] S.3.4 Measure FPS (target: ≥30 FPS for editor viewport)
- [x] S.3.5 Measure rendering latency (target: <16ms frame time)
- [x] S.3.6 Test SPIR-V → MSL conversion pipeline
- [x] S.3.7 Test shader compilation and error reporting
- [x] S.3.8 Evaluate MoltenVK fallback (if Metal not viable)
- [x] S.3.9 Evaluate software rendering fallback (if MoltenVK not viable)
- [x] S.3.10 **Decision Gate**: Metal viable? If not, plan fallback (+4-8 weeks)?

### S.4 Spike: WPF↔Avalonia Interop PoC (2-3 weeks)
- [x] S.4.1 Implement `AvaloniaInWpfHost` (embed Avalonia Window in WPF HwndHost)
- [x] S.4.2 Test basic embedding (Avalonia panel in WPF window)
- [x] S.4.3 Test keyboard input forwarding (WPF → Avalonia)
- [x] S.4.4 Test mouse input forwarding (WPF → Avalonia)
- [x] S.4.5 Test IME input forwarding (Chinese/Japanese/Korean)
- [x] S.4.6 Test layout sync between WPF parent and Avalonia child
- [x] S.4.7 Measure input latency (target: <5ms additional latency)
- [x] S.4.8 Test clipboard operations (copy/paste between WPF and Avalonia)
- [x] S.4.9 Test file dialog forwarding
- [x] S.4.10 **Decision Gate**: Interop viable for Phase 1-2?

### S.5 Spike: Linux Vulkan Rendering (1-2 weeks)
- [x] S.5.1 Set up Linux development environment (Ubuntu 22.04+)
- [x] S.5.2 Install Vulkan drivers and validation layers
- [x] S.5.3 Build Stride from source targeting Linux
- [x] S.5.4 Test editor 3D viewport rendering via Vulkan
- [x] S.5.5 Measure FPS (target: ≥30 FPS for editor viewport)
- [x] S.5.6 Measure rendering latency
- [x] S.5.7 Test shader compilation (HLSL → SPIR-V → Vulkan)
- [x] S.5.8 **Decision Gate**: Vulkan viable for editor viewport?

### S.6 Spike: PropertyGrid DataGrid PoC (1-2 weeks)
- [x] S.6.1 Create basic Avalonia PropertyGrid using `Avalonia.Controls.DataGrid`
- [x] S.6.2 Implement property binding (INotifyPropertyChanged)
- [x] S.6.3 Implement basic property editors (String, Numeric, Boolean)
- [x] S.6.4 Test property validation and error display
- [x] S.6.5 Compare UX with WPF PropertyGrid (side-by-side)
- [x] S.6.6 Test keyboard navigation (tab order, arrow keys)
- [x] S.6.7 Test inline editing (flyout-based)
- [x] S.6.8 **Decision Gate**: DataGrid-based approach viable?

### S.7 Spike: Avalonia 12 Breaking Changes (1 week)
- [x] S.7.1 Evaluate `Window` → `TopLevel` API changes
- [x] S.7.2 Evaluate renderer changes (Compositor, immediate mode)
- [x] S.7.3 Evaluate input system changes
- [x] S.7.4 Evaluate `NativeControlHost` / `HwndHost` changes
- [x] S.7.5 Document breaking changes affecting migration plan
- [x] S.7.6 **Decision Gate**: Any show-stoppers?

### S.8 Spike: Plugin API Surface Analysis (1 week)
- [x] S.8.1 Identify all public API surfaces that plugins consume from WPF projects
- [x] S.8.2 Document API surface in `docs/plugin-api-surface.md`
- [x] S.8.3 Identify breaking changes for plugin developers
- [x] S.8.4 Draft migration guide for plugin authors
- [x] S.8.5 **Decision Gate**: API surface documented and migration guide drafted?

---

## 1. Phase 1: Infrastructure & Foundation (Year 1, Q2-Q4 — 16-20 weeks)

### Q2: Fork Setup & Build System
- [ ] 1.1 Create `Stridefork/Dock` repository, fork `wieslawsoltes/Dock`
- [ ] 1.2 Create `Stridefork/RoslynPad` repository, fork `roslynpad/RoslynPad`
- [ ] 1.3 Set up dual-build system (conditional MSBuild targets for WPF/Avalonia)
- [ ] 1.4 Update `Stride.Build.Sdk.Editor` for Avalonia targets
- [ ] 1.5 Create `Stride.Core.Presentation.Avalonia` project
- [ ] 1.6 Create `Stride.Core.Presentation.Interop` project
- [ ] 1.7 Configure CI/CD for forked repositories (GitHub Actions)
- [ ] 1.8 Establish code review guidelines for forked repos

### Q3: Interop Layer & Base Controls
- [ ] 1.9 Implement `AvaloniaInWpfHost` (embed Avalonia Window in WPF HwndHost)
- [ ] 1.10 Implement `WpfInAvaloniaHost` (embed WPF Window in Avalonia NativeControlHost) — fallback
- [ ] 1.11 Create interop service (clipboard, file dialogs, dispatcher bridging)
- [ ] 1.12 Port base classes: `WindowBase`, `UserControl`, `ViewModel` binding infrastructure
- [ ] 1.13 Port core converters from WPF `IValueConverter` to Avalonia `IValueConverter`
- [ ] 1.14 Create Avalonia resource dictionaries (styles, colors, brushes)
- [ ] 1.15 Port `Stride.Core.Presentation.Wpf.Controls` base controls
- [ ] 1.16 PoC: Host simple Avalonia panel ("About" dialog) inside WPF Game Studio

### Q4: CI/CD & Testing Infrastructure
- [ ] 1.17 Set up `Avalonia.Headless` test project for UI unit tests
- [ ] 1.18 Add GitHub Actions workflows for Avalonia build (Windows/Linux/macOS)
- [ ] 1.19 Verify dual-build: same solution produces both WPF and Avalonia targets
- [ ] 1.20 Document build matrix and CI/CD pipeline
- [ ] 1.21 Establish code review and merge guidelines for forked repos
- [ ] 1.22 Create migration tracking dashboard (progress, blockers, decisions)

**Milestone 1: End of Phase 0** (Year 1, Q1 end)
- [x] All 8 Spikes completed
- [x] Decision gates passed (or fallback plans activated)
- [x] Proceed to Phase 1

**Milestone 2: End of Phase 1** (Year 1, Q4 end)
- [ ] Dual-build system functional
- [ ] Interop layer implemented and tested
- [ ] Base controls ported
- [ ] CI/CD pipeline operational
- [ ] Proceed to Phase 2

---

## 2. Phase 2: Core UI Components (Year 2, Q1-Q2 — 20-24 weeks)

### Q1: PropertyGrid & Basic Controls
- [ ] 2.1 Implement Avalonia `PropertyGrid` control (DataGrid-based)
- [ ] 2.2 Implement property editor types: StringEditor, NumericEditor, BooleanEditor, EnumEditor
- [ ] 2.3 Implement complex editors: ColorEditor, VectorEditor, QuaternionEditor, AssetReferenceEditor
- [ ] 2.4 Implement PropertyGrid search/filter and category grouping
- [ ] 2.5 Implement custom `IPropertyEditorProvider` registry for extensibility
- [ ] 2.6 Port `EditableListBox` control
- [ ] 2.7 Port `WatermarkTextBox` control
- [ ] 2.8 Port custom `ColorPicker` control
- [ ] 2.9 Port custom `TreeView` with lazy loading and drag-drop
- [ ] 2.10 Port additional custom controls from `Stride.Core.Presentation.Wpf.Controls`

### Q2: Docking & Themes
- [ ] 2.11 Adapt `Dock.Serializer.Xml` for Stride layout compatibility
- [ ] 2.12 Adapt Stride's `AvalonDockHelper.IsVisible` to Dock.Avalonia
- [ ] 2.13 Integrate customized Dock.Avalonia + Dock.Model
- [ ] 2.14 Port layout persistence (save/restore) to Dock.Avalonia format
- [ ] 2.15 Port AvalonDock pane templates/themes to Dock.Avalonia templates
- [ ] 2.16 Implement context-sensitive tool pane visibility
- [ ] 2.17 Port all custom Stride editor themes and styles to Avalonia
- [ ] 2.18 Port common dialogs: OpenFileDialog, SaveFileDialog, FolderBrowserDialog
- [ ] 2.19 Port message dialogs (Info, Warning, Error, Yes/No/Cancel)
- [ ] 2.20 Create side-by-side test harness comparing WPF vs Avalonia controls
- [ ] 2.21 Create regression test suite for all core controls (interaction + visual)

**Milestone 3: End of Phase 2** (Year 2, Q2 end)
- [ ] PropertyGrid feature parity ≥90% with WPF version
- [ ] Dock.Avalonia integrated and tested
- [ ] All core controls ported
- [ ] Themes and styles migrated
- [ ] Proceed to Phase 3

---

## 3. Phase 3: Asset Editors (Year 2, Q3-Q4 — 24-32 weeks)

### Q3: Code Editors & UI Editors
- [ ] 3.1 Integrate forked `RoslynPad.Avalonia` NuGet package
- [ ] 3.2 Adapt Stride's `ScriptTextEditor` wrapper for Avalonia
- [ ] 3.3 Adapt Stride's `RoslynHost` for Avalonia
- [ ] 3.4 Adapt Stride's `RoslynWorkspace` for Avalonia
- [ ] 3.5 Port `ScriptEditorView` — code editor with syntax highlighting and IntelliSense
- [ ] 3.6 Port script file tree navigation panel
- [ ] 3.7 Port `UIEditorView` — WYSIWYG canvas with element selection and property binding
- [ ] 3.8 Port `VisualScriptEditorView` — node graph canvas with connections and library panel

### Q4: Specialized Editors
- [ ] 3.9 Port `CurveEditorView` — animation curve editor with keyframes and tangents
- [ ] 3.10 Port `MaterialEditorView` — shader graph editor with preview
- [ ] 3.11 Port asset picker dialog (thumbnail gallery with search and filtering)
- [ ] 3.12 Port SpriteEditor, SoundEditor (if applicable)
- [ ] 3.13 Wire each Avalonia editor into WPF host via interop bridge
- [ ] 3.14 Validate each editor in interop host (focus, input, layout behavior)
- [ ] 3.15 Create automated rendering tests for each editor (screenshot comparison)
- [ ] 3.16 Document editor migration status and known issues

**Milestone 4: End of Phase 3** (Year 2, Q4 end)
- [ ] All 8 Asset Editors migrated
- [ ] RoslynPad.Avalonia integrated and working
- [ ] Editors validated in interop host
- [ ] Proceed to Phase 4

---

## 4. Phase 4: Game Studio & 3D Preview (Year 3, Q1-Q2 — 24-32 weeks)

### Q1: GameEngineHost Cross-Platform
- [ ] 4.1 Define `IGameHost` interface for cross-platform HWND hosting
- [ ] 4.2 Implement `Win32GameHost` — SetParent + Win32 message forwarding
- [ ] 4.3 Implement `X11GameHost` — XReparentWindow + X11 event monitoring
- [ ] 4.4 Implement `MacGameHost` — NSView embedding + NSEvent monitoring
- [ ] 4.5 Implement `AvaloniaGameHost` wrapping `IGameHost` in NativeControlHost
- [ ] 4.6 Create `GameContext` abstraction for different platforms
- [ ] 4.7 Port input forwarding system (keyboard, mouse) to platform-agnostic events
- [ ] 4.8 Port DPI-aware resize logic for the 3D viewport
- [ ] 4.9 Test 3D preview rendering on all 3 platforms (Windows/Linux/macOS)

### Q2: Main Window & Shell
- [ ] 4.10 Integrate 3D preview into Avalonia editor layout
- [ ] 4.11 Wire preview asset selection and property change updates
- [ ] 4.12 Port menu bar (File, Edit, View, Assets, Scene, Tools, Help)
- [ ] 4.13 Port toolbar (icons, dropdowns, toggle states)
- [ ] 4.14 Port status bar (progress, messages, clickable items)
- [ ] 4.15 Port startup wizard / new project dialog
- [ ] 4.16 Port settings/preferences dialog
- [ ] 4.17 Port project settings dialog
- [ ] 4.18 Port session management (recent projects, auto-save, crash recovery)
- [ ] 4.19 Switch main entry point from WPF `Application` to Avalonia `AppBuilder`
- [ ] 4.20 Validate full Game Studio on all 3 platforms
- [ ] 4.21 Document known issues and workarounds

**Milestone 5: End of Phase 4** (Year 3, Q2 end)
- [ ] GameEngineHost cross-platform working
- [ ] Main window shell migrated
- [ ] All 3 platforms runnable
- [ ] Proceed to Post-Migration

---

## 5. Post-Migration: Validation & Polish (Year 3, Q3-Q4 — 12-16 weeks)

### Q3: Cleanup & Optimization
- [ ] 5.1 Remove WPF interop bridge
- [ ] 5.2 Archive or remove WPF projects from solution
- [ ] 5.3 Performance tuning (renderer settings, UI virtualization)
- [ ] 5.4 Memory optimization
- [ ] 5.5 Update public API documentation
- [ ] 5.6 Document migration guide for plugin developers

### Q4: Validation & Release
- [ ] 5.7 Full feature regression test on Windows
- [ ] 5.8 Full feature regression test on Linux (Ubuntu 22.04+)
- [ ] 5.9 Full feature regression test on macOS (13+)
- [ ] 5.10 Performance benchmark comparison (WPF vs Avalonia)
- [ ] 5.11 Accessibility audit (keyboard navigation, screen reader)
- [ ] 5.12 Plugin compatibility validation
- [ ] 5.13 Community beta testing period (2 weeks)
- [ ] 5.14 Address feedback and edge cases
- [ ] 5.15 Finalize CI/CD: all 3 platforms build, test, and package
- [ ] 5.16 Release Stride Game Studio with Avalonia UI

**Milestone 6: Release Gate** (Year 3, Q4 end)
- [ ] All tests pass on all 3 platforms
- [ ] Performance benchmarks met or exceeded
- [ ] Plugin compatibility verified
- [ ] Community beta feedback addressed
- [ ] Release Stride Game Studio with Avalonia UI

---

## 6. Timeline Summary

| Phase | Duration | Cumulative | Buffer | Risk-Adjusted Total |
|-------|----------|------------|--------|-------------------|
| **Phase 0**: Spike Validation | 12-16 weeks | 3-4 months | 2 weeks | 14-18 weeks |
| **Phase 1**: Infrastructure | 16-20 weeks | 7-9 months | 3 weeks | 19-23 weeks |
| **Phase 2**: Core Components | 20-24 weeks | 12-15 months | 3 weeks | 23-27 weeks |
| **Phase 3**: Asset Editors | 24-32 weeks | 18-23 months | 4 weeks | 28-36 weeks |
| **Phase 4**: Game Studio | 24-32 weeks | 24-31 months | 4 weeks | 28-36 weeks |
| **Phase 5**: Post-Migration | 12-16 weeks | 27-35 months | 2 weeks | 14-18 weeks |
| **Total Phase Time** | **108-140 weeks** | — | — | — |
| **Total Buffer** | **18 weeks** | — | — | — |
| **Risk-Adjusted Total** | **126-158 weeks** | **2.4-3.0 years** | — | — |

**Note**: Buffer is split per-phase rather than one global pool. This prevents a single phase overrun from consuming the entire contingency. Per-phase buffers are:
- Phase 0: 2 weeks (spike schedule uncertainty is low — each spike is short)
- Phase 1: 3 weeks (build system + interop unknowns)
- Phase 2: 3 weeks (PropertyGrid complexity may surprise)
- Phase 3: 4 weeks (editor migration difficulty is hard to estimate precisely)
- Phase 4: 4 weeks (cross-platform 3D preview is highest-risk)
- Phase 5: 2 weeks (validation may find edge cases)

---

## 7. Critical Path Analysis

### Critical Path Diagram

```
Phase 0 (Spikes) ──► Phase 1 (Infrastructure) ──► Phase 2 (Core Components)
         │                                                     │
         │                                                     ▼
         └──────────────────────────┐             Phase 3 (Asset Editors)
                                    │                      │
                                    ▼                      ▼
                            Phase 4 (Game Studio & 3D Preview)
                                    │
                                    ▼
                            Phase 5 (Validation & Release)
```

### Parallel Tracks (non-critical path)

```
Phase 2 ──► Fork Maintenance Track (parallel, low risk)
  │            Dock.Avalonia (ongoing upstream sync)
  │            RoslynPad.Avalonia (ongoing upstream sync)
  ▼
Phase 3 ──► Plugin Compatibility Track (parallel, can be deferred)
                Plugin API docs (Phase 0 start)
                Migration guide for plugin authors (Phase 3 start)
                Community outreach (Phase 3-4)
```

### Critical Path Itemized (LONGEST chain with maximum durations)

| Step | Duration | Depends On | Notes |
|------|----------|------------|-------|
| S.1 (Dock spike) | 3 wks | None | Critical — Dock parity affects all UI |
| S.4 (Interop spike) | 3 wks | None | Critical — Interop enables incremental migration |
| S.6 (PropertyGrid spike) | 2 wks | None | Critical — PropertyGrid is most-used control |
| S.7 (Avalonia 12 eval) | 1 wk | S.1, S.4, S.6 | Consolidate spike findings |
| Phase 1 | 20 wks | All spikes passed | Foundation, dual-build, interop, base controls |
| Phase 2 Q1 (PropertyGrid) | 12 wks | Phase 1 | PropertyGrid migration |
| Phase 2 Q2 (Docking) | 12 wks | Phase 1, S.1 | Docking integration |
| Phase 3 | 32 wks | Phase 2 | Asset editors (longest phase) |
| Phase 4 | 32 wks | Phase 3 | Game Studio shell + 3D preview |
| Phase 5 | 16 wks | Phase 4 | Validation, cleanup, release |
| **Total Critical Path** | **136 wks** | — | With max durations and no buffer overrun |

### Critical Path Risks

1. **Phase 3 is the longest chain** — 32 weeks. Any slippage here directly extends the timeline. Consider parallelizing editor work: ScriptEditor + VisualScriptEditor can be done simultaneously by different engineers.

2. **Phase 4 (3D Preview) is the highest-risk chain** — cross-platform rendering on Linux/macOS has unknowns that Phase 0 spikes (S.3, S.5) can only partially de-risk.

3. **Interop quality (Phase 1) determines all subsequent phases** — if the WPF↔Avalonia interop has fundamental issues, every downstream task that depends on incremental migration is affected. This is a "single point of failure" in the critical path.

4. **Fork maintenance is OFF critical path** — upstream changes to Dock/RoslynPad can be merged asynchronously. This is deliberately non-blocking.

---

## 8. Risk Register

| ID | Risk | Impact | P | I | RPN | Phase | Mitigation | Contingency |
|----|------|--------|---|---|-----|-------|------------|-------------|
| R01 | **Dock.Avalonia missing AvalonDock features** (layout serialization, floating windows, auto-hide, MDI tabs) | High — blocks Phase 2 docking migration | M | H | 12 | 0 | S.1 Spike: exhaustive feature mapping early. Evaluate Actipro Avalonia as fallback in parallel. | Fallback to Actipro Avalonia (commercial). Add 4-6 weeks for integration. |
| R02 | **macOS Metal rendering fails or has poor performance** (<30 FPS, shader compilation errors) | High — blocks Phase 4 cross-platform 3D preview | M | H | 12 | 0 | S.3 Spike: measure FPS, latency, MSL pipeline on real Apple Silicon hardware. | Fallback to MoltenVK (Vulkan-on-Metal). Add 4-8 weeks. |
| R03 | **Linux Vulkan rendering issues** (driver compatibility, shader compilation, window system) | Medium — blocks Linux editor 3D viewport | M | M | 9 | 0 | S.5 Spike: test on Ubuntu 22.04+ with multiple GPU vendors (NVIDIA/AMD/Intel). | Fallback to software rendering for editor viewport. Add 4-6 weeks. |
| R04 | **WPF↔Avalonia interop too slow** (>5ms input latency, IME issues, layout sync problems) | High — undermines entire incremental migration strategy | M | H | 12 | 1 | S.4 Spike: measure latency. Test IME, clipboard, layout sync. Keep WPF-in-Avalonia fallback path ready. | Skip incremental — go directly to pure Avalonia (Phase 3 approach). Adds major risk upstream. |
| R05 | **PropertyGrid behavior differences** cause subtle UX regressions | Medium — user frustration, support burden | L | M | 6 | 2 | S.6 Spike + Phase 2 Q1: side-by-side test harness, automated pixel-diff tests, comprehensive keyboard navigation tests. | Accept <10% gap as known issue, fix in post-migration polish. |
| R06 | **Plugin ecosystem breakage** — third-party plugins reference WPF internals | High — ecosystem disruption, user backlash | H | H | 16 | 0 | S.8 Spike: document API surface early. Provide Stride.Core.Presentation.Interop bridge. 2 release cycle soft deprecation. | Extend WPF support timeline. Maintain parallel WPF branch for plugin compatibility. |
| R07 | **Team composition delays** — can't hire/assign Avalonia specialists | High — no migration happens without engineers | M | H | 12 | ALL | Start recruiting in Year 1 Q4 (6 months before Phase 1 ends). Cross-train existing WPF engineers. | Contract external Avalonia specialists. Use freelancers for Phase 2 control migration. |
| R08 | **Fork maintenance burden** — upstream Dock/RoslynPad diverge from Stridefork | Medium — accumulating technical debt in forked repos | M | M | 9 | ALL | Assign dedicated maintainers per fork. Quarterly upstream sync schedule. Document all customizations. | If fork divergence is too high: re-evaluate dependency, consider alternatives. |
| R09 | **Avalonia 12 breaking changes** — API changes during migration | Medium — unexpected refactoring costs | L | M | 6 | 1 | S.7 Spike: evaluate Window→TopLevel, Compositor, input system, NativeControlHost changes. Target a pinned Avalonia version. | If breaking changes are severe: delay Avalonia version bump until Phase 5. |
| R10 | **Wayland compatibility in Phase 4** — no native window embedding | Medium — Linux users on Wayland get degraded experience | M | M | 9 | 4 | Document as known limitation. Pure Avalonia (Post-Phase 4) removes this constraint naturally. | Provide XWayland fallback. Contribute upstream to Avalonia for Wayland NativeControlHost support. |
| R11 | **Migration fatigue** — team burnout on 2.5+ year project | Medium — attrition, quality decline, schedule slips | M | M | 9 | ALL | Ship value each phase. Public milestones. Rotate team members across phases. Celebrate Phase completions. | Reduce scope: prioritize cross-platform benefit over pixel-perfect WPF parity. |
| R12 | **Community resistance to Avalonia migration** — users prefer familiar WPF | Medium — reputation damage, slow adoption | L | M | 6 | 5 | Community beta period (Phase 5). Document migration benefits (cross-platform, 19x perf). Migration guide for plugin authors. | Maintain WPF branch for 2 release cycles. Allow users to choose WPF or Avalonia during transition. |

**Legend**: P = Probability (L/M/H), I = Impact (L/M/H), RPN = Risk Priority Number (P×I: 1-4=Low, 5-8=Medium, 9-12=High, 13-16=Critical)

### Top 3 Risks by RPN

| Rank | ID | Risk | RPN | Phase |
|------|----|------|-----|-------|
| 1 | R06 | Plugin ecosystem breakage | 16 | 0 |
| 2 | R01 | Dock.Avalonia feature parity | 12 | 0 |
| 3 | R02 | macOS Metal rendering | 12 | 0 |
| 3 | R04 | Interop latency | 12 | 1 |

### Risk Mitigation Schedule

| When | Action | Risk Addressed |
|------|--------|----------------|
| **Before Phase 0** | Begin recruiting Avalonia specialists | R07 |
| **Phase 0 (S.1)** | Dock.Avalonia feature mapping + Actipro evaluation | R01 |
| **Phase 0 (S.3, S.5)** | macOS Metal + Linux Vulkan validation | R02, R03 |
| **Phase 0 (S.4)** | Interop latency benchmarks | R04 |
| **Phase 0 (S.6)** | PropertyGrid approach validation | R05 |
| **Phase 0 (S.8)** | Plugin API surface documentation | R06 |
| **Phase 0 (S.7)** | Avalonia 12 breaking change audit | R09 |
| **Phase 1 Q2** | Assign fork maintainers | R08 |
| **Phase 2 Q2** | Side-by-side test harness running | R05 |
| **Phase 4 Q2** | Wayland compatibility assessment | R10 |
| **Phase 5** | Community beta + plugin migration guide | R06, R12 |

---

## 9. Resource Plan

### Core Team (Full Duration)

| Role | Count | Phases | Responsibilities | Key Skills |
|------|-------|--------|-----------------|------------|
| **Technical Lead** | 1 | ALL | Architecture decisions, milestone gates, risk management, code reviews | Stride codebase deep knowledge, WPF + Avalonia, cross-platform |
| **Avalonia Specialist #1** | 1 | Phase 1-5 | UI control migration, PropertyGrid, themes/styles, interop bridge | Avalonia API, WPF migration patterns, MVVM |
| **Avalonia Specialist #2** | 1 | Phase 2-5 | Docking integration, asset editors, custom controls | AvaloniaDock/Dock.Avalonia, complex controls |
| **Full-Stack Engineer #1** | 1 | Phase 2-5 | ScriptEditor + RoslynPad, VisualScriptEditor, CurveEditor | Roslyn/RoslynPad, node graph editors |
| **Full-Stack Engineer #2** | 1 | Phase 3-5 | UIEditor, MaterialEditor, asset picker, plugin compatibility | WPF asset editors, cross-platform testing |

### Specialist Roles (Phased)

| Role | Count | Phases | Duration | Responsibilities |
|------|-------|--------|----------|-----------------|
| **Rendering Engineer** | 1 | Phase 0 + Phase 4 | ~40 weeks | GameEngineHost cross-platform, 3D preview, Vulkan/Metal |
| **Build/DevOps Engineer** | 1 | Phase 0-1 | ~36 weeks | Dual-build system, CI/CD, forked repo config |
| **QA Engineer** | 1 | Phase 2-5 | ~80 weeks | Test harness, regression tests, platform validation |
| **Technical Writer** | 0.5 | Phase 3-5 | ~28 weeks | API docs, migration guide, plugin docs |

### Team Size By Phase

```
Phase 0:  3-4 people  (Tech Lead + 2 Specialists + Rendering [part-time])
Phase 1:  4-5 people  (Tech Lead + 2 Specialists + DevOps + Rendering [part-time])
Phase 2:  5-6 people  (Tech Lead + 2 Specialists + Full-stack #1 + QA + DevOps [part-time])
Phase 3:  6-7 people  (Tech Lead + 2 Specialists + 2 Full-stack + QA + Writer [part-time])
Phase 4:  6-7 people  (Tech Lead + 2 Specialists + Full-stack #1 + Rendering + QA + Writer [part-time])
Phase 5:  5-6 people  (Tech Lead + 2 Specialists + QA + Writer + Community Manager)
```

### Resource Scaling Options

| Scenario | Adjustment | Impact |
|----------|-----------|--------|
| **Accelerated timeline** | +2 engineers (cross-training 2 existing WPF devs to Avalonia) | -20% Phase 2-3 duration, but +25% coordination cost |
| **Budget constrained** | -1 Full-stack engineer (Phase 3 editors sequential instead of parallel) | +50% Phase 3 duration (48 weeks instead of 32) |
| **Talent shortage** | Contract Dock.Avalonia patch to upstream maintainer | Reduces fork maintenance burden. Cost: ~$20-40k |

### Hiring Timeline

| Role | When to Hire | Lead Time | Source |
|------|-------------|-----------|--------|
| Tech Lead | Day 1 | Internal promotion | Existing Stride contributor |
| Avalonia Specialists | Month 3 (Phase 0 end) | 3 months | Avalonia community, .NET ecosystem |
| Full-Stack Engineers | Month 9 (Phase 1 end) | 3 months | Game dev community, Roslyn contributors |
| Rendering Engineer | Month 0 + Month 18 | 3 months | Graphics dev, Vulkan/Metal experience |
| QA Engineer | Month 9 (Phase 1 end) | 2 months | .NET testing, UI automation |
| DevOps Engineer | Month 0 | 2 months | GitHub Actions, MSBuild, cross-platform |

---

## 10. Decision Gates (Detailed)

| Gate | Phase | Timing | Gate Criteria | Green (✅ Proceed) | Yellow (⚠️ with Conditions) | Red (❌ Re-Evaluate) |
|------|-------|--------|---------------|-------------------|---------------------------|---------------------|
| **DG-0** | Phase 0 | Week 1 | Migration feasibility confirmed | All spike proposals approved, team assembled | 1-2 spikes need more time, delay Phase 0 start by 2-4 weeks | Migration not feasible with current team/technology |
| **DG-1** | Phase 0 | Week 12-16 | All 8 spikes complete | All 8 spikes pass w/ green decisions | 1-2 spikes fail → execute fallbacks, add 4-8 weeks | 3+ spikes fail → full re-evaluation of migration |
| **DG-2** | Phase 1 | Week 28-36 | Infrastructure ready | Dual-build works, interop tested, CI/CD green | Interop has known issues → fix before Phase 2 | Build system not functional → re-evaluate approach |
| **DG-3** | Phase 2 | Week 48-60 | Core UI component parity | PropertyGrid + Dock ≥90% feature parity, test suite passing | Gap <10% → acceptable, fill gaps in Phase 3 | Gap >20% → add 4-8 weeks for gap closure |
| **DG-4** | Phase 3 | Week 72-92 | All asset editors migrated | All 8 editors migrated, tested in interop host, RoslynPad working | 1-2 editors have minor defects → acceptable, fix in Phase 4 | >3 editors have blocking defects → add 4-8 weeks |
| **DG-5** | Phase 4 | Week 96-124 | Full Game Studio on Avalonia | All 3 platforms runnable, main shell operational, 3D preview works | 1 platform has defects → fix before Phase 5 | >1 platform unusable → re-evaluate cross-platform strategy |
| **DG-6** | Phase 5 | Week 136-158 | Release readiness | All tests pass, benchmarks met, beta feedback addressed, plugins verified | Critical defects found → delay release, fix, re-test | Show-stopper found → patch and re-certify |

### Decision Gate Process

```
┌─────────────────────────────────────────────────────────────┐
│                   DECISION GATE PROCESS                       │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  2 weeks before gate: Collect status reports                 │
│  1 week before gate:   Run verification suite                │
│  Gate day:             Review meeting (Tech Lead + PM)       │
│                        Decision documented in migration log  │
│                                                               │
│  Outcomes:                                                    │
│    GREEN  ──► Proceed to next phase                          │
│    YELLOW ──► Proceed with conditions (documented)           │
│    RED    ──► Pause, re-evaluate, escalate                   │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

### Gate Artifacts Required

| Gate | Required Artifacts |
|------|-------------------|
| DG-0 | Approved migration proposal, team roster, spike schedule |
| DG-1 | All spike reports, decision logs, fallback plans documented |
| DG-2 | CI/CD pipeline operational, interop test results, build matrix docs |
| DG-3 | Side-by-side test harness results, feature parity report, known issues list |
| DG-4 | Editor migration status matrix, interop validation results, screenshot comparison gallery |
| DG-5 | Cross-platform test matrix (Windows/Linux/macOS), platform-specific known issues |
| DG-6 | Full regression test report, performance benchmark comparison, plugin compatibility report, beta feedback summary |

---

## 11. Fallback Plans

| Scenario | Fallback | Cost | Trigger |
|----------|----------|------|---------|
| **Dock.Avalonia feature gap** | Migrate to Actipro Avalonia (commercial) | +4-6 weeks, ~$1k/developer license | S.1 Spike finds ≥3 missing critical features |
| **macOS Metal not viable** | MoltenVK (Vulkan-on-Metal) for editor viewport | +4-8 weeks | S.3 Spike: <30 FPS or MSL pipeline failures |
| **macOS MoltenVK also fails** | Software rendering fallback (WARP-like) for editor viewport only | +8-12 weeks | S.3 Spike: both Metal and MoltenVK fail |
| **WPF↔Avalonia interop too slow** | Skip incremental migration → direct pure Avalonia switch | +8-12 weeks (Phase 1 is still needed) | S.4 Spike: >5ms additional input latency or IME broken |
| **Plugin ecosystem backlash** | Extend WPF support for 2 additional release cycles | Low (maintenance cost) | Community feedback from beta period |
| **Cannot hire Avalonia specialists** | Cross-train existing WPF engineers + contract specialists | Variable | No candidates found by Month 6 |
| **Avalonia 12 breaking changes** | Pin to Avalonia 11.x for migration, upgrade in Phase 5 | +4-8 weeks for version upgrade | S.7 Spike: changes require major refactoring |

---

## 12. Milestone Definitions and Success Criteria

### Milestone 1: Spike Validation Complete
- **Timing**: Year 1, Q1 (Week 12-16)
- **Definition**: All 8 technical spikes completed with documented findings and decisions
- **Success Criteria**:
  - All spike reports published to `docs/spikes/`
  - Decision gates for each spike documented
  - Fallback plans ready for any failed spikes
  - Phase 1 detailed plan finalized
  - Team fully assembled (Tech Lead + Avalonia Specialists + DevOps)

### Milestone 2: Infrastructure Foundation
- **Timing**: Year 1, Q4 (Week 28-36)
- **Definition**: Dual-build system, interop layer, and base controls operational
- **Success Criteria**:
  - `Stride.Core.Presentation.Avalonia` project builds
  - `Stride.Core.Presentation.Interop` project builds
  - Same solution can produce WPF and Avalonia targets (`-p:UseAvalonia=true`)
  - Avalonia panel hosted inside WPF Game Studio (PoC)
  - CI/CD pipeline builds both targets
  - Base control converters ported and tested

### Milestone 3: Core UI Component Parity
- **Timing**: Year 2, Q2 (Week 48-60)
- **Definition**: PropertyGrid, Dock, and all core controls feature-complete
- **Success Criteria**:
  - PropertyGrid feature parity ≥90% (measured by test harness)
  - Dock.Avalonia integration complete with layout persistence
  - All custom controls (EditableListBox, TreeView, ColorPicker, etc.) ported
  - Themes and styles visually match WPF version
  - Regression test suite covers ≥80% of control interactions
  - Side-by-side test harness reports specific gaps (≤10%)

### Milestone 4: Asset Editors Migration Complete
- **Timing**: Year 2, Q4 (Week 72-92)
- **Definition**: All 8 asset editors running in Avalonia via interop
- **Success Criteria**:
  - ScriptEditor with RoslynPad.Avalonia: syntax highlighting, IntelliSense, diagnostics working
  - UIEditorView: WYSIWYG canvas operational with element selection
  - VisualScriptEditorView: node graph with connections and library panel
  - CurveEditorView: animation curves, keyframes, tangents functional
  - MaterialEditorView: shader graph with preview
  - Asset picker dialog: thumbnail gallery, search, filtering
  - Each editor validated in interop host (focus, input, layout)

### Milestone 5: Game Studio Cross-Platform
- **Timing**: Year 3, Q2 (Week 96-124)
- **Definition**: Pure Avalonia Game Studio running on all 3 platforms
- **Success Criteria**:
  - GameEngineHost working on Windows (DirectX), Linux (Vulkan), macOS (Metal)
  - Menu bar, toolbar, status bar functional
  - Startup wizard, settings dialogs, session management working
  - 3D preview rendering ≥30 FPS on each platform
  - Input forwarding latency <16ms (platform-native, not via interop)
  - Full Avalonia Application entry point (`AppBuilder`) operational

### Milestone 6: Release Gate
- **Timing**: Year 3, Q4 (Week 136-158)
- **Definition**: Stride Game Studio with Avalonia UI released
- **Success Criteria**:
  - All regression tests pass: Windows, Linux, macOS
  - Performance benchmarks: Avalonia ≥ WPF in all measured categories
  - Accessibility audit: keyboard navigation, screen reader compatibility
  - Plugin compatibility: top 10 community plugins verified working
  - Community beta: 2-week period with <5 critical bug reports
  - Documentation: migration guide for plugin authors published
  - WPF projects archived/removed from main solution

---

## 13. Forked Repository Maintenance Plan

| Repository | Source | Maintainer Assignment | Update Cadence |
|------------|--------|----------------------|----------------|
| `Stridefork/Dock` | `wieslawsoltes/Dock` | Avalonia Specialist #1 | Quarterly sync with upstream |
| `Stridefork/RoslynPad` | `roslynpad/RoslynPad` | Full-Stack Engineer #1 | Quarterly sync with upstream |

**Maintenance Responsibilities**:
- Track upstream changes and merge relevant fixes
- Maintain Stride-specific customizations
- Document all deviations from upstream
- Security patch responsiveness (<72 hours for critical)

---

## 14. Communication & Reporting

### Regular Cadence
| Artifact | Frequency | Audience | Content |
|----------|-----------|----------|---------|
| **Migration Status Report** | Weekly | Team + Management | Progress %, blockers, decisions needed |
| **Phase Progress Dashboard** | Bi-weekly | All stakeholders | Gantt chart, milestone tracking, risk status |
| **Decision Gate Report** | Per gate | Tech Lead + PM | Gate criteria results, artifacts, decision |
| **Risk Register Update** | Monthly | Tech Lead | Risk status changes, new risks, mitigated risks |
| **Community Update** | Per phase | Discord/GitHub | What's done, what's next, FAQ |

### Dashboard Metrics
- **Progress %**: Completed tasks / total tasks per phase
- **Feature Parity %**: (WPF features matched) / (total WPF features)
- **Platform Coverage**: Tasks green on Windows / Linux / macOS
- **Risk Status**: # red / yellow / green risks
- **Buffer Burn Rate**: Used buffer / allocated buffer per phase

---

## 15. Glossary

| Term | Definition |
|------|-----------|
| **Avalonia 12** | Cross-platform UI framework, target framework for Stride editor migration |
| **Interop Bridge** | WPF↔Avalonia communication layer during incremental migration |
| **Dual-Build System** | MSBuild conditional targets producing both WPF and Avalonia builds |
| **GameEngineHost** | Component hosting Stride's 3D rendering engine inside the editor UI |
| **AvalonDock** | WPF docking library currently used by Stride (to be replaced) |
| **Dock.Avalonia** | Avalonia-native docking library (replacement for AvalonDock) |
| **RoslynPad** | Code editor control wrapping Roslyn (C# scripting, IntelliSense) |
| **Quantum** | Stride's ViewModel layer — shared between WPF and Avalonia |
| **Stridefork** | Organization hosting forked dependencies with Stride-specific patches |
| **MoltenVK** | Vulkan-on-Metal translation layer (macOS fallback) |
| **RPN** | Risk Priority Number (Probability × Impact) |
| **Phase Buffer** | Risk contingency allocated to a specific phase (not shared pool) |
