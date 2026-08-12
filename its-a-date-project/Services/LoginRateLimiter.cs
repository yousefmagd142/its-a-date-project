using System.Collections.Concurrent;

namespace its_a_date_project.Services
{
    /// <summary>Simple in-memory lockout for the admin login — there's only one shared password
    /// protecting the whole site, so it needs basic brute-force protection once deployed publicly.
    /// Resets on app restart; that's an acceptable tradeoff at this scale.</summary>
    public class LoginRateLimiter
    {
        private const int MaxAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(10);

        private class Entry
        {
            public int Count;
            public DateTime? LockedUntilUtc;
        }

        private readonly ConcurrentDictionary<string, Entry> _entries = new();

        public bool IsLockedOut(string key)
        {
            if (_entries.TryGetValue(key, out var entry) && entry.LockedUntilUtc is { } until)
            {
                if (DateTime.UtcNow < until) return true;
                _entries.TryRemove(key, out _);
            }
            return false;
        }

        public void RegisterFailure(string key)
        {
            var entry = _entries.GetOrAdd(key, _ => new Entry());
            lock (entry)
            {
                entry.Count++;
                if (entry.Count >= MaxAttempts)
                    entry.LockedUntilUtc = DateTime.UtcNow.Add(LockoutDuration);
            }
        }

        public void RegisterSuccess(string key) => _entries.TryRemove(key, out _);
    }
}
