using MassTransit;

namespace WriteOffService.Application.Messaging.Producers
{
    public class WriteOffsRequestProducer
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public WriteOffsRequestProducer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task SendWriteOffMessageAsync(WriteOffInventoryMessage message)
        {
            await _publishEndpoint.Publish(message);
        }
    }
}