using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPcap;
using PacketDotNet;
using SharpPcap.LibPcap;
using NetCard;

namespace ReadingPackage
{
    class BaseRead
    {
        private static Byte[] KeyWord;
        private static string StegWord;
        static void Main(string[] args)
        {
            if (args.Length == 0){
                Console.WriteLine("no KeyWord.. by by..");
                Environment.Exit(0);
            }

            var devices = CaptureDeviceList.Instance;
            int numberDevice = -1;
            StegWord = args[1];
            foreach (var dev in devices) {
                Console.WriteLine(dev);
            }

            Console.WriteLine("enter device number.. (see count of flag) ");
            numberDevice = Convert.ToInt32(Console.ReadLine());
            if (numberDevice != -1)
            {
                ICaptureDevice Device = devices[numberDevice];

                Encoding EncodUnicode = Encoding.Unicode;
                KeyWord = EncodUnicode.GetBytes(args[0]);

                Device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPaketArrival);

                Device.Open(DeviceMode.Normal);

                Console.WriteLine("Listein {0}", Device.Description);

                Device.StartCapture();

                Console.ReadLine();

                Device.StopCapture();
                Device.Close();
            }
            else {
                Console.WriteLine("Device not active..");
                Environment.Exit(0);
            }
             
          }

          static void device_OnPaketArrival(object sender, CaptureEventArgs e) {
              Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
              var tcpPacket = TcpPacket.GetEncapsulated(packet);
            //  TcpPacket tcpPacket = packet.Extract(typeof(TcpPacket)) as TcpPacket;
              //string KeyWord = "qwe";
              if (tcpPacket != null) {
                  String SPort = tcpPacket.SourcePort.ToString();
                  var DPort = tcpPacket.DestinationPort.ToString();
                  var Data = Encoding.Unicode.GetString(tcpPacket.PayloadData); //Encoding.ASCII.GetString(tcpPacket.PayloadData).ToString();

                  if (OptionTCP.UnsafeCompareByte(tcpPacket.PayloadData, KeyWord))
                  {  //if  if (OptionTCP.UnsafeCompareByte(tcpPacket.PayloadData, KeyWord) && SPort = tcpPacket.SourcePort.ToString() == 1768)
                    Console.WriteLine("eeeeeeeess");
                    try
                    {
                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.StartInfo.FileName = "dropTcpPacket.exe";
                        proc.StartInfo.Arguments = StegWord;
                        proc.Start();
                    }
                    catch (Exception exp) {
                        Console.WriteLine("errro .. {0}", exp.ToString());
                    }
                  }
                  else Console.WriteLine("nnnnnnnnoo: {0}", Encoding.Unicode.GetString(KeyWord));

                  Console.WriteLine("Sport: {0},  DPort: {1}, Data: {2}", SPort, DPort, Data);
                  Console.WriteLine("==========================================================");
       
              }
            
              
          }
        }
    }

   

