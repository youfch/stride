## ADDED Requirements

### Requirement: Avalonia RoslynPad integration
The system SHALL integrate the Avalonia version of RoslynPad for code editing.

#### Scenario: Editor initialization
- **WHEN** a C# script is opened
- **THEN** `RoslynPad.Avalonia.Editor` is initialized with the script content
- **AND** the Roslyn workspace is configured with Stride engine assemblies and NuGet references

#### Scenario: Syntax highlighting
- **WHEN** C# code is displayed
- **THEN** syntax highlighting matches the WPF RoslynPad colors and font styling
- **AND** semantic highlighting (symbol types) is supported

#### Scenario: IntelliSense
- **WHEN** typing in the editor
- **THEN** completion suggestions are shown
- **AND** parameter hints are displayed for method calls
- **AND** quick info tooltips appear on hover
- **AND** signature help shows overloads

#### Scenario: Diagnostics
- **WHEN** code contains errors or warnings
- **THEN** error squiggles are shown inline
- **AND** the error list panel displays all diagnostics
- **AND** code fixes and refactoring suggestions are available

#### Scenario: Code completion commit
- **WHEN** a completion is selected
- **THEN** the commit behavior matches WPF RoslynPad (Enter to commit, Tab to commit with indent)
- **AND** completion filters intelligently based on context

### Requirement: Code lens and inline hints
The system SHALL provide inline code information for scripts.

#### Scenario: Code lens
- **WHEN** a method or class is displayed
- **THEN** reference counts are shown above the declaration
- **AND** clicking reference count shows the referencing locations

### Requirement: File tree navigation
The system SHALL provide a file tree panel for script navigation.

#### Scenario: File tree
- **WHEN** the script editor is open
- **THEN** a file tree shows the project's script files
- **AND** files can be opened by clicking in the tree
- **AND** file operations (rename, delete, add) are available via context menu
