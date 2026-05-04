using System.Threading.Tasks;

namespace UserLogin.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string email, string otp);
    }
}