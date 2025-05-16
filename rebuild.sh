#!/bin/bash
# Script to fix the project

# 1. Fix the csproj file
echo "Updating Shared.Grpc.csproj..."
# Replace your current Shared.Grpc.csproj with the fixed version

# 2. Check and remove Class1.cs if it exists
if [ -f Shared/Shared.Grpc/Class1.cs ]; then
  echo "Found Class1.cs file. Removing..."
  rm Shared/Shared.Grpc/Class1.cs
  echo "Class1.cs removed."
else
  echo "No Class1.cs found."
fi

# 3. Rebuild the package
echo "Rebuilding the package..."
make clean-pkgs
make rebuild-pkgs

# 4. Inspect the package content
echo "Inspecting package content..."
make check-services
make inspect-proto

# 5. Install the package
echo "Installing the package to projects..."
make install

echo "All steps completed. Check for any errors in the output above."