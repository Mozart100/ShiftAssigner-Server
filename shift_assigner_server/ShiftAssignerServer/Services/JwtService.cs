using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ShiftAssignerServer.Services
{
    public class JwtService
    {

        public record JwtTokenClaims(string UserId, string Role, string Tenant);
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(string key, string issuer, string audience)
        {
            _key = key;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateToken(JwtTokenClaims tokenClaims)
        {
            return GenerateToken(tokenClaims.UserId, tokenClaims.Role, tokenClaims.Tenant);
        }

        public string GenerateToken(string userId, string role, string tenant, int expiryMinutes = 60)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("UserId cannot be null or empty", nameof(userId));
            if (string.IsNullOrEmpty(role))
                throw new ArgumentException("Role cannot be null or empty", nameof(role));
            if (string.IsNullOrEmpty(tenant))
                throw new ArgumentException("Tenant cannot be null or empty", nameof(tenant));


            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role),
                new Claim("tenant", tenant)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public JwtTokenClaims ParseToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));

            var tokenHandler = new JwtSecurityTokenHandler();
            
            if (!tokenHandler.CanReadToken(token))
                throw new ArgumentException("Invalid token format", nameof(token));

            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Extract claims
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var tenantClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "tenant")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new InvalidOperationException("Token does not contain a valid user ID claim");
            if (string.IsNullOrEmpty(roleClaim))
                throw new InvalidOperationException("Token does not contain a valid role claim");
            if (string.IsNullOrEmpty(tenantClaim))
                throw new InvalidOperationException("Token does not contain a valid tenant claim");

            return new JwtTokenClaims(userIdClaim, roleClaim, tenantClaim);
        }

        public JwtTokenClaims ValidateAndParseToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                
                // Extract claims from validated token
                var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;
                var tenantClaim = principal.FindFirst("tenant")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    throw new InvalidOperationException("Token does not contain a valid user ID claim");
                if (string.IsNullOrEmpty(roleClaim))
                    throw new InvalidOperationException("Token does not contain a valid role claim");
                if (string.IsNullOrEmpty(tenantClaim))
                    throw new InvalidOperationException("Token does not contain a valid tenant claim");

                return new JwtTokenClaims(userIdClaim, roleClaim, tenantClaim);
            }
            catch (SecurityTokenException ex)
            {
                throw new UnauthorizedAccessException("Token validation failed", ex);
            }
        }
    }
}
