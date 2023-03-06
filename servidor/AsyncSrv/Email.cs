using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AsyncSrv
{
    public class Email
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("origen")]
        public string? Origen { get; set; }
        [JsonPropertyName("destino")]
        public string? Destino { get; set; }
        [JsonPropertyName("asunto")]
        public string? Asunto { get; set; }
        [JsonPropertyName("cuerpo")]
        public string? Cuerpo { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Email FromJson(string json)
        {
            return JsonSerializer.Deserialize<Email>(json);
        }

        public static List<Email> ListFromJson(string json)
        {
            return JsonSerializer.Deserialize<List<Email>>(json);
        }
    }
}
