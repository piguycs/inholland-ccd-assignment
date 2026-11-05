using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RequestImage.Dto;
using RequestImage.Helpers;

namespace RequestImage.Functions;

public class UpdateImage(ILogger<UpdateImage> logger, HttpClient httpClient, IImageHelper imageHelper)
{
    private const string ImageApi = "https://picsum.photos/1200/800";

    [Function(nameof(UpdateImage))]
    [BlobOutput("images/{ImageJobId}/{JobId}.png")]
    public async Task<byte[]> Run([QueueTrigger("image-update-queue")] WeatherQueueMessage message)
    {
        logger.LogInformation("Fetching image for job {JobId} in {Region}", message.JobId, message.Region);

        using var response = await httpClient.GetAsync(ImageApi);
        response.EnsureSuccessStatusCode();

        await using var imageStream = await response.Content.ReadAsStreamAsync();

        var text = $"{message.Region}\n{message.Temperature:0.#} °C\n{message.Description}";
        var image = imageHelper.AddTextToImage(imageStream, (text, (30, 30), 36, "FFFFFF"));

        logger.LogInformation("Created weather image for job {JobId}: {Size} bytes", message.JobId, image.Length);

        return image;
    }
}