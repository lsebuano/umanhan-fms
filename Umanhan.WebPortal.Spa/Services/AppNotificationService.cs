using Radzen;

namespace Umanhan.WebPortal.Spa.Services
{
    public class AppNotificationService
    {
        private readonly NotificationService _notificationService;

        public AppNotificationService(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public Task ShowInfo(string message, string? title = null)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Info,
                Summary = title ?? "Information",
                Detail = message,
                Duration = 8000
            });
            return Task.CompletedTask;
        }

        public Task ShowSuccess(string message, string? title = null)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = title ?? "Success",
                Detail = message,
                Duration = 8000
            });
            return Task.CompletedTask;
        }

        public Task ShowWarning(string message, string? title = null)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = title ?? "Warning",
                Detail = message,
                Duration = 8000
            });
            return Task.CompletedTask;
        }

        public Task ShowError(string message, string? title = null)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = title ?? "Error",
                Detail = message,
                Duration = 10000
            });
            return Task.CompletedTask;
        }
    }
}
