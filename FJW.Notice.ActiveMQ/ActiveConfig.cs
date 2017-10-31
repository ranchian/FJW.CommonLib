
namespace FJW.Notice.ActiveMQ
{
    public class ActiveConfig
    {
        public string Url { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public static ActiveConfig GetDefault()
        {
            return new ActiveConfig { Url = "tcp://192.168.1.11:61616/" };
        }
    }
}
