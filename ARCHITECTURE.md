# Aetherveil — Architecture Notes

Last updated: Phase 7A

---

## Project Overview

- **Engine:** Unity 6 (6000.4.8f1)
- **Networking:** FishNet 4.7.2R (Tugboat transport, port 7770)
- **Render Pipeline:** URP
- **Input:** Legacy Input Manager (not New Input System)
- **UI:** uGUI + TextMeshPro
- **Save path:** `Application.persistentDataPath/save.json`

---

## Scene Structure

| Scene | Purpose |
|---|---|
| `PrototypeScene` | Main playable scene (village/starter zone) |
| `ForestScene` | Second zone |
| `SampleScene` | Unused Unity default |

All scenes must be registered in **File → Build Profiles** (Unity 6 equivalent of Build Settings).

---

## Networking Architecture (FishNet)

### Key Decisions

- **Transport:** Tugboat (UDP), default port 7770
- **Authority model:** Client-authoritative movement (`NetworkTransform` owner mode)
- **Player spawning:** FishNet `PlayerSpawner` on `NetworkManager` prefab
- **NetworkManager prefab location:** `Assets/FishNet/Demos/Prefabs/NetworkManager`

### Player Spawning Flow
1. Host/client clicks Host Game → `NetworkHUD.StartHost()` / `StartClient()`
2. FishNet's `PlayerSpawner` instantiates `Assets/Prefabs/Player.prefab` per client
3. `NetworkPlayerSetup.OnStartClient()` fires on all clients:
   - **Owner:** enables `PlayerController`, `NPCInteractor`, assigns camera, wires UI
   - **Non-owner:** disables input components

### Player Prefab Components
- `NetworkObject` — required for any networked object
- `NetworkTransform` — syncs position/rotation (Component Configuration: CharacterController, Client Authoritative)
- `NetworkPlayerSetup` — ownership-based component gating
- `PlayerController` (starts **disabled** — enabled by NetworkPlayerSetup for owner only)
- `NPCInteractor` (starts **disabled** — enabled by NetworkPlayerSetup for owner only)

### UI Systems and Network Spawn
All UI that references Player components must find them **lazily** — the Player doesn't exist in the scene at startup, FishNet spawns it after `Start()` runs.

| Script | Pattern used |
|---|---|
| `PlayerUI` | Finds player in `Update()` until found |
| `EnemyAI` | `TryFindPlayer()` called in `Update()` until found |
| `TargetUI` | Finds `TargetingController` lazily in `Update()` |
| `EquipmentUI` | `CachePlayerComponents()` called lazily on window open |
| `HotbarUI` | `SetAbilityController()` called by `NetworkPlayerSetup` on spawn |
| `GameSaveManager` | `SetPlayer()` called by `NetworkPlayerSetup` on spawn |

### Zone Transitions in Multiplayer
`ZoneTransition` uses Unity's `SceneManager.LoadScene()` which destroys FishNet's NetworkManager on scene change — **this breaks multiplayer**. Zone transitions are blocked when `InstanceFinder.IsServerStarted || InstanceFinder.IsClientStarted`. Proper multiplayer scene transitions via FishNet's `NetworkSceneManager` are deferred to a later phase.

---

## Save System

- **Format:** JSON via `JsonUtility`
- **Location:** `Application.persistentDataPath/save.json`
- **Asset lookup:** `GameRegistry` ScriptableObject at `Assets/Scripts/Data/GameRegistry.asset` — all `ItemData` and `QuestData` assets must be registered here for save/load to work
- **Trigger:** F5 manual save, auto-save on quit (`OnApplicationQuit`), auto-load on player spawn
- **Save/load entry point:** `GameSaveManager.SetPlayer()` — called by `NetworkPlayerSetup` after FishNet spawns local player
- **Note:** `OnApplicationQuit` null-checks the player since FishNet destroys network objects before quit fires in editor

---

## Inventory & Items

- `ItemData` ScriptableObject — add `baseValue` (gold) field for vendor pricing
- Sell price = `baseValue / 2` (enforced in `VendorSlotUI` and `VendorUI`)
- `Inventory.ClearAll()` exists for save/load use
- Equipment bonuses tracked as `bonusHealth` / `bonusDamage` on `CombatStats`, separate from base stats

---

## Quest System

- Only supports **kill quests** currently (`targetEnemyName` + `requiredKills`)
- `QuestLog.RegisterKill()` strips `(Clone)` suffix from enemy names for matching
- `QuestStatus` enum: `Active` → `Complete` → `TurnedIn`
- `QuestGiver` is driven by `NPCDialogue` — quest accept/decline happens via dialogue UI, not direct interaction

---

## Dialogue System

- `DialogueData` ScriptableObject: `npcName`, `string[] pages`, optional `questOffer`
- `NPCDialogue` selects dialogue state based on quest status: `questOfferDialogue`, `questActiveDialogue`, `questCompleteDialogue`, `defaultDialogue`
- Quest accept/decline fires through `DialogueUI` events (`OnQuestAccepted`, `OnQuestDeclined`)
- Turn-in triggers on `OnDialogueClosed` when `questCompleteDialogue` is showing
- `NPCInteractor` uses `FindObjectsInactive.Include` because `DialogueUI` starts inactive

---

## Vendor System

- `VendorData` ScriptableObject: vendor name + `List<VendorItem>` (item + optional price override)
- Buy price: `priceOverride > 0 ? priceOverride : item.baseValue`
- Sell price: `baseValue / 2` (minimum 1g)
- `NPCVendor` component on merchant NPCs — no `NPCDialogue` needed for pure merchants
- `NPCInteractor` priority: `NPCDialogue` takes precedence over `NPCVendor` if both present

---

## Combat

- `CombatStats.DetectionRange = 0` on enemies means the enemy won't detect the player — check this field in the Inspector if enemies seem passive
- `EnemyAI` leash distance: 12 units (configurable via `leashDistance` field)
- Enemy respawn: `EnemySpawnPoint` instantiates enemy and calls `SetSpawnPoint()` on `EnemyAI`

---

## Known Issues / Deferred Work

| Issue | Status |
|---|---|
| Multiplayer zone transitions | Blocked — needs FishNet `NetworkSceneManager` |
| Pause/Main menu | Deferred to Phase 9 |
| `EquipmentWindowController` duplicate toggle | Harmless, can remove the component |
| `VendorUI.isBuyTab` unused field warning | Minor, can suppress or remove |
| FishNet assets not committed to git | Large package, consider `.gitignore` |

---

## Phase Completion

```
Phase 1 - Core Character        ✅
Phase 2 - Combat                ✅
Phase 3 - Progression           ✅
Phase 4 - Inventory & Equipment ✅
Phase 5 - Quests & NPCs         ✅
Phase 6A - Dialogue System      ✅
Phase 6B - Vendors & Economy    ✅
Phase 6C - Character Sheet      ✅
Phase 6D - Enemy Respawning     ✅
Phase 6E - Save System          ✅
Phase 6F - Multiple Zones       ✅
Phase 7A - Networking Foundation ✅
Phase 7B - Multiplayer Movement  ← Next
Phase 7C - Multiplayer Combat
Phase 7D - Multiplayer Loot
Phase 7E - Multiplayer Quests
Phase 8  - Content Production
Phase 9  - Polish & Launch
```
