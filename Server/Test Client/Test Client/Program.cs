using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Test_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Attempting to connect to server localhost at port 8080");
            Console.WriteLine("Press any key to connect.");
            Console.ReadKey();

            ///TcpClient client = StartClient("localhost", 8080);
            TcpClient client = new TcpClient("localhost", 8080);

            // Tell the server you've connected and want to play
            SendStringToServer(client, "Hello!");
            
            while (true)
            {
                byte[] receivedBuffer = new byte[100];
                NetworkStream stream = client.GetStream();

                stream.Read(receivedBuffer, 0, receivedBuffer.Length);

                string msg = Encoding.ASCII.GetString(receivedBuffer, 0, receivedBuffer.Length);

                Console.WriteLine("Message received from server: " + msg);

                if (msg[0] != 0)
                {
                    break;
                }
            }

            Console.ReadKey();

        }

        static TcpClient StartClient(string address, int port)
        {
            TcpClient client = new TcpClient(address, port);
            return client;
        }

        static void SendStringToServer(TcpClient client, string msg)
        {
            Console.WriteLine("Sending message to server: " + msg);
            int byteCount = Encoding.ASCII.GetByteCount(msg);
            byte[] sendData = new byte[byteCount];
            sendData = Encoding.ASCII.GetBytes(msg);
            NetworkStream stream = client.GetStream();
            stream.Write(sendData, 0, sendData.Length);
        }
    }
}
