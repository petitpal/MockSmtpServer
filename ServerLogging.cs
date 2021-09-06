using System;

namespace MockSmtpServer
{
    public class ServerLogging : IServerLogging
    {
        public void LogMessage(string message)
        {
            var originalColour = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{message}");
            Console.ForegroundColor = originalColour;
        }

        public void LogError(Exception ex)
        {
            var originalColour = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error -> {ex.Message}");
            Console.ForegroundColor = originalColour;
        }
    }
}
