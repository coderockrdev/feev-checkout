namespace FeevCheckout.Libraries.Interfaces;

public class Parcela
{
    public required int NumeroParcela { get; set; }

    public required string DescricaoSolicitacao { get; set; }

    public required string TxId { get; set; }

    public required string Brcode { get; set; }

    public required string Situacao { get; set; }

    public required string Location { get; set; }

    public required DateTime DataHoraCriacao { get; set; }

    public required DateTime DataVencimento { get; set; }

    public required DateTime DataHoraExpiracao { get; set; }

    public required int DiasValidadeAposVencimento { get; set; }

    public required decimal Valor { get; set; }

    public required decimal PercentualMulta { get; set; }

    public required decimal PercentualJurosMes { get; set; }

    public required decimal PercentualAbatimento { get; set; }

    public required decimal PercentualDesconto { get; set; }

    public required DateTime? DataLimiteDesconto { get; set; }
}

public class IncluirCobrancaPixResponse
{
    public required int CodigoCredor { get; set; }

    public required int CodigoContaCorrente { get; set; }

    public required int? CodigoAcordo { get; set; }

    public required int? CodigoContrato { get; set; }

    public required int? CodigoContratoParceiro { get; set; }

    public required string? CpfCnpjBeneficiario { get; set; }

    public required string? NomeBeneficiario { get; set; }

    public required string CpfCnpjPagador { get; set; }

    public required string NomePagador { get; set; }

    public required List<Parcela> Parcelas { get; set; }
}
