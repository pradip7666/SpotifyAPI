namespace SpotifyAPI.Middleware
{
    public interface IMiniProfilerWrapper
    {
        IDisposable Step(string name);
    }

}
