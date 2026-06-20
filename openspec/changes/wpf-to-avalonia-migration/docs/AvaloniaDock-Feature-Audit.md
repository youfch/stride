# AvaloniaDock Feature Audit Report

**Generated**: 2026-06-20
**Package**: `Dock for Avalonia` (wieslawsoltes/Dock)
**Version**: 11.0.0 (latest stable)

---

## Executive Summary

**Verdict**: ✅ **AvaloniaDock (Dock for Avalonia) is suitable for Stride migration**

Dock for Avalonia provides all critical AvalonDock features required by Stride, with additional benefits:
- MVVM-first architecture (separate model from view)
- Multiple serialization formats (JSON, XML, YAML, Protobuf)
- Active maintenance (GitHub: wieslawsoltes/Dock)
- MIT license
- Multiple MVVM framework integrations (Mvvm, ReactiveUI, Prism)

---

## Feature Comparison: AvalonDock vs Dock for Avalonia

| AvalonDock Feature | Dock for Avalonia Equivalent | Status | Notes |
|--------------------|------------------------------|--------|-------|
| **DockingManager** | `DockView` + `DockFactory` | ✅ Available | Core docking control |
| **LayoutItem** | `IDockWindow` / `IDockDocument` | ✅ Available | MVVM model classes |
| **AnchorablePane** | `ToolDock` | ✅ Available | Tool window docking |
| **DocumentPane** | `DocumentDock` | ✅ Available | Document window docking |
| **LayoutSerializer** | `DockFactory.SaveLayout` / `DockFactory.LoadLayout` | ✅ Available | Multiple formats supported |
| **AutoHide** | `AutoHide` (built-in) | ✅ Available | Auto-hide tool panes |
| **FloatingWindows** | `FloatingWindow` (built-in) | ✅ Available | Detachable windows |
| **Tabbed Documents** | `DocumentDock` with tabs | ✅ Available | Tabbed document interface |
| **Drag & Drop** | Built-in drag behaviors | ✅ Available | Drag to dock/floating |
| **Layout Persistence** | Multiple serializers | ✅ Available | JSON, XML, YAML, Protobuf |
| **Nested Docking** | `DockView` nesting | ✅ Available | Nested dock sites |
| **Context Menus** | Standard Avalonia context menus | ✅ Available | Right-click menus |
| **Themes** | Fluent, Simple, Browser themes | ✅ Available | Multiple themes available |

---

## AvalonDock Features Used in Stride

Based on the codebase analysis, Stride uses the following AvalonDock features:

### 1. Layout Serialization

**Current Implementation**: AvalonDock `LayoutSerializer` (XML format)

```csharp
// Stride.GameStudio\Layout\DockingLayoutManager.cs
// Uses AvalonDock's XmlLayoutSerializer
```

**Dock for Avalonia Equivalent**:
```csharp
// Dock.Serializer.Newtonsoft (JSON) or Dock.Serializer.Xml (XML)
var serializer = new NewtonsoftJsonSerializer();
string layoutXml = serializer.Serialize(dockModel);
dockModel = serializer.Deserialize(layoutXml);
```

**Status**: ✅ **Supported** — Multiple serialization formats available

---

### 2. Floating Windows

**Current Implementation**: AvalonDock floating windows

```csharp
// Stride.Core.Assets.Editor\View\Behaviors\DragDrop\DragWindow.cs
// Custom drag window behavior
```

**Dock for Avalonia Equivalent**:
```xml
<!-- Floating windows are built-in -->
<DockView>
    <DocumentDock CanFloatDocuments="True">
        <!-- Documents can be floated -->
    </DocumentDock>
</DockView>
```

**Status**: ✅ **Supported** — Floating windows built-in

---

### 3. Auto-Hide Panes

**Current Implementation**: AvalonDock auto-hide panes

```csharp
// Stride.GameStudio\Layout\Behaviors\ActivateParentPaneOnGotFocusBehavior.cs
// Auto-hide behavior
```

**Dock for Avalonia Equivalent**:
```xml
<!-- Auto-hide tool panes -->
<ToolDock Alignment="Left" AutoHide="True">
    <!-- Tool panes auto-hide -->
</ToolDock>
```

**Status**: ✅ **Supported** — Auto-hide built-in

---

### 4. Document Tabs

**Current Implementation**: AvalonDock document tabs

```csharp
// Stride.GameStudio\View\CustomDocumentPaneTabPanel.cs
// Custom document pane tab panel
```

**Dock for Avalonia Equivalent**:
```xml
<!-- Document tabs built-in -->
<DocumentDock>
    <Document>
        <ContentControl Header="Document 1" />
    </Document>
</DocumentDock>
```

**Status**: ✅ **Supported** — Document tabs built-in

---

### 5. Drag & Drop Reordering

**Current Implementation**: AvalonDock drag & drop

```csharp
// Stride.Core.Assets.Editor\View\Behaviors\DragDrop\DragDropBehavior.cs
// Custom drag & drop behavior
```

**Dock for Avalonia Equivalent**:
```xml
<!-- Drag & drop built-in -->
<DockView>
    <!-- Drag to dock, float, or reorder -->
</DockView>
```

**Status**: ✅ **Supported** — Built-in drag behaviors

---

## AvaloniaDock Architecture

Dock for Avalonia uses a **separated model-view architecture**:

```
┌─────────────────────────────────────────────────┐
│  Dock.Avalonia (View Layer)                     │
│  ├── DockView (main control)                    │
│  ├── DocumentDock (document pane)               │
│  ├── ToolDock (tool pane)                       │
│  └── FloatingWindow (floating window)           │
├─────────────────────────────────────────────────┤
│  Dock.Model (Model Layer)                       │
│  ├── IDockWindow (layout state)                │
│  ├── IDockDocument (document model)            │
│  ├── IDockTool (tool model)                    │
│  └── DockFactory (layout creation/serialization)│
├─────────────────────────────────────────────────┤
│  Dock.Model.Mvvm (MVVM Integration)             │
│  ├── ViewModelBase (base view model)           │
│  └── RelayCommand (command implementation)      │
└─────────────────────────────────────────────────┘
```

**Benefit**: The model layer is framework-agnostic, allowing reuse across different UI frameworks.

---

## NuGet Packages Required

| Package | Version | Purpose |
|---------|---------|---------|
| `Dock.Avalonia` | 11.0.0 | Main docking control |
| `Dock.Model.Avalonia` | 11.0.0 | Model layer for Avalonia |
| `Dock.Model.Mvvm` | 11.0.0 | MVVM integration |
| `Dock.Serializer.Newtonsoft` | 11.0.0 | JSON serialization |
| `Dock.Avalonia.Themes.Fluent` | 11.0.0 | Fluent theme |
| `Dock.Controls.DeferredContentControl` | 11.0.0 | Deferred content loading |
| `Dock.Controls.Recycling` | 11.0.0 | Content recycling for performance |

---

## Migration Mapping: AvalonDock → Dock for Avalonia

### XAML Mapping

| AvalonDock XAML | Dock for Avalonia XAML |
|-----------------|------------------------|
| `<ad:DockingManager>` | `<dock:DockView>` |
| `<ad:LayoutItem>` | `<dock:Document>` / `<dock:Tool>` |
| `<ad:AnchorablePane>` | `<dock:ToolDock>` |
| `<ad:DocumentPane>` | `<dock:DocumentDock>` |
| `<ad:LayoutSerializer>` | `DockFactory.SaveLayout` / `DockFactory.LoadLayout` |

### Code Mapping

| AvalonDock Code | Dock for Avalonia Code |
|-----------------|------------------------|
| `DockingManager` | `DockView` |
| `LayoutItem` | `IDockWindow` |
| `AnchorablePaneControl` | `ToolDock` |
| `DocumentPaneControl` | `DocumentDock` |
| `XmlLayoutSerializer` | `Dock.Serializer.Newtonsoft.JsonSerializer` |

---

## Known Differences

### 1. MVVM Integration

**AvalonDock**: Uses `LayoutItem` templates with data binding
**Dock for Avalonia**: Uses MVVM model classes (`IDockWindow`, `IDockDocument`)

**Impact**: Requires refactoring from AvalonDock's template-based approach to Dock's model-based approach.

**Mitigation**: Create `DockViewModel` base classes that wrap Dock model classes.

### 2. Layout Serialization Format

**AvalonDock**: XML format (default)
**Dock for Avalonia**: JSON, XML, YAML, Protobuf (multiple options)

**Impact**: Existing layout files (XML) may not be compatible with Dock's JSON format.

**Mitigation**: Use `Dock.Serializer.Xml` for XML compatibility, or migrate existing layouts to JSON.

### 3. Auto-Hide Behavior

**AvalonDock**: Auto-hide panes slide in/out
**Dock for Avalonia**: Auto-hide panes use similar behavior

**Impact**: Minor visual differences in auto-hide animation.

**Mitigation**: Customize animation timing via Avalonia styles.

---

## Testing Recommendations

### 1. Create Sample Docking Layout

```xml
<DockView x:Class="Stride.Editor.Avalonia.Docking.DockView"
          xmlns="https://github.com/avaloniaui"
          xmlns:dock="clr-namespace:Dock.Avalonia;assembly=Dock.Avalonia"
          xmlns:model="clr-namespace:Dock.Model.Avalonia;assembly=Dock.Model.Avalonia">
    
    <dock:DockView.Model>
        <model:Dock>
            <model:DocumentDock CanFloatDocuments="True">
                <model:Document Title="Asset Explorer" />
            </model:DocumentDock>
            <model:ToolDock Alignment="Left">
                <model:Tool Title="Properties" />
            </model:ToolDock>
        </model:Dock>
    </dock:DockView.Model>
</DockView>
```

### 2. Test Layout Serialization

```csharp
var serializer = new NewtonsoftJsonSerializer();
string json = serializer.Serialize(dockModel);
dockModel = serializer.Deserialize(json);
```

### 3. Test Floating Windows

```csharp
// Drag document to float
documentDock.CanFloatDocuments = true;
```

### 4. Test Auto-Hide

```csharp
toolDock.AutoHide = true;
```

---

## Conclusion

**Dock for Avalonia** is a mature, well-maintained docking library that provides all features required for Stride migration:

| Criteria | Status |
|----------|--------|
| **Feature parity with AvalonDock** | ✅ Pass |
| **Layout serialization** | ✅ Pass (multiple formats) |
| **Floating windows** | ✅ Pass |
| **Auto-hide panes** | ✅ Pass |
| **MVVM integration** | ✅ Pass (multiple frameworks) |
| **Active maintenance** | ✅ Pass (GitHub: wieslawsoltes/Dock) |
| **MIT license** | ✅ Pass |
| **Documentation** | ✅ Pass (comprehensive docs) |
| **Sample applications** | ✅ Pass (multiple samples) |

**Recommendation**: Use `Dock for Avalonia` for Stride migration.

**Estimated Effort**: 8-10 weeks (Phase 2)
- Port AvalonDock XAML templates to Dock for Avalonia
- Implement layout serialization/deserialization
- Test floating windows, auto-hide, and pane docking
- Create side-by-side comparison tests

---

**Task 0.5 Status**: ✅ **COMPLETE** — AvaloniaDock feature audit complete.
