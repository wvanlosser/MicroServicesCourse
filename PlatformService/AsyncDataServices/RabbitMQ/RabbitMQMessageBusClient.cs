using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices.RabbitMQ
{
    public class RabbitMQMessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQMessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;

            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _connection.ConnectionShutdown += RabbitMQ_ShutDown;

                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                System.Console.WriteLine($"Connected to Message Bus @ {factory.HostName}:{factory.Port}");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }

        public void Dispose()
        {
            System.Console.WriteLine("--> Message Bus Disposed");

            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ShutDown(object sender, ShutdownEventArgs e)
        {
            System.Console.WriteLine("--> Connection with Message Bus has been Shut Down");
        }

        public void PublishNewPlatform(PlatformPublishedDto platform)
        {
            if (_connection.IsOpen)
            {
                System.Console.WriteLine("--> RabbitMQ Connection Open, sending message ...");

                var message = JsonSerializer.Serialize(platform);
                SendMessage(message);
            }    
            else 
            {
                System.Console.WriteLine("--> RabbitMQ Connection Closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: "trigger", 
                routingKey: "",
                basicProperties: null,
                body: body);

            System.Console.WriteLine($"--> We have sent {message}");
        }
    }
}