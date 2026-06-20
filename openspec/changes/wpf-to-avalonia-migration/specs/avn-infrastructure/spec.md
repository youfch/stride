## ADDED Requirements

### Requirement: Avalonia project scaffolding
The system SHALL provide an Avalonia project template and build configuration for all editor projects.

#### Scenario: Create new Avalonia project
- **WHEN** developer runs `dotnet new stride-avalonia-editor`
- **THEN** a new Avalonia-based editor project is created with correct SDK references

#### Scenario: Dual-build WPF/Avalonia
- **WHEN** building with `-p:UseAvalonia=true`
- **THEN** the project uses Avalonia SDK and dependencies
- **AND** WPF-specific code is excluded via conditional compilation

#### Scenario: Cross-platform target
- **WHEN** building Avalonia target on Windows
- **THEN** the output runs on Windows, Linux, and macOS via Avalonia.Desktop

### Requirement: Shared service layer
The system SHALL provide platform-agnostic base services for the editor (DI container, dispatcher, logging, clipboard, file dialogs).

#### Scenario: IDispatcherService abstraction
- **WHEN** code dispatches to UI thread
- **THEN** the dispatcher works identically on Avalonia and WPF
- **AND** the underlying implementation maps to Avalonia Dispatcher

#### Scenario: File dialog mapping
- **WHEN** opening a file dialog
- **THEN** WPF `OpenFileDialog` → Avalonia `OpenFileDialog`
- **AND** filter patterns, multi-select, and default directory all behave identically

#### Scenario: Clipboard access
- **WHEN** setting clipboard content
- **THEN** text and images are available on the OS clipboard
- **AND** works on Windows, Linux, and macOS

### Requirement: Conditional MSBuild SDK
The build system SHALL support conditional framework targeting for WPF vs Avalonia targets.

#### Scenario: SDK selection
- **WHEN** `UseAvalonia=true` is not set
- **THEN** the project builds as WPF (net8.0-windows)
- **AND** all existing WPF dependencies are resolved

#### Scenario: Target framework switch
- **WHEN** `UseAvalonia=true`
- **THEN** the project targets Avalonia.Desktop
- **AND** NuGet dependencies switch to Avalonia equivalents

### Requirement: CI/CD pipeline
The CI/CD pipeline SHALL build and test both WPF and Avalonia targets.

#### Scenario: Dual CI build
- **WHEN** CI runs
- **THEN** both WPF and Avalonia configurations are built
- **AND** Avalonia build is validated on Windows, Linux, and macOS agents

#### Scenario: Avalonia headless testing
- **WHEN** running Avalonia UI tests
- **THEN** they run headlessly on all platforms via `Avalonia.Headless`
- **AND** test results are reported in the CI pipeline
