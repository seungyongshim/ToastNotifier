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

                string localtime = json["@timestamp"].ToString("yyyy-MM-dd HH:mm:ss") + "(" + json["@timestamp"].ToString("ddd") + ")";
                string title = System.Environment.NewLine + json.jsonMessage["monitor_name"] + ": " + json.jsonMessage["trigger_name"];
                string hosts = GetHosts(json.jsonMessage["host_name"].Value);

                var level = json.jsonMessage["severity"].ToString();

                var sb = new StringBuilder();
                StringBuilder message = sb.AppendLine(string.Format($"[{level}] {localtime}"))
                    .AppendLine(title)
                    .AppendLine(hosts);

                string sendMsg = message.ToString();
                if (sendMsg.Length > msgLength)
                    sendMsg = string.Concat(message.ToString().Remove(msgLength), "...");

                notificationActor.Tell(
                    ((NotificationLevel)Enum.Parse(typeof(NotificationLevel), GetSeverity(level), true), sendMsg)
                );
            });
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
