using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using BKSystem.IO;

namespace CardGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port;
            ParseArgs(out port, args);

            ServerConnectionManager.Instance.InitializeServer(IPAddress.Any, port);

            Debug.Log("Waiting for connection...");
            ServerConnectionManager.Instance.ConnectToClient(0);
            Debug.Log("Waiting for connection...");
            ServerConnectionManager.Instance.ConnectToClient(1);
            Debug.Log("Both players connected! Starting game...");

            CGC_SetPlayerID setPlayer0 = new CGC_SetPlayerID(0);
            ServerConnectionManager.Instance.TransmitStream(setPlayer0.PackCommand(), 0);
            CGC_SetPlayerID setPlayer1 = new CGC_SetPlayerID(1);
            ServerConnectionManager.Instance.TransmitStream(setPlayer1.PackCommand(), 1);

            bool playing = true;

            CardGameManager cgManager = new CardGameManager();
            cgManager.RunGameLogic();

            while (playing)
            {

                SGCommand player1Command = ServerConnectionManager.Instance.ReceiveStream(0);
                if (player1Command != null)
                {
                    player1Command.ExecuteCommand(cgManager);
                }

                SGCommand player2Command = ServerConnectionManager.Instance.ReceiveStream(1);
                if (player2Command != null)
                {
                    player2Command.ExecuteCommand(cgManager);
                }

                bool connected = ServerConnectionManager.Instance.Connected();
                playing = connected;
            }


            /* TO-DO list
             *  1. Replace all "CGVisualManager.instance.AddCommand(command)" calls with transmitting
             *      the appropriate bitstream to each client
             *  2. Replace all the scripts in the unity project with these ones
             *  3. Re-build this project using the scripts in the unity folder so both projects use the same files
             *  4. Write a servercommand system similar to CGCommand for
             *     sending requests back to the server
             *  5. Document how everything works!
             *  6. Add wait in main server loop
             * 
             * 
             * 
             * 
             */
        }

        static void ParseArgs(out int port, string[] args)
        {
            port = 8080;
            for(int i = 0; i < args.Length - 1; i++)
            {
                if(args[i] == "-port")
                {
                    i++;
                    port = int.Parse(args[i]);
                }
                if(args[i] == "-cardDB")
                {
                    i++;
                    string path = args[i];
                    if (!path.EndsWith("/"))
                    {
                        path += "/";
                    }
                    if(!path.EndsWith("JSON/"))
                    {
                        path += "JSON/";
                    }
                    CardData.SetJSONPath(path);
                }
            }
        }
    }
}
