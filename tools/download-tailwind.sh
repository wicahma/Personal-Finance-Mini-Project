#!/usr/bin/env bash
# download-tailwind.sh
# Downloads the Tailwind CSS Standalone CLI v3 for macOS (arm64 or x64).
# Run once from the solution root before building.

VERSION="v3.4.17"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUT="$SCRIPT_DIR/tailwindcss-macos"

if [ -f "$OUT" ]; then
    echo "tailwindcss-macos already exists at $OUT - skipping download."
    exit 0
fi

ARCH=$(uname -m)
if [ "$ARCH" = "arm64" ]; then
    URL="https://github.com/tailwindlabs/tailwindcss/releases/download/$VERSION/tailwindcss-macos-arm64"
else
    URL="https://github.com/tailwindlabs/tailwindcss/releases/download/$VERSION/tailwindcss-macos-x64"
fi

echo "Downloading Tailwind CSS Standalone CLI $VERSION for macOS ($ARCH)..."
curl -L "$URL" -o "$OUT"
chmod +x "$OUT"
echo "Saved to: $OUT"
