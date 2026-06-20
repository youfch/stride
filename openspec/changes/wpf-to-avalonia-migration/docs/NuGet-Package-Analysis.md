# Third-Party NuGet Package Analysis

**Generated**: 2026-06-20
**Scope**: All editor and presentation projects

## Summary

| Category | Packages | Migration Impact |
|----------|----------|------------------|
| **WPF-Specific (Must Replace)** | 6 | 🔴 Critical |
| **Cross-Platform (Compatible)** | 9 | ✅ No change needed |
| **Windows-Only (Needs Investigation)** | 1 | 🟡 Medium |
| **Game Engine Dependencies** | 4 | 🟡 Medium (engine-level) |

---

## 🔴 WPF-Specific Packages (Must Replace)

### 1. Dirkster.AvalonDock

| Property | Value |
|----------|-------|
| **Package** | `Dirkster.AvalonDock` |
| **Version** | (check NuGet for latest) |
| **Project** | `Stride.Core.Assets.Editor` |
| **Purpose** | WPF docking layout (multi-pane, floating windows, auto-hide) |
| **Impact** | 🔴 Critical — Core UI component |

**Avalonia Equivalent**: `AvaloniaDock` + `Dock.Model`

| AvalonDock Feature | AvaloniaDock Equivalent |
|--------------------|------------------------|
| `DockingManager` | `DockView` / `DockFactory` |
| `LayoutItem` | `IDockWindow` / `IDockDocument` |
| `AnchorablePane` | `ToolDock` |
| `DocumentPane` | `DocumentDock` |
| `LayoutSerializer` | `DockFactory.SaveLayout` / `DockFactory.LoadLayout` |
| `AutoHide` | `AutoHide` (built-in) |
| `FloatingWindows` | `FloatingWindow` (built-in) |

**Migration Effort**: 8-10 weeks (Phase 2)
- Port all AvalonDock XAML templates to AvaloniaDock
- Implement layout serialization/deserialization
- Test floating windows, auto-hide, and pane docking behavior
- Create side-by-side comparison tests

**Recommendation**: Use [Dock for Avalonia](https://github.com/wieslawsoltes/Dock) (MIT license, active maintenance).

**✅ 2026-06-20 验证结果**:
- 上游仓库已同步最新代码（Avalonia 12.0.0）
- 构建验证通过：`dotnet build` 0 错误 0 警告
- 本地 NuGet 输出：`local-nuget/` 目录
- **无需 fork 定制**，直接使用上游 NuGet 包

**NuGet 包列表**:
| 包 | 版本 | 本地路径 |
|----|------|----------|
| `Dock.Avalonia` | 12.0.0 | `local-nuget/Dock.Avalonia.dll` |
| `Dock.Model` | 12.0.0 | `local-nuget/Dock.Model.dll` |
| `Dock.Controls.Recycling` | 12.0.0 | `local-nuget/Dock.Controls.Recycling.dll` |
| `Dock.Controls.DeferredContentControl` | 12.0.0 | `local-nuget/Dock.Controls.DeferredContentControl.dll` |
| `Dock.Controls.ProportionalStackPanel` | 12.0.0 | `local-nuget/Dock.Controls.ProportionalStackPanel.dll` |
| `Dock.MarkupExtension` | 12.0.0 | `local-nuget/Dock.MarkupExtension.dll` |
| `Dock.Settings` | 12.0.0 | `local-nuget/Dock.Settings.dll` |

---

### 2. Microsoft.Xaml.Behaviors.Wpf

| Property | Value |
|----------|-------|
| **Package** | `Microsoft.Xaml.Behaviors.Wpf` |
| **Version** | (check NuGet for latest) |
| **Project** | `Stride.Core.Presentation.Wpf` |
| **Purpose** | WPF attached behaviors (Trigger, EventTrigger, InvokeCommandAction, etc.) |
| **Impact** | 🔴 High |

**Avalonia Equivalent**: `Avalonia.Interactivity` (built-in)

| WPF Behavior | Avalonia Equivalent |
|--------------|---------------------|
| `Interaction` | `Avalonia.Interactivity.Interaction` (built-in) |
| `Behavior<T>` | `Avalonia.Interactivity.Behavior<T>` (built-in) |
| `Trigger` | `Trigger` (built-in) |
| `EventTrigger` | `EventTrigger` (built-in) |
| `InvokeCommandAction` | `InvokeCommandAction` (built-in) |
| `DataTrigger` | `Trigger` with `When` condition |

**Migration Effort**: 2-3 weeks
- Most behaviors are built into Avalonia
- Custom behaviors need to be ported to `Avalonia.Interactivity.Behavior<T>`
- XAML syntax differs slightly (use `Avalonia.Markup.Xaml` namespace)

**Recommendation**: Use Avalonia's built-in `Avalonia.Interactivity` namespace. No external package needed.

---

### 3. Microsoft-WindowsAPICodePack-Shell

| Property | Value |
|----------|-------|
| **Package** | `Microsoft-WindowsAPICodePack-Shell` |
| **Version** | (check NuGet for latest) |
| **Project** | `Stride.Core.Presentation.Dialogs` |
| **Purpose** | Windows Common Dialog API (OpenFileDialog, SaveFileDialog with custom options) |
| **Impact** | 🟡 Medium |

**Avalonia Equivalent**: `Avalonia.Platform` (built-in)

| Windows API Feature | Avalonia Equivalent |
|---------------------|---------------------|
| `CommonOpenFileDialog` | `Avalonia.Platform.FilePicker` |
| `FileDialog` | `Avalonia.Platform.FilePicker` |
| `FileDialogFilter` | `FilePickerFileType` |
| `FileDialogResult` | `FilePickerResult` |

**Migration Effort**: 1-2 weeks
- Avalonia's `FilePicker` provides similar functionality
- Some advanced Windows-specific features may need custom implementation
- Test file dialogs on Windows, Linux, and macOS

**Recommendation**: Use Avalonia's built-in `Avalonia.Platform.FilePicker`. For Windows-specific advanced features, consider `Microsoft-WindowsAPICodePack-Shell` via interop (temporary) or implement custom wrapper.

---

### 4. Stride.GraphX.WPF.Controls

| Property | Value |
|----------|-------|
| **Package** | `Stride.GraphX.WPF.Controls` |
| **Version** | (check NuGet for latest) |
| **Project** | `Stride.Core.Presentation.Graph` |
| **Purpose** | WPF graph/node diagram controls |
| **Impact** | 🔴 High |

**Avalonia Equivalent**: Custom implementation or community package

| WPF Feature | Avalonia Equivalent |
|-------------|---------------------|
| `GraphCanvas` | Custom `Canvas` + `Control` |
| `GraphNode` | Custom `Control` |
| `GraphEdge` | Custom `Control` (Path/Line) |
| `GraphLayout` | Custom layout algorithm |

**Migration Effort**: 4-6 weeks
- No direct Avalonia equivalent exists
- Must implement custom graph controls
- Consider using Avalonia's `Canvas` + custom `Control` classes
- Or use community graph library (if available)

**Recommendation**: Implement custom graph controls using Avalonia's `Canvas` and `Control` classes. Alternatively, research Avalonia graph libraries (e.g., `Avalonia.Graph`, `GraphX.Avalonia` if available).

---

### 5. RoslynPad.Editor.Windows

| Property | Value |
|----------|-------|
| **Package** | `RoslynPad.Editor.Windows` |
| **Version** | (check NuGet for latest) |
| **Project** | `Stride.Assets.Presentation` |
| **Purpose** | WPF code editor with syntax highlighting, IntelliSense, diagnostics |
| **Impact** | 🔴 High |

**Avalonia Equivalent**: `RoslynPad.Avalonia.Editor`

| WPF Feature | Avalonia Feature |
|-------------|------------------|
| `RoslynCodeEditor` | `RoslynPad.Avalonia.Editor` |
| Syntax highlighting | Same (via Roslyn) |
| IntelliSense | Same (via Roslyn) |
| Diagnostics | Same (via Roslyn) |
| Code completion | Same (via Roslyn) |
| Code lens | Same (via Roslyn) |

**Migration Effort**: 2-3 weeks
- RoslynPad.Avalonia provides same API surface
- Minimal ViewModel changes needed
- Test syntax highlighting, IntelliSense, and diagnostics

**Recommendation**: Use [RoslynPad.Avalonia](https://github.com/aelij/RoslynPad) NuGet package.

**✅ 2026-06-20 验证结果**:
- 上游仓库已同步最新代码（Avalonia 12.0.4）
- 构建验证通过：`dotnet build` 0 错误 0 警告
- 本地 NuGet 输出：`local-nuget/` 目录
- **无需 fork 定制**，直接使用上游 NuGet 包

**NuGet 包列表**:
| 包 | 版本 | 本地路径 |
|----|------|----------|
| `RoslynPad.Editor.Avalonia` | 12.0.4 | `local-nuget/RoslynPad.Editor.Avalonia.dll` |
| `RoslynPad.Roslyn.Avalonia` | 12.0.4 | `local-nuget/RoslynPad.Roslyn.Avalonia.dll` |
| `RoslynPad.Roslyn` | 12.0.4 | `local-nuget/RoslynPad.Roslyn.dll` |
| `RoslynPad.Themes` | 12.0.4 | `local-nuget/RoslynPad.Themes.dll` |

---

### 6. RoslynPad.Roslyn.Windows / RoslynPad.Roslyn

| Property | Value |
|----------|-------|
| **Package** | `RoslynPad.Roslyn.Windows`, `RoslynPad.Roslyn` |
| **Version** | (check NuGet for latest) |
| **Project** | `Stride.Assets.Presentation` |
| **Purpose** | Roslyn API for code analysis (WPF-specific) |
| **Impact** | 🔴 High |

**Avalonia Equivalent**: `RoslynPad.Roslyn.Avalonia` (or cross-platform Roslyn)

| WPF Feature | Avalonia Feature |
|-------------|------------------|
| Roslyn workspace | Same (cross-platform Roslyn) |
| Syntax analysis | Same (cross-platform Roslyn) |
| Semantic analysis | Same (cross-platform Roslyn) |

**Migration Effort**: 1-2 weeks
- Roslyn is cross-platform (.NET Standard)
- Use `Microsoft.CodeAnalysis` packages directly
- No WPF-specific dependencies

**Recommendation**: Use cross-platform `Microsoft.CodeAnalysis` packages. RoslynPad.Avalonia wraps these for Avalonia.

---

### 7. AvalonEdit

| Property | Value |
|----------|-------|
| **Package** | `AvalonEdit` |
| **Version** | (check NuGet for latest) |
| **Project** | `Stride.Assets.Presentation` |
| **Purpose** | WPF text editor with syntax highlighting |
| **Impact** | 🟡 Medium |

**Avalonia Equivalent**: `AvaloniaEdit`

| WPF Feature | Avalonia Feature |
|-------------|------------------|
| `TextEditor` | `AvaloniaEdit.TextEditor` |
| Syntax highlighting | Same (via AvalonEdit port) |
| Folding | `AvaloniaEdit.Folding` |
| Word wrap | Same |

**Migration Effort**: 1-2 weeks
- AvaloniaEdit is a port of AvalonEdit for Avalonia
- API is similar but not identical
- Test syntax highlighting, folding, and word wrap

**Recommendation**: Use [AvaloniaEdit](https://github.com/AvaloniaCommunity/AvaloniaEdit) NuGet package.

---

## ✅ Cross-Platform Packages (Compatible)

These packages work with both WPF and Avalonia. No changes needed.

| Package | Purpose | Notes |
|---------|---------|-------|
| `System.Reactive` | Reactive Extensions | Cross-platform, works with Avalonia |
| `System.Reactive.Linq` | LINQ to Rx | Cross-platform, works with Avalonia |
| `Microsoft.CodeAnalysis.CSharp.Features` | Roslyn features | Cross-platform, works with Avalonia |
| `Microsoft.CodeAnalysis.CSharp.Scripting` | Roslyn scripting | Cross-platform, works with Avalonia |
| `ServiceWire` | IPC | Cross-platform, works with Avalonia |
| `SSH.NET` | SSH client | Cross-platform, works with Avalonia |
| `DotRecast.Detour` | Navigation mesh | Cross-platform, works with Avalonia |
| `Mono.Cecil` | Assembly manipulation | Cross-platform, works with Avalonia |
| `SharpDX` | DirectX | ⚠️ Windows-only (see below) |

---

## 🟡 Windows-Only Packages (Needs Investigation)

### SharpDX

| Property | Value |
|----------|-------|
| **Package** | `SharpDX` |
| **Project** | `Stride.GameStudio` |
| **Purpose** | DirectX interop for rendering |
| **Impact** | 🟡 Medium |

**Cross-Platform Status**: SharpDX is Windows-only (DirectX).

**Migration Options**:
1. **Keep SharpDX for Windows** — Use conditional compilation (`#if AVALONIA` vs `#if WPF`)
2. **Replace with Avalonia's rendering** — Avalonia uses DirectX on Windows, Vulkan on Linux, Metal on macOS
3. **Use Stride's rendering backend** — Stride already has cross-platform rendering abstraction

**Recommendation**: Use Stride's existing rendering abstraction. SharpDX is used for editor-specific rendering (thumbnails, preview). Port to Avalonia's `NativeControlHost` + platform-specific rendering.

---

## Game Engine Dependencies (Stride Internal)

These are Stride engine packages, not NuGet packages. They will be migrated along with the editor.

| Package | Purpose | Migration |
|---------|---------|-----------|
| `Stride.Core` | Core engine | Reuse as-is (cross-platform) |
| `Stride.Core.Quantum` | ViewModel sync | Reuse as-is (cross-platform) |
| `Stride.Core.Presentation` | Shared ViewModel base | Reuse as-is (cross-platform) |
| `Stride.Core.Assets` | Asset management | Reuse as-is (cross-platform) |
| `Stride.Graphics` | Graphics API | Reuse as-is (cross-platform) |
| `Stride.Engine` | Game engine | Reuse as-is (cross-platform) |

---

## Migration Priority Matrix

| Package | Priority | Phase | Effort |
|---------|----------|-------|--------|
| `Dirkster.AvalonDock` | 🔴 Critical | Phase 2 | 8-10 weeks |
| `RoslynPad.Editor.Windows` | 🔴 Critical | Phase 3 | 2-3 weeks |
| `Stride.GraphX.WPF.Controls` | 🔴 High | Phase 2 | 4-6 weeks |
| `Microsoft.Xaml.Behaviors.Wpf` | 🔴 High | Phase 1 | 2-3 weeks |
| `AvalonEdit` | 🟡 Medium | Phase 3 | 1-2 weeks |
| `Microsoft-WindowsAPICodePack-Shell` | 🟡 Medium | Phase 2 | 1-2 weeks |
| `RoslynPad.Roslyn.Windows` | 🟡 Medium | Phase 3 | 1-2 weeks |
| `SharpDX` | 🟡 Medium | Phase 4 | 2-4 weeks |

---

## Recommended Package Versions

| Package | Current (WPF) | Avalonia Equivalent | Version |
|---------|---------------|---------------------|---------|
| AvalonDock | `Dirkster.AvalonDock` | `AvaloniaDock` | 4.0.0+ |
| Behaviors | `Microsoft.Xaml.Behaviors.Wpf` | `Avalonia.Interactivity` | Built-in |
| RoslynPad | `RoslynPad.Editor.Windows` | `RoslynPad.Avalonia.Editor` | Latest |
| AvalonEdit | `AvalonEdit` | `AvaloniaEdit` | 1.0.0+ |
| FileDialog | `Microsoft-WindowsAPICodePack-Shell` | `Avalonia.Platform.FilePicker` | Built-in |
| GraphX | `Stride.GraphX.WPF.Controls` | Custom implementation | TBD |

---

## Open Questions

1. **AvaloniaDock maturity**: Does AvaloniaDock support all AvalonDock features (layout serialization, floating windows, auto-hide, MDI tabs)?
2. **GraphX replacement**: Is there an Avalonia graph library, or must we implement custom?
3. **RoslynPad.Avalonia API**: Does RoslynPad.Avalonia support all RoslynPad features (code lens, inline diagnostics, completion commit behaviors)?
4. **SharpDX usage**: How extensively is SharpDX used? Can we replace with Stride's rendering abstraction?
5. **AvaloniaEdit compatibility**: Does AvaloniaEdit support all AvalonEdit features used (folding, syntax highlighting, word wrap)?

---

**Next Task**: 0.4 — Set up Avalonia 12 spike project to validate cross-platform build
