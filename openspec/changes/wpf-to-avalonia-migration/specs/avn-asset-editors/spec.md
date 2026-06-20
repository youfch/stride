## ADDED Requirements

### Requirement: ScriptEditorView (RoslynPad)
The system SHALL provide a code editor view for C# scripts using RoslynPad.Avalonia.

#### Scenario: Code editing
- **WHEN** a script file is opened
- **THEN** the code is displayed with syntax highlighting
- **AND** IntelliSense provides completions, parameter hints, and quick info
- **AND** diagnostics (errors/warnings) are shown inline

#### Scenario: File management
- **WHEN** scripts are organized in a project
- **THEN** a file tree shows the script file hierarchy
- **AND** files can be created, renamed, deleted in the tree
- **AND** changes are tracked with unsaved indicator

### Requirement: UIEditorView
The system SHALL provide a UI editor view for designing game UI layouts.

#### Scenario: Canvas editing
- **WHEN** editing a UI asset
- **THEN** a WYSIWYG canvas is displayed
- **AND** UI elements can be added, selected, moved, and resized
- **AND** snap-to-grid and alignment guides are shown

#### Scenario: Property binding
- **WHEN** a UI element property is modified
- **THEN** the change is reflected in the canvas immediately
- **AND** data bindings to game properties are displayed

### Requirement: VisualScriptEditorView
The system SHALL provide a visual node-based editor for shaders and scripts.

#### Scenario: Node graph editing
- **WHEN** editing a visual script
- **THEN** a node graph canvas is displayed
- **AND** nodes can be added from a library panel
- **AND** nodes can be connected by dragging between ports
- **AND** the graph can be panned and zoomed

#### Scenario: Node execution
- **WHEN** the graph is compiled
- **THEN** errors and warnings are shown on the affected nodes
- **AND** execution order is displayed visually

### Requirement: CurveEditorView
The system SHALL provide a curve editor for animation and property curves.

#### Scenario: Curve manipulation
- **WHEN** editing animation curves
- **THEN** keyframes are displayed and draggable on the curve
- **AND** tangent handles allow curve shape adjustment
- **AND** the curve view supports zoom and pan

#### Scenario: Multiple curves
- **WHEN** multiple properties are animated
- **THEN** each curve is shown with a distinct color
- **AND** curves can be individually selected and edited
- **AND** a legend identifies each curve

### Requirement: MaterialEditorView
The system SHALL provide a material editor for shader graph composition.

#### Scenario: Material graph
- **WHEN** editing a material
- **THEN** a shader node graph is displayed
- **AND** material properties are exposed in a property panel
- **AND** the preview updates in real-time

#### Scenario: Preview rendering
- **WHEN** the material is modified
- **THEN** the 3D preview re-renders with the updated material
- **AND** preview quality matches the WPF material editor
