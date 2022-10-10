// See https://aka.ms/new-console-template for more information

using Lib;

if (args.Length == 0)
{
    throw new ArgumentException("Port is not provided");
}

if (!int.TryParse(args[0], out var port))
{
    throw new ArgumentException($"Failed to parse port (value = {args[0]}");
}

await Connections.StartListening(port, "Child Process");