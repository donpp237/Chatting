using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ChatServer.Models
{
    public class AsyncObject
    {
        public byte[] Buffer;
        public Socket WorkingSocket;
        public readonly int BufferSize;

        public AsyncObject(int bufferSize)
        {
            BufferSize = bufferSize;
            Buffer = new byte[BufferSize];
        }

        public void ClearBuffer()
        {
            Array.Clear(Buffer, 0, BufferSize);
        }
    }

    public class Server
    {
        /// <summary>
        /// 메시지 수신시 발생하는 Event Delegate
        /// </summary>
        public delegate void ReceivedMessage(string message);
        public event ReceivedMessage ReceivedEvent;

        /// <summary>
        /// 신규 사용자 접속시 발생하는 Event Delegate
        /// </summary>
        public delegate void ConnectClient();
        public event ConnectClient ConnectClientEvent;

        /// <summary>
        /// 서버 소켓
        /// </summary>
        private Socket m_server;

        /// <summary>
        /// 접속 사용자 대화명 및 소켓 정보
        /// </summary>
        private Dictionary<string, Socket> m_clients;

        public Server()
        {
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 5000);

            m_clients = new Dictionary<string, Socket>();

            m_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            m_server.Bind(serverEP);
            m_server.Listen(10);
            m_server.BeginAccept(AcceptCallback, null);
        }

        public List<string> GetConnectedClients()
        {
            return m_clients.Keys.ToList();
        }

        /// <summary>
        /// 소켓 접속
        /// </summary>
        private void AcceptCallback(IAsyncResult ar)
        {
            Socket client = m_server.EndAccept(ar);
            m_server.BeginAccept(AcceptCallback, null);

            AsyncObject obj = new AsyncObject(1024);
            obj.WorkingSocket = client;

            TcpClient cli = new TcpClient();
            cli.Client = client;

            NetworkStream stream = cli.GetStream();
            byte[] buffer = new byte[cli.ReceiveBufferSize];
            int bytes = stream.Read(buffer, 0, buffer.Length);
            string userName = Encoding.Default.GetString(buffer, 0, bytes);

            m_clients.Add(userName, client);

            ConnectClientEvent?.Invoke();
            client.BeginReceive(obj.Buffer, 0, obj.Buffer.Length, 0, DataReceivced, obj);
        }

        /// <summary>
        /// 데이터 수신
        /// </summary>
        private void DataReceivced(IAsyncResult ar)
        {
            AsyncObject obj = (AsyncObject)ar.AsyncState;
            Socket client = obj.WorkingSocket;
            try
            {
                int received = client.EndReceive(ar);
                if (received <= 0)
                {
                    client.Close();
                    return;
                }
            }
            catch
            {
                KeyValuePair<string, Socket> foundClient = m_clients.FirstOrDefault((x) => x.Value == client);
                if (foundClient.Equals(default) == false)
                    m_clients.Remove(foundClient.Key);

                ConnectClientEvent?.Invoke();
                return;
            }

            try
            {
                string text = Encoding.Default.GetString(obj.Buffer);

                obj.ClearBuffer();
                client.BeginReceive(obj.Buffer, 0, obj.Buffer.Length, 0, DataReceivced, obj);

                ReceivedEvent?.Invoke(text);
                Send(text, client);
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// 데이터 송신
        /// </summary>
        public void Send(string message, Socket exceptSocket = null)
        {
            if (message.Length <= 0)
                return;

            try
            {
                byte[] msg = Encoding.Default.GetBytes(message);

                for (int idx = 0; idx < m_clients.Count; idx++)
                {
                    Socket clientSocket = m_clients.ElementAt(idx).Value;
                    if ((exceptSocket != null)
                        && (clientSocket == exceptSocket))
                        continue;

                    clientSocket.Send(msg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
