namespace FeevCheckout.Libraries.Interfaces;

public class Boleto
{
    public required int NumeroBoleto { get; set; }

    public required int NumeroParcela { get; set; }

    public required DateTime Vencimento { get; set; }

    public required decimal Valor { get; set; }

    public required string NossoNumero { get; set; }

    public required string LinhaDigitavel { get; set; }

    public required string SituacaoBoleto { get; set; }

    public required string UltimaOcorrenciaBancaria { get; set; }

    public required int CodigoUltimaOcorrenciaBancaria { get; set; }

    public required DateTime DataLimitePagamento { get; set; }

    public required string LinkBoleto { get; set; }

    public required string Banco { get; set; }

    public required string Agencia { get; set; }

    public required string Conta { get; set; }
}

public class InserirFaturaResponse
{
    public required string Evento { get; set; }

    public required string Banco { get; set; }

    public required string Agencia { get; set; }

    public required string Conta { get; set; }

    public required int CodigoFatura { get; set; }

    public required string? CodigoFaturaParceiro { get; set; }

    public required string LinkCarne { get; set; }

    public required string Base64Carne { get; set; }

    public required List<Boleto> Boletos { get; set; }
}
