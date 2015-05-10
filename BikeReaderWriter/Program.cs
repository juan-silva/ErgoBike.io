using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using System.Collections;

//
using System;
using System.IO.Ports;


namespace BikeReader
{
    class Program
    {

        static short heartrate = 0;
        static private SerialPort serialPort;
        static long lastSend = 0;
        static long lastRead = 0;
        static int toRead = 0;
        static int read = 0;
        static byte[] re = new byte[18];

        static byte cur_cmd = 0x40;
        static int watt_step = 5;
        static int cur_watts = 100;
        static int next_watts = 0;
        static byte SET_WATTS = 0x51;
        static byte READ_DATA = 0x40;

        static int receiveCount = 100;
        static int expected = 0;
        static int steps = 0;
        static int firstFound = 0;
        static int readFirst = 0;
        static void Main(string[] args)
        {
            Start();
            int stpCtr = 0;
            while (steps < 4000)
            {
                Update();
                if ((steps - stpCtr) > 20)
                {
                    next_watts = cur_watts + watt_step;
                    stpCtr = stpCtr + 20;
                }
            }
        }

        static void Start()
        {
            serialPort = new SerialPort("COM1", 9600);
            serialPort.Close();
            serialPort.Open();
            serialPort.ReadTimeout = 500;
        }

        // Update is called once per frame
        static void Update()
        {


            if (DateTime.Now.Ticks - lastSend > 5000000)
            {
                //UPDATE TIME
                lastSend = DateTime.Now.Ticks;
                byte[] send;

                if(next_watts > 0){
                    //SEND "SET WATTS" COMMAND
                    send = new byte[3];
                    send[0] = SET_WATTS; 
                    send[1] = 0x00;
                    send[2] = (byte)((next_watts) / 5);
                    serialPort.Write(send, 0, 3);
                    expected = 2;
                    cur_cmd = SET_WATTS;
                }
                else{
                    //SEND "READ DATA" COMMAND
                    send = new byte[2];
                    send[0] = READ_DATA;
                    send[1] = 0x00;
                    serialPort.Write(send, 0, 2);
                    expected = 18;
                    cur_cmd = READ_DATA;
                }

                
                receiveCount = 0;
                firstFound = 0;
                //Console.WriteLine("Request Sent");
                steps++;
            }

            try
            {
                if (receiveCount < expected)
                {
                    //UPDATE TIME
                    lastRead = DateTime.Now.Ticks;
                    if (firstFound == 0)
                    {
                        readFirst = serialPort.Read(re, 0, 1);
                        // Console.Write("ReadFirst attempt: ");
                        // Console.WriteLine(re[0] + " " + re[1] + " " + re[2] + " " + re[3] + " " + re[4] + " " + re[5] + " " + re[6] + " " + re[7] + " " + re[8] + " " + re[9] + " " + re[10] + " " + re[11] + " " + re[12] + " " + re[13] + " " + re[14] + " " + re[15] + " " + re[16] + " " + re[17]);
                        if (readFirst > 0 && re[0] == cur_cmd)
                        {
                            firstFound = 1;
                        }
                    }
                    if (firstFound == 1)
                    {
                        //ATTEMPT TO READ ALL Expected BYTES
                        int readNow = serialPort.Read(re, receiveCount, expected - receiveCount);
                        //Console.WriteLine("Read :" + readNow.ToString() + " : Write at position " + receiveCount.ToString());
                        //Console.WriteLine(re[0] + " " + re[1] + " " + re[2] + " " + re[3] + " " + re[4] + " " + re[5] + " " + re[6] + " " + re[7] + " " + re[8] + " " + re[9] + " " + re[10] + " " + re[11] + " " + re[12] + " " + re[13] + " " + re[14] + " " + re[15] + " " + re[16] + " " + re[17]);
                        receiveCount += readNow;

                        if (receiveCount >= expected)
                        {
                            if (cur_cmd == SET_WATTS)
                            {
                                Console.WriteLine("Pos0: " + re[0]);
                                Console.WriteLine("Pos1: " + re[1]);
                                Console.WriteLine("Pos2: " + re[2]);
                                cur_watts = next_watts;
                                next_watts = 0;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("=====Cockpit Information====");
                                Console.WriteLine("Step : " + steps.ToString());
                                Console.WriteLine("Watts: " + re[4]);
                                Console.WriteLine("RPM  : " + re[5]);
                                Console.WriteLine("Speed: " + re[6]);
                                Console.WriteLine("Pulse: " + re[13]);
                                Console.WriteLine("Gear : " + re[15]);


                                //Console.WriteLine(re[0] + " " + re[1] + " " + re[2] + " " + re[3] + " " + re[4] + " " + re[5] + " " + re[6] + " " + re[7] + " " + re[8] + " " + re[9] + " " + re[10] + " " + re[11] + " " + re[12] + " " + re[13] + " " + re[14] + " " + re[15] + " " + re[16] + " " + re[17]);
                                Console.WriteLine("===========================");
                            }
                        }
                    }
                    else
                    {
                        //Console.WriteLine("No useful data");
                    }
                }
            }
            catch (TimeoutException)
            {
            }

        }




    }
}
