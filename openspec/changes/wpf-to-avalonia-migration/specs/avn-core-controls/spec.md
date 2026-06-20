## ADDED Requirements

### Requirement: Avalonia PropertyGrid
The system SHALL provide a PropertyGrid control in Avalonia that matches the WPF PropertyGrid functionality and UX.

#### Scenario: PropertyGrid display
- **WHEN** an asset or object is selected
- **THEN** PropertyGrid displays all public properties grouped by category
- **AND** property values are editable inline
- **AND** read-only properties are visually distinct

#### Scenario: Property editor types
- **WHEN** a property is of type string, int, float, bool, Color, Vector3, Quaternion, Enum, or AssetReference
- **THEN** the appropriate editor control is displayed (TextBox, CheckBox, ColorEditor, EnumDropdown, etc.)

#### Scenario: Inline editing
- **WHEN** a property value is clicked
- **THEN** it becomes editable inline (TextBox for text, Slider for floats, etc.)
- **AND** changes are committed on Enter or focus loss
- **AND** changes can be cancelled with Escape

#### Scenario: Search/filter
- **WHEN** typing in the PropertyGrid search box
- **THEN** properties are filtered by name matching the search text
- **AND** categories containing no matching properties are hidden

#### Scenario: Custom property editors
- **WHEN** a property has a custom editor registered
- **THEN** the custom editor is used instead of the default type-based editor
- **AND** registration follows the same API as WPF's IPropertyEditorProvider

### Requirement: Avalonia TreeView
The system SHALL provide a TreeView control matching WPF TreeView functionality.

#### Scenario: TreeView hierarchy
- **WHEN** a tree node is expanded
- **THEN** child nodes are loaded and displayed with proper indentation
- **AND** lazy loading is supported for large trees

#### Scenario: Drag and drop
- **WHEN** a tree node is dragged
- **THEN** drag visual feedback is shown
- **AND** drop targets highlight on hover
- **AND** drag/drop events match WPF semantics

### Requirement: Core controls parity
The system SHALL provide Avalonia equivalents of all custom WPF controls in Stride.Core.Presentation.Wpf.

#### Scenario: EditableListBox
- **WHEN** items are shown in EditableListBox
- **THEN** items can be added, removed, reordered
- **AND** inline renaming works on double-click
- **AND** validation is shown per item

#### Scenario: WatermarkTextBox
- **WHEN** TextBox is empty and unfocused
- **THEN** watermark text is displayed
- **AND** watermark disappears when typing or focused

#### Scenario: ColorPicker
- **WHEN** user clicks the ColorPicker button
- **THEN** a color picker popup is shown
- **AND** supports RGB, HSL, and hexadecimal input
- **AND** includes an eyedropper tool
- **AND** color history is shown

### Requirement: Custom Themes/Styles
The system SHALL provide Avalonia themes/styling that matches the current WPF Stride editor look and feel.

#### Scenario: Theme parity
- **WHEN** the editor is displayed in Avalonia
- **THEN** colors, fonts, spacing, and control sizing match the WPF editor
- **AND** dark theme is supported (matching current Stride dark theme)

#### Scenario: Control templates
- **WHEN** a control is rendered
- **THEN** it uses the custom Stride control template
- **AND** hover, pressed, focused, disabled states match WPF behavior
