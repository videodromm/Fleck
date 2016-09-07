using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//namespace WebSockets.Server

namespace VDWebsocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Store the subscribed clients.
            var clients = new List<IWebSocketConnection>();

            // Initialize the WebSocket server connection.
            var server = new WebSocketServer("ws://127.0.0.1:8181");

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    // Add the incoming connection to our list.
                    clients.Add(socket);

                    // Inform the others that someone has just joined the conversation.
                    foreach (var client in clients)
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
                    // Remove the disconnected client from the list.
                    clients.Remove(socket);

                    // Inform the others that someone left the conversation.
                    foreach (var client in clients)
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
                    foreach (var client in clients)
                    {
                        client.Send(socket.ConnectionInfo.Id + " says: <strong>" + message + "</strong>");
                    }
                };
            });

            // Wait for a key press to close...
            Console.ReadLine();
        }
    }
}
