{ pkgs, ... }: {
  packages = [
    pkgs.azurite
    pkgs.azure-cli
    pkgs.bicep
    pkgs.powershell
  ];

  languages = {
    dotnet = {
        enable = true;
        package = pkgs.dotnet-sdk_10;
    };
  };

  processes = {
    azurite.exec = ''
      azurite --location .azurite --debug .azurite/debug.log --skipApiVersionCheck
    '';

    api.exec = ''
      dotnet watch --project src/Api
    '';

    weather-worker.exec = ''
      dotnet watch --project src/WeatherWorker
    '';

    image-worker.exec = ''
      dotnet watch --project src/ImageWorker
    '';
  };
}
