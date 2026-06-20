## ADDED Requirements

### Requirement: Main window shell
The system SHALL provide the main Game Studio window with menu bar, toolbar, and status bar.

#### Scenario: Window structure
- **WHEN** Game Studio starts
- **THEN** the main window is displayed with menu bar at top, toolbar below, status bar at bottom
- **AND** the docking layout fills the remaining space
- **AND** window title shows the current project name

#### Scenario: Menu bar
- **WHEN** user clicks any menu
- **THEN** the dropdown matches WPF menu structure (File, Edit, View, Assets, Scene, Tools, Help)
- **AND** keyboard shortcuts are displayed and functional
- **AND** disabled menu items are greyed out

#### Scenario: Toolbar
- **WHEN** toolbar buttons are clicked
- **THEN** they perform the same actions as WPF toolbar
- **AND** toolbar states (enabled/disabled, checked) reflect current context

#### Scenario: Status bar
- **WHEN** a long operation runs
- **THEN** the status bar shows progress
- **AND** status messages and warnings are displayed
- **AND** clicking status items opens relevant panels

### Requirement: Session management
The system SHALL support creating, opening, and saving projects/sessions.

#### Scenario: New project wizard
- **WHEN** "New Project" is selected
- **THEN** a wizard dialog walks through project creation (name, template, location)
- **AND** project templates match existing WPF options

#### Scenario: Recent projects
- **WHEN** "Open Recent" is clicked
- **THEN** a list of recent projects is shown
- **AND** clicking a project opens it directly
- **AND** projects can be pinned to the list

### Requirement: Settings dialogs
The system SHALL provide settings/preferences dialogs matching WPF functionality.

#### Scenario: Editor settings
- **WHEN** "Settings" is opened from the menu
- **THEN** a settings dialog is shown with categories (General, Editor, Source Control, etc.)
- **AND** settings are organized in a tree view
- **AND** changes are applied immediately or require restart as appropriate

#### Scenario: Project settings
- **WHEN** "Project Settings" is opened
- **THEN** project-specific settings are shown (rendering, physics, audio, etc.)
- **AND** settings are organized by category
- **AND** validation is shown for invalid values
