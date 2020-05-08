using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Lifetime.Clear;
using ToastNotifications.Messages;
using ToastNotifier.CustomNotificationMessage;

namespace BLUECATS.ToastNotifier.Actors
{
    public class NotificationActor : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }

        public static Props Props(Notifier notifier)
        {
            return Akka.Actor.Props.Create(() => new NotificationActor(notifier));
        }

        public NotificationActor(Notifier notifier)
        {
            Receive<(NotificationLevel,string)>( _ =>
            {
                var (lv, msg) = _;

                switch (lv)
                {
                    case NotificationLevel.Debug:
                        notifier.ShowSuccess(msg);
                        break;
                    case NotificationLevel.Info:
                        notifier.ShowInformation(msg);
                        break;
                    case NotificationLevel.Warning:
                        notifier.ShowWarning(msg);
                        break;
                    case NotificationLevel.Error:
                        notifier.ShowCustomMessage(msg, 9);
                        break;
                    case NotificationLevel.Alert_Level1:
                        notifier.ShowCustomMessage(msg, 1);
                        break;
                    case NotificationLevel.Alert_Level2:
                        notifier.ShowCustomMessage(msg, 2);
                        break;
                    case NotificationLevel.Alert_Level3:
                        notifier.ShowCustomMessage(msg, 3);
                        break;
                    case NotificationLevel.Alert_Level4:
                        notifier.ShowCustomMessage(msg, 4);
                        break;
                    case NotificationLevel.Alert_Level5:
                        notifier.ShowCustomMessage(msg, 5);
                        break;
                }
                
                BecomeStacked(Delaying);
            });
        }

        private void Delaying()
        {
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(0.5), Self, DelayMessage.Instance, Self);
            Receive<DelayMessage>(_ =>
            {
                Stash.Unstash();
                UnbecomeStacked();
            });

            ReceiveAny(_ => Stash.Stash());
        }

        internal class DelayMessage
        {
            public static DelayMessage Instance { get; } = new DelayMessage();
        }

        internal class ClearedMessage
        {
            public static ClearedMessage Instance { get; } = new ClearedMessage();
        }
    }
}
