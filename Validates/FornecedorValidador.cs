using TesteBluData.Models;

namespace TesteBluData.Validates
{
    public static class FornecedorValidator
    {
        public static string? ValidarFornecedor(Fornecedor fornecedor, Empresa? empresa)
        {
            if (empresa == null)
                return "Empresa inválida.";

            if (string.IsNullOrWhiteSpace(fornecedor.Nome))
                return "Nome do fornecedor é obrigatório.";

            if (string.IsNullOrWhiteSpace(fornecedor.CPFouCNPJ))
                return "CPF ou CNPJ é obrigatório.";

            if (string.Equals(empresa.UF?.Trim(), "PR", StringComparison.OrdinalIgnoreCase) && fornecedor.CPFouCNPJ.Length == 14)
            {
                if (!fornecedor.DataNascimento.HasValue)
                    return "Fornecedor pessoa física precisa informar Data de Nascimento";

                var idade = DateTime.Today.Year - fornecedor.DataNascimento.Value.Year;
                if (fornecedor.DataNascimento.Value > DateTime.Today.AddYears(-idade)) idade--;

                if (idade < 18)
                    return "Fornecedor pessoa física menor de idade não pode ser cadastrado para essa empresa";
            }

            if (fornecedor.CPFouCNPJ.Length <= 14)
            {
                if (!ValidadorCpfCnpj.IsCpf(fornecedor.CPFouCNPJ))
                    return "CPF inválido.";

                if (string.IsNullOrEmpty(fornecedor.RG))
                    return "RG obrigatório para fornecedor pessoa física";

                if (!fornecedor.DataNascimento.HasValue)
                    return "Data de nascimento obrigatória para fornecedor pessoa física";
            }
            else if (fornecedor.CPFouCNPJ.Length == 18)
            {
                if (!ValidadorCpfCnpj.ValidarCNPJ(fornecedor.CPFouCNPJ))
                    return "CNPJ Invalido";
            }

                return null;
        }
    }
}
