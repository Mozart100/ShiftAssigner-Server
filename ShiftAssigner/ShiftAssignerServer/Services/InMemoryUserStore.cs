using System;
using System.Collections.Concurrent;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Services
{
    /// <summary>
    /// Simple in-memory store that keeps Person objects and credential hashes.
    /// </summary>
    public class InMemoryUserStore
    {
        private readonly ConcurrentDictionary<string, PersonBase> _users = new();
        private readonly ConcurrentDictionary<string, string> _passwordHashes = new();

        /// <summary>
        /// Adds a person and their password hash to the store. Returns true on success.
        /// </summary>
        public bool Add(PersonBase person, string passwordHash)
        {
            var added = _users.TryAdd(person.Id, person);
            if (added)
            {
                _passwordHashes[person.Id] = passwordHash;
            }
            return added;
        }

        /// <summary>
        /// Get a person by id (or null if not found).
        /// </summary>
        public PersonBase? Get(string id)
        {
            _users.TryGetValue(id, out var p);
            return p;
        }

        /// <summary>
        /// Validate a user's password (hash comparison). Returns false if user not found.
        /// </summary>
        public bool ValidatePassword(string id, string passwordHash)
        {
            if (_passwordHashes.TryGetValue(id, out var stored))
            {
                return stored == passwordHash;
            }
            return false;
        }
    }
}
