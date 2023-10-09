namespace demo_invoice_processor.Shared
{
    public static class Helpers
    {
        public static int GetQuarter(this DateTime date)
        {
            return (date.Month + 2) / 3;
        }
    }
}
