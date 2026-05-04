using System;
using System.Collections.Concurrent;

namespace UserLogin.Services
{
    public class OtpService
    {
        // Stores OTPs in memory (temp storage, not database)
        private readonly ConcurrentDictionary<string, OtpData> _otpStore = new();

        // Generate random 6-digit OTP
        public string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        // Store OTP with email (valid for 5 minutes)
        public void StoreOtp(string email, string otp)
        {
            _otpStore[email.ToLower()] = new OtpData
            {
                Otp = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5)
            };
        }

        // Verify OTP and remove after successful verification
        public bool VerifyOtp(string email, string otp)
        {
            var key = email.ToLower();
            if (_otpStore.TryGetValue(key, out var otpData))
            {
                // Check if OTP matches AND not expired
                if (otpData.Otp == otp && otpData.ExpiryTime > DateTime.UtcNow)
                {
                    _otpStore.TryRemove(key, out _); // Remove after use (one-time)
                    return true;
                }
            }
            return false;
        }

        private class OtpData
        {
            public string Otp { get; set; }
            public DateTime ExpiryTime { get; set; }
        }
    }
}