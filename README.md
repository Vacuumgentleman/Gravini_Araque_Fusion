## **Gun Arena — Unity 6 + Photon Fusion 2 (Host Mode)**
  **Gun Arena** is a compact multiplayer prototype built with **Unity 6**  
  and **Photon Fusion 2** using **Host Mode networking**.  
  Two players (Host & Client) enter a 3D arena, move around,  
  shoot synchronized projectiles, take hit-flash feedback,  
  and the match ends automatically when a player reaches **3 total hits**.

---
## Authors

- **Juan Sebastian Gravini Contreras**  
- **Leydi Tatiana Araque Uzcátegui**

---
## Gameplay

- Move your character using **WASD**.
- Shoot using **Space**.
- Every projectile hit increases the enemy’s hit counter.
- Players flash red when hit.
- A UI label above the player shows their current hits (local UI).
- The game ends automatically when a player reaches **3 total hits**.

---

## Networking (Photon Fusion 2)

- Host Mode architecture (one peer acts as server).
- Automatic player spawning on connection.
- Networked movement, rotation, color, hits, and projectiles.
- Projectile spawning predicted on the client and validated by host.
- Networked hit counter with RPC feedback.
- Local-only UI for hit display.
- Third-person camera assigned only to the local player.

---

## Scenes

 **Arena**
   - Main and only scene.
   - Spawns Host/Client.
   - Contains UI, Runner, Player prefab, and Projectile prefab.

---

## UI Features

- Hit counter on each player, visible only to the local owner.
- Flash feedback on hit (color change).
- Automatic match end screens: **Win** / **Lose**.
- Menu UI included.

---

## Project Structure

- **Scripts**
  - Player.cs  
  - Ball.cs  
  - UIHits.cs  
  - Spawner / NetworkRunner setup
- **Prefabs**
  - Player  
  - Projectile (Ball)  
- **Scenes**
  - Arena
- **UI**
  - Canvas  
  - TMP Text Elements
- **Materials**
  - Player materials  
  - Flash-hit material

---

## Controls

- **Move:** W / A / S / D  
- **Shoot:** Space  
- **Camera:** Third-person, follows local player only  

---

## How to Play

1. Open Unity 6.
2. Enter your Fusion **App ID** inside:
   `Fusion → Realtime → App Settings`
3. Open the **Arena** scene.
4. Press Play → this becomes the **Host**.
5. Make a Build and run it → choose **Client**.
6. Both players spawn automatically.
7. Move, shoot, and try to hit the other player 3 times.
8. The match ends automatically after 3 hits.

---

## Screenshots

### Menu
![Imagen de WhatsApp 2025-11-14 a las 22 28 51_e2986d4c](https://github.com/user-attachments/assets/8776107b-2c56-490d-982a-9b8a3ea2e108)


### Match Results (Win / Lose)
![Imagen de WhatsApp 2025-11-14 a las 23 00 52_ae18d2d0](https://github.com/user-attachments/assets/25a64afc-d994-47d6-a0ca-bd4e58584ad3)

![Imagen de WhatsApp 2025-11-14 a las 23 01 50_7daea2df](https://github.com/user-attachments/assets/21234e2e-71b8-45ba-b7de-d2c571b2cfe5)


---

## Videos

### Gameplay Demo



https://github.com/user-attachments/assets/43017515-4f82-4dad-a61f-f21b05d1fce1

