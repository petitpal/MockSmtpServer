using System.Net;
using System.Net.Mail;

namespace MockSmtpServer
{
    class Program
    {
        /// <summary>
        /// Mock SMTP server - accepts literally anything it can and spits it out to the console.
        /// Designed for basic testing of email functionality, without the need to configure a real SMTP account
        /// </summary>
        /// <param name="port">Port number that the server will run on - defaults to 44368</param>
        /// <param name="ip">IP address for the server - defaults to 127.0.0.1</param>
        /// <param name="dns">Server DNS - defaults to mocksmtp.com</param>
        static void Main(int port = 44368, string ip = "127.0.0.1", string dns = "mocksmtp.com")
        {
            var smtpServer = new SmtpServer(new ServerLogging());
            smtpServer.RunServer(port, IPAddress.Parse(ip), dns);
        }
    }
}
