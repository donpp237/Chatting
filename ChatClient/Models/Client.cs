using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient.Models
{
    public class Client
    {
        /// <summary>
        /// 메시지 수신시 발생하는 Event Delegate
        /// </summary>
        public delegate void ReceivedMessage(string message);
        public event ReceivedMessage ReceivedEvent;

        /// <summary>
        /// 클라이언트 소켓
        /// </summary>
        private TcpClient m_client;

        /// <summary>
        /// 서버 연결
        /// </summary>
        public void Connect(string userName, string ip, int port)
        {
            try
            {
                m_client = new TcpClient();
                m_client.Connect(ip, port);
                Send(userName);

                Thread receiveThread = new Thread(new ThreadStart(Receive));
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 데이터 수신
        /// </summary>
        private void Receive()
        {
            while (m_client != null)
            {
                if (m_client.Connected)
                {
                    try
                    {
                        NetworkStream stream = m_client.GetStream();
                        byte[] buffer = new byte[m_client.ReceiveBufferSize];
                        int bytes = stream.Read(buffer, 0, buffer.Length);
                        if (bytes <= 0)
                            continue;

                        string message = Encoding.Default.GetString(buffer, 0, bytes);
                        ReceivedEvent?.Invoke(message);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 데이터 송신
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            if (message.Length <= 0)
                return;

            try
            {
                NetworkStream stream = m_client.GetStream();
                byte[] buffer = Encoding.Default.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
