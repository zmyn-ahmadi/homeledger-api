using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers
{
    [Mapper]
    public partial class ExpenseMapper
    {
        public partial ExpenseDTO ToDto(Expense expense);

        public partial Expense ToDomain(ExpenseDTO expenseDto);

        public partial Expense ToEntity(CreateExpenseDTO dto);

        public partial ExpenseDTO UpdateEntity(UpdateExpenseDTO dto, Expense entity);

        public partial ExpenseDTO CreateEntity(ExpenseDTO dto);
    }
}
