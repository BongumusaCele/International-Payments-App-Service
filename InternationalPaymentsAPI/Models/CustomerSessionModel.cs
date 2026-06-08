namespace InternationalPaymentsAPI.Models
{
    public class CustomerSessionModel
    {
        public int session_Id { get; set; }

        public int customer_Id { get; set; }

        public DateTime login_Time { get; set; }

        public DateTime? logout_Time { get; set; }

        public bool is_Active { get; set; }

        public string session_Token_Hash { get; set; }

        public DateTime expires_On { get; set; }

        public CustomerModel Customer { get; set; }
    }
}
