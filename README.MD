# MockSmtpServer

## Purpose
For now, this is just a hacked together server implementation so I don't need to send myself 20K emails for test purposes

## Future
Later on I'm intending to fill this out a bit more and impelment it in a half decent way - with some better message handling, ability to output to file, ability to request specific mail server failure types, etc

## Using
It's as simple as just building and running. If you like at Program.cs you'll see the following parameter:

> static void Main(int port = 44368, string ip = "127.0.0.1", string dns = "mocksmtp.com")

Generally you'll only ever want to change the port number - this is the equivalent of port 25 on a real SMTP server. You're unlikely to want to run this on anything other than 127.0.0.1. Note that the dns only impact what the SMTP Server reports back as and has no networking implications. This program uses System.CommandLine.DragonFruit so you can call it from the command line using the parameter names as arguments. For example:

> MockSmtpServer --port 1234 --dns "I wanna call it this"

Your client code you can now call this like a normal SMTP server - assuming SMTP servers usually just spit out emails to the console and ignore them...

> using var client = new SmtpClient("127.0.0.1", 443678);
> client.Send("sender@test.com", "ida@test.com", "Some interesting news!", "It isn't really interesting");

## Warnings
This is very much hacked together in 30 mins for test purposes!
