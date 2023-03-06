//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using System.Text;
//using System.Xml.Serialization;
//using MsgLib;

//namespace AsyncSrv
//{
//    // State object for reading client data asynchronously  
//    public class StateObject
//    {
//        // Client  socket.  
//        public Socket workSocket = null;
//        // Size of receive buffer.   !!!!
//        public const int BufferSize = 10; // 1024;
//        // Receive buffer.  
//        public byte[] buffer = new byte[BufferSize];
//        // Received data string.  
//        public StringBuilder sb = new StringBuilder();
//    }

//    public class AsynchronousSocketListener
//    {
//        private static int PORT = 80;

//        // Thread signal.  
//        public static ManualResetEvent allDone = new ManualResetEvent(false);

//        public static int Main(String[] args)
//        {
//            StartListening();
//            return 0;
//        }

//        public static void StartListening()
//        {
//            // Establish the local endpoint for the socket.  
//            //IPAddress ipAddress = GetLocalIpAddress();
//            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT);

//            // Create a TCP/IP socket.  
//            Socket listener = new Socket(ipAddress.AddressFamily,
//                SocketType.Stream, ProtocolType.Tcp);

//            // Bind the socket to the local endpoint and listen for incoming connections.  
//            try
//            {
//                listener.Bind(localEndPoint);
//                listener.Listen(100);

//                while (true)
//                {
//                    // Set the event to nonsignaled state.  
//                    allDone.Reset();

//                    // Start an asynchronous socket to listen for connections.  
//                    Console.WriteLine("Waiting for a connection at {0}...", localEndPoint);
//                    listener.BeginAccept(
//                        new AsyncCallback(AcceptCallback),
//                        listener);

//                    // Wait until a connection is made before continuing.  
//                    allDone.WaitOne();
//                }

//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.ToString());
//            }

//        }

//        public static void AcceptCallback(IAsyncResult ar)
//        {
//            // Signal the main thread to continue.  
//            allDone.Set();

//            // Get the socket that handles the client request.  
//            Socket listener = (Socket)ar.AsyncState;
//            Socket handler = listener.EndAccept(ar);

//            // Create the state object.  
//            StateObject state = new StateObject();
//            state.workSocket = handler;
//            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
//                new AsyncCallback(ReadCallback), state);
//        }

//        public static void ReadCallback(IAsyncResult ar)
//        {
//            Console.Write("_"); // Trace
//            // Retrieve the state object and the handler socket  
//            // from the asynchronous state object.  
//            StateObject state = (StateObject)ar.AsyncState;
//            Socket handler = state.workSocket;

//            // Read data from the client socket.   
//            int bytesRead = handler.EndReceive(ar);

//            // Gets the amount of data that has been received from the network and 
//            // is available to be read.
//            if (handler.Available > 0)
//            {
//                Console.Write("0"); // Trace
//                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
//                // Not all data received. Get more.  
//                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
//            }
//            else
//            {
//                if (bytesRead > 0)
//                {
//                    Console.Write("1"); // Trace
//                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
//                }
//                if (state.sb.Length > 1)
//                {
//                    Console.WriteLine("2"); // Trace
//                    Console.WriteLine(state.sb.ToString());

//                    byte[] byteArray = Encoding.ASCII.GetBytes(state.sb.ToString());
//                    MemoryStream stream = new MemoryStream(byteArray);
//                    Mensaje recibido = (Mensaje)new XmlSerializer(typeof(Mensaje)).Deserialize(stream);

//                    // All the data has been read from the client. Display it on the console.  
//                    Console.WriteLine("Read {0} bytes from socket.\n{1}",
//                        byteArray.Length, recibido);
//                    // Echo the data back to the client.  
//                    Send(handler, recibido);
//                }
//                else
//                {
//                    // If nothing has been received
//                    Console.Write("3"); // Trace
//                }
//            }

//        }

//        private static void Send(Socket handler, Mensaje data)
//        {
//            // Convert the message
//            XmlSerializer serializer = new XmlSerializer(typeof(Mensaje));
//            Stream stream = new MemoryStream();
//            serializer.Serialize(stream, data);
//            byte[] byteData = ((MemoryStream)stream).ToArray();
//            // Begin sending the data to the remote device.  
//            handler.BeginSend(byteData, 0, byteData.Length, 0,
//            new AsyncCallback(SendCallback), handler);
//        }

//        private static void SendCallback(IAsyncResult ar)
//        {
//            try
//            {
//                // Retrieve the socket from the state object.  
//                Socket handler = (Socket)ar.AsyncState;

//                // Complete sending the data to the remote device.  
//                int bytesSent = handler.EndSend(ar);
//                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

//                handler.Shutdown(SocketShutdown.Both);
//                handler.Close();
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.ToString());
//            }
//        }

//        private static IPAddress GetLocalIpAddress()
//        {
//            List<IPAddress> ipAddressList = new List<IPAddress>();
//            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
//            IPAddress ipAddress = ipHostInfo.AddressList[0];
//            int t = ipHostInfo.AddressList.Length;
//            string ip;
//            for (int i = 0; i < t; i++)
//            {
//                ip = ipHostInfo.AddressList[i].ToString();
//                if (ip.Contains(".") && !ip.Equals("127.0.0.1"))
//                {
//                    ipAddressList.Add(ipHostInfo.AddressList[i]);
//                }
//            }
//            if (ipAddressList.Count == 1)
//            {
//                return ipAddressList[0];
//            }
//            else
//            {
//                int i = 0;
//                foreach (IPAddress ipa in ipAddressList)
//                {
//                    Console.WriteLine($"[{i++}]: {ipa}");
//                }
//                t = ipAddressList.Count - 1;
//                System.Console.Write($"Opción [0-{t}]: ");
//                string s = Console.ReadLine();
//                if (Int32.TryParse(s, out int j))
//                {
//                    if ((j >= 0) && (j <= t))
//                    {
//                        return ipAddressList[j];
//                    }
//                }
//                return null;
//            }
//        }

//    }


//}
using System;
using System.Collections.Generic;
using RestSharp; //dotnet add package RestSharp

namespace TodoApiConsumer
{
    class Program
    {
        private const string BASEURL = "http://localhost/psep/correos.xml";

        static void Main(string[] args)
        {
            TodoItem item1 = GetItems(3);
            Console.WriteLine(item1.Name);

            List<TodoItem> list = GetItems();
            Console.WriteLine(list[0].Name);

            TodoItem item2 = new TodoItem { Name = "Guardar ropa", IsComplete = true };
            Console.WriteLine(item2);
            item2 = PostItem(item2);
            Console.WriteLine(item2);

            TodoItem item3 = GetItems(4);
            bool b = item3.IsComplete;
            item3.IsComplete = !b;
            PutItem(4, item3);
            item3 = GetItems(4);
            if (item3.IsComplete != b)
            {
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("NOK");
            }

            DeleteItem(5);
        }

        private static TodoItem GetItems(int id)
        {
            var client = new RestClient(BASEURL);
            var request = new RestRequest($"/TodoItems/{id}", Method.GET);
            var response = client.Execute(request);
            //Console.WriteLine(response.Content);
            //Console.WriteLine(response.StatusCode);//NotFound|OK
            return TodoItem.FromJson(response.Content);
        }

        private static List<TodoItem> GetItems()
        {
            var client = new RestClient(BASEURL);
            var request = new RestRequest("TodoItems", Method.GET);
            var response = client.Execute(request);
            //Console.WriteLine(response.Content);
            return TodoItem.ListFromJson(response.Content);
        }

        private static TodoItem PostItem(TodoItem item)
        {
            var client = new RestClient(BASEURL);
            var request = new RestRequest("TodoItems", Method.POST);
            //request.AddParameter("data", data);
            request.AddJsonBody(item.ToJson());
            var response = client.Execute(request);
            //Console.WriteLine(response.Content);
            //Console.WriteLine(response.StatusCode);//NotFound|Created
            return TodoItem.FromJson(response.Content);
        }

        private static void PutItem(int id, TodoItem item)
        {
            var client = new RestClient(BASEURL);
            // var request = new RestRequest("TodoItems", Method.PUT);
            // request.AddParameter("id", id);
            // request.AddParameter("data", data);
            var request = new RestRequest($"/TodoItems/{id}", Method.PUT);
            request.AddJsonBody(item.ToJson());
            var response = client.Execute(request);
            //Console.WriteLine(response.StatusCode);//NoContent|BadRequest
        }

        private static void DeleteItem(int id)
        {
            var client = new RestClient(BASEURL);
            var request = new RestRequest($"TodoItems/{id}", Method.DELETE);
            var response = client.Execute(request);
            //Console.WriteLine(response.StatusCode);//NotFound|NoContent
        }
    }
}