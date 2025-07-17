namespace JobBoard.Shared.Auth;

public static class CookieHelper
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
}
