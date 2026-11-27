# NFS Heat – Glowboard Bloom Fix

Frosty mod for Need for Speed Heat that reduces night bloom on glowing signs and billboards by lowering their emissive intensity to 35% of vanilla (411 sign meshes using `Shaders/World/Signs/SGN_*`). Lighting/VE files are untouched, so it works with any lighting mod.

_MADE FOR COMPATIBILITY WITH EVOLUTION WORKS MOD, SINCE THAT CAN TAKE BLOOM LEVELS A BIT TOO EXTREME._

## Install
Download `Glowboard Bloom Fix.fbmod` from Releases, add it in Frosty Mod Manager, place it at the bottom of the load order, apply.

## Build
Requires Frosty Editor 1.0.6.3. Edit the paths in `src/load.cs`, then:

```
csc -platform:x64 -r:FrostySdk.dll -r:FrostyCore.dll -out:build.exe src\build.cs src\load.cs
build.exe sign_meshes.txt 0.35 "Glowboard Bloom Fix.fbmod" cfg.json
```

`0.35` is the emissive multiplier. `cfg.json` is a Frosty config (see `cfg.example.json`).
