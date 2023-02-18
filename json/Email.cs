using System.Text.Json.Serialization;

public class Email
{
    [JsonPropertyName("email_origen")]
    public string? EmailOrigen { get; set; }

    [JsonPropertyName("email_destino")]
    public string? EmailDestino { get; set; }

    [JsonPropertyName("asunto")]
    public string? Asunto { get; set; }

    [JsonPropertyName("cuerpo")]
    public string? Cuerpo { get; set; }

    public Email(string EmailOrigen, string EmailDestino, string Asunto, string Cuerpo)
    {
        this.EmailOrigen = EmailOrigen;
        this.EmailDestino = EmailDestino;
        this.Asunto = Asunto;
        this.Cuerpo = Cuerpo;
    }
}
