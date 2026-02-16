using DomainEntities.Requests;

namespace Core.Interfaces;

public interface ITelegramClient
{
    Task SendBroadcastAsync(BroadcastRequest broadcastRequest);
}