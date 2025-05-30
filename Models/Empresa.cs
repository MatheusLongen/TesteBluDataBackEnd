namespace TesteBluData.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        public string UF { get; set; } = null!;
        public string NomeFantasia { get; set; } = null!;
        public string CNPJ { get; set; } = null!;
        public ICollection<Fornecedor> Fornecedores { get; set; } = new List<Fornecedor>();
    }
}