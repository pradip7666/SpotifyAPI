using StackExchange.Profiling;
using System;

namespace SpotifyAPI.Middleware
{
    public class MiniProfilerWrapper : IMiniProfilerWrapper
    {
        public IDisposable Step(string name)
        {
            return MiniProfiler.Current?.Step(name);
        }
    }
}