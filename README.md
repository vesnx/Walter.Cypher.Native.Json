# Walter.Cypher.Native.Json

The `Walter.Cypher.Native.Json` NuGet package provides a set of custom converters designed to enhance the security and privacy of your .NET applications by protecting sensitive data, especially useful in adhering to GDPR requirements. These converters can be easily integrated with the `System.Text.Json` serialization and deserialization process, obfuscating sensitive information such as IP addresses, strings, dates, and numbers.


## Available Converters

- **GDPRCollectionOfStringConverter**: Protects GDPR sensitive string collections, ideal for personal data like email lists.
- **GDPRIPAddressConverter**: Obfuscates IP addresses to ensure privacy and compliance.
- **GDPRIPAddressListConverter**: Safeguards lists of IP addresses, useful for configuration data or logging.
- **GDPRObfuscatedDateTimeConverter**: Obfuscates dates, suitable for sensitive date information like birthdates or issue dates.
- **GDPRObfuscatedStringConverter**: General purpose obfuscation for single strings, applicable to names, credit card numbers, etc.
- **GDPRObfuscatedIntConverter**: Protects sensitive integer values, such as credit card CCVs.


## Getting Started
To use this package, first install it via NuGet
```c#

Install-Package Walter.Cypher.Native.Json


```

## Use Case: Secure User Profile Serialization
This example demonstrates configuring System.Text.Json to use the provided GDPR converters for both serialization and deserialization processes, ensuring that sensitive data is adequately protected according to GDPR guidelines.

You can integrate the conerters as show below where we configer the json serializer context to use a few of the GDPR converters
```c#

    [JsonSerializable(typeof(UserProfile))]
    [JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Metadata,
            Converters = [typeof(GDPRIPAddressListConverter), typeof(GDPRObfuscatedStringConverter), typeof(GDPRObfuscatedIntConverter),typeof(GDPRObfuscatedDateTimeConverter) ],
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
            WriteIndented = true
)]
    partial class UserProfileDataConverter : System.Text.Json.Serialization.JsonSerializerContext
    {
    }
```
You can then use these converters in a class or record, the bellow sample uses a compination of property names and converters to remove any inferable information from the json string
```c#
    internal record UserProfile
    {
        [JsonPropertyName("a")]
        [JsonConverter(typeof(GDPRObfuscatedStringConverter))]
        public required string Name { get; set; }

        [JsonPropertyName("b")]
        [JsonConverter(typeof(GDPRObfuscatedStringConverter))]
        public required string Email { get; set; }

        [JsonPropertyName("c")]
        [JsonConverter(typeof(GDPRObfuscatedDateTimeConverter))]
        public DateTime DateOfBirth { get; set; }

        [JsonPropertyName("d")]
        [JsonConverter(typeof(GDPRIPAddressListConverter))]
        public List<IPAddress> Devices { get; set; } = [];
    }
```

First we will use DI to integrate the walter framework and use a shared secrt password

 ```c#
    //secure the json using a protected password
    using var service = new ServiceCollection()
                            .AddLogging(option =>
                            {
                                //configure your logging as you would normally do
                                option.AddConsole();
                                option.SetMinimumLevel(LogLevel.Information);
                            })
                            .UseSymmetricEncryption("May$ecr!tPa$$w0rd")                                    
                            .BuildServiceProvider();

    service.AddLoggingForWalter();//enable logging for the Walter framework classes
```

## Serializing and Deserializing Securely
Use the UserProfileDataConverter for serialization and deserialization, ensuring data is encrypted and obfuscated according to the converters' specifications.
 ```c#
            /*
            save to json and store or send to a insecure location the profile to disk. 
            note that data in transit can be read even if TLS secured using proxy or man in the middle. 
             */
            var profile = new UserProfile() { 
                Email = "My@email.com", 
                Name = "Jo Coder", 
                DateOfBirth = new DateTime(2001, 7, 16), 
                Devices=[IPAddress.Parse("192.168.1.1"),IPAddress.Parse("192.168.1.14"), IPAddress.Loopback]
            };

            var json= System.Text.Json.JsonSerializer.Serialize(profile, UserProfileDataConverter.Default.UserProfile);
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MySupperApp");
            var fileName = Path.Combine(directory, "Data1.json");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            //use inversion of control and generate a ILogger without dependency injection
            Inverse.GetLogger("MyConsoleApp")?.LogInformation("Cyphered json:\n{json}", json);


            await File.WriteAllTextAsync(fileName, json).ConfigureAwait(false);
```

## Reading and Validating Data
Read the encrypted JSON from storage or after transmission, and deserialize it back into the UserProfile class, automatically decrypting and validating the data.
 ```c#
var cypheredJson = await File.ReadAllTextAsync("path_to_encrypted_json").ConfigureAwait(false);

if (cypheredJson.IsValidJson<UserProfile>(UserProfileDataConverter.Default.UserProfile, out var user))
{
    // Use the deserialized and decrypted `user` object
}
```

## A working copy for use in a console application

The follwing is a working example of how you could use this in a console application
the application would need the follwing 3 NuGet packages 
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging.Console
- Walter.Cypher.Native.Json" Version

```c# 
 //secure the json using a protected password
 using var service = new ServiceCollection()
                         .AddLogging(option =>
                         {
                             //configure your logging as you would normally do
                             option.AddConsole();
                             option.SetMinimumLevel(LogLevel.Information);
                         })
                         .UseSymmetricEncryption("May$ecr!tPa$$w0rd")                                    
                         .BuildServiceProvider();

 service.AddLoggingForWalter();//enable logging for the Walter framework classes




 //... rest of your code 
 
 /*
 save to json and store or send to a insecure location the profile to disk. 
 Data in transit can be read even if TLS secured using proxy or man in the middle. 

  */
 var profile = new UserProfile() { 
     Email = "My@email.com", 
     Name = "Jo Coder", 
     DateOfBirth = new DateTime(2001, 7, 16), 
     Devices=[IPAddress.Parse("192.168.1.1"),IPAddress.Parse("192.168.1.14"), IPAddress.Loopback]
 };

 var json= System.Text.Json.JsonSerializer.Serialize(profile, UserProfileDataConverter.Default.UserProfile);
 var fileName= Path.GetTempFileName();

 //use inversion of control and generate a ILogger without dependency injection
 Inverse.GetLogger("MyConsoleApp")?.LogInformation("Cyphered json:\n{json}", json);


 await File.WriteAllTextAsync(fileName, json).ConfigureAwait(false);

 //... rest of your code 


 /*
  Read the json back in to a class using this simple extension method
  */
 var cypheredJson= await File.ReadAllTextAsync(fileName).ConfigureAwait(false);

 //use string extension method to generate json from a string
 if (cypheredJson.IsValidJson<UserProfile>(UserProfileDataConverter.Default.UserProfile, out UserProfile? user))
 { 
     //... user is not null and holds decrypted values as the console will show
     Inverse.GetLogger("MyConsoleApp")?.LogInformation("Profile:\n{profile}", user.ToString());
 }

 if(File.Exists(fileName))
 { 

     File.Delete(fileName);
 }

```

Executing the code would generate UserProfile json similar to:
```json
{
  "a": "OUj9ZN5lEiMT7XhJxNPBSltciqz/n3OQ6l/HXtRF0mo=",
  "b": "5UdHMH\u002BlUuBs2O8vIwdh72i0NT7xPSWzR9LTyL6iqW8=",
  "c": "R4mDF\u002Bv\u002ByKPmklBzX6mBvR/XlXAw0XS5QX7lKT4YH6MnZRTxvtEBb6jFoQeoC0LS",
  "d": [
    "3HBOsVj9gM7zI9dPyRWXqEjyChl3y0PY3wMV9bOHsKw=",
    "nqBDETdVp6Oa1OqW87pShahygOij9DCbD\u002BMfuiS4Oeg=",
    "ojUnkX\u002BaVOJ9gegz5DrtLQvmDxGu9g3BNnlHDRE/GaU="
  ]
}
```
Repetative serialization of the UserProfile will always generate differnt json text even if using the same values making it hard to reverse engineer. 

