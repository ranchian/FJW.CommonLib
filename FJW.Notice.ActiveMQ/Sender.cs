
using System;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;

namespace FJW.Notice.ActiveMQ
{
    public class Sender: IDisposable
    {
        private readonly IConnection _connect;
        
        private readonly ISession _session;

        private readonly IMessageProducer _producer;
        
        public Sender(string name, ActiveConfig config)
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
            _producer = _session.CreateProducer(new ActiveMQQueue(name));
        }

        public void Send(string json)
        {
            var msg = _producer.CreateTextMessage(json);
            _producer.Send(msg);
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

                if (_producer != null)
                {
                    _producer.Dispose();    
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }


        ~Sender()
        {
            Dispose(false);
        }
    }
}
