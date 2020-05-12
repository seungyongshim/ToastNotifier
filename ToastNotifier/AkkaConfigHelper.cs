using System;

namespace ToastNotifier
{

    public static class AkkaConfigHelper
    {
        public static int GetValidatedAuthority(Akka.Configuration.Config config)
        {
            var authority = config.GetInt("ui.notification.authority-level");

            if (authority < 1 || authority > 5)
            {
                throw new Exception("authority-level은 1~5까지 지정할 수 있습니다.");
            }

            return authority;
        }
    }

}