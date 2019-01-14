using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace AsyncTest
{
    public partial class ClientForm : Form
    {
        private Socket clientSock;  /* client Socket */
        private Socket cbSock;      /* client Async Callback Socket */
        private byte[] recvBuffer;

        private const int MAXSIZE = 4096;		/* 4096  */
        private string HOST = "127.0.0.1";
        private int PORT = 3333;

        public ClientForm()
        {
            InitializeComponent();
            recvBuffer = new byte[MAXSIZE];

            this.DoInit();
        }

        public void DoInit()
        {
            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.BeginConnect();
        }

        /*----------------------*
		 *		Connection		*
		 *----------------------*/
        public void BeginConnect()
        {

            if (this.tbDebug.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    tbDebug.Text = "서버 접속 대기 중";
                }));
            }
            else
            {
                tbDebug.Text = "서버 접속 대기 중";
            }

            try
            {
                clientSock.BeginConnect(HOST, PORT, new AsyncCallback(ConnectCallBack), clientSock);
            }
            catch (SocketException se)
            {
                /*서버 접속 실패 */

                this.Invoke(new MethodInvoker(delegate
                {
                    tbDebug.Text += "\r\n서버접속 실패하였습니다. " + se.NativeErrorCode;

                }));
                this.DoInit();

            }

        }

        /*----------------------*
		 * ##### CallBack #####	*
		 *		Connection		*
		 *----------------------*/
        private void ConnectCallBack(IAsyncResult IAR)
        {
            try
            {
                // 보류중인 연결을 완성
                Socket tempSock = (Socket)IAR.AsyncState;
                IPEndPoint svrEP = (IPEndPoint)tempSock.RemoteEndPoint;

                if (this.tbDebug.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        tbDebug.Text += "\r\n 서버로 접속 성공 : " + svrEP.Address;
                    }));
                }
                else
                {
                    tbDebug.Text += "\r\n 서버로 접속 성공 : " + svrEP.Address;
                }

                tempSock.EndConnect(IAR);
                cbSock = tempSock;
                cbSock.BeginReceive(this.recvBuffer, 0, recvBuffer.Length, SocketFlags.None,
                                    new AsyncCallback(OnReceiveCallBack), cbSock);
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.NotConnected)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        tbDebug.Text += "\r\n서버 접속 실패 CallBack " + se.Message;
                    }));

                    this.BeginConnect();
                }
            }
        }

        /*----------------------*
		 *		   Send 		*
		 *----------------------*/
        public void BeginSend(string message)
        {
            try
            {
                /* 연결 성공시 */
                if (clientSock.Connected)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(message + "<eof>");
                    clientSock.BeginSend(buffer, 0, buffer.Length, SocketFlags.None,
                                    new AsyncCallback(SendCallBack), message);
                }
            }
            catch (SocketException e)
            {
                tbDebug.Text = "\r\n전송 에러 : " + e.Message;
            }
        }

        /*----------------------*
		 * ##### CallBack #####	*
		 *		   Send 		*
		 *----------------------*/
        private void SendCallBack(IAsyncResult IAR)
        {
            string message = (string)IAR.AsyncState;
            this.Invoke(new MethodInvoker(delegate
            {
                tbDebug.Text += "\r\n전송 완료 CallBack : " + message;
            }));
        }

        /*----------------------*
		 *		  Receive 		*
		 *----------------------*/
        public void Receive()
        {
            cbSock.BeginReceive(this.recvBuffer, 0, recvBuffer.Length,
                SocketFlags.None, new AsyncCallback(OnReceiveCallBack), cbSock);
        }

        /*----------------------*
		 * ##### CallBack #####	*
		 *		  Receive 		*
		 *----------------------*/
        private void OnReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                //Socket tempSock = (Socket)IAR.AsyncState;
                //int nReadSize = tempSock.EndReceive(IAR);
                //if (nReadSize != 0)
                //{
                //    string message = Encoding.UTF8.GetString(recvBuffer, 0, nReadSize);
                //    this.Invoke(new MethodInvoker(delegate
                //    {
                //        tbDebug.Text += "\r\n서버로 데이터 수신 : " + message;
                //        tbDebug.Text += "\r\n" + nReadSize + "bytes";
                //    }));
                //}
                //this.Receive();
                string content = string.Empty;
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Read data from the client socket
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it it not there, read more data
                    content = state.sb.ToString();
                    if (content.IndexOf("<eof>") > -1)
                    {
                        // All the data has been read from the
                        // client. DIsplay it on the console.

                        Console.WriteLine("Read {0} bytes from socket \nData : {1}", content.Length, content);

                        this.Invoke(new MethodInvoker(delegate
                        {
                            tbDebug.Text += "\r\n서버로 데이터 수신 : " + content;

                        }));


                    }
                    else
                    {
                        // Not all data received. Get more
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(OnReceiveCallBack), state);
                    }
                }



            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.ConnectionReset)
                {
                    this.BeginConnect();
                }
            }
        }

        private void btHello_Click(object sender, EventArgs e)
        {
            this.BeginSend("HELLO");
        }

        private void btHI_Click(object sender, EventArgs e)
        {
            this.BeginSend("HI");
        }

        private void btnAn_Click(object sender, EventArgs e)
        {
            this.BeginSend("안녕한글도 되니? ※");
        }
    }

    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}
