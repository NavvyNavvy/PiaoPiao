# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

PiaoPiao is a Chinese game remake (a personal re-creation of a decade-old game). It uses a client-server architecture with .NET Framework 4.5 and DirectX (via SharpDX) for rendering.

## Building

```
msbuild ppNetTest.sln
```

The solution contains 7 projects:
- **Data** - Shared game library (rendering, maps, input, resources, sounds, XML)
- **ClientPublic** - Client/server networking library
- **MainC** - Client application (WinForms)
- **MainS** - Server application (console)
- **Room** - Room/player data management
- **ppNetTest** - Network test utility
- **PPTestC** - Client test utility

## Architecture

### Client-Server Model
- `ClientPublic` provides `ClientC` (client-side networking) and `ClientS` (server-side networking)
- `MainS` (server) uses `ClientS` to handle up to 6 concurrent client connections
- `MainC` (client) uses `ClientC` to connect to the server

### Key Modules
- **Data/DXRender** - DirectX rendering engine (Sprite, Shader, Font, RenderEngine)
- **Data/MapsManager** - Tile-based map system (Layer, SpriteMap, MapManager)
- **Data/Resources** - Resource management (ResManager, ResPic, ResItem)
- **Data/Sounds** - DirectSound audio engine
- **Data/XML** - XML configuration and data loading (XmlManager, Maps, SpritePic)
- **Data/Globals** - Global state via `Global` and `GlobalB` classes
- **Room** - Room state management (Rooms, PlayerData)

### Data Flow
1. `MainC` loads XML config via `XmlManager` → creates `ClientC` connection
2. `MainS` creates `Rooms` and `ClientS` server → accepts connections
3. `Room` library mediates game state between clients and server
4. `Data` library handles rendering, input, maps, and resources for client
