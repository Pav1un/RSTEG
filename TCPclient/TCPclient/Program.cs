using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCPclient
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient("192.168.0.2", 1786);
            string parametrs =  args[0] + " " + args[1];
            try {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "ReadingPackage.exe";
                process.StartInfo.Arguments = parametrs;
                process.Start();
            }
            catch (Exception e) {
                Console.WriteLine("error.. {0}", e.ToString());
            }
            bool done = false;
            Console.WriteLine("Enter bye to end..");
            while (!done)
            {
                Console.WriteLine("enter your text..");
               // Console.WriteLine(s);
                string mess = Console.ReadLine();

                SendMess(client, mess);
                /*string res = ReadRes(client);
                Console.WriteLine("Res " + res);
                done = res.Equals("BYE");*/
            }


        }

        private static void SendMess(TcpClient client, string mess)
        {
            byte[] bts = Encoding.Unicode.GetBytes(mess);
            client.GetStream().Write(bts, 0, bts.Length);
        }

        private static string ReadRes(TcpClient client)
        {
            byte[] buf = new byte[256];
            int totread = 0;
            do
            {
                int read = client.GetStream().Read(buf, totread, buf.Length - totread);
                totread += read;
            }
            while (client.GetStream().DataAvailable);
            return Encoding.Unicode.GetString(buf, 0, totread);
        }

    }
}
