using System;

if (args.Length == 0)
{
    Console.Error.WriteLine("Password argument is required.");
    Environment.Exit(1);
}

Console.Write(BCrypt.Net.BCrypt.HashPassword(args[0]));
