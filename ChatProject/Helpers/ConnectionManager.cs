using System.Collections.Concurrent;

namespace ChatProject.Helpers;

// Handles and (hopefully) simplifies multiple connections and groups them by user
public class ConnectionManager
{
    private readonly ConcurrentDictionary<string, Dictionary<string, HashSet<int>>> _userConnections = new();
    
    public void AddConnection(string userId, string connectionId, List<int> channelIds)
    {
        _userConnections.AddOrUpdate(userId, 
            _ => new Dictionary<string, HashSet<int>> {{connectionId, new HashSet<int>(channelIds)}},
            (_, connections) =>
            {
                connections[connectionId] = new HashSet<int>(channelIds);
                return connections;
            }
        );
    }

    public void RemoveConnection(string userId, string connectionId)
    {
        if (_userConnections.TryGetValue(userId!, out var connections))
        {
            connections.Remove(connectionId);

            if (connections.Count == 0)
            {
                _userConnections.TryRemove(userId!, out _);
            }
        }
    }

    public List<string> GetConnections(string userId)
    {
        return _userConnections.TryGetValue(userId, out var connections) ? connections.Keys.ToList() : new List<string>();
    }

    public bool IsInChannel(string userId, string connectionId, int channelId)
    {
        return _userConnections.TryGetValue(userId!, out var connections) && connections[connectionId].Contains(channelId);
    }
}