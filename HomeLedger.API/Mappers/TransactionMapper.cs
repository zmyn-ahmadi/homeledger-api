using HomeLedger.API.DTOs.TransactionDTO;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class TransactionMapper
{
    public partial TransactionDTO ToDto(Transaction entity);

    public partial Transaction ToEntity(TransactionDTO dto);
}
