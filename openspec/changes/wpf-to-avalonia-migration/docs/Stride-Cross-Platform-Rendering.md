# Stride Cross-Platform Rendering Analysis

> **Task 0.7**: Validate that Stride can render on all platforms and document the rendering pipeline for Avalonia migration planning.

---

## 1. Rendering API Matrix Per Platform

Stride supports **4 graphics backends**, selected at compile-time via MSBuild conditional symbols. There is **no native Metal or OpenGL backend** - macOS/iOS rendering goes through **MoltenVK** (Vulkan-on-Metal translation layer).

| Graphics API | Enum Value | Symbol | Windows | Linux | macOS | iOS | Android |
|---|---|---|---|---|---|---|---|
| **Direct3D 11** | `Direct3D11` | `STRIDE_GRAPHICS_API_DIRECT3D11` | ✅ Full | ❌ | ❌ | ❌ | ❌ |
| **Direct3D 12** | `Direct3D12` | `STRIDE_GRAPHICS_API_DIRECT3D12` | ✅ Full | ❌ | ❌ | ❌ | ❌ |
| **Vulkan** | `Vulkan` | `STRIDE_GRAPHICS_API_VULKAN` | ✅ | ✅ | ✅ (via MoltenVK) | ✅ (via MoltenVK) | ✅ |
| **Null (Stub)** | `Null` | `STRIDE_GRAPHICS_API_NULL` | ✅ (stub) | ✅ (stub) | ✅ (stub) | ✅ (stub) | ✅ (stub) |

**Key finding**: Cross-platform rendering is achieved through Vulkan as the universal backend. Direct3D is Windows-only. Vulkan on macOS is translated to Metal via **MoltenVK** (a bundled dylib that exports `libvulkan.1.dylib`).

### 1.1 Source File Locations

| Backend | Directory | Key Files |
|---|---|---|
| Shared D3D | `sources/engine/Stride.Graphics/Direct3D/` | `GraphicsAdapterFactory.Direct3D.cs`, `GraphicsProfileHelper.cs`, `SwapChainGraphicsPresenter.Direct3D.cs` |
| D3D11 | `sources/engine/Stride.Graphics/Direct3D11/` | `GraphicsDevice.Direct3D11.cs`, `GraphicsDeviceFeatures.Direct3D11.cs`, `CommandList.Direct3D11.cs` |
| D3D12 | `sources/engine/Stride.Graphics/Direct3D12/` | `GraphicsDevice.Direct3D12.cs`, `GraphicsDeviceFeatures.Direct3D12.cs`, `EnhancedBarriers.Direct3D12.cs` |
| Vulkan | `sources/engine/Stride.Graphics/Vulkan/` | `GraphicsDevice.Vulkan.cs`, `GraphicsAdapterFactory.Vulkan.cs`, `BundledMoltenVK.cs`, `Texture.Apple.Vulkan.cs` |
| Null | `sources/engine/Stride.Graphics/Null/` | `GraphicsDevice.Null.cs`, `GraphicsAdapterFactory.Null.cs` (18 stub files) |

---

## 2. Graphics Platform Enum

Defined in `sources/engine/Stride/Graphics/GraphicsPlatform.cs`:

```csharp
public enum GraphicsPlatform
{
    Null,
    Direct3D11,
    Direct3D12,
    Vulkan,
}
```

The Null backend serves as a **template for implementing new backends** - as documented in `NullHelper.cs`:
> "Implementing a new graphic backend requires copying all the content of the Null graphic backend to a new folder and start implementing all the members."

---

## 3. Graphics Adapter Creation Flow

The `GraphicsAdapterFactory` (in `sources/engine/Stride.Graphics/GraphicsAdapterFactory.cs`) is a `static partial class` with `Initialize()`/`Reset()`/`Dispose()` pattern.

### 3.1 Direct3D Adapter Creation (`GraphicsAdapterFactory.Direct3D.cs`)
- Uses **Silk.NET DXGI**
- Creates `IDXGIFactory1+`, queries up to factory version 7
- Enumerates adapters via `EnumAdapters1` or `EnumAdapterByGpuPreference` (DXGI 1.6+)
- Supports `STRIDE_GRAPHICS_SOFTWARE_RENDERING=1` env var for WARP software adapter
- Default: `GpuPreference.HighPerformance`

### 3.2 Vulkan Adapter Creation (`GraphicsAdapterFactory.Vulkan.cs`)
- Uses **Vortice.Vulkan** bindings
- Creates `VkInstance` targeting API 1.3
- Enumerates physical devices with driver info (`VkPhysicalDeviceDriverProperties`)
- **macOS library resolution** (tries in order):
  1. Homebrew: `/opt/homebrew/lib/libvulkan.1.dylib`
  2. Homebrew (x86): `/usr/local/lib/libvulkan.1.dylib`
  3. Bundled MoltenVK: `runtimes/osx-{arm64|x64}/native/libvulkan.1.dylib`
  4. Bare `libvulkan.1.dylib` (dyld fallback)
- Creates surface extensions per-platform (Win32, Xlib, XCB, Metal surface)

### 3.3 Null Adapter Creation (`GraphicsAdapterFactory.Null.cs`)
- Empty - returns no adapters.

---

## 4. Graphics Device Creation Per API

### 4.1 Direct3D 11
- **Profile range**: Level 9.1 through Level 11.x
- **Native**: `ID3D11Device` + `ID3D11DeviceContext`
- **IsDeferred**: `false` (immediate mode only)
- **Workarounds**: Intel GPUs force level >= 10.0; RenderDoc forces level >= 10.0
- **Debug**: Queries `ID3D11InfoQueue` for validation

### 4.2 Direct3D 12
- **Profile range**: Level 11.0+ (forced minimum)
- **Native**: `ID3D12Device` + `ID3D12CommandQueue`
- **IsDeferred**: `true` (multi-threaded command list building)
- **Enhanced Barriers**: **Required** (D3D12_FEATURE_D3D12_OPTIONS12 check)
  - Minimum drivers: NVIDIA 531.18+, AMD 23.5.2+, Intel 31.0.101.4032+
  - Minimum OS: Windows 10 21H2 or Windows 11
- **Agility SDK**: Auto-activates app-local D3D12Core.dll (version 619)
- **Debug**: GPU-Based Validation (GBV), DRED forced on

### 4.3 Vulkan
- **API version**: Targets Vulkan 1.3
- **Native**: `VkDevice` + `VkQueue`
- **IsDeferred**: `true` (multi-threaded)
- **Required features**: Timeline semaphores, swapchain, portability subset (MoltenVK), tessellation (if profile >= 11.0), anisotropic filtering, depth clamp
- **macOS** (via MoltenVK): `VK_EXT_metal_objects` for IOSurface-backed CVPixelBuffer imports
- **Android**: `VK_ANDROID_external_memory_android_hardware_buffer` chain (7 extensions)
- **Profile enforcement**: `GraphicsProfileHelper.Vulkan.cs` is **entirely commented out**. Vulkan feature detection is runtime-based via `VkPhysicalDeviceFeatures`.
- **Per-format feature detection**: **Stub** - returns `MultisampleCount.None` for all formats

### 4.4 Null
- Stub implementation - all methods call `NullHelper.ToImplement()` which no-ops

---

## 5. MoltenVK Integration (macOS Rendering)

File: `sources/engine/Stride.Graphics/Vulkan/BundledMoltenVK.cs`

A **module initializer** that auto-runs on macOS:

1. **ICD pinning**: Writes a synthesized `MoltenVK_icd.json` manifest pointing to the absolute path of bundled `libvulkan.1.dylib`
2. **Validation layer pinning**: Rewrites Homebrew's validation layer manifest with absolute `library_path`
3. **Environment setup**: Uses native `setenv()` via P/Invoke to `libc` so the Vulkan loader (`getenv`) picks up the overrides

This means on macOS, Stride uses **Vulkan API calls** which MoltenVK translates to Metal under the hood. The bundled dylib lives at `runtimes/osx-{arch}/native/libvulkan.1.dylib`.

---

## 6. Window Creation Per Platform

### 6.1 Platform Abstraction Architecture

```
Game.Run(GameContext)
  └─ GamePlatform.Create(game)         [compile-time dispatch]
       ├─ STRIDE_PLATFORM_UWP          → GamePlatformUWP
       ├─ STRIDE_PLATFORM_ANDROID      → GamePlatformAndroid
       ├─ STRIDE_PLATFORM_IOS          → GamePlatformiOS
       └─ STRIDE_PLATFORM_DESKTOP      → GamePlatformDesktop (all other desktops)

  └─ gamePlatform.Run(Context)
       └─ CreateWindow(context)
            └─ GetSupportedGameWindow(type)
                 ├─ DesktopSDL        → GameWindowSDL
                 ├─ DesktopWinForms   → GameWindowWinforms
                 └─ Headless          → GameWindowHeadless
```

### 6.2 WinForms (Windows Desktop)
- **`GameWindowWinforms`** (`sources/engine/Stride.Games/Desktop/GameWindowWinforms.cs`)
  - Wraps `System.Windows.Forms.Control` as rendering surface
  - Creates `WindowHandle(AppContextType.DesktopWinForms, Control, Control.Handle)`
- **`GameForm`** (`sources/engine/Stride.Games/Desktop/GameForm.cs`)
  - Custom `Form` subclass (~500 lines): DPI handling, touch input, fullscreen toggle, power management, resize events
- **`WindowsMessageLoop`**: Classic `PeekMessage` + `Application.DoEvents` pump
- **Condition**: `#if (STRIDE_GRAPHICS_API_DIRECT3D || STRIDE_GRAPHICS_API_VULKAN) && (STRIDE_UI_WINFORMS || STRIDE_UI_WPF)`

### 6.3 SDL (Linux, macOS, Android, iOS)
- **`GameWindowSDL`** (`sources/engine/Stride.Games/SDL/GameWindowSDL.cs`)
  - Wraps `Stride.Graphics.SDL.Window` from Silk.NET SDL
  - Creates `WindowHandle(AppContextType.DesktopSDL, window, window.Handle)`
- **`Stride.Graphics.SDL.Window`** (`sources/engine/Stride.Graphics/SDL/Window.cs`)
  - Low-level SDL window, extracts platform-specific handles:
    - **Windows**: `info.Info.Win.Hwnd`
    - **Linux (X11)**: `info.Info.X11.Window` + `info.Info.X11.Display`
    - **Android**: `info.Info.Android.Window` + `info.Info.Android.Surface`
    - **macOS**: `info.Info.Cocoa.Window`
  - **Linux workaround**: Forces `SDL_VIDEODRIVER=x11` for Wayland multi-context issues
- **`SDLMessageLoop`**: SDL event pump (`PollEvent` + `Window.ProcessEvent`)
- **`GameFormSDL`**: Extends `Window` with app lifecycle events
- **Condition**: `#if STRIDE_UI_SDL`

### 6.4 Headless
- **`GameWindowHeadless`** (`sources/engine/Stride.Games/GameWindowHeadless.cs`)
  - No native window handle
  - Simple `while(!Exiting)` loop
  - Combined with `HeadlessGraphicsPresenter` for automated testing

### 6.5 Context Auto-Detection

When no `GameContext` is passed, `GameContextFactory` auto-detects:

| Runtime Condition | Selected Context Type |
|---|---|
| Windows + WinFormsBackend enabled | `DesktopWinForms` |
| Android | `Android` |
| iOS/tvOS/watchOS | `iOS` |
| Everything else | `DesktopSDL` |

---

## 7. Rendering Initialization Flow

```
Game.Run(GameContext)
  └─ GameContextFactory.NewGameContext(type)     → creates appropriate context
  └─ GamePlatform.Create(game)                   → platform-specific subclass
  └─ gamePlatform.Run(context)
       └─ CreateWindow(context)                  → GameWindow subclass
       └─ gameWindow.Run()
            ├─ InitCallback()
            │    └─ GameBase.InitializeBeforeRun()
            │         └─ IGraphicsDeviceManager.CreateDevice()
            │              └─ GraphicsDeviceManager.ChangeOrCreateDevice()
            │                   └─ FindBestDevice() → GraphicsPlatform.FindBestDevices()
            │                   └─ graphicsDeviceFactory.CreateDevice(info)
            │                        └─ GraphicsDevice.New(adapter, flags, nativeWindow, profile)
            │                             └─ InitializePlatformDevice() [per-backend partial]
            │                             └─ Create GraphicsDeviceFeatures
            │                             └─ Create SwapChainGraphicsPresenter
            │                                  └─ CreateSwapChain(backBufferWidth, height, format, ...)
            └─ RunCallback()                     (per-frame)
                 └─ GameBase.Tick()
                      ├─ Update(gameTime)
                      └─ Draw(gameTime)
                           ├─ IGraphicsDeviceManager.BeginDraw()
                           └─ Present()
```

### Presenter Choice

In `GamePlatform.CreateDevice()` (line 425-445 of `GamePlatform.cs`):
```csharp
graphicsDevice.Presenter = gameWindow is GameWindowHeadless
    ? new HeadlessGraphicsPresenter(graphicsDevice, info.PresentationParameters)
    : new SwapChainGraphicsPresenter(graphicsDevice, info.PresentationParameters);
```

---

## 8. Editor 3D Preview Rendering (WPF Integration)

### 8.1 Architecture: Win32 HWND Embedding

The Stride Editor embeds the 3D game engine via **Win32 HWND parenting**:

```
WPF Window (GameStudioWindow / EntityHierarchyEditorView)
  └─ ContentPresenter (SceneView)
       └─ GameEngineHost (WPF FrameworkElement)
            └─ [Win32: SetParent to WPF's HwndSource.Handle]
                 └─ EmbeddedGameForm (WinForms Form, TopLevel=false)
                      └─ Stride Game Engine (DirectX/Vulkan rendering to form's HWND)
```

### 8.2 Key Files

| File | Purpose |
|---|---|
| `sources/presentation/Stride.Core.Presentation.Wpf/Controls/GameEngineHost.cs` | WPF `FrameworkElement` that hosts a child HWND via `SetParent`. Implements `IWin32Window`, `IKeyboardInputSink`. Tracks position via `LayoutUpdated`, forwards messages. |
| `sources/editor/Stride.Editor/Engine/EmbeddedGameForm.cs` | WinForms `GameForm` subclass that forwards keyboard/mouse messages to `GameEngineHost` via `WndProc` |
| `sources/editor/Stride.Editor/Engine/EmbeddedGame.cs` | `Game` subclass for borderless, embedded rendering |
| `sources/editor/Stride.Editor/EditorGame/Game/EditorServiceGame.cs` | Base class for editor games with service management, throttling |
| `sources/editor/Stride.Editor/Preview/GameStudioPreviewService.cs` | Creates `EmbeddedGameForm` + `GameEngineHost`, runs `PreviewGame` on background STA thread |
| `sources/editor/Stride.Editor/Preview/PreviewGame.cs` | Specialized `EditorServiceGame` for asset previews |
| `sources/editor/Stride.Editor/Preview/View/StridePreviewView.cs` | WPF `Control` wrapping a `ContentPresenter` named `PART_StrideView` |

### 8.3 Threading Model
- **Game runs on a dedicated background STA thread** (both preview and scene editor)
- **WPF UI runs on main STA thread**
- Communication: `Game.Script.AddTask()` + `Dispatcher.InvokeAsync()`
- `EditorGameController` provides `InvokeAsync`/`LowPriorityInvokeAsync` for cross-thread marshalling

### 8.4 Message Forwarding
`EmbeddedGameForm.WndProc` intercepts `WM_KEYDOWN`, `WM_KEYUP`, `WM_MOUSEWHEEL`, mouse button messages, and `WM_CONTEXTMENU`, then calls `Host.ForwardMessage()` which dispatches into WPF's routed event system.

### 8.5 Current WPF Dependencies
The editor is **entirely WPF-based**:
- `Stride.Core.Presentation.Wpf` - core presentation library with controls, themes
- `GameEngineHost` in `Stride.Core.Presentation.Wpf` - the HWND embedding control
- `StridePreviewView` in `Stride.Editor.Preview.View` - WPF Control subclass
- `GameStudioWindow.xaml` - WPF main window with AvalonDock docking

---

## 9. Platform-Specific Limitations

### 9.1 Direct3D 12 Required Features
- **Enhanced Barriers** require Windows 10 21H2+ / Windows 11 with recent GPU drivers
- **No D3D12 on Linux** (even via Wine/Proton is not supported)

### 9.2 Vulkan on macOS (MoltenVK)
- **Not a full Vulkan implementation** - MoltenVK is a portability driver with limitations:
  - No tessellation shaders (emulated)
  - No geometry shaders (emulated via software fallback)
  - Limited to Metal feature set
  - `VK_KHR_portability_subset` extension marks unsupported features
- **Per-format feature queries are stubs** - `GraphicsDeviceFeatures.Vulkan.cs` returns `MultisampleCount.None` for all formats
- **macOS profile enforcement is disabled** - `GraphicsProfileHelper.Vulkan.cs` is entirely commented out

### 9.3 SDL on Linux
- **Forces X11** via `SDL_VIDEODRIVER=x11` workaround (Wayland multi-context issues)

### 9.4 Vulkan Features (Desktop)
- Timeline semaphores are **required** (core in Vulkan 1.2)
- Per-format multisampling detection is **not implemented** for Vulkan

### 9.5 Null Backend
- Complete stub - cannot render anything; only for headless/testing

---

## 10. Compile-Time Graphics API Selection

Defined via `sources/sdk/Stride.Build.Sdk/` MSBuild logic:

| Define | Graphics API |
|---|---|
| `STRIDE_GRAPHICS_API_NULL` | Null / No rendering |
| `STRIDE_GRAPHICS_API_DIRECT3D11` | Direct3D 11 |
| `STRIDE_GRAPHICS_API_DIRECT3D12` | Direct3D 12 |
| `STRIDE_GRAPHICS_API_VULKAN` | Vulkan |
| `STRIDE_GRAPHICS_API_DIRECT3D` | Shared D3D code (used alongside D3D11 or D3D12) |

Only **one** graphics API can be active per build (mutually exclusive via MSBuild conditions).

---

## 11. Integration Recommendations for Avalonia

### 11.1 Critical Components to Port

| WPF Component | Avalonia Equivalent | Notes |
|---|---|---|
| `GameEngineHost` (HWND embedding) | **New: `AvaloniaGameEngineHost`** | Custom `NativeControlHost` subclass using platform-specific native embedding |
| Win32 `SetParent` parenting | `NativeControlHost` + AvnWin32 | Avalonia's `NativeControlHost` wraps HWND embedding on Windows |
| X11 window parenting | `NativeControlHost` + XEmbed | On Linux, use XEmbed protocol with SDL's X11 window |
| macOS NSView parenting | `NativeControlHost` + NSView | On macOS, embed SDL's Cocoa NSView as child |
| `EmbeddedGameForm.WndProc` message forwarding | Platform-native message pump | Each platform needs its own input forwarding mechanism |
| `GameEngineHost.KeyboardInputSink` | Avalonia `IInputElement` / `IKeyboardNavigationHandler` | Keyboard focus management |
| WPF `HwndSource` | `TopLevel.TryGetPlatformHandle()` | Access native window handle from Avalonia |

### 11.2 Approach: `NativeControlHost` Strategy

Avalonia provides `NativeControlHost` which can embed native child windows:

```csharp
// Conceptual approach
public class AvaloniaGameEngineHost : NativeControlHost
{
    private IntPtr childHandle;
    
    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        // 1. Create SDL or WinForms window with childHandle
        // 2. Reparent to Avalonia's native parent window
        // 3. Return the handle for Avalonia to manage sizing
    }
    
    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        // Cleanup
    }
}
```

**Platform-specific embedding**:
- **Windows**: `SetParent(hwnd, avaloniaParentHwnd)` + handle `WM_SIZE`/`WM_MOVE`
- **Linux**: XEmbed via `XReparentWindow` with the SDL window's X11 handle
- **macOS**: `[parentView addSubview:childView]` with Cocoa NSView

### 11.3 Input Forwarding Strategy

Instead of the current `WndProc` → `GameEngineHost.ForwardMessage()` pattern:

1. **Use SDL windowing directly** everywhere (avoid WinForms dependency)
2. Let the SDL window be a child of the native Avalonia window
3. Use the existing `SDLMessageLoop` or poll SDL events in the game loop
4. Forward input events from the SDL window to the Avalonia window via platform-specific calls

### 11.4 Threading Model Compatibility

The current STA game thread pattern is compatible with Avalonia:
- Game runs on background thread (unchanged)
- Avalonia has its own `Dispatcher` (analogous to WPF)
- Use `Dispatcher.UIThread.InvokeAsync()` for main thread marshalling

### 11.5 Recommended Phased Migration

| Phase | Scope | Effort |
|---|---|---|
| **1. SDL-first windowing** | Make `GameWindowSDL` the default on all platforms (replace WinForms) | Medium |
| **2. Avalonia `NativeControlHost`** | Create `AvaloniaGameEngineHost` that embeds SDL window | High |
| **3. Editor control migration** | Replace all `GameEngineHost` references in editor code | Medium |
| **4. Input system** | Adapt input forwarding to work through Avalonia events | High |
| **5. Remove WPF dependency** | Remove `Stride.Core.Presentation.Wpf`, `EmbeddedGameForm`, `GameForm` | Medium |

---

## 12. Risk Assessment

| Risk | Severity | Mitigation |
|---|---|---|
| **MoltenVK limitations on macOS** | Medium | Test with complex shaders; document unsupported features; consider fallback rendering paths |
| **Vulkan per-format feature detection is stub** | Medium | Implement `GraphicsDeviceFeatures.Vulkan.cs` properly before production use |
| **Linux X11-only SDL workaround** | Medium | Monitor Wayland support improvements in Silk.NET SDL; test on both X11 and Wayland |
| **No cross-platform DX12** | Low | Vulkan is the cross-platform path; DX12 stays Windows-only |
| **Direct3D 12 Enhanced Barriers requirement** | Low | High driver requirements (NVIDIA 531+, AMD 23.5.2+); provide D3D11 fallback |
| **Avalonia `NativeControlHost` limitations** | Medium | `NativeControlHost` may not support all platforms equally; test on Windows, Linux, macOS |
| **Input forwarding complexity** | High | Each platform needs different input marshalling; consider consolidating on SDL input |
| **Editor threading** | Low | Current STA game thread pattern works with Avalonia's dispatcher |

---

## 13. Key File Index

| File Path | Relevance |
|---|---|
| `sources/engine/Stride.Graphics/GraphicsPlatform.cs` | Graphics API enum |
| `sources/engine/Stride.Graphics/GraphicsAdapterFactory.cs` | Adapter factory base |
| `sources/engine/Stride.Graphics/Direct3D/GraphicsAdapterFactory.Direct3D.cs` | DXGI adapter creation |
| `sources/engine/Stride.Graphics/Vulkan/GraphicsAdapterFactory.Vulkan.cs` | Vulkan adapter creation |
| `sources/engine/Stride.Graphics/Vulkan/BundledMoltenVK.cs` | macOS MoltenVK bootstrap |
| `sources/engine/Stride.Graphics/Vulkan/GraphicsDeviceFeatures.Vulkan.cs` | Vulkan feature detection (stub) |
| `sources/engine/Stride.Graphics/Vulkan/Texture.Apple.Vulkan.cs` | macOS IOSurface import |
| `sources/engine/Stride.Graphics/Null/GraphicsAdapterFactory.Null.cs` | Null backend template |
| `sources/engine/Stride.Games/GamePlatform.cs` | Platform factory + device creation |
| `sources/engine/Stride.Games/Desktop/GamePlatformDesktop.cs` | Desktop platform dispatch |
| `sources/engine/Stride.Games/Desktop/GameWindowWinforms.cs` | WinForms window |
| `sources/engine/Stride.Games/Desktop/GameForm.cs` | WinForms GameForm (~500 lines) |
| `sources/engine/Stride.Games/SDL/GameWindowSDL.cs` | SDL window (cross-platform) |
| `sources/engine/Stride.Games/SDL/GameFormSDL.cs` | SDL game form |
| `sources/engine/Stride.Graphics/SDL/Window.cs` | Low-level SDL window (handle extraction) |
| `sources/engine/Stride.Games/GameContext.cs` | Game context base |
| `sources/engine/Stride.Games/GameContextFactory.cs` | Context auto-detection |
| `sources/engine/Stride.Games/GraphicsDeviceManager.cs` | Device lifecycle manager |
| `sources/engine/Stride.Engine/Engine/Game.cs` | High-level game initialization |
| `sources/presentation/Stride.Core.Presentation.Wpf/Controls/GameEngineHost.cs` | WPF HWND embedding control |
| `sources/editor/Stride.Editor/Engine/EmbeddedGameForm.cs` | WinForms editor game form |
| `sources/editor/Stride.Editor/Engine/EmbeddedGame.cs` | Embedded game base |
| `sources/editor/Stride.Editor/EditorGame/Game/EditorServiceGame.cs` | Editor game services |
| `sources/editor/Stride.Editor/Preview/GameStudioPreviewService.cs` | Preview service |
| `sources/editor/Stride.Editor/Preview/View/StridePreviewView.cs` | WPF preview control |