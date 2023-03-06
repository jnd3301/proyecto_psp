using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml.Serialization;
using MsgLib;
using System.Security.Cryptography;
using RestSharp;
using System.Text.Json;

namespace AsyncSrv
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.   !!!!
        public const int BufferSize = 10; // 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {
        private const string BASEURL = "https://localhost:7053/";
        private static int PORT = 11000;
        private static String numMenu = "";

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static int Main(String[] args)
        {
            
            StartListening();
            return 0;
        }

        public static void StartListening()
        {
            // Establish the local endpoint for the socket.  
            IPAddress ipAddress = GetLocalIpAddress();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection at {0}...", localEndPoint);
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            Console.Write("_"); // Trace
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            // Gets the amount of data that has been received from the network and 
            // is available to be read.
            if (handler.Available > 0)
            {
                Console.Write("0"); // Trace
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                // Not all data received. Get more.  
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            else
            {
                if (bytesRead > 0)
                {
                    Console.Write("1"); // Trace
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                }
                if (state.sb.Length > 1)
                {
                    Console.WriteLine("2"); // Trace
                    Console.WriteLine(state.sb.ToString());

                    byte[] byteArray = Encoding.ASCII.GetBytes(state.sb.ToString());
                    MemoryStream stream = new MemoryStream(byteArray);
                    Mensaje recibido = (Mensaje)new XmlSerializer(typeof(Mensaje)).Deserialize(stream);

                    // All the data has been read from the client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket.\n{1}",
                        byteArray.Length, recibido);

                    if (numMenu == "1")
                    {
                        Console.WriteLine("Opción1");
                        // Actualizar cuerpo

                    }else if (numMenu == "2"){
                        Console.WriteLine("Opción2");
                        // Borrar mail
                    }else
                    {
                        Console.WriteLine("Opción3");
                    }

                    // Echo the data back to the client. 
                    Send(handler, recibido);
                }
                else
                {
                    // If nothing has been received
                    Console.Write("3"); // Trace
                }
            }

        }

        private static void Send(Socket handler, Mensaje data)
        {
            var client = new RestClient(BASEURL);

            var request = new RestRequest("TodoItems", Method.Get);
            var response = client.Execute(request);
            //Console.WriteLine(response.Content);
            var json = Email.ListFromJson(response.Content);
            Console.WriteLine(json[0].Cuerpo);
            // Enscriptar el cuerp
            var jsonEncrypt = encrypt(json[0].Cuerpo);
            Console.WriteLine(jsonEncrypt);

            // Serializar
            // XmlSerializer serializer = new XmlSerializer(typeof(Mensaje));
            var serializer = JsonSerializer.Serialize(jsonEncrypt);
            Stream stream = new MemoryStream();
            //serializer.Serialize(stream, data);
            byte[] byteData = Encoding.UTF8.GetBytes(serializer);
            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
        }

        private static Aes getAes()
        {
            Aes myAes = Aes.Create();
            byte[] key = Encoding.UTF8.GetBytes("123");
            Array.Resize(ref key, 32);
            byte[] iv = Encoding.UTF8.GetBytes("123");
            Array.Resize(ref iv, 16);
            myAes.Key = key;
            myAes.IV = iv;
            return myAes;
        }

        private static string encrypt(string txt)
        {
            using (Aes myAes = getAes())
            {

                // Encrypt the string to an array of bytes.
                byte[] encrypted = EncryptStringToBytes_Aes(txt, myAes.Key, myAes.IV);
                return System.Text.Encoding.UTF8.GetString(encrypted);
            }
        }

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }


        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static IPAddress GetLocalIpAddress()
        {
            List<IPAddress> ipAddressList = new List<IPAddress>();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            int t = ipHostInfo.AddressList.Length;
            string ip;
            for (int i = 0; i < t; i++)
            {
                ip = ipHostInfo.AddressList[i].ToString();
                if (ip.Contains(".") && !ip.Equals("127.0.0.1"))
                {
                    ipAddressList.Add(ipHostInfo.AddressList[i]);
                }
            }
            if (ipAddressList.Count == 1)
            {
                return ipAddressList[0];
            }
            else
            {
                int i = 0;
                foreach (IPAddress ipa in ipAddressList)
                {
                    Console.WriteLine($"[{i++}]: {ipa}");
                }
                t = ipAddressList.Count - 1;
                System.Console.Write($"Opción [0-{t}]: ");
                string s = Console.ReadLine();
                if (Int32.TryParse(s, out int j))
                {
                    if ((j >= 0) && (j <= t))
                    {
                        return ipAddressList[j];
                    }
                }
                return null;
            }
        }

    }


}