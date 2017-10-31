using System;

using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;

namespace FJW.Notice.ActiveMQ
{
    public class Receiver : IDisposable
    {

        private readonly IConnection _connect;

        private readonly ISession _session;

        private readonly IMessageConsumer _consumer;

        public Receiver(string name, ActiveConfig config)
        {
            var factory = new ConnectionFactory(config.Url);
            if (string.IsNullOrEmpty(config.Password) || string.IsNullOrEmpty(config.UserName))
            {
                _connect = factory.CreateConnection();
            }
            else
            {
                _connect = factory.CreateConnection(config.UserName, config.Password);
            }
            _session = _connect.CreateSession();
            _consumer = _session.CreateConsumer(new ActiveMQQueue(name), null, false);
        }

        public string Receive()
        {
            var msg = _consumer.Receive();
            var txtMsg = msg as ITextMessage;
            if (txtMsg != null)
            {
                return txtMsg.Text;
            }
            return null;
        }


        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connect != null)
                {
                    _connect.Dispose();
                }
                if (_session != null)
                {
                    _session.Dispose();
                }
                if (_consumer != null)
                {
                    _consumer.Dispose();
                }
            }
        }

        ~Receiver()
        {
            Dispose(false);
        }
    }
}
