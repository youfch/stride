## ADDED Requirements

### Requirement: Cross-platform game engine host
The system SHALL provide an interface and platform-specific implementations for embedding the Stride game rendering window into the editor UI.

#### Scenario: Host interface
- **WHEN** the editor needs to host a 3D preview
- **THEN** it uses the `IGameHost` interface to create and manage the native rendering window
- **AND** the interface abstracts platform-specific window management

#### Scenario: Windows implementation
- **WHEN** running on Windows
- **THEN** the `Win32GameHost` creates an HWND and parents it via `SetParent`
- **AND** the rendering engine renders to that HWND via DirectX
- **AND** input events are forwarded from the host window

#### Scenario: Linux implementation
- **WHEN** running on Linux (X11)
- **THEN** the `X11GameHost` creates an X11 window and reparents it via `XReparentWindow`
- **AND** the rendering engine renders via Vulkan
- **AND** input events are monitored via XNextEvent/XCheckTypedEvent

#### Scenario: macOS implementation
- **WHEN** running on macOS
- **THEN** the `MacGameHost` creates an NSView and embeds it via Quartz
- **AND** the rendering engine renders via Metal
- **AND** input events are monitored via NSEvent monitoring

### Requirement: Input forwarding system
The system SHALL forward keyboard and mouse events from the editor to the game rendering engine.

#### Scenario: Keyboard events
- **WHEN** a key is pressed while the 3D preview has focus
- **THEN** the key event is forwarded to the Stride input system
- **AND** special keys (WASD, arrows, modifiers) are correctly translated

#### Scenario: Mouse events
- **WHEN** mouse buttons are clicked in the 3D preview
- **THEN** the button state is forwarded to the Stride input system
- **AND** mouse movement/delta is forwarded

### Requirement: DPI-aware resizing
The system SHALL properly handle DPI scaling and window resize for the 3D preview.

#### Scenario: DPI change
- **WHEN** the editor window is moved to a different DPI monitor
- **THEN** the 3D preview is resized to match the new DPI
- **AND** no visual artifacts or scaling issues occur

#### Scenario: Window resize
- **WHEN** the editor window is resized
- **THEN** the 3D preview fills the available space
- **AND** the rendering resolution matches the pixel dimensions of the host area

### Requirement: Avalonia NativeControlHost integration
The system SHALL integrate the game engine host with Avalonia's NativeControlHost control.

#### Scenario: Host embedding in Avalonia
- **WHEN** the 3D preview is shown in an Avalonia window
- **THEN** the native host window is embedded via `NativeControlHost`
- **AND** the host is properly arranged and sized within the Avalonia layout

#### Scenario: Focus management
- **WHEN** the 3D preview area is clicked
- **THEN** focus is transferred to the game engine window
- **AND** input events are captured until focus leaves the preview area
