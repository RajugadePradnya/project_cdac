namespace RapidReachApi.Models
{
    public class ForgotPasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
