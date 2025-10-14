$ROOT = git rev-parse --show-toplevel

New-Item $env:PREFIX/bin/mason -ItemType Directory
Copy-Item "$ROOT/bin/Mason/*" "$env:PREFIX/bin/mason" -Recurse
