{ pkgs, ... }: {
  languages = {
    dotnet = {
        enable = true;
        package = pkgs.dotnet-sdk_10;
    };
  };
}
