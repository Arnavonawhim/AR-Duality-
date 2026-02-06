# AR Camera Not Starting - Troubleshooting Guide

## Issue Identified
The AR Camera is missing required components for Unity 6.3 AR Foundation.

## Solution Steps

### 1. Select Main Camera (under XR Origin → Camera Offset)
In your hierarchy:
```
XR Origin
├── Camera Offset
│   └── Main Camera  ← SELECT THIS
```

### 2. Add Required AR Camera Components
With Main Camera selected, click **Add Component** and add:

#### A. AR Camera Manager
- Search: "AR Camera Manager"
- Component: `UnityEngine.XR.ARFoundation.ARCameraManager`

#### B. AR Camera Background
- Search: "AR Camera Background"
- Component: `UnityEngine.XR.ARFoundation.ARCameraBackground`
- **CRITICAL**: This renders the device camera feed

### 3. Configure Main Camera
Set these properties:
- **Clear Flags**: Solid Color
- **Background**: Black (R: 0, G: 0, B: 0, A: 0)
- **Culling Mask**: Everything
- **Depth**: 0 or higher

### 4. Verify XR Plugin Management
**Edit → Project Settings → XR Plug-in Management**

#### Android Tab:
- ☑ **ARCore** enabled

#### iOS Tab (if building for iOS):
- ☑ **ARKit** enabled

### 5. Build Settings Platform
**File → Build Settings**
- Platform: **Android** or **iOS**
- Click **Switch Platform** if not already selected

## Camera Feed Test (Editor)
In Unity Editor, the camera won't show real AR, but should NOT crash. The actual camera feed only works on device.

## On-Device Testing Required
AR Foundation REQUIRES actual device testing. In editor you'll only see:
- Black screen (normal - no device camera)
- UI elements should still appear
- Touch/click should trigger placement logic

## Quick Verification Checklist
- [ ] Main Camera has AR Camera Manager
- [ ] Main Camera has AR Camera Background
- [ ] AR Session exists in scene
- [ ] XR Plug-in Management has ARCore/ARKit enabled
- [ ] Build platform is Android or iOS

## If Still Not Working
Check console for:
- "XR provider not initialized" → XR Plugins not enabled
- "ARSession failed to initialize" → Missing AR Session component
- "ARCameraBackground requires ARCameraManager" → Missing component on camera
