namespace Umanhan.Dtos
{
    public class ExpenseTypeDto : BaseDto
    {
        public Guid TypeId { get; set; }

        public string ExpenseTypeName { get; set; }
        public string Group { get; set; }

        public FarmActivityExpenseDto FarmActivityExpense { get; set; }
    }
}
