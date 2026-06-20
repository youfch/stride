## ADDED Requirements

### Requirement: ViewModel sharing
The system SHALL enable sharing ViewModel instances between WPF and Avalonia UI layers.

#### Scenario: Shared ViewModel
- **WHEN** a ViewModel is created by Stride.Core.Quantum
- **THEN** it can be bound to both WPF and Avalonia views
- **AND** property change notifications work on both frameworks
- **AND** command bindings (ICommand) are framework-agnostic

#### Scenario: Data binding bridge
- **WHEN** a ViewModel property changes
- **THEN** both WPF and Avalonia bindings are updated
- **AND** the bridge ensures UI thread marshaling for both frameworks

### Requirement: WPF hosting Avalonia (Phase 1-2)
The system SHALL allow Avalonia controls to be hosted inside a WPF application during migration.

#### Scenario: AvaloniaWindow inside WPF
- **WHEN** an Avalonia control is created
- **THEN** it is rendered in an Avalonia `Window` whose HWND is parented to a WPF `HwndHost`
- **AND** the Avalonia control renders correctly within the WPF layout
- **AND** input events (keyboard, mouse) are forwarded between frameworks

#### Scenario: Layout synchronization
- **WHEN** the WPF parent window is resized
- **THEN** the embedded Avalonia window is resized to match
- **AND** DPI changes are propagated to both frameworks

#### Scenario: Lifetime management
- **WHEN** the WPF parent window is closed
- **THEN** the embedded Avalonia window is properly disposed
- **AND** no memory leaks or dangling HWNDs remain

### Requirement: Avalonia hosting WPF (fallback)
The system MAY support hosting WPF controls inside an Avalonia window as a fallback for unported controls.

#### Scenario: WPFHost inside Avalonia
- **WHEN** a WPF control must be shown in an Avalonia application
- **THEN** it is rendered in a WPF window whose HWND is embedded via Avalonia's NativeControlHost
- **AND** the WPF control functions correctly within the Avalonia window
