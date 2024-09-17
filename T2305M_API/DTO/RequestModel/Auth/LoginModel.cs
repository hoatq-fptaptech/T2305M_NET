using System;
using System.ComponentModel.DataAnnotations;

namespace T2305M_API.DTO.RequestModel.Auth
{
	public class LoginModel
	{
        [Required]
        public string email { get; set; }
        [Required]
        [MinLength(6)]
        public string password { get; set; }
    }
}

