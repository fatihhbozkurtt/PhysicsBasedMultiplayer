# 🎮 PhysicsBasedMultiplayer (FishNet)

A basic multiplayer prototype built with **Unity** and **FishNet** showcasing real-time movement, shooting mechanics, and object interaction. Players can move, shoot projectiles, pick up and drop items, and take damage through networked interactions.

---

## 🕹️ Controls

| Key          | Action                 |
|------------- |------------------------|
| W, A, S, D   | Move                   |
| Space        | Jump                   |
| Left Click   | Shoot projectile       |
| 1            | Spawn pickable object  |
| 2            | Despawn pickable obj   |
| E            | Pick up object         |
| Q            | Drop object            |
| F            | Change player color    |
| R            | Decrease hp manually   |

---

## 🚀 Features

- 🔫 **Networked projectile shooting**
- 🧍 **Smooth movement and jumping**
- 🎒 **Pick-up and drop system**
- 💡 **Damage system with health tracking**
- 🧠 **Dedicated server logic**
- 🧩 Built using **FishNet** for modern multiplayer networking

---

## 🖼️ UI Overview

- Each player has a health bar (UI Text) displayed locally.
- Health is reduced when hit by a projectile.
- Health updates are only shown to the corresponding player for security and clarity.

---

## 📌 Implementation Decisions

### 🔄 1. Network Authority Management
I ensured that each player’s actions could only be influenced by themselves or the server. This was achieved using FishNet’s `IsOwner` and `IsServer` checks, ensuring that only the owning player can control their character and that the server manages all important game logic, such as damage application and object spawning.

### 💨 2. Server-Authoritative Movement
To prevent clients from sending manipulated movement data, player movements and actions (like jumping and shooting) are all handled server-side. This provides a more secure and consistent experience, with the server serving as the final authority for all physics interactions.

### 🎮 3. Smooth Player Experience
The camera follows the player’s movement and adjusts the player’s view based on mouse input, while input handling ensures smooth character movement with WASD and responsive jump mechanics, alongside appropriate gravity forces to simulate realistic physics.

### 🔐 4. Efficient Network Calls
Network calls such as spawning projectiles and updating health were implemented with FishNet’s `ServerRpc` and `ObserversRpc`, ensuring that these actions are efficiently communicated between clients and the server while maintaining synchronization across all instances of the game.

---

## ❗ Potential Challenges

Since this project is built using FishNet for multiplayer networking, several challenges needed to be carefully addressed:

### 🔄 1. State Synchronization
Ensuring accurate synchronization of player actions such as movement, jumping, and shooting across all clients was essential. To maintain consistency and fairness, a server-authoritative approach was implemented, especially for projectile movement and collision detection.

### 🔐 2. Authority Management
Operations must be correctly restricted to either the **owner** or the **server** using checks like `IsOwner` and `IsServer`. Failing to handle authority properly could result in unintended behaviors, such as clients causing damage to themselves or others unfairly.

### 🧠 3. RPC Communication
Proper and efficient usage of `ServerRpc` and `ObserversRpc` was necessary to ensure correct replication of projectile spawning, damage handling, and other networked events. Incorrect use could lead to visual inconsistencies or missing updates on clients.

### 🎯 4. Collision Handling
Projectile collisions were processed **only on the server**, and damage was applied through the `HealthController` component to ensure security and prevent exploitative client-side behavior such as fake hits.

### 💬 5. UI Updates
Each player’s health was displayed on their UI. This required careful handling to make sure only the local player's health was shown and updated correctly, without exposing other players’ internal states unnecessarily.

---

## 🛠️ Technologies Used

- Unity 6
- FishNet (Multiplayer networking)
- C#
- Unity UI Toolkit / Canvas System

---

## 📌 Notes

- FishNet transport must be properly configured for multiplayer to work.
- Tested with 2+ clients via Unity editor & standalone builds.
- Built as a learning exercise to understand core multiplayer principles using a modern networking library.

---

## 🙋‍♂️ Author

**Fatih Bozkurt**  
[LinkedIn](https://www.linkedin.com/in/fatih-bozkurt-9bb915212) | [GitHub](https://github.com/fatihhbozkurtt)  
Unity Game Developer

---



