using System;

namespace Delivery_API.DTOs.Auth
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Mail { get; set; }
    }
}