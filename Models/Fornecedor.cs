using System.Text.Json.Serialization;

namespace TesteBluData.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }

        [JsonIgnore]
        public Empresa? Empresa { get; set; }
        public string Nome { get; set; } = null!;
        public string CPFouCNPJ { get; set; } = null!;
        public DateTime DataCadastro { get; set; }
        public bool PessoaFisica { get; set; }
        public string? RG { get; set; }
        public DateTime? DataNascimento { get; set; }
        public ICollection<Telefone> Telefones { get; set; } = new List<Telefone>();
    }
}