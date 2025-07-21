using System.Security.Claims;

namespace JobBoard.Infrastructure.Auth;

public static class AuthHelper
{

    public static void SetTokenInCookie(HttpContext context, string token, JwtSetting jwtSetting)
    {
        var cookieOptions = new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.Strict,
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddMinutes(jwtSetting.ExpirationInMinute)
        };
        context.Response.Cookies.Append("access_token", token, cookieOptions);
    }

    public static Guid GetUserId(HttpContext context)
    {
        var token = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid.TryParse(token, out Guid result);
        return result;
    }
}
