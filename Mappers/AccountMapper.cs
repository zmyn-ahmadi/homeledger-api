using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

[Mapper]
public partial class AccountMapper
{
    public partial AccountDTO ToDto(Account entity);

    public partial Account ToEntity(CreateAccountDTO dto);

    public partial void UpdateEntity(int id, UpdateAccountDTO dto, Account entity);
}
