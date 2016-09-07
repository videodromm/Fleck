using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//namespace WebSockets.Server

namespace VDWebsocketServer
{
    class VDServer
    {
        static void Main(string[] args)
        {
            FleckLog.Level = LogLevel.Debug;
            // Store the subscribed clients.
            var allClients = new List<IWebSocketConnection>();

            // Initialize the WebSocket server connection.
            var server = new WebSocketServer("ws://127.0.0.1:8181");

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("On open!");
                    // Add the incoming connection to our list.
                    allClients.Add(socket);

                    // Inform the others that someone has just joined the conversation.
                    foreach (var client in allClients)
                    {
                        // Check the connection unique ID and display a different welcome message!
                        if (client.ConnectionInfo.Id != socket.ConnectionInfo.Id)
                        {
                            client.Send("<i>" + socket.ConnectionInfo.Id + " joined the conversation.</i>");
                            Console.WriteLine(socket.ConnectionInfo.Id + " joined the conversation.");
                        }
                        else
                        {
                            client.Send("<i>You have just joined the conversation.</i>");
                            Console.WriteLine("You have just joined the conversation.");
                        }
                    }
                };
                socket.OnPing = ping =>
                {
                    Console.WriteLine("pinged");

                };
                socket.OnPong = pong =>
                {
                    Console.WriteLine("ponged");

                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("On close!");
                    // Remove the disconnected client from the list.
                    allClients.Remove(socket);

                    // Inform the others that someone left the conversation.
                    foreach (var client in allClients)
                    {
                        if (client.ConnectionInfo.Id != socket.ConnectionInfo.Id)
                        {
                            client.Send("<i>" + socket.ConnectionInfo.Id + " left the chat room.</i>");
                            Console.WriteLine(socket.ConnectionInfo.Id + " left the chat room.");
                        }
                    }
                };

                socket.OnMessage = message =>
                {
                    // Send the message to everyone!
                    // Also, send the client connection's unique identifier in order to recognize who is who.
                    Console.WriteLine("Received: " + message);
                    allClients.ToList().ForEach(s => s.Send(message));
                };
            });

            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (var socket in allClients.ToList())
                {
                    socket.Send(input);
                }
                input = Console.ReadLine();
            }
        }
    }
}
