{
  description = "Development tools for the weather image Azure Functions app";

  inputs.nixpkgs.url = "github:NixOS/nixpkgs/nixos-26.05";

  outputs = { nixpkgs, ... }:
    let
      systems = [ "x86_64-linux" "aarch64-linux" ];
      forAllSystems = nixpkgs.lib.genAttrs systems;
    in {
      devShells = forAllSystems (system:
        let pkgs = import nixpkgs { inherit system; };
        in {
          default = pkgs.mkShell {
            packages = with pkgs; [
              dotnet-sdk_10
              powershell
              azure-cli
              azure-functions-core-tools
              bicep
              zip
              unzip
            ];

            # Keep Azure CLI from downloading a non-Nix Bicep executable.
            AZURE_BICEP_USE_BINARY_FROM_PATH = "true";
            DOTNET_CLI_TELEMETRY_OPTOUT = "1";
            POWERSHELL_TELEMETRY_OPTOUT = "1";
            AZURE_CORE_COLLECT_TELEMETRY = "false";
            FUNCTIONS_CORE_TOOLS_TELEMETRY_OPTOUT = "1";
            DOTNET_NOLOGO = "1";
          };
        });
    };
}
