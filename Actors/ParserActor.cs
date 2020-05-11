using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
using ToastNotifications;
using ToastNotifications.Messages;
using System.Reflection;
using Akka.Configuration;
using Akka.Streams.Kafka.Settings;
using System.Text.RegularExpressions;
using ToastNotifier.CustomNotificationMessage;

namespace BLUECATS.ToastNotifier.Actors
{
    public class ParserActor : ReceiveActor
    {
        public static Props Props(IActorRef notificationActor)
        {
            return Akka.Actor.Props.Create(() => new ParserActor(notificationActor));
        }

        public ParserActor(IActorRef notificationActor)
        {
            Receive<ConsumeResult<Null, string>>(msg =>
            {
                var msgLength = AkkaHelper.ReadConfigurationFromHoconFile(Assembly.GetExecutingAssembly(), "conf")
                                    .WithFallback(ConfigurationFactory
                                    .FromResource<ConsumerSettings<object, object>>("Akka.Streams.Kafka.reference.conf"))
                                    .GetInt("ui.notification.message-length");

                dynamic json = JsonConvert.DeserializeObject(msg.Value, new JsonSerializerSettings()
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                });

                var level = json.jsonMessage["severity"].ToString();
                if (!CheckAuthority(level))
                    return;

                string localtime = json["@timestamp"].ToString("yyyy-MM-dd HH:mm:ss.ff") + "(" + json["@timestamp"].ToString("ddd") + ")";
                string alertInfo = System.Environment.NewLine + json.jsonMessage["monitor_name"] + " : " + json.jsonMessage["trigger_name"];
                string hosts = GetHosts(json.jsonMessage["host_name"].Value);

                var title = string.Format($"[{level}] {localtime}");
                var message = new StringBuilder().AppendLine(alertInfo).AppendLine(hosts).ToString();

                if (message.Length > msgLength)
                    message = string.Concat(message.ToString().Remove(msgLength), "...");

                var content = new NotificationMessage() { Title = title, Message = message };
                notificationActor.Tell(
                    ((NotificationLevel)Enum.Parse(typeof(NotificationLevel), GetSeverity(level), true), content)
                );
            });
        }

        private bool CheckAuthority(string level)
        {
            var authority = AkkaHelper.ReadConfigurationFromHoconFile(Assembly.GetExecutingAssembly(), "conf")
                            .WithFallback(ConfigurationFactory
                            .FromResource<ConsumerSettings<object, object>>("Akka.Streams.Kafka.reference.conf"))
                            .GetInt("ui.notification.authority-level");

            if (authority < Int32.Parse(level))
                return false;

            return true;
        }

        private string GetHosts(string value)
        {
            if (value == "{}")
                return "테스트 입니다.";
            var hosts = value.Trim('{', '}').Split(',').Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);
            return string.Join(", ", hosts.Select(x => x.Value));
        }

        private string GetSeverity(string severtityLevel)
        {
            var severity = string.Empty;

            switch (severtityLevel)
            {
                case "1":
                    severity = NotificationLevel.Alert_Level1.ToString();
                    break;
                case "2":
                    severity = NotificationLevel.Alert_Level2.ToString();
                    break;
                case "3":
                    severity = NotificationLevel.Alert_Level3.ToString();
                    break;
                case "4":
                    severity = NotificationLevel.Alert_Level4.ToString();
                    break;
                case "5":
                    severity = NotificationLevel.Alert_Level5.ToString();
                    break;
                default:
                    severity = NotificationLevel.Error.ToString();
                    break;
            }

            return severity;
        }
    }
}
