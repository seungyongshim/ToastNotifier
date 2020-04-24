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

                string localtime = json["@timestamp"].ToString("yyyy-MM-dd HH:mm:ss")+"("+ json["@timestamp"].ToString("ddd")+ ")";
                string title = "[" + json.jsonMessage["site"] + "]" + Regex.Replace(json.jsonMessage["product_names"].ToString(), @"[|\n|\r|\s]", string.Empty)
                                + System.Environment.NewLine + json.jsonMessage["monitor_name"] + ": "+json.jsonMessage["trigger"];
                string summary = Regex.Replace(json.jsonMessage["host"].ToString(), @"[|\n|\r|\s]", string.Empty);

                var sb = new StringBuilder();
                StringBuilder message = sb.AppendLine(string.Format($"{localtime}"))
                    .AppendLine(title)
                    .AppendLine("[Summary] " + summary);

                string sendMsg = message.ToString();
                if (sendMsg.Length > msgLength)
                    sendMsg = string.Concat(message.ToString().Remove(msgLength), "...");
     
                notificationActor.Tell(
                    ((NotificationLevel)Enum.Parse(typeof(NotificationLevel), json.jsonMessage["severity"].ToString(), true), sendMsg)
                );
            });

        }
    }
}
