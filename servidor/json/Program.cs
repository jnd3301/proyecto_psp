using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SerializeToFile
{
    public class Program
    {
        public static async Task Main()
        {
            // MUESTRA DE UN OBJETO EMAIL
            var email = new Email("jona@correo.com", "rober@correo.com", "Saludo!", "Este es el cuerpo de mi mensaje!");
            string fileName = "Email.json";

            // Create a new instance of the Aes
            Aes myAes = Aes.Create();

            // 1_ ENCRIPTO EL CUERPO DEL ANTERIOR OBJETO EMAIL
            string? original = email.Cuerpo;
            byte[] encrypted = Herramientas.EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);
            
            // 2_ SERIALIZO PARA SU ENVIO EL EMAIL ASYNC
            await Herramientas.SerializarEmailAsync(fileName, email);

            Console.WriteLine();

            // 2_ DESERIALIZO EL OBJETO ENVIADO EMAIL ASYNC 
            await Herramientas.DeserializarEmailAsync(fileName);

            // 1_ DESENCRIPTO EL CUERPO DEL OBJETO EMAIL ENVIADO
            string roundtrip = Herramientas.DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);

            Console.WriteLine();

            //Display the original data and the decrypted data.
            Console.WriteLine("Original:        {0}", original);
            Console.WriteLine("Encriptado:      {0}", Convert.ToBase64String(encrypted, 0, encrypted.Length));
            Console.WriteLine("Desencriptado:   {0}", roundtrip);

        }
    }
}