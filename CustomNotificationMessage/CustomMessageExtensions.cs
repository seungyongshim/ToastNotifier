using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Core;

namespace ToastNotifier.CustomNotificationMessage
{
    public static class CustomMessageExtensions
    {
        public static void ShowCustomMessage(this Notifier notifier,
            string message,
            MessageOptions messageOptions = null)
        {
            notifier.Notify(() => new CustomMessageViewModel(message, messageOptions));
        }
    }
}
