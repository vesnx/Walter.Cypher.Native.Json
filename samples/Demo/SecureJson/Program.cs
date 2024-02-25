using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Net;

using Walter;

namespace SecureJson
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //secure the json using a protected password
            using var service = new ServiceCollection()
                                    .AddLogging(option =>
                                    {
                                        //configure your logging as you would normally do
                                        option.AddConsole();
                                        option.SetMinimumLevel(LogLevel.Trace);
                                    })
                                    .UseSymmetricEncryption("May$ecr!tPa$$w0rd")
                                    .BuildServiceProvider();

            service.AddLoggingForWalter();//enable logging for the Walter framework classes




            //... rest of your code 

            /*
            save to json and store or send to a insecure location the profile to disk. 
            Data in transit can be read even if TLS secured using proxy or man in the middle. 

             */
            var profile = new UserProfile()
            {
                Email = "My@email.com",
                Name = "Jo Coder",
                DateOfBirth = new DateTime(2001, 7, 16),
                Devices = [IPAddress.Parse("192.168.1.1"), IPAddress.Parse("192.168.1.14"), IPAddress.Loopback]
            };

            var json = System.Text.Json.JsonSerializer.Serialize(profile, UserProfileDataConverter.Default.UserProfile);
            var fileName = Path.GetTempFileName();

            //use inversion of control and generate a ILogger without dependency injection
            Inverse.GetLogger("MyConsoleApp")?.LogInformation("Cyphered json:\n{json}", json);


            await File.WriteAllTextAsync(fileName, json).ConfigureAwait(false);

            //... rest of your code 


            /*
             Read the json back in to a class using this simple extension method
             */
            var cypheredJson = await File.ReadAllTextAsync(fileName).ConfigureAwait(false);

            //use string extension method to generate json from a string
            if (cypheredJson.IsValidJson<UserProfile>(UserProfileDataConverter.Default.UserProfile, out UserProfile? user))
            {
                //... user is not null and holds decrypted values as the console will show
                Inverse.GetLogger("MyConsoleApp")?.LogInformation("Profile:\n{profile}", user.ToString());
            }

            if (File.Exists(fileName))
            {

                File.Delete(fileName);
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
