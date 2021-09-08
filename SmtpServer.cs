using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MockSmtpServer
{
    public class SmtpServer
    {
        private readonly IServerLogging _logging = default;

        public SmtpServer(IServerLogging logging)
        {
            _logging = logging;
        }

        public void RunServer(int port, IPAddress ip, string serverDnsName)
        {
            const int PauseTimeMs = 100;
            int queueNumber = 0;

            TcpListener server = null;
            try
            {
                server = new TcpListener(ip, port);
                server.Start();

                _logging.LogMessage($"SMTP Server -> {serverDnsName}:{port} listening");

                while (true)
                {
                    try
                    {
                        using (TcpClient client = server.AcceptTcpClient())
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                WriteMessage(stream, $"220 {serverDnsName}");


                                // 451 timeout
                                // HELO > MAIL FROM: (only one - 501 malformated) + RCPT TO: (can be many)> DATA (503 if mail/rcpt not used first)> (QUIT)
                                // RSET > clear envelope, back to mail from
                                // NOOP > just a response

                                Thread.Sleep(PauseTimeMs);
                                // HELO relay.example.com
                                var clientMessage = ReadMessage(stream);
                                if (clientMessage != "")
                                {
                                    var caller = clientMessage.Split(" ")[1].Replace("\n\r", "");
                                    WriteMessage(stream, $"250 {serverDnsName}, I am glad to meet you");
                                    Thread.Sleep(PauseTimeMs);

                                    // MAIL FROM:<bob@example.com>
                                    clientMessage = ReadMessage(stream);
                                    WriteMessage(stream, "250 Ok");
                                    Thread.Sleep(PauseTimeMs);

                                    // RCPT TO:<alice@example.com>
                                    clientMessage = ReadMessage(stream);
                                    WriteMessage(stream, "250 Ok");
                                    Thread.Sleep(PauseTimeMs);

                                    // DATA
                                    clientMessage = ReadMessage(stream);
                                    WriteMessage(stream, "354 End data with <CR><LF>/<CR><LF>");
                                    Thread.Sleep(PauseTimeMs);

                                    // MAIL data
                                    clientMessage = ReadMessage(stream);
                                    while (!clientMessage.EndsWith(string.Concat(Environment.NewLine, ".", Environment.NewLine)))
                                    {
                                        clientMessage = string.Concat(clientMessage, ReadMessage(stream));
                                    }

                                    queueNumber += 1;
                                    WriteMessage(stream, $"250 Ok: queued as {queueNumber}");

                                    if (queueNumber == int.MaxValue) queueNumber = 0;

                                    WriteMessage(stream, "221 Bye");
                                    _logging.LogMessage("");
                                }
                                Thread.Sleep(PauseTimeMs);
                            }

                            client.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logging.LogError(ex);
                    }

                }
            }
            catch (SocketException e)
            {
                _logging.LogError(e);
            }
            finally
            {
                server.Stop();
            }
        }

        private void WriteMessage(NetworkStream stream, string message, bool eom = true)
        {
            stream.Write(Encoding.ASCII.GetBytes(message));
            if (eom) stream.Write(Encoding.ASCII.GetBytes("\r\n"));
            stream.Flush();
            _logging.LogMessage(message);
        }

        private string ReadMessage(NetworkStream stream)
        {
            const int BufferSize = 4096;

            var buffer = new byte[BufferSize];
            var data = new StringBuilder();

            while (stream.DataAvailable)
            {
                var bytes = stream.Read(buffer, 0, BufferSize);
                data.Append(Encoding.ASCII.GetString(buffer[0..bytes]));
            }

            _logging.LogMessage(data.ToString());
            return data.ToString();
        }
    }
}
