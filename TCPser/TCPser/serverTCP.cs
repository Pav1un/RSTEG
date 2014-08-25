using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace connectionTCP
{
    class serverTCP
    {
        static void HandleClientThread(object obj)
        {
            TcpClient client = obj as TcpClient;
            bool done = false;
            while (!done)
            {
                string rec = ReadMes(client);
                Console.WriteLine("rec: {0}", rec);
                done = rec.Equals("bye");
               if (done)
                    SendResponse(client, "BYE");
               else
                    SendResponse(client,"OK");
            }
            client.Close();
            Console.WriteLine("Connect close..");
            
        }

        private static string ReadMes(TcpClient client)
        {
            byte[] buf = new byte[256];
            int totRead = 0;
            do
            {
                int read = client.GetStream().Read(buf, totRead, buf.Length - totRead);
                totRead += read;
            }
            while (client.GetStream().DataAvailable);
            return Encoding.Unicode.GetString(buf, 0, totRead);
        }

        private static void SendResponse(TcpClient client, string mess)
        {
            byte[] byt = Encoding.Unicode.GetBytes(mess);
            client.GetStream().Write(byt, 0, byt.Length);
        }
        
        
        
        static void Main(string[] args)
        {

            IPAddress host = IPAddress.Parse("192.168.0.2");
            TcpListener listener = new System.Net.Sockets.TcpListener(host, 1786);
            listener.Start();
            while (true)
            {
                Console.WriteLine("Connecting...");
                TcpClient client = listener.AcceptTcpClient();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleClientThread));
                thread.Start(client);
                if (client.Connected) {
                    try
                    {
                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.StartInfo.FileName = "ReadingPackage.exe";
                        proc.StartInfo.Arguments = args[0];
                        proc.Start();
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.ToString());
                    }
                    
                }
            }
                
        }
    }
}
