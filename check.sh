#!/bin/bash

NUPKG_FILE="./nupkgs/Shared.Grpc.1.0.0.nupkg"
EXTRACT_DIR="temp_nupkg"

# 1. Rozpakuj nupkg
mkdir -p "$EXTRACT_DIR"
unzip -o "$NUPKG_FILE" -d "$EXTRACT_DIR"

# 2. Znajdź wszystkie DLL w folderze lib
DLLS=$(find "$EXTRACT_DIR/lib" -name "*.dll")

echo "Znalezione DLL:"
echo "$DLLS"

# 3. Dla każdej DLL wypisz klasy za pomocą monodis
for dll in $DLLS; do
    echo "=== Klasy w $dll ==="
    monodis --typelist "$dll" | grep "TypeDef" | sed 's/^.*Name: //'
done

# 4. Posprzątaj
rm -rf "$EXTRACT_DIR"
