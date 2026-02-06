# AR Scene Setup Guide - Unity 6.3

## Step 1: Create AR Scene
1. **File → New Scene → Save As** `ARScene`

## Step 2: XR Origin Setup
1. **GameObject → XR → XR Origin (Mobile AR)**
   - This auto-creates: XR Origin, AR Session, AR Camera
2. Select **XR Origin** and add components:
   - **AR Plane Manager**
   - **AR Raycast Manager**
3. Add **ARSessionController** script to XR Origin

## Step 3: AR Plane Visualization (Required for Detection)
1. **Create → 3D Object → Quad** → name it `AR Plane`
2. Create material: **Create → Material** → name `PlaneMaterial`
   - Set shader to **Universal Render Pipeline/Lit**
   - Set **Surface Type** to Transparent
   - Set **Base Color** alpha to ~0.3
   - Set color to light blue/green
3. Apply material to Quad
4. Add **AR Plane** component to Quad
5. **Create → Prefabs** folder if not exists
6. Drag Quad into Prefabs folder → Delete from scene
7. Assign prefab to **AR Plane Manager → Plane Prefab**

## Step 4: Robot Prefab
1. Use your existing robot model (robot.fbx or robotwlegs.fbx)
2. Add **ARRobotController** script to robot prefab
3. Ensure robot has:
   - Rigidbody (set to Kinematic)
   - Collider
4. Save as prefab in Prefabs folder

## Step 5: UI Setup
### Canvas Setup
1. **GameObject → UI → Canvas**
2. Set **Render Mode** to **Screen Space - Overlay**
3. Add **Canvas Scaler** → Set to **Scale With Screen Size**, Reference: 1080x1920

### Placement UI (shown before robot placed)
1. Create **Panel** → name `PlacementUI`
2. Add **Text** child → name `InstructionText`
   - Anchor: Top Center
   - Text: "Move phone to detect surfaces"
   - Font Size: 40, Color: White

### Gameplay UI (shown after robot placed)
1. Create **Panel** → name `GameplayUI`
2. **Left Button**: 
   - Create **Button** → anchor bottom-left
   - Size: 150x150, Text: "←"
3. **Right Button**: 
   - Create **Button** → anchor bottom-right (offset from left)
   - Size: 150x150, Text: "→"
4. **Exit Button**:
   - Create **Button** → anchor top-right
   - Size: 120x50, Text: "EXIT"
5. **Scale Slider**:
   - Create **Slider** → anchor bottom-center
   - Min: 0, Max: 1, Value: 0.5
6. Set `GameplayUI` **inactive** by default

### ARUIManager Setup
1. Add **ARUIManager** script to Canvas
2. Assign references:
   - Session Controller → XR Origin
   - Left/Right/Exit Buttons
   - Scale Slider
   - Instruction Text

## Step 6: ARSessionController Configuration
Select **XR Origin** and assign:
- AR Session → AR Session object
- Plane Manager → AR Plane Manager component
- Raycast Manager → AR Raycast Manager component
- Robot Prefab → Your robot prefab
- Placement UI → PlacementUI panel
- Gameplay UI → GameplayUI panel
- Instruction Text → InstructionText

## Step 7: GameDataManager
1. **Create Empty** → name `GameDataManager`
2. Add **GameDataManager** script
3. This persists across scenes (DontDestroyOnLoad)

## Step 8: Build Settings
1. **File → Build Settings**
2. Add scenes: MainGame, ARScene
3. **Player Settings → XR Plug-in Management**:
   - Android: Enable **ARCore**
   - iOS: Enable **ARKit**
4. **Player Settings → Other Settings**:
   - Android: Minimum API 24+
   - iOS: Requires ARKit checked

## Step 9: Test Hierarchy
Your ARScene hierarchy should look like:
```
ARScene
├── XR Origin
│   ├── Camera Offset
│   │   └── Main Camera (AR Camera)
│   └── [ARSessionController, ARPlaneManager, ARRaycastManager]
├── AR Session
├── GameDataManager
├── Canvas
│   ├── PlacementUI
│   │   └── InstructionText
│   └── GameplayUI (inactive)
│       ├── LeftButton
│       ├── RightButton
│       ├── ExitButton
│       └── ScaleSlider
├── EventSystem
└── Directional Light
```

## Verification Checklist
- [ ] XR Origin has all AR Manager components
- [ ] Plane prefab assigned to AR Plane Manager
- [ ] Robot prefab assigned to ARSessionController
- [ ] All UI elements connected in ARUIManager
- [ ] GameDataManager in scene
- [ ] ARScene added to Build Settings
