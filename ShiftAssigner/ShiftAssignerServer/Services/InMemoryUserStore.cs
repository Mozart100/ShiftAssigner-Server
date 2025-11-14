using System;
using System.Collections.Concurrent;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Services
{
    public class InMemoryUserStore
    {
        private readonly ConcurrentDictionary<string, UserRecord> _users = new();

        public bool Add(UserRecord user)
        {
            return _users.TryAdd(user.Id, user);
        }

        public UserRecord? Get(string id)
        {
            _users.TryGetValue(id, out var u);
            return u;
        }
    }

    public class UserRecord
    {
        public string Id { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Tenant { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
    }
}
