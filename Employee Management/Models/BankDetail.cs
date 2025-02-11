namespace Employee_Management.Models
{
    public class BankDetail
    {
        public int BankDetailId { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        //One-to-One with Employee
        public Employee Employee { get; set;}
    }
}
