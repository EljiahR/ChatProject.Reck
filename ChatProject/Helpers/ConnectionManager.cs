using System.Collections.Concurrent;
using NuGet.Packaging;

namespace ChatProject.Helpers;

// Handles and (hopefully) simplifies multiple connections and groups them by user
public class ConnectionManager
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();
    private readonly ConcurrentDictionary<string, HashSet<int>> _userChannels = new();
    
    public void AddConnection(string userId, string connectionId)
    {
        _userConnections.AddOrUpdate(userId, 
            _ => new HashSet<string>() {connectionId},
            (_, connections) =>
            {
                connections.Add(connectionId);
                return connections;
            }
        );
    }
    
    public void AddChannel(string userId, int channelId)
    {
        _userChannels.AddOrUpdate(userId, 
            _ => new HashSet<int>() {channelId},
            (_, channels) =>
            {
                channels.Add(channelId);
                return channels;
            }
        );
    }
    
    public void AddChannels(string userId, int[] channelIds)
    {
        _userChannels.AddOrUpdate(userId, 
            _ => channelIds.ToHashSet(),
            (_, channels) =>
            {
                channels.AddRange(channelIds);
                return channels;
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
    
    public void RemoveChannel(string userId, int channelId)
    {
        if (_userChannels.TryGetValue(userId!, out var channels))
        {
            channels.Remove(channelId);
        }
    }

    public List<int> GetChannels(string userId)
    {
        return _userChannels[userId].ToList();
    }

    public List<string> GetConnections(string userId)
    {
        return _userConnections.TryGetValue(userId, out var connections) ? connections.ToList() : new List<string>();
    }

    public bool IsInChannel(string userId, int channelId)
    {
        return _userChannels.TryGetValue(userId!, out var channels) && channels.Contains(channelId);
    }
}