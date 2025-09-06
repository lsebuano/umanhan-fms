namespace Umanhan.Dtos
{
    public class ExpenseTypeDto
    {
        public Guid TypeId { get; set; }

        public string ExpenseTypeName { get; set; }

        public FarmActivityExpenseDto FarmActivityExpense { get; set; }
    }
}
