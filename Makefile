NUPKG_DIR = ./nupkgs
SERVICE_DIRS = auth-service/AuthService auth-service/AuthService.Tests  # Projekty, do których dodajemy paczki
SERVICE_CSPROJ = auth-service/AuthService/AuthService.csproj \
				gateway/Gateway.API/Gateway.APi.csproj \
				email-service/EmailService/EmailService.csproj
SERVICE_SLN = auth-service/AuthService.sln \
				gateway/Gateway.sln \
				email-service/EmailService.sln \


				
install-pkgs:
	@echo "Installing packages from folder $(NUPKG_DIR)..."
	@for service in $(SERVICE_CSPROJ); do \
		echo "Installing packages for $$service..."; \
		dotnet add $$service package Shared.Grpc; \
	done

pack-pkgs:
	@mkdir -p nupkgs
	@rm -rf nupkgs/*
	dotnet nuget locals all --clear
	dotnet pack Shared/Shared.Grpc/Shared.Grpc.csproj -c Release -o nupkgs

clean: 
	@echo "Cleaning build artifacts..."
	@rm -rf Shared/Shared.Grpc/obj
	@rm -rf auth-service/AuthService/obj auth-service/AuthService/bin
	@rm -rf gateway/Gateway.API/obj gateway/Gateway.API/bin


# Restore packages
restore-pkgs:
	@echo "Restoring packages..."
	@for service in $(SERVICE_SLN); do \
		echo "Restoring packages for $$service..."; \
		dotnet restore $$service --no-cache; \
	done


proto:
	rm -rf Shared/Shared.Grpc/pb/*
	@protoc --proto_path=Shared/Shared.Grpc/protos \
			--csharp_out=Shared/Shared.Grpc/pb \
			--grpc_out=Shared/Shared.Grpc/pb \
			--plugin=protoc-gen-grpc=/usr/local/bin/grpc_csharp_plugin \
			Shared/Shared.Grpc/protos/**/*.proto 

setup-environment:
	@cp env.example .env
	@find env -type f -name '*.env.example' | while read -r file; do \
		cp "$$file" "$${file%.env.example}.env"; \
	done
	@echo ".env files created successfuly - project ready to start"
	@echo "use 'docker-compose up -d' command to start project 🚀"

setup-dev-environment:
	@cp auth-service/AuthService/env.example auth-service/AuthService/.env
	@cp workshop-service/WorkshopService/env.example workshop-service/WorkshopService/.env
	@echo "Services ready to develop 💻"




.PHONY: proto restore-pkgs pack-pkgs install-pkgs clean setup-environment setup-dev-environment