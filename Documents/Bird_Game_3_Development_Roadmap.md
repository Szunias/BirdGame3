# Bird Game 3 - Development Roadmap & AI Tasks
**Based on:** GDD v2.4 & Technical Guidelines v1.1
**Goal:** Step-by-step implementation plan optimized for AI-assisted development.

---

## 🟢 PHASE 1: FOUNDATION & CORE LOOP (The "Greybox" Prototype)
*Goal: A playable multiplayer prototype where birds can fly, grab eggs, and score points.*

### Day 1: Project Setup & Networking Base
- [ ] **Task 1.1: Package Installation**
    - Install Netcode for GameObjects (NGO).
    - Install Unity Input System (New).
    - Install Unity Transport.
    - Install ParrelSync (for local multiplayer testing).
- [ ] **Task 1.2: Folder Structure**
    - Create folder hierarchy exactly as defined in *Technical Guidelines Section 3.1*.
    - Create Assembly Definitions (`BirdGame.Core`, `BirdGame.Network`, etc.).
- [ ] **Task 1.3: Input System Setup**
    - Create `BirdInputActions.inputactions`.
    - Generate C# class.
    - Write `Bird_Input.cs` wrapper (Events-based).
- [ ] **Task 1.4: Network Bootstrap**
    - Create Basic Network Manager scene.
    - Implement `Net_Bootstrap.cs` to auto-start Host/Client for testing.

### Day 2: The Bird Controller (Movement)
- [ ] **Task 2.1: Basic Movement Script**
    - Create `Bird_Movement.cs` (CharacterController based).
    - Implement Ground Movement (Walk/Sprint).
    - Reference *Technical Guidelines Section 10.2* for template.
- [ ] **Task 2.2: Unlimited Flight Logic**
    - Implement Flight State (Gravity management).
    - Implement Dive mechanic (Speed boost down).
    - Implement Hover (Zero gravity input).
- [ ] **Task 2.3: Network Synchronization**
    - Add `NetworkTransform` to Bird Prefab.
    - Configure interpolation for smooth movement.
    - Ensure `Bird_Movement` only runs logic on `IsOwner`.

### Day 3: The Egg System (Physics & Networking)
*Critical: This is the hardest part due to network physics.*
- [ ] **Task 3.1: Interfaces & Base**
    - Create `ICarryable` and `IBirdCarrier` interfaces.
    - Create `Egg_Base.cs`.
- [ ] **Task 3.2: Egg Physics & Pooling**
    - Create `Mgr_Pool.cs` (Singleton).
    - Implement `IPoolable` on Eggs.
    - Setup Egg Rigidbody (Kinematic on Client, Dynamic on Server).
- [ ] **Task 3.3: Pickup Logic (Server Auth)**
    - Write `Egg_Pickup_Net.cs`.
    - Implement `RequestPickupServerRpc`.
    - Implement `OnPickupClientRpc` (visual attachment to bird).

### Day 4: Throwing & Dropping
- [ ] **Task 4.1: Throw Calculation**
    - Implement Arc Calculation in `Bird_EggCarrier.cs`.
    - Create visual Trajectory Renderer.
- [ ] **Task 4.2: Throw Networking**
    - Implement `RequestThrowServerRpc(Vector3 velocity)`.
    - Server applies force to Rigidbody.
    - Client switches to Kinematic=false (simulated) or relies on NetworkTransform.
- [ ] **Task 4.3: Egg Splat (Visuals)**
    - Create `FX_EggSplat.cs` (UI Overlay).
    - Implement `IBlindable` interface.

### Day 5: The Nest & Scoring
- [ ] **Task 5.1: Nest Logic**
    - Create `Nest_Core.cs`.
    - Implement `Storage` list (NetworkList).
    - Implement Steal detection zone.
- [ ] **Task 5.2: Scoring Manager**
    - Create `Mgr_Score.cs` (NetworkBehaviour).
    - Implement `AddScore(Team, Amount)`.
    - Sync Score to all clients via `NetworkVariable`.
- [ ] **Task 5.3: Steal Mechanic**
    - Implement "Channeling" logic (Hold button to steal).
    - Trigger `OnStealSuccess` event.

---

## 🟡 PHASE 2: GAME LOOP & SYSTEMS
*Goal: Turning the mechanics into a structured match.*

### Day 6: Match Flow Manager
- [ ] **Task 6.1: State Machine**
    - Create `Mgr_Match.cs`.
    - Implement States: `Waiting`, `Scramble`, `Heist`, `Frenzy`, `End`.
    - Sync Match Timer via `NetworkVariable`.
- [ ] **Task 6.2: Phase Logic**
    - Implement Egg Spawning rules per phase.
    - Implement Respawn Time modification (5s vs 3s).

### Day 7: UI Implementation
- [ ] **Task 7.1: HUD System**
    - Create `UI_HUD.cs`.
    - Connect Score, Time, and Ammo (Egg count) to Managers.
- [ ] **Task 7.2: World UI**
    - Create Floating Damage Numbers / Score Popups (Pooled).
    - Create Nest Indicators (off-screen arrows).

### Day 8: Ability System Architecture
- [ ] **Task 8.1: Data Structures**
    - Create `Data_BirdStats` (ScriptableObject).
    - Create `Data_AbilityConfig` (ScriptableObject).
- [ ] **Task 8.2: Ability Controller**
    - Create `Bird_AbilityController.cs`.
    - Implement Cooldown management.
    - Implement `Ability_Base` abstract class.

### Day 9: Implementing "The Starter Bird" (Toucan)
- [ ] **Task 9.1: Beak Slam**
    - Write `Ability_BeakSlam.cs`.
    - Implement `IStunnable` interface.
- [ ] **Task 9.2: Fruit Toss**
    - Write `Ability_FruitToss.cs` (Projectile logic).
    - Use Object Pool for fruits.

### Day 10: Polish & Feedback
- [ ] **Task 10.1: Audio Manager (Wwise Prep)**
    - Create `Mgr_Audio.cs` wrapper.
    - Create `Bird_AudioHandler.cs` (Event listener).
- [ ] **Task 10.2: Visual Feedback**
    - Add "Loot Beam" shader/script for dropped eggs.
    - Add screen shake on impact.

---

## 🔴 PHASE 3: CONTENT SCALING (AI Heavy)
*Goal: Fleshing out the roster and map.*

### Day 11-12: Bird Roster Implementation (Batch 1)
- [ ] **Hummingbird:** `Ability_Zip`, `Ability_NectarRush`.
- [ ] **Shoebill:** `Ability_DeathStare` (Raycast logic), `Ability_Intimidate`.
- [ ] **Penguin:** `Ability_IceWall` (Mesh generation), `Ability_BellySlide`.

### Day 13-14: Bird Roster Implementation (Batch 2)
- [ ] **Owl:** `Ability_NightVision` (Shader replacement/Outline), `Passive_Silent`.
- [ ] **Pelican:** `Ability_Scoop` (Complex parenting logic - "No Russian Dolls" rule).
- [ ] **Cassowary:** `Ability_MurderKick` (High force physics), `Ability_Rampage`.

### Day 15: Map Mechanics
- [ ] **Jump Pads:** Create `Map_JumpPad.cs` (Physics launch).
- [ ] **Biomes:** Create Data assets for different ground friction/visuals.
- [ ] **Dynamic Events:** Implement `Mgr_Events.cs` (Worm Rain, Blackout).

### Day 16: Voice Chat & Anti-Toxicity
- [ ] **Voice Integration:** Set up Vivox/Photon Voice packages.
- [ ] **Parrot Recorder:** Implement Circular Buffer for `Ability_Chatter`.
- [ ] **Safety:** Implement Mute/Report UI.

---

## 🤖 AI PROMPTING GUIDE
*How to use this roadmap with your AI Assistant.*

**1. Context First:**
Always start a session by asking the AI to read `Bird_Game_3_Technical_Guidelines.md` to refresh context on naming conventions and networking rules.

**2. Atomic Requests:**
Don't ask: *"Make the bird movement."*
Do ask: *"Write `Bird_Movement.cs` based on Task 2.1 and 2.2. It must support unlimited flight, use CharacterController, and read speed values from a ScriptableObject. Ensure network synchronization is handled via NetworkTransform, so only the owner runs the logic."*

**3. Code Review Mode:**
After the AI writes a script, ask: *"Review this code against the Technical Guidelines. Does it use UnityEvents? Is it Server Authoritative where needed?"*

**4. Error Handling:**
If a script fails, paste the specific error *and* the relevant section of the script. Ask the AI to *only* fix that function, not rewrite the whole file unless necessary.
