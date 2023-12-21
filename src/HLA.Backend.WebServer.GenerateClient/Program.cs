// See https://aka.ms/new-console-template for more information
using HLA.Backend.WebServer.GenerateClient;
using System.Security.Cryptography;
using System.Text;

Guid guid = Guid.NewGuid();
Console.WriteLine($"ClientId: {guid.ToString().ToUpper()}");

Console.WriteLine($"ClientSecret: {ApiKey.Generate()}");
Console.ReadLine();
