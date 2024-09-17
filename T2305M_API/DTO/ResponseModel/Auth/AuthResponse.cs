using System;
namespace T2305M_API.DTO.ResponseModel.Auth
{
	public class AuthResponse
	{
		public string jwt { get; set; }
		public DateTime expired_at { get; set; }
	}
}

