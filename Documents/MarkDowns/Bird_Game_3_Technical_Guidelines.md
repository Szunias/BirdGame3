# Bird Game 3 - Technical Coding Guidelines
**Version:** 1.1 - November 2024

---

## 1. INPUT SYSTEM STRATEGY (Added in v1.1)
Bird Game 3 strictly uses the **Unity Input System Package (New)**. Do NOT use `Input.GetKey` or the legacy input manager.

### 1.1 Architecture
*   **Input Action Asset:** `BirdInputActions.inputactions`
*   **Generation:** Generate C# Class enabled (`BirdInputActions.cs`)
*   **Wrapper Script:** `Bird_Input.cs` (The ONLY place input is read)

### 1.2 Action Maps
*   **Gameplay (Action Map)**
    *   Move (Vector2)
    *   Look (Vector2)
    *   Jump (Button)
    *   Ability1 (Button)
    *   Ability2 (Button)
    *   Interact/Steal (Button)
    *   ThrowEgg (Button)
    *   DropEgg (Button)
*   **UI (Action Map)**
    *   Pause (Button)
    *   Scoreboard (Button)

### 1.3 Bird_Input.cs Pattern
```csharp
public class Bird_Input : MonoBehaviour
{
    // Input reads ONLY here, broadcasts via events
    public UnityEvent OnJumpPressed;
    public UnityEvent OnJumpReleased;
    public UnityEvent OnAbility1Pressed;
    public UnityEvent OnAbility2Pressed;
    public UnityEvent OnThrowEggPressed;
    public UnityEvent<Vector2> OnMoveInput;

    private PlayerInput _playerInput;

    // Other scripts subscribe to events, never read input directly
}
```
**WARNING:** NEVER use `Input.GetKey()` anywhere. All input goes through `Bird_Input` events.

---

## 2. NAMING CONVENTIONS

### 2.1 Script Naming Format
All scripts follow the pattern: `[Category]_[Function].cs`
This groups related scripts in file explorers and makes searching predictable.

**Category Prefixes:**
*   `Bird_` - Player bird logic (e.g., `Bird_Movement`, `Bird_Health`)
*   `Ability_` - Bird abilities (e.g., `Ability_DeathStare`, `Ability_Scoop`)
*   `Egg_` - Egg mechanics (e.g., `Egg_Physics`, `Egg_Splat`)
*   `Nest_` - Nest functionality (e.g., `Nest_Storage`, `Nest_Hatchery`)
*   `Map_` - Map/environment (e.g., `Map_JumpPad`, `Map_Biome`)
*   `UI_` - User interface (e.g., `UI_Scoreboard`, `UI_EggCounter`)
*   `Net_` - Networking specific (e.g., `Net_Lobby`, `Net_Spawner`)
*   `Audio_` - Audio systems (e.g., `Audio_BirdVoice`, `Audio_Ambience`)
*   `FX_` - Visual effects (e.g., `FX_GoldenGlow`, `FX_EggSplat`)
*   `Data_` - ScriptableObjects (e.g., `Data_BirdStats`, `Data_AbilityConfig`)
*   `Mgr_` - Manager/Singleton (e.g., `Mgr_Match`, `Mgr_Pool`)

### 2.2 Network Script Suffixes
For multiplayer scripts, add suffixes to clarify execution context:
*   `_Net` - Contains NetworkBehaviour, handles sync (e.g., `Egg_Pickup_Net.cs`)
*   `_Visual` - Client-only visual effects (e.g., `Egg_Pickup_Visual.cs`)
*   `_Server` - Server-authoritative logic only (e.g., `Nest_Steal_Server.cs`)

### 2.3 Variable Naming
```csharp
// Private fields: camelCase with underscore prefix
private float _flySpeed;
private bool _isCarryingEgg;

// Serialized fields: camelCase, no prefix
[SerializeField] private float flySpeed = 10f;

// Public properties: PascalCase
public float FlySpeed => _flySpeed;
public bool IsCarryingEgg { get; private set; }

// Constants: SCREAMING_SNAKE_CASE
private const float MAX_FLIGHT_SPEED = 15f;
private const int MAX_EGG_CARRY = 3;
```

---

## 3. PROJECT FOLDER STRUCTURE

### 3.1 Root Structure
*   `Assets/`
    *   `_Project/` (All game-specific assets)
        *   `Scripts/`
        *   `Prefabs/`
        *   `ScriptableObjects/`
        *   `Art/`
        *   `Audio/`
        *   `Scenes/`
    *   `Plugins/` (Third-party assets)
    *   `Resources/` (Runtime-loaded assets only)

### 3.2 Scripts Folder Detail
*   `Scripts/`
    *   `Bird/`
        *   `Core/` (Movement, Health, Input, EggCarrier)
        *   `Abilities/` (All ability scripts grouped by bird)
        *   `Visual/`
    *   `Egg/`
        *   `Core/`
        *   `Effects/`
    *   `Nest/`
    *   `Map/`
    *   `UI/`
    *   `Network/`
        *   `Core/`
        *   `Sync/`
    *   `Audio/`
    *   `Managers/`
    *   `Data/`
    *   `Interfaces/`
    *   `Utilities/`

### 3.3 Assembly Definitions
Use Assembly Definitions (`.asmdef`) to reduce compile times and enforce dependencies.

**Assemblies:**
*   `BirdGame.Core` (NO dependencies)
*   `BirdGame.Bird` (depends on: Core)
*   `BirdGame.Gameplay` (depends on: Core)
*   `BirdGame.UI` (depends on: Core)
*   `BirdGame.Network` (depends on: Core, Bird, Gameplay)
*   `BirdGame.Audio` (depends on: Core)

**Dependency Rules:**
*   Core has ZERO dependencies.
*   Bird and Gameplay depend ONLY on Core, not each other.
*   Interactions go through Interfaces in Core.
*   Network assembly can see everything.

---

## 4. SCRIPT COMMUNICATION PATTERNS

### 4.1 The Problem
With 200+ small scripts, avoiding spaghetti dependencies is key. Use Events, Interfaces, and Service Locators.

### 4.2 Pattern 1: UnityEvents (Inspector-Configurable)
Best for: Design-time connections, audio/VFX responses.
```csharp
public class Bird_Health : MonoBehaviour
{
    public UnityEvent OnDamaged;
    public void TakeDamage(int amount) {
        _currentHealth -= amount;
        OnDamaged?.Invoke();
    }
}
```

### 4.3 Pattern 2: C# Events/Actions (Code-Only)
Best for: Performance-critical paths, runtime subscriptions, system-level events.
```csharp
public class Mgr_Match : MonoBehaviour
{
    public static event Action OnMatchStarted;
}
// Always unsubscribe in OnDisable!
```

### 4.4 Pattern 3: Interface + GetComponent
Best for: Collision/trigger interactions where type is unknown.
```csharp
void OnCollisionEnter(Collision col)
{
    if (col.gameObject.TryGetComponent<IDamageable>(out var target))
    {
        target.TakeDamage(15);
    }
}
```

### 4.5 Pattern 4: Service Locator (Managers)
Best for: Accessing singleton managers without tight coupling.
```csharp
Services.Pool.Spawn(eggPrefab, position);
Services.Audio.PlaySFX("EggPickup");
```

---

## 5. DATA ARCHITECTURE (SCRIPTABLE OBJECTS)

### 5.1 Why ScriptableObjects
All bird stats, ability configs, and game settings live in ScriptableObjects - NOT hardcoded in scripts.

### 5.2 Bird Data Structure
```csharp
[CreateAssetMenu]
public class Data_BirdStats : ScriptableObject
{
    public string birdName;
    public BirdTier tier;
    public float groundSpeed = 10f;
    public Data_AbilityConfig ability1;
    // ...
}
```

### 5.3 Ability Data Structure
`Data_AbilityConfig` holds name, icon, cooldown, damage, range, and VFX references.

---

## 6. NETWORKING GUIDELINES

### 6.1 Network Architecture
Bird Game 3 uses **CLIENT-SERVER** architecture with **SERVER AUTHORITY**.

| SYSTEM | AUTHORITY | REASON |
| :--- | :--- | :--- |
| Egg pickup/drop | SERVER | Prevent cheating |
| Nest stealing | SERVER | Prevent cheating |
| Damage/Health | SERVER | Prevent cheating |
| Movement | CLIENT + validation | Responsiveness |
| Abilities | CLIENT + SERVER verify | Responsive feel |
| Visual effects | CLIENT only | No sync needed |

### 6.2 Script Structure for Network Objects
Use `NetworkBehaviour`, `NetworkVariable`, `[ServerRpc]`, and `[ClientRpc]`.
*   **ServerRpc:** Client requests action (e.g., `RequestPickupServerRpc`).
*   **ClientRpc:** Server notifies clients (e.g., `OnPickupClientRpc`).
*   **NetworkVariable:** Sync state (e.g., `_isPickedUp`).

### 6.3 Network Variables
Use `readPerm: Everyone`, `writePerm: Server` for most variables.

### 6.4 Network Physics (Critical for Eggs)
**WARNING:** AI often writes physics code that works locally but jitters in multiplayer.
*   **Use NetworkTransform:** Set Interpolation = true.
*   **Rigidbody:** KINEMATIC on clients, DYNAMIC on server only.
*   **Throwing Flow:**
    1.  Client requests throw.
    2.  Server validates and spawns object.
    3.  NetworkTransform syncs position to clients.

---

## 7. OBJECT POOLING STANDARDS

### 7.1 Why Pool Everything
Instantiate/Destroy causes garbage collection spikes (stutters). Pool everything: Eggs, Projectiles, VFX, Audio sources.

### 7.2 Pool Manager Interface
Use `Mgr_Pool.Spawn()` and `Mgr_Pool.Despawn()`.
**WARNING:** NEVER use `Destroy()` on pooled objects.

### 7.3 Poolable Object Contract
Implement `IPoolable` interface (`OnSpawn`, `OnDespawn`) to reset state.

---

## 8. INTERFACE DEFINITIONS

### 8.1 Core Interfaces
*   `IDamageable`: TakeDamage(amount, source)
*   `IDamageSource`: Damage, Type, Owner
*   `ICarryable`: Weight, OnPickedUp, OnDropped (for Eggs)
*   `IBirdCarrier`: CarryCapacity, TryPickup, DropAll
*   `IBlindable`: ApplyBlind (Egg Splat)
*   `IStunnable`: ApplyStun (Death Stare)
*   `IFearable`: ApplyFear (Potoo)

---

## 9. AUDIO INTEGRATION STANDARDS

### 9.1 Audio Middleware
Use **Wwise** for audio integration. All audio playback goes through Wwise events.

### 9.2 Audio Event Naming
Format: `[Category]_[Object]_[Action]` (e.g., `Bird_Hummingbird_Fly`, `Egg_Hatch_Normal`).

### 9.3 Audio Script Pattern
Audio scripts (`Bird_AudioHandler`) ONLY play sounds. They subscribe to events from logic scripts. They do NOT contain game logic.

### 9.4 Voice Chat Integration
Use Vivox/Photon Voice.
*   `Audio_VoiceChat_Proximity.cs`
*   `Audio_VoiceChat_Team.cs`

### 9.5 Parrot Voice Recording (Critical Implementation)
**Circular Audio Buffer Pattern:**
*   Record last 4 seconds of enemy voice into a circular RAM buffer.
*   **NEVER write to disk.**
*   Buffer is LOCAL to each client.
*   Parrot ability replays this buffer with pitch shift filter.

---

## 10. AI PROMPTING TEMPLATES
(Use these templates when asking AI to write code)

### 10.1 Standard Script Request
"Write [ScriptName].cs. PURPOSE: [One thing]. REQUIREMENTS: Single Responsibility, [SerializeField], UnityEvents, Interfaces. NETWORK: [Server/Client/Both]."

### 10.2 Bug Fix Request Template
"Fix bug in [ScriptName].cs. CURRENT BEHAVIOR: [X]. EXPECTED: [Y]. CONSTRAINTS: Don't change public API."

---

## 11. CODE REVIEW CHECKLIST
*   Single Responsibility?
*   Naming correct?
*   Null checks included?
*   No `Find()` or `GetComponent` in `Update()`?
*   Events unsubscribed in `OnDisable()`?
*   Network authority correct?
*   Using Object Pool?

---

## APPENDIX A: COMPLETE SCRIPT INDEX
(Refer to the full document for the list of core scripts, ability scripts, and managers.)

## APPENDIX B: AI PROMPT LIBRARY
(Refer to the full document for specific prompt examples like "New Bird Movement System" or "Egg Splat Blind Effect".)

---
**END OF DOCUMENT**
