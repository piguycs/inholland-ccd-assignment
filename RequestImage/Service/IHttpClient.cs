namespace RequestImage.Service;

public interface IHttpClient
{
    Task<T?> GetAsync<T>(string url);
}