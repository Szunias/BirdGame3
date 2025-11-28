# BIRD GAME 3: THE GREAT EGG HEIST - GAME DESIGN DOCUMENT
**Version:** 2.4
**Genre:** Competitive Multiplayer Action
**Platform:** PC (Unity)
**Players:** 8 (4 teams of 2)
**Match Duration:** 15 minutes

---

## TABLE OF CONTENTS
1. Executive Summary
2. Core Concept & Unique Selling Points
3. Gameplay Overview
   * 3B. Balance Mechanics (Anti-Camp, Bounty, Lonely Wolf, Events)
4. Match Structure & Flow (15-minute format)
5. Scoring System
6. Bird Roster & Abilities (12 Birds)
7. Egg System
8. Nest Mechanics
9. Map Design & Biomes (incl. Jump Pads)
10. Combat System
11. Audio Design Guidelines
12. UI/UX Design
13. Progression & Unlocks
    * 13B. Anti-Toxicity & Player Safety
14. Technical Requirements (incl. Voice Chat)
* Appendix A: Quick Reference Tables
* Appendix A.4: Economy Values Reference (NEW)

---

## 1. EXECUTIVE SUMMARY

### 1.1 Elevator Pitch
**Bird Game 3: The Great Egg Heist** is a fast-paced competitive multiplayer game where teams of unique birds compete to steal eggs from each other's nests. Combining the hero-based gameplay of *Overwatch* with the chaotic fun of *Fall Guys*, players must balance offensive raids, defensive positioning, and strategic egg management across 15-minute matches filled with constant action and dopamine-driven rewards.

### 1.2 Core Fantasy
Players experience the thrill of being a cunning bird thief - sneaking into enemy territory, snatching precious eggs, and making daring escapes while teammates provide cover. Every match delivers moments of tension, triumph, and hilarious chaos.

### 1.3 Target Audience
*   **Primary:** Casual competitive players (ages 13-25) who enjoy quick-session multiplayer games.
*   **Secondary:** Fans of hero shooters looking for a lighter, more accessible experience.
*   **Tertiary:** Content creators seeking clip-worthy moments and streaming potential.

### 1.4 Key Pillars
*   **STEAL:** The core loop centers on the satisfaction of taking what belongs to others.
*   **ESCAPE:** Heart-pounding chases create memorable moments.
*   **PROTECT:** Defending your nest creates strategic depth.
*   **EVOLVE:** Constant progression and rewards keep players engaged.

---

## 2. CORE CONCEPT & UNIQUE SELLING POINTS

### 2.1 What Makes This Game Different
While the market has hero shooters (*Overwatch*, *Valorant*) and party games (*Fall Guys*, *Gang Beasts*), Bird Game 3 occupies a unique space: a hero-based game where the primary objective is **theft rather than elimination**. This creates fundamentally different gameplay patterns and emotional experiences.

### 2.2 The Soft Asymmetry System
Unlike hard role locks, every bird CAN do everything - steal, fight, defend. However, each bird has clear specializations that make them better at certain roles. This creates team composition strategy without forcing players into roles they don't enjoy.

**Role Spectrum:**
Birds exist on a spectrum from Thief-specialized to Brawler-specialized:
*   **THIEF-LEANING:** Faster, quicker steal time, lower HP, escape-focused abilities.
*   **BALANCED:** Average stats, versatile abilities, jack-of-all-trades.
*   **DISRUPTOR:** Chaos-focused, neither pure thief nor brawler, annoyance specialists.
*   **BRAWLER-LEANING:** Slower, longer steal time, higher HP, combat-focused abilities.
*   **TANK:** Slowest, longest steal time, highest HP/damage, area denial specialists.

### 2.3 Continuous Reward Loop
Traditional objective games have delayed gratification - you work toward a goal and get rewarded at the end. Bird Game 3 provides constant micro-rewards throughout the match: points for every egg picked up, bonus points for steals, streak rewards, and satisfying audio/visual feedback for every action.

---

## 3. GAMEPLAY OVERVIEW

### 3.1 Basic Loop
The fundamental gameplay loop is: **FIND eggs -> GRAB eggs -> RETURN to nest -> HATCH for points.**
However, the most rewarding path is: **INFILTRATE enemy nest -> STEAL their eggs -> ESCAPE with loot -> HATCH stolen goods for bonus points.**

### 3.2 Movement & Flight
All birds have full 3D movement with **UNLIMITED flight capabilities** (except Cassowary who is ground-bound but compensated with superior ground speed). Flight is a core identity of being a bird - restricting it would feel wrong. Balance comes from other mechanics, not flight limitations.

**Movement Stats (Base Values):**
*   **Ground Speed:** 100% (varies by bird: 80-130%)
*   **Flight Speed:** 110% of ground speed (flying is slightly faster)
*   **Vertical Climb Speed:** 80% of ground speed (ascending is slower)
*   **Dive Speed:** 150% of ground speed (diving is fast)

**Flight is UNLIMITED - no stamina meter.**

**Why Unlimited Flight Works:**
*   Nests are enclosed structures - must land to interact.
*   Stealing requires grounded channeling - vulnerable moment.
*   Carrying eggs slows ALL movement including flight.
*   Many abilities are ground-targeted or require landing.
*   Cassowary's ground dominance creates no-fly zones.

**High Altitude Turbulence (Anti-Sky-Camp):**
To prevent teams from camping at maximum altitude with eggs, turbulence zones exist above the playable area:
*   **Altitude limit:** 30 meters above highest terrain point.
*   **Above limit:** Strong wind pushes players downward.
*   Camera shake makes aiming difficult.
*   **Audio:** Howling wind warning.
*   Not a hard ceiling - you CAN fly there, but it's miserable.

### 3.3 Carrying Eggs
Players can carry up to 3 eggs simultaneously (varies by bird). Carrying eggs affects movement:
*   **1 egg:** 100% speed (no penalty)
*   **2 eggs:** 85% speed
*   **3 eggs:** 70% speed
*   **Death while carrying:** ALL carried eggs drop on the ground.

### 3.4 Team Dynamics
Teams of 2 must coordinate roles dynamically. Common strategies include:
*   **Split Push:** Both players raid different enemy nests simultaneously.
*   **Buddy System:** One steals while the other provides combat support.
*   **Fortress:** One defends nest while the other raids.
*   **Bait and Switch:** One draws attention while the other sneaks.

### 3.5 Voice Chat System
Voice chat is a **CORE FEATURE**, not optional. Communication between teammates and proximity-based voice with enemies creates emergent gameplay moments that are essential for streaming and content creation.

**Team Voice (Always On):**
*   Teammates hear each other at full volume regardless of distance.
*   Clear, crisp audio - this is your tactical channel.
*   Push-to-mute option for coughing/background noise.

**Proximity Voice (The Secret Sauce):**
*   Enemies within 15 meters can hear you talk.
*   Volume scales with distance (closer = louder).
*   Creates incredible moments: hearing panic, trash talk, sneaky whispers.
*   **Owl's 'Dead Silent' passive MUTES proximity voice** - true stealth.
*   **Potoo's 'Nightmare Screech' is proximity-broadcasted** for maximum jumpscare.

**Voice Indicators:**
*   Small speaker icon when teammate is talking.
*   Directional indicator when enemy voice is heard nearby.
*   No indicator for WHO is talking among enemies - adds uncertainty.

**Content Creator Features:**
*   'Streamer Mode' - replaces enemy names with bird types.
*   All voice chat can be captured in replays.
*   Post-match 'Best Moments' auto-clips loud voice peaks.

---

## 3B. BALANCE MECHANICS

### 3B.1 Anti-Camping System
To prevent defensive 'turtle' strategies from making the game boring, teams are punished for staying near their nest too long.

**Lazy Bird Debuff:**
*   **Triggers when:** Both teammates within 25m of own nest for 45+ seconds.
*   **Effect:** -25% damage dealt, nest stops producing eggs.
*   **Visual:** Birds start yawning, feathers look dull.
*   **Reset:** Either player leaves radius for 10+ seconds.
*   **Exception:** Does not trigger during Frenzy phase.

### 3B.2 Bounty System
The leading team becomes a high-value target, creating comeback opportunities and preventing runaway victories.

**How Bounty Works:**
*   Team with highest score gets 'BOUNTY' marker (visible to all).
*   Bounty updates every 90 seconds to current leader (slowed for 15 min matches).
*   **Stealing from Bounty nest:** +60 points (instead of +40).
*   **Killing Bounty player:** +20 points (instead of +10).
*   Bounty team has small golden crown icon above heads.
*   Creates natural target - everyone hunts the leader.

**Bounty Activity Requirement (Anti-Passive Play):**
*   Bounty team must score points every 60 seconds.
*   If inactive for 60s: Position revealed through walls (**WALLHACK**) to all enemies.
*   Wallhack lasts until Bounty team scores any points.
*   Prevents 'hide in corner with lead' strategy.
*   Forces leaders to stay engaged with the game.

### 3B.2B Lonely Wolf System (Disconnect Protection)
In a 15-minute match, losing your teammate to disconnect would be devastating. The Lonely Wolf system gives solo players a fighting chance.

**Activation:**
*   Triggers when teammate disconnects for 30+ seconds.
*   Also triggers if teammate is AFK-kicked.
*   Does NOT trigger if teammate reconnects within 30 seconds.

**Lonely Wolf Buff:**
*   +25% damage dealt.
*   +20% movement speed.
*   2x faster egg stealing (0.5x steal time).
*   Respawn time reduced to 3 seconds (even in Heist phase).
*   **Visual:** Faint wolf-howl aura effect.

**Design Intent:**
*   Not meant to make 1v2 easy - just possible.
*   Prevents instant 'game over' feeling.
*   Encourages remaining player to keep fighting.
*   Creates underdog narrative potential for clips.

### 3B.3 Egg Throwing
A new core mechanic that adds depth to escapes, teamwork, and even combat.

**Throw Mechanics:**
*   **New input:** THROW EGG (separate from abilities).
*   Can throw one egg at a time while carrying multiple.
*   **Throw range:** 15 meters, arc trajectory.
*   **Teammate catch:** Auto-catch if within 5m of landing spot.
*   **Enemy hit:** 15 damage + EGG SPLAT (yolk blinds screen for 2s) - devastating escape tool.
*   **Miss:** Egg lands on ground with LOOT BEAM, anyone can pick up.

**Egg Splat Effect:**
*   Yellow yolk splatter covers 60% of enemy screen.
*   Lasts 2 seconds, fades gradually.
*   **Audio:** Gross splat sound + muffled hearing.
*   Much stronger than simple slow - real escape potential.

**Strategic Uses:**
*   Pass to faster teammate for escape.
*   Throw over walls to waiting partner.
*   Desperation throw at chaser to slow them.
*   Fake throw to bait enemy reaction.

### 3B.4 Dynamic Events
TWO random events trigger during the Heist phase (approximately 4:00-5:00 and 7:00-8:00). Events last 30-60 seconds and provide mid-match excitement to break routine in longer matches.

**Event Pool:**
*   **WORM RAIN:** 30 neutral eggs fall from sky over 25 seconds across whole map.
*   **BLACKOUT:** Visibility reduced to 15m for all players for 45 seconds (stealth birds thrive).
*   **WINDSTORM:** Strong wind pushes all flying birds in random direction, changes every 10s.
*   **EGG FEVER:** All hatch times reduced by 60% for 60 seconds.
*   **GOLDEN HOUR:** All points earned are 1.5x for 45 seconds.
*   **PREDATOR ALERT:** One random team's nest location revealed to all for 30 seconds.
*   Event announced 10 seconds before activation with dramatic audio/visual.
*   Second event is always different from first (no repeats).

### 3B.5 The Golden Egg (Frenzy Finale)
At 12:00 (one minute into Frenzy, three minutes before match end), a massive Golden Egg spawns at map center. This creates an epic 3-minute war for its massive point value.

**Golden Egg Properties:**
*   Spawns at exact center of map with beam of light.
*   **HEAVY:** Carrier moves at 40% speed, cannot fly.
*   **TWO-BIRD CARRY:** If both teammates carry together, 70% speed.
*   **Value:** +250 points upon delivery to any nest.
*   Cannot be hatched - points awarded on delivery.
*   Drops on death, 5-second pickup immunity (prevents instant re-grab).

**Golden Egg Explosion (TIME'S UP):**
*   If Golden Egg is not delivered by 15:00, it doesn't just disappear - it **EXPLODES**.
*   **MEGA KNOCKBACK:** All players within 50m launched into the air.
*   **RAGDOLL:** Everyone enters ragdoll physics for 3 seconds.
*   **CAMERA:** Cinematic slow-mo captures the chaos.
*   **AUDIO:** Massive explosion followed by comedic 'TIME'S UP!' announcer.
*   **NO POINTS:** Explosion awards zero points to anyone.
*   Creates perfect viral clip moment at end of close matches.

**Why This Works:**
*   Forces final confrontation at known location.
*   Comeback potential: +250 can swing games.
*   Requires team coordination (two-bird carry).
*   Creates highlight-reel moments.

---

## 4. MATCH STRUCTURE & FLOW

### 4.1 Match Length
Every match lasts exactly **15 minutes (900 seconds)**. This extended duration allows for strategic depth, elaborate heist plans, meaningful comebacks, and epic final battles. The longer format requires careful economy tuning to prevent stagnation.

### 4.2 Match Phases

**Phase 1: SCRAMBLE (0:00 - 3:00)**
Three minutes of initial chaos. Players spawn in center, grab eggs, establish territory, and set up defenses. Extended duration allows for first skirmishes and strategic positioning before the main heist phase.
*   **Eggs in center:** 60-70 Normal Eggs, 5 Shiny Eggs.
*   **Respawn during Scramble:** Instant (no delay).
*   **Strategy:** Grab eggs, scout enemy nests, establish defensive positions, first contact with enemies.

**Phase 2: HEIST (3:00 - 11:00)**
The 8-minute heart of the game. This is where complex strategies unfold. Owl players can execute multi-step infiltrations. Teams coordinate elaborate heists. Death has real consequences with 5-second respawn. Two Dynamic Events occur during this phase to break routine.
*   **Neutral egg spawn rate:** 1 egg per 25 seconds per biome (slowed).
*   **Nest egg production:** 1 egg per 60 seconds per nest (forces stealing).
*   **Respawn time:** 5 seconds (death matters).
*   **Dynamic Events:** TWO events trigger (around 4:00-5:00 and 7:00-8:00).
*   **Shiny Egg respawn:** Every 3.5 minutes.
*   **Strategy:** Execute elaborate heists, coordinate team attacks, punish enemy deaths.

**Phase 3: FRENZY (11:00 - 15:00)**
Four-minute finale. All nest contents visible through walls. Stealing rewards doubled. Golden Egg spawns at 12:00, giving 3 full minutes of war over its massive point value. Faster respawns enable constant action.
*   All nest contents visible globally (wallhack on eggs).
*   **Steal point bonus:** 2x multiplier.
*   **Respawn time:** 3 seconds (back to fast).
*   **Golden Egg spawns at 12:00** (3 minutes before end).
*   **Strategy:** All-out war, protect lead or execute desperate comeback.

### 4.3 Victory Condition
Team with the highest score at 15:00 wins. Ties are broken by: 1) Most successful steals, 2) Most eggs hatched, 3) Sudden death overtime (first point wins).

### 4.4 Pacing Safeguards
15 minutes requires careful pacing to prevent mid-game boredom:
*   TWO Dynamic Events in Heist phase break routine.
*   Slow egg production forces constant aggression.
*   5-second respawn makes death meaningful.
*   Bounty system keeps leading team under pressure.
*   Shiny Eggs as mini-objectives throughout.

---

## 5. SCORING SYSTEM

### 5.1 Design Philosophy
Points should flow constantly. Players should feel rewarded for EVERY action, not just major achievements. The scoring system is designed to trigger dopamine responses frequently while still rewarding skilled play with significantly higher scores.

### 5.2 Point Values
| ACTION | POINTS | NOTES |
| :--- | :--- | :--- |
| Pick up neutral egg | +5 | Small pop sound |
| Deliver egg to nest | +15 | Satisfying cha-ching |
| Hatch egg (Normal) | +25 | Egg crack + chick chirp |
| Hatch egg (Shiny) | +75 | Golden sparkle effect |
| **STEAL from enemy nest** | **+40** | **LOUD steal sound, enemy alert** |
| Eliminate enemy | +10 | Combat reward |
| Eliminate egg carrier | +20 | Bonus for stopping thief |
| Assist (damage within 3s) | +5 | Teamwork reward |
| Revenge kill | +15 | Kill your killer |

### 5.3 Streak Bonuses
Consecutive successful steals without dying grant escalating bonuses:
*   **2 steals:** "Double Trouble" +20 bonus.
*   **3 steals:** "Stealing Spree" +40 bonus, announcer callout.
*   **4 steals:** "Master Thief" +60 bonus, special visual effect.
*   **5+ steals:** "Legendary Heist" +100 bonus, global announcement.

### 5.4 Frenzy Phase Modifiers
During the final 2 minutes:
*   **All steal points:** 2x multiplier.
*   **Hatch points:** 1.5x multiplier.
*   **Elimination points:** Unchanged (prevents camping).

---

## 6. BIRD ROSTER & ABILITIES

### 6.1 Roster Overview
The initial roster contains 12 unique birds spanning the role spectrum. Each bird has distinct stats, one passive ability, and two active abilities (one on short cooldown, one on long cooldown).

### 6.1B Team Composition Rules
To prevent degenerate strategies and ensure variety:
*   **UNIQUE BIRD PER TEAM:** Each team must have two DIFFERENT birds.
    *   No 2x Shoebill (permanent stun lock with alternating Death Stares).
    *   No 2x Penguin (infinite ice wall fortress).
    *   No 2x Potoo (overlapping fear = no counterplay).
*   This forces strategic team composition decisions.
*   Mirror matches between teams ARE allowed (Team A and Team B can both have Eagle).

### 6.2 Stat Definitions
*   **HP:** Health points (death at 0, base is 100).
*   **Speed:** Movement speed percentage (100% is baseline).
*   **Damage:** Attack damage modifier (100% is baseline).
*   **Steal Time:** Seconds to grab egg from enemy nest (base is 1.0s).
*   **Carry Capacity:** Maximum eggs that can be carried.

### 6.3 HUMMINGBIRD (Thief Tier)
**Identity:** The Ultimate Escape Artist
The fastest bird in the game, Hummingbird excels at quick in-and-out raids. Fragile but nearly impossible to catch when played well.

*   **HP:** 60
*   **Speed:** 130%
*   **Steal Time:** 0.5s
*   **Carry:** 2 eggs

**Passive: Hover**
Can remain stationary in mid-air without consuming flight stamina. Perfect for scouting and waiting for opportunities.

**Ability 1: Zip (8s cooldown)**
Three rapid micro-dashes in quick succession. Each dash covers 5 meters and can be aimed independently. Excellent for juking pursuers or navigating obstacles.

**Ability 2: Nectar Rush (20s cooldown)**
Become invisible for 4 seconds while moving. Breaking invisibility (attacking or taking damage) ends the effect early. Leaves subtle shimmer effect visible to attentive players.

**Playstyle**
Hit and run. Get in, grab one or two eggs, zip out before anyone can react. Avoid direct combat at all costs - you will lose. Your value is in being annoying and impossible to pin down.

### 6.4 OWL (Thief Tier)
**Identity:** The Silent Predator
Owl trades raw speed for stealth and information gathering. The premier infiltrator who excels at undetected heists.

*   **HP:** 70
*   **Speed:** 115%
*   **Steal Time:** 0.6s
*   **Carry:** 3 eggs

**Passive: Dead Silent**
Movement produces no sound. Footsteps, wing flaps, and landing sounds are completely eliminated. Enemy audio cues will not trigger from Owl's movement. ADDITIONALLY: Owl's proximity voice chat is MUTED - enemies cannot hear Owl talking nearby. True stealth.

**Ability 1: Silent Swoop (10s cooldown)**
A silent dash that leaves no trail. Unlike other movement abilities, this dash produces no visual or audio indicators for enemies. Covers 8 meters.

**Ability 2: Night Vision (25s cooldown)**
For 4 seconds, see all enemies through walls with red outlines. Also reveals how many eggs each enemy is carrying. Information is shared with teammate.

**Playstyle**
Information is power. Use Night Vision to identify vulnerable targets, then Silent Swoop into position for undetected steals. Communicate enemy positions to your teammate constantly.

### 6.5 SEAGULL (Disruptor Tier)
**Identity:** The Annoying Pest
Seagull exists to make everyone's life difficult. Not the best at stealing or fighting, but exceptional at ruining enemy plans.

*   **HP:** 65
*   **Speed:** 120%
*   **Steal Time:** 0.7s
*   **Carry:** 3 eggs

**Passive: Pest**
Enemy abilities have +20% longer cooldowns when Seagull is within 15 meters. This effect is not visible to enemies - they'll just feel like their abilities are "off."

**Ability 1: SCREAM (6s cooldown)**
Emit an ear-piercing screech. All enemies within 10 meters have their vision blurred and audio distorted for 2 seconds. Does not deal damage but creates chaos.

**Ability 2: Chip Theft (15s cooldown)**
The signature move - force an enemy to DROP one egg they're carrying. Requires being within 4 meters. The egg pops out in a HIGH ARC toward Seagull (like coins in Sonic), giving Seagull priority to catch it mid-air. If missed, egg lands with LOOT BEAM (visible light pillar) and MAGNETIC PICKUP radius (3m auto-collect). Cannot be used on enemies at their own nest. Server-authoritative to prevent netcode exploits.

**Playstyle**
Be everywhere. Be annoying. Your job isn't to top the scoreboard - it's to make sure the enemy team can't execute their plans. Chip Theft during an enemy escape is devastatingly satisfying.

### 6.6 POTOO / OPIUM BIRD (Disruptor Tier)
**Identity:** The Nightmare
Potoo is unsettling. Its abilities focus on fear, ambush, and psychological warfare. The ultimate jumpscare bird.

*   **HP:** 75
*   **Speed:** 100%
*   **Steal Time:** 0.8s
*   **Carry:** 3 eggs

**Passive: Uncanny**
Does not appear on enemy minimaps. Ever. Enemies must use actual vision to track Potoo's location.

**Ability 1: Freeze (5s cooldown)**
Stand completely still for 1 second to become invisible. Remain invisible as long as you don't move. Taking damage or using abilities breaks the invisibility. Perfect for ambushes near enemy nests.

**Ability 2: Nightmare Screech (22s cooldown)**
Unleash a terrifying scream. All enemies within 12 meters are feared for 1.5 seconds (forced to move away from Potoo) and DROP ALL EGGS they're carrying. The screech is BROADCAST through proximity voice at maximum volume - enemies hear it through their headphones for maximum jumpscare effect. The ultimate heist interrupter.

**Playstyle**
Ambush predator. Hide near enemy routes, wait for egg carriers, then Nightmare Screech to steal their haul. Psychological warfare - enemies will become paranoid about every corner.

### 6.7 TOUCAN (Balanced Tier)
**Identity:** The Reliable All-Rounder
Toucan does everything reasonably well. The recommended starter bird for new players, with intuitive abilities and forgiving stats.

*   **HP:** 85
*   **Speed:** 105%
*   **Steal Time:** 0.8s
*   **Carry:** 3 eggs

**Passive: Big Ears**
Hears enemy footsteps and wing flaps from 50% greater distance. Audio cues are also slightly louder. Helps with awareness and detecting incoming threats.

**Ability 1: Beak Slam (8s cooldown)**
Melee attack that stuns the target for 1 second. Deals moderate damage (30). Simple but effective for both offense and defense.

**Ability 2: Fruit Toss (12s cooldown)**
Throw a fruit projectile that explodes on impact, creating a 5-meter area that slows enemies by 40% for 3 seconds. Good for cutting off escape routes or covering your own retreat.

**Playstyle**
Adaptable. Can fill whatever role the team needs. Use Beak Slam to interrupt stealers at your nest, or Fruit Toss to escape after a successful heist. Jack of all trades, master of none.

### 6.8 PELICAN (Balanced Tier)
**Identity:** The Hauler
Pelican's giant beak isn't just for show - it can carry more eggs than anyone else. The high-risk, high-reward transporter.

*   **HP:** 90
*   **Speed:** 95%
*   **Steal Time:** 1.0s
*   **Carry:** **5 eggs (!)**

**Passive: Deep Pouch**
Base carrying capacity is 5 eggs instead of 3. This is Pelican's defining trait - one successful heist can swing a game.

**Ability 1: Scoop (10s cooldown)**
Grab an enemy OR ALLY in your beak, carry them for up to 2 seconds, then spit them out in a direction of your choice. Enemies cannot act while scooped. ALLIES keep their eggs and can be transported to safety - the ultimate 'Pelican Uber' play. **ANTI-GRIEF:** Allies can press JUMP to instantly eject with upward boost, preventing toxic teammates from throwing them off map.
*   **Scoop Edge Cases (Programmer Notes):**
    *   **NO RUSSIAN DOLLS:** Cannot scoop a Pelican who already has someone in their beak.
    *   **HEAVY BURDEN:** Passenger's egg weight ADDS to your slowdown.
    *   Example: Pelican (5 eggs) scoops ally with 3 eggs = slowdown as if carrying 8 eggs.
    *   This prevents 'Pelican Tank' exploit of fast transport with massive egg haul.
    *   Cannot scoop enemies who are currently invulnerable or in Rampage.

**Ability 2: Fish Slap (14s cooldown)**
Slap enemies with a large fish, dealing moderate damage (25) and applying knockback plus 30% slow for 2 seconds. Good for creating distance when escaping with a full pouch.

**Playstyle**
Go big or go home. A full 5-egg haul is game-changing, but you're slow and vulnerable while loaded. Coordinate with your teammate for protection during escapes. Pro tip: Scoop your Hummingbird teammate who has 2 eggs, transport them to safety while they keep their haul.

### 6.9 PARROT (Balanced Tier)
**Identity:** The Trickster
Parrot uses deception and mimicry to confuse enemies. High skill ceiling with abilities that reward creativity.

*   **HP:** 80
*   **Speed:** 110%
*   **Steal Time:** 0.8s
*   **Carry:** 3 eggs

**Passive: Colorful**
Enemies who see Parrot (line of sight) become marked for your team for 5 seconds. Turning the tables on scouting - they see you, but now your whole team sees them.

**Ability 1: Mimic (12s cooldown)**
Copy the last ability used by any enemy within 20 meters. The copied ability has 80% effectiveness. Resets when a new ability is copied. High skill expression potential.

**Ability 2: Chatter (18s cooldown)**
Create an audio decoy at target location within 30 meters. The decoy plays the last 3 seconds of ACTUAL enemy voice chat captured from nearby enemies, but with **AUDIO FILTER** applied (pitch-shifted 'helium voice' or 'deep fried bass' effect). This makes the voice recognizable but unintelligible - preventing toxicity transmission while keeping the comedic effect. If no voice was captured, plays generic bird sounds. Players can disable 'Allow Enemy Voice Recording' in settings (replaces with procedural bird babble).

**Playstyle**
Mind games. Use Chatter to create distractions, then strike from unexpected angles. Mimic rewards game knowledge - knowing what ability to copy and when is the difference between good and great Parrot players.

### 6.10 EAGLE (Brawler Tier)
**Identity:** The Aerial Assassin
Eagle dominates from above. High damage dive attacks make it deadly against thieves trying to escape, but slower steal times mean it's better at hunting than stealing.

*   **HP:** 100
*   **Speed:** 100%
*   **Steal Time:** 1.2s
*   **Carry:** 3 eggs

**Passive: Eagle Eye**
Vision range is increased by 30%. Can spot enemies from further away, especially useful when patrolling from high altitude.

**Ability 1: Dive Bomb (10s cooldown)**
Plunge downward at high speed, dealing massive damage (50) to the first enemy hit and applying knockdown for 1 second. Must be at least 10 meters above target. Signature assassination tool.

**Ability 2: Talon Grab (16s cooldown)**
Grab an enemy and carry them upward for 3 seconds, then release. Enemies take fall damage based on height. Cannot use other abilities while carrying. Ultimate punishment for caught thieves.

**Playstyle**
Sky predator. Patrol high above, spot fleeing thieves, dive bomb to intercept. You're the team's closer - let your partner steal while you ensure enemies can't escape with your eggs.

### 6.11 SHOEBILL (Brawler Tier)
**Identity:** The Intimidator
Shoebill doesn't need to chase - its terrifying presence makes enemies not want to come near. The ultimate nest guardian.

*   **HP:** 110
*   **Speed:** 90%
*   **Steal Time:** 1.3s
*   **Carry:** 3 eggs

**Passive: Menacing Aura**
Enemies within 10 meters deal 10% less damage. The closer they are, the more they feel the pressure.

**Ability 1: Death Stare (9s cooldown)**
Lock eyes with a target for 0.5 seconds, then stun them for 1.5 seconds. Requires maintaining line of sight during the lock-on. Iconic Shoebill behavior translated into gameplay.

**Ability 2: Intimidating Presence (20s cooldown)**
Activate an aura for 5 seconds. All enemies within 15 meters are slowed by 30% and have their ability cooldowns frozen (abilities already on cooldown stay on cooldown, abilities off cooldown cannot be used).

**Playstyle**
Stand your ground. Position near your nest and dare enemies to approach. Death Stare fleeing thieves to stop escapes. Your mere presence changes how enemies play around your territory.

### 6.12 VULTURE (Brawler Tier)
**Identity:** The Finisher
Vulture cleans up wounded enemies. The longer a fight goes, the stronger Vulture becomes.

*   **HP:** 105
*   **Speed:** 95%
*   **Steal Time:** 1.2s
*   **Carry:** 3 eggs

**Passive: Scavenger**
Heal 25 HP on every elimination or assist. Additionally, dealing damage to enemies below 30% HP deals 25% bonus damage.

**Ability 1: Carrion Dive (8s cooldown)**
Dash toward an enemy. Range is 10 meters normally, but extends to 20 meters if the target is below 30% HP. Deals 20 damage (25 against low HP targets).

**Ability 2: Death Circle (18s cooldown)**
Create a 10-meter zone that lasts 4 seconds. Enemies inside take 10 damage per second and are revealed through walls. Excellent for area denial around nests.

**Playstyle**
Patient predator. Let teammates soften targets, then swoop in for the kill. Your sustain from Scavenger passive lets you take prolonged fights other birds would lose.

### 6.13 CASSOWARY (Tank Tier)
**Identity:** The Grounded Juggernaut
Cassowary cannot fly. This massive limitation is offset by being the most dangerous bird in direct combat. If Cassowary catches you, you die.

*   **HP:** 140
*   **Speed:** 85%
*   **Steal Time:** 1.5s
*   **Carry:** 3 eggs

**Passive: Grounded**
Cannot fly. However, immune to knockback effects and stun durations are reduced by 50%. Also the fastest ground runner when sprinting (110% ground speed during sprint). CAN USE JUMP PADS scattered across maps - these launch Cassowary high into the air for epic aerial Murder Kicks.

**Ability 1: Murder Kick (7s cooldown)**
A devastating kick dealing 60 damage with massive knockback. If the target has less than 40 HP, this ability instantly kills them regardless of remaining health. The most feared ability in the game.

**Ability 2: Rampage (24s cooldown)**
For 4 seconds, gain 100% damage immunity and 40% movement speed. Cannot be stopped by any crowd control. Use to force your way into fortified positions or guarantee an escape.

**Playstyle**
Ground control. You can't chase flying birds, but they have to land eventually. Control chokepoints and ground routes. One Murder Kick can turn a fight instantly.

### 6.14 PENGUIN (Tank Tier)
**Identity:** The Zone Controller
Penguin shapes the battlefield itself. Ice paths, walls, and area control make Penguin invaluable for strategic teams.

*   **HP:** 120
*   **Speed:** 80% / 140%* (80% base speed, 140% when sliding on ice)
*   **Steal Time:** 1.3s
*   **Carry:** 3 eggs

**Passive: Cold Blooded**
While on ice surfaces (natural or created), gain 25% damage resistance in addition to the speed bonus. Penguin is strongest in its element.

**Ability 1: Belly Slide (6s cooldown)**
Enter a slide that lasts 3 seconds or until cancelled. While sliding: move at 140% speed, leave an ice trail behind you, and enemies you hit take 15 damage and are knocked aside. The ice trail persists for 8 seconds.

**Ability 2: Ice Wall (16s cooldown)**
Create a wall of ice (5 meters wide, 3 meters tall) that blocks movement and projectiles. Wall has 150 HP and lasts 6 seconds or until destroyed. Can be placed to block nest entrances or cut off escape routes.

**Playstyle**
Terrain manipulation. Create ice paths that benefit you and hinder enemies. Block key passages with Ice Wall. You're slow normally but blazing fast on your own ice trails.

---

## 7. EGG SYSTEM

### 7.1 Design Philosophy
Eggs are the currency of the game - they represent points, power, and prestige. The egg system is intentionally simple with only two types, keeping focus on the core heist gameplay rather than complex resource management.

### 7.2 Egg Types

**Normal Eggs (White/Cream colored)**
*   **Spawn:** Neutral locations across map, nest production.
*   **Hatch Time:** 5 seconds.
*   **Hatch Reward:** +25 points, small buff (random).
*   **Buffs include:** 10% speed for 30s, 15 HP heal, 2s ability cooldown reduction.

**Shiny Eggs (Golden with sparkle effect)**
*   **Spawn:** 5 at match start in center, respawn every 3.5 minutes during Heist.
*   **Hatch Time:** 10 seconds.
*   **Hatch Reward:** +75 points + SUPER CHARGE MODE.

**Super Charge Mode (Shiny Egg Buff)**
When you hatch a Shiny Egg, you become **SUPERCHARGED** for 15 seconds (extended for 15-min matches):
*   **VISUAL:** Your bird glows golden, 20% larger model size.
*   **AUDIO:** Epic power-up music sting, crackling energy sounds.
*   **ABILITY 1:** No cooldown - spam infinitely during Super Charge.
*   **SPEED:** +15% movement speed.
*   **PRESENCE:** Enemies see 'SUPER CHARGED' warning when you're nearby.
*   This creates viral 'OH NO RUN!' moments when someone hatches a Shiny.

### 7.3 Egg Interactions
*   **Picking Up:** Instant pickup on contact with neutral eggs. Held eggs are visible on the player's character model.
*   **Carrying:** Players carry eggs in a visible pouch/talon. Enemy players can see how many eggs a bird is carrying. This information is crucial for decision-making.
*   **Dropping:** Eggs drop on death or when using certain abilities (Nightmare Screech forces drops). Dropped eggs have enhanced visibility:
    *   **LOOT BEAM:** Vertical light pillar visible from distance.
    *   **OUTLINE:** Glowing edge visible through walls within 10m.
    *   **MAGNETIC PICKUP:** Auto-collect within 3m radius.
    *   **DURATION:** Dropped eggs remain for 30 seconds before despawning.
    *   Any player can pick up dropped eggs.
*   **Stealing:** Stealing from enemy nests requires channeling (time varies by bird). During the channel, the stealing player is vulnerable and the defending team receives an alert.
*   **Hatching:** Only possible at your own nest. Select an egg and begin hatching. Hatching can be interrupted by taking damage. Interrupted hatch progress resets to 0.

---

## 8. NEST MECHANICS

### 8.1 Nest Overview
Each team has one nest that serves as their spawn point, storage facility, and hatching station. Nests are indestructible to prevent griefing and snowball effects.

### 8.2 Nest Functions
*   **Respawn Point:** Players respawn at their nest after death. Respawn time is 3 seconds during Heist phase, 2 seconds during Frenzy.
*   **Egg Storage:** Nests can hold unlimited eggs. Stored eggs are protected until an enemy successfully steals them.
*   **Egg Production:** Nests automatically generate 1 Normal Egg every 30 seconds. Maximum stockpile from production is 5 eggs (production pauses when cap is reached).
*   **Hatching Station:** Two hatching slots are available. Players can hatch two eggs simultaneously. Hatching progress shows as a visual meter.

### 8.3 Nest Alerts
When enemies enter nest proximity (15 meters), both team members receive:
*   **Audio:** Warning chirp sound.
*   **Visual:** Nest icon flashes on HUD.
*   **Minimap:** Enemy position revealed within nest radius.

### 8.4 Stealing Mechanics
To steal from an enemy nest:
1.  Enter nest interaction radius (5 meters from center).
2.  Hold interact button to begin steal channel.
3.  Channel time varies by bird (0.5s - 1.5s).
4.  Channel is interrupted by taking damage.
5.  **Successful steal:** Grab one egg, +40 points, defenders alerted.

### 8.5 Nest Visual Design
Each nest should reflect the bird team's biome home while maintaining readability. The nest must be clearly visible from a distance and its ownership instantly recognizable.

---

## 9. MAP DESIGN & BIOMES

### 9.1 Map Philosophy
Maps are composed of modular hex-tile biomes arranged around a central neutral zone. This modular approach allows for procedural variety while maintaining balanced distances between nests.

### 9.2 Map Structure
Each map contains:
*   1 Central Zone (neutral, starting scramble area)
*   4 Biome Tiles (one per team, arranged around center)
*   4 Transition Zones (buffer areas between biomes)

### 9.3 Biome Types

**ICE BIOME (Penguin home)**
*   **Terrain:** Frozen lakes, ice caves, snow drifts.
*   **Feature:** Slippery surfaces increase movement speed but reduce control.
*   **Hazard:** Thin ice patches that break, causing fall damage.
*   **Advantage:** Penguin gains significant bonuses here.

**JUNGLE BIOME (Hummingbird/Toucan/Parrot home)**
*   **Terrain:** Dense canopy, vines, flowers, waterfalls.
*   **Feature:** Multiple vertical layers (ground, mid-canopy, high canopy).
*   **Hazard:** Carnivorous plants that slow players who touch them.
*   **Advantage:** Many hiding spots, favors agile birds.

**SWAMP BIOME (Shoebill/Pelican home)**
*   **Terrain:** Murky water, dead trees, fog, reeds.
*   **Feature:** Low visibility, ambient sounds mask footsteps.
*   **Hazard:** Deep water areas that slow non-water birds.
*   **Advantage:** Difficult to navigate for unfamiliar players.

**DESERT BIOME (Vulture/Eagle home)**
*   **Terrain:** Sand dunes, rock formations, cacti, oases.
*   **Feature:** Open sightlines, minimal cover.
*   **Hazard:** Sandstorms periodically reduce visibility for all players.
*   **Advantage:** Eagles excel with clear dive lines.

**MOUNTAIN BIOME (Cassowary home)**
*   **Terrain:** Rocky cliffs, caves, narrow paths, geysers.
*   **Feature:** Extreme verticality with few flight-friendly zones.
*   **Hazard:** Falling rocks, unstable ledges.
*   **Advantage:** Ground-based combat favors Cassowary.

**NIGHT FOREST BIOME (Owl/Potoo home)**
*   **Terrain:** Dark woods, glowing mushrooms, hollow trees.
*   **Feature:** Permanent twilight, limited visibility range.
*   **Hazard:** Pitfall traps hidden in shadows.
*   **Advantage:** Stealth birds thrive in low light.

### 9.4 Central Zone
The central neutral zone is where the Scramble phase occurs. It contains:
*   Open arena with scattered cover.
*   Elevated platforms for aerial advantage.
*   Multiple exit paths to each biome.
*   Initial egg spawns for Scramble phase.

### 9.5 Jump Pads (Vertical Mobility)
To ensure ground-bound birds (Cassowary) remain viable on vertical maps, natural Jump Pads are placed throughout.

**Jump Pad Types by Biome:**
*   **ICE BIOME:** Geyser vents that blast upward periodically.
*   **JUNGLE BIOME:** Giant bouncy mushrooms.
*   **SWAMP BIOME:** Bubbling mud pits that erupt.
*   **DESERT BIOME:** Dust devils / thermal updrafts.
*   **MOUNTAIN BIOME:** Steam vents from volcanic activity.
*   **NIGHT FOREST:** Magical glowing springboards.

**Jump Pad Mechanics:**
*   Launch height: 20-25 meters (enough to reach most platforms).
*   Any bird can use them, but Cassowary NEEDS them.
*   Brief invulnerability during launch (0.5s).
*   Cooldown: 3 seconds per pad after use.
*   Creates epic anti-air moments: Cassowary launches -> Murder Kick on flying target.

---

## 10. COMBAT SYSTEM

### 10.1 Combat Philosophy
Combat in Bird Game 3 is secondary to the heist objectives but crucial for defending and intercepting. Fights should be quick and decisive - this is not a game about extended duels.

### 10.2 Basic Attack
All birds have a basic attack (peck/scratch) that deals moderate damage:
*   **Damage:** 15 per hit (modified by bird's damage stat).
*   **Attack Speed:** 2 attacks per second.
*   **Range:** Melee (2 meters).

### 10.3 Health and Death
Health does not regenerate automatically. Health can only be recovered through:
*   Respawning at nest (full health).
*   Hatching eggs (small heal from Normal egg buffs).
*   Bird-specific abilities (Vulture passive, etc.).

**Upon death:**
*   All carried eggs drop at death location.
*   3-second respawn timer begins (2s in Frenzy).
*   Respawn at nest with full health.

### 10.4 Crowd Control
Several CC types exist in the game:
*   **Stun:** Cannot move or use abilities.
*   **Slow:** Movement speed reduced by percentage.
*   **Knockback:** Forced movement in a direction.
*   **Knockdown:** Brief ground state, unable to act.
*   **Fear:** Forced movement away from source.
*   **Silence:** Cannot use abilities (can still move and attack).

### 10.5 Combat Tips for Implementation
*   **Hitstop:** Brief pause on hit for satisfying feedback.
*   **Knockback:** Slight push on all attacks for physicality.
*   **Audio:** Distinct hit sounds for connecting vs missing.
*   **VFX:** Clear impact effects, especially for abilities.

---

## 11. AUDIO DESIGN GUIDELINES

### 11.1 Audio Pillars
Sound design should reinforce four core experiences:
1.  **REWARD:** Every positive action should have satisfying audio feedback.
2.  **INFORMATION:** Audio cues communicate game state without requiring visual attention.
3.  **PERSONALITY:** Each bird should have a distinct audio identity.
4.  **SOCIAL:** Voice chat integration makes audio a multiplayer experience.

### 11.2 Voice Chat Audio Integration

**Technical Requirements:**
*   Low-latency VOIP (target <50ms).
*   Spatial audio for proximity voice (HRTF recommended).
*   Automatic gain control to normalize volume levels.
*   Noise gate to reduce background noise.
*   Echo cancellation for speakers users.

**Proximity Voice Mixing:**
*   Enemy voices duck (reduce volume) game SFX by 20% when speaking.
*   Proximity voices have slight radio/walkie effect for clarity.
*   Distance attenuation: 100% at 5m, 50% at 10m, 10% at 15m, 0% beyond.
*   Vertical distance counts - voices above/below are quieter.

**Special Voice Interactions:**
*   **Owl:** Proximity voice MUTED (passive ability).
*   **Potoo Screech:** Plays through proximity voice system at MAX volume.
*   **Seagull SCREAM:** Distorts enemy voice audio temporarily.
*   **Parrot Chatter:** Can replay last 3 seconds of captured enemy voice.

### 11.3 Critical Audio Cues

**Scoring Sounds (Reward Layer):**
*   **+5 points (egg pickup):** Soft 'pop' - cheerful, quick.
*   **+15 points (delivery):** Cash register 'cha-ching' - satisfying, clear.
*   **+25 points (hatch):** Egg crack + chick chirp - organic, cute.
*   **+40 points (steal):** Dramatic steal sting - triumphant, bold.
*   **Streak bonus:** Escalating fanfare - builds excitement.

**Alert Sounds (Information Layer):**
*   **Nest proximity warning:** Urgent chirp - raises awareness.
*   **Teammate down:** Distress call - prompts response.
*   **Frenzy phase start:** Dramatic music shift - tension increase.
*   **Low health:** Heartbeat/panting - personal urgency.

**Bird Vocalizations (Personality Layer):**
Each bird should have unique sounds for: Movement (footsteps, wingbeats, landing), Abilities, Taking damage, Eliminating enemies, Being eliminated, Picking up eggs.

### 11.3 Spatial Audio
3D audio is critical for gameplay information:
*   Enemy footsteps should be clearly directional.
*   Ability sounds should indicate position and distance.
*   Nest alerts should have clear directional component.
*   Owl's Silent passive removes these cues - powerful advantage.

### 11.4 Music
Dynamic music system with intensity layers:
*   **Scramble phase:** High energy, chaotic, playful.
*   **Heist phase (neutral):** Medium tempo, tension building.
*   **Heist phase (combat):** Percussion intensifies.
*   **Heist phase (carrying eggs):** Sneaky, staccato.
*   **Frenzy phase:** Maximum intensity, driving rhythm.
*   **Victory/Defeat:** Triumphant fanfare / Sad trombone.

---

## 12. UI/UX DESIGN

### 12.1 HUD Elements
**Always Visible:**
*   **Score:** Your team's points (large, top center).
*   **Enemy Scores:** Other teams' points (smaller, top corners).
*   **Timer:** Match time remaining.
*   **Minimap:** Player positions, nest locations.
*   **Egg Counter:** How many eggs you're carrying (near crosshair).
*   **Health Bar:** Your current HP.
*   **Ability Icons:** Cooldown status for both abilities.
*   **Teammate Status:** Partner's health and position indicator.

**Contextual Elements:**
*   **Nest Alert:** Flashes when enemies near your nest.
*   **Steal Prompt:** Appears when near enemy nest.
*   **Hatch Prompt:** Appears when at your nest with eggs.
*   **Point Popups:** +5, +15, etc. floating numbers.
*   **Streak Banner:** "STEALING SPREE!" across screen.

### 12.2 Scoring Feedback
Points should feel GOOD to earn:
*   Numbers pop up and float upward.
*   Color coding: White (normal), Gold (bonus), Red (combat).
*   Sound accompanies every point gain.
*   Score counter pulses when updated.
*   Big bonuses get screen shake and particle effects.

### 12.3 Match Flow Screens
*   **Pre-Match:** Bird Selection (stats visible), Team Preview, Map Preview, Countdown.
*   **Post-Match:** Final Scoreboard (teams ranked), MVP Highlight, Personal Stats, XP Gain (bar fill), Play Again Button.

---

## 13. PROGRESSION & UNLOCKS

### 13.1 XP System
Every match awards XP based on performance:
*   **Base completion XP:** 100
*   **Winning team bonus:** +50 XP
*   **Score conversion:** 1 XP per 10 points scored
*   **First match of day:** 2x XP

### 13.2 Player Level
Player level is a prestige indicator:
*   **Levels 1-50:** Unlock cosmetics at milestone levels.
*   **Level 50+:** Prestige system (reset with rewards).
*   Level displayed on player card and in-match.

### 13.3 Unlockable Content
*   **Bird Skins:** Color/Pattern variants, Rare skins.
*   **Nest Decorations:** Styles, flair, effects.
*   **Egg Cosmetics:** Patterns, Hatch effects.
*   **Player Titles:** Earned through achievements (e.g., "Egg Baron," "Murder Machine").

### 13.4 Achievements
Achievements reward specific playstyles and milestones:
*   "First Heist": Complete your first steal.
*   "Egg-cellent": Hatch 100 eggs total.
*   "Master Thief": Get a 5-steal streak.
*   "Defender": Stop 50 enemy steals.
*   "One of Each": Win a match with every bird.

---

## 13B. ANTI-TOXICITY & PLAYER SAFETY

### 13B.1 Voice Chat Safety
Voice chat is core to the experience but requires safeguards against abuse.

**Parrot Voice Recording Consent:**
*   **Setting:** 'Allow Enemy Voice Recording' (default: ON).
*   **When OFF:** Parrot's Chatter plays procedural bird babble instead.
*   **Audio filter ALWAYS applied:** Pitch shift or distortion. Prevents intelligible transmission of slurs/toxicity.

**General Voice Safety:**
*   One-click mute for any player.
*   Mute persists across matches (remembered).
*   Report button with voice clip attachment.
*   Auto-moderation: Volume spike detection for screamers.
*   Streamer Mode: Replaces enemy names, optional voice pitch shift.

### 13B.2 Anti-Griefing Measures
Teammate griefing must be impossible or heavily punished.

**Pelican Scoop Protection:**
*   Allies can **ALWAYS** eject (press JUMP).
*   Eject gives upward boost - cannot be thrown into hazards.
*   Repeated unwanted scoops trigger auto-immunity for victim.

**General Anti-Grief:**
*   No friendly fire damage.
*   Cannot block teammate's nest entrance.
*   Cannot destroy or interact negatively with teammate's eggs.
*   AFK detection: 30 seconds idle = kick from match.
*   Repeated reports = matchmaking isolation.

### 13B.3 Accessibility Options
*   Colorblind modes for egg/team identification.
*   Screen shake intensity slider (0-100%).
*   Visual indicators for audio cues (footstep direction, voice proximity).
*   Subtitle system for important callouts.
*   Remappable controls for all inputs.

---

## 14. TECHNICAL REQUIREMENTS

### 14.1 Engine
**Unity** (recommended version: 2022.3 LTS or newer)

### 14.2 Networking
*   8-player lobbies (4 teams x 2 players).
*   Client-server architecture for cheat prevention.
*   Target tick rate: 60 Hz.
*   Lag compensation for abilities and combat.
*   Server-authoritative egg transactions (steal, pickup, drop).

### 14.3 Voice Chat Infrastructure
*   Integrated VOIP - NOT optional, core feature.
*   **Recommended:** Vivox, Photon Voice, or Discord GameSDK.
*   Spatial audio processing for proximity voice.
*   Target latency: <50ms voice transmission.
*   Bandwidth per player: ~32kbps for voice.
*   Server-side voice relay for consistency.
*   Mute/report functionality required for moderation.

### 14.4 Platform Targets
*   **Primary:** PC (Steam).
*   **Stretch:** Console (PlayStation, Xbox, Switch).

### 14.4 Performance Targets
*   **Minimum:** 30 FPS at 1080p (low settings).
*   **Recommended:** 60 FPS at 1080p (medium settings).
*   **High-end:** 144+ FPS at 1440p (high settings).

### 14.5 Audio Requirements
*   Wwise or FMOD integration for dynamic audio.
*   3D spatial audio support.
*   Dynamic music layering system.
*   Estimated sound events: 300+ unique sounds.

### 14.6 Art Style Direction
Semi-stylized 3D with bright, readable colors. Think *Overwatch* meets *Angry Birds*. Characters should be instantly recognizable at distance. Biomes should have distinct color palettes for quick orientation.

---

## APPENDIX A: QUICK REFERENCE

### A.1 Bird Stats At-A-Glance
| BIRD | HP | SPEED | STEAL | CARRY | TIER | ROLE |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| Hummingbird | 60 | 130% | 0.5s | 2 | Thief | Escape Artist |
| Owl | 70 | 115% | 0.6s | 3 | Thief | Infiltrator |
| Seagull | 65 | 120% | 0.7s | 3 | Disruptor | Pest |
| Potoo | 75 | 100% | 0.8s | 3 | Disruptor | Ambusher |
| Toucan | 85 | 105% | 0.8s | 3 | Balanced | All-Rounder |
| Pelican | 90 | 95% | 1.0s | 5 | Balanced | Hauler |
| Parrot | 80 | 110% | 0.8s | 3 | Balanced | Trickster |
| Eagle | 100 | 100% | 1.2s | 3 | Brawler | Assassin |
| Shoebill | 110 | 90% | 1.3s | 3 | Brawler | Guardian |
| Vulture | 105 | 95% | 1.2s | 3 | Brawler | Finisher |
| Cassowary | 140 | 85% | 1.5s | 3 | Tank | Juggernaut |
| Penguin | 120 | 80/140% | 1.3s | 3 | Tank | Controller |

### A.2 Scoring Quick Reference
*(See Section 5.2)*

### A.3 Match Timeline
| TIME | PHASE | KEY CHARACTERISTICS |
| :--- | :--- | :--- |
| 0:00-3:00 | SCRAMBLE | 60-70 eggs in center, instant respawn, establish territory |
| 3:00-11:00 | HEIST | Slow spawns (25s), 5s respawn, 2 events (~4:00 and ~7:30) |
| 11:00-15:00 | FRENZY | Nests visible, 2x steal, 3s respawn, Golden Egg at 12:00 |

### A.4 Economy Values Quick Reference
| PARAMETER | VALUE | NOTES |
| :--- | :--- | :--- |
| Match Duration | 15:00 (900s) | Extended format |
| Nest Egg Production | 1 per 60s | Forces stealing |
| Neutral Egg Spawn | 1 per 25s/biome | Slowed economy |
| Shiny Egg Respawn | Every 3.5 min | Major event |
| Heist Respawn Time | 5 seconds | Death matters |
| Frenzy Respawn Time | 3 seconds | Fast finale |
| Super Charge Duration | 15 seconds | Extended buff |
| Dynamic Events | 2 per match | ~4:00 and ~7:30 |
| Golden Egg Spawn | 12:00 | 3 min before end |

---

**END OF DOCUMENT**

*This Game Design Document represents the foundational vision for Bird Game 3: The Great Egg Heist. All values, mechanics, and systems are subject to iteration based on playtesting feedback.*
