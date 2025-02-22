using Contracts;
using MassTransit;

namespace MovementService.Infrastructure.Messaging.Producers;

public class MovementRequestProducer
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MovementRequestProducer(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task SendMoveInventoryMessageAsync(MoveInventoryMessage message)
    {
        await _publishEndpoint.Publish(message);
    }
}