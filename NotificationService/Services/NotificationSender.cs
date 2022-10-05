namespace NotificationService.Services
{
    public class NotificationSender : BackgroundTask
    {
        private readonly ILogger<NotificationSender> _logger;

        public NotificationSender(ILogger<NotificationSender> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} NotificationService is starting");

            cancellationToken.Register(() => _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} NotificationService is stopping"));

            var timeout = 60000; // 1 minute
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} NotificationService is running");

                try
                {
                    var queue = Data.Notifications.GetNotifications(false); // get only notifications not yet sent
                    foreach (var pushMessage in queue)
                    {
                        try
                        {
                            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Pushing notification #{pushMessage.Id}...");
                            var result = PushNotification.Send((PushNotification.MobileType)(pushMessage.MobileType), pushMessage.DeviceToken, pushMessage.JsonPayload);

                            if (result.HasErrors)
                            {
                                _logger.LogError($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} NotificationService error: {result.Response} {Environment.NewLine} {pushMessage.DeviceToken} {Environment.NewLine} {pushMessage.JsonPayload}");
                                
                                // ToDo: save response as error
                            }
                            else
                            {
                                _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Push notification #{pushMessage.Id} sent.");

                                // ToDo: mark as sent and save
                            }
                        }
                        catch (Exception err)
                        {
                            _logger.LogError($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} NotificationService error: {err.Message} {Environment.NewLine} {err.StackTrace}");
                        }
                    }
                }
                catch (Exception err)
                {
                    _logger.LogError($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} NotificationService error: {err.Message} {Environment.NewLine} {err.StackTrace}");
                }
                await Task.Delay(timeout, cancellationToken);
            }
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} NotificationPusher is stopping");
        }
    }
}
