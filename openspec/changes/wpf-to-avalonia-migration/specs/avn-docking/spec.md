## ADDED Requirements

### Requirement: Multi-pane docking layout
The system SHALL provide a docking system supporting multi-pane layouts identical to AvalonDock behavior.

#### Scenario: Tool pane docking
- **WHEN** a tool pane is dragged
- **THEN** drag preview shows valid drop zones (left, right, top, bottom, center tab, floating)
- **AND** the pane snaps to the selected drop zone
- **AND** the layout animates smoothly during docking

#### Scenario: Document pane tabbing
- **WHEN** multiple documents are open
- **THEN** they appear as tabs in the document area
- **AND** tabs can be reordered by dragging
- **AND** tabs can be dragged out to float or dock to other positions

#### Scenario: Floating windows
- **WHEN** a pane is undocked
- **THEN** it becomes a floating window
- **AND** can be re-docked into the main layout
- **AND** floating windows are independent of the main window

#### Scenario: Auto-hide
- **WHEN** auto-hide is enabled for a tool pane
- **THEN** the pane collapses to a tab on the edge of the window
- **AND** expands on hover or click
- **AND** auto-hide behavior matches AvalonDock

### Requirement: Layout persistence
The system SHALL save and restore docking layouts across sessions.

#### Scenario: Save layout
- **WHEN** the editor is closed
- **THEN** the current docking layout (pane positions, sizes, open documents) is serialized to disk

#### Scenario: Restore layout
- **WHEN** the editor is opened
- **THEN** the previous docking layout is restored
- **AND** panes that reference missing assets show an appropriate placeholder

#### Scenario: Reset layout
- **WHEN** user selects "Reset Layout"
- **THEN** the docking layout returns to factory defaults
- **AND** custom layouts are preserved as a backup

### Requirement: Context-sensitive tool visibility
The system SHALL show/hide tool panes based on the current editor context.

#### Scenario: Asset editor pane
- **WHEN** an asset is opened
- **THEN** relevant tool panes (PropertyGrid, Asset Preview, Scene Hierarchy) are shown
- **AND** irrelevant panes are hidden or minimized
- **AND** visibility rules match the current WPF behavior

#### Scenario: Tool pane registry
- **WHEN** a new tool pane is registered
- **THEN** it appears in the View menu
- **AND** its visibility can be toggled
- **AND** its default position is configurable
