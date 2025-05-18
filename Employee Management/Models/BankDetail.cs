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
        public int EmployeeId { get; set; }  // Foreign Key
        public Employee Employee { get; set;}
    }
}
