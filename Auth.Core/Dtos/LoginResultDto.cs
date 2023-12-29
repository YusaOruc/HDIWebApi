﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Dtos
{
    public class LoginResultDto
    {
        public string Role { get; set; }
        public string Token { get; set; }
        public SignInResult SignInResult { get; set; }
    }
}
