using MassTransit;

namespace ReportService.Infrastructure.Messaging.Producers
{
    public class ReportProducer 
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public ReportProducer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task SendCreateReportMessageAsync(string reportType)
        {
            /*var message = new CreateReportMessage
            {
                ReportType = reportType
            };

            await _publishEndpoint.Publish(message);*/
        }
    }
}