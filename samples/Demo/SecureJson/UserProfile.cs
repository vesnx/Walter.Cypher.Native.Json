// ***********************************************************************
// Assembly         : SecureJson
// Author           : Walter Verhoeven
// Created          : Thu 22-Feb-2024
//
// Last Modified By : Walter Verhoeven
// Last Modified On : Thu 22-Feb-2024
// ***********************************************************************
// <copyright file="UserProfile.cs" company="SecureJson">
//     Copyright (c) VESNX SA. All rights reserved.
// </copyright>
// <summary>
// Native AOT compatible json obfuscation.
// </summary>
// ***********************************************************************
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecureJson
{
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
}
