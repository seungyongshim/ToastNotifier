using Akka.Actor;
using Akka.Event;

namespace BLUECATS.ToastNotifier.Actors
{
    public class EventSubscribeActor : ReceiveActor, IWithUnboundedStash
    {
        public IStash Stash { get; set; }

        public static Props Props(IActorRef notificationActor)
        {
            return Akka.Actor.Props.Create(() => new EventSubscribeActor(notificationActor));
        }

        public EventSubscribeActor(IActorRef notificationActor)
        {
            Receive<Error>(m => { notificationActor.Tell(m.ToString()); });
        }
    }
}
