using System.Text.Json.Serialization;

namespace TesteBluData.Models
{
    public class Telefone
    {
        public int Id { get; set; }
        public int FornecedorId { get; set; }

        [JsonIgnore]
        public Fornecedor? Fornecedor { get; set; }
        public string Numero { get; set; } = null!;
    }
}