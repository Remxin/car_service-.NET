﻿using Microsoft.AspNetCore.Identity;

namespace AuthService.Services;

public class PasswordService
{
    private readonly PasswordHasher<object> _hasher = new();

    public string HashPassword(string password)
    {
        return _hasher.HashPassword(null, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}