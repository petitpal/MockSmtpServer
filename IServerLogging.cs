using System;

namespace MockSmtpServer
{
    public interface IServerLogging
    {
        void LogError(Exception ex);
        void LogMessage(string message);
    }
}