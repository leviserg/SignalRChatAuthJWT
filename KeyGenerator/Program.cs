using System.Security.Cryptography;
using System.Text.Json;

using var key = RSA.Create();
var privateKey = Convert.ToBase64String(key.ExportRSAPrivateKey());
var publicKey = Convert.ToBase64String(key.ExportRSAPublicKey());

Console.WriteLine("public:");
Console.WriteLine(publicKey);
Console.WriteLine("private:");
Console.WriteLine(privateKey);

var json = JsonSerializer.Serialize(new
{
    Public = publicKey,
    Private = privateKey
}, new JsonSerializerOptions { WriteIndented = true });

File.WriteAllText("rsaKey.json", json);
Console.ReadKey();