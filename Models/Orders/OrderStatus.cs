namespace Plando.Models.Orders
{
    public enum OrderStatus
    {
        NEW,            // created
        CANCELLED,     // client cancelled order
        IN_PROGRESS,     // laundry is processing order
        FINISHED,       // laundry finished order
        PASSED,         // client picked order from laundry
    }
}