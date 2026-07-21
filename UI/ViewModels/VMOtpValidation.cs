namespace UI.ViewModels
{
    public class VMOtpValidation
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool FirstLogin { get; set; }
        public string OtpQrCodeUrl { get; set; } = string.Empty;
    }
}
