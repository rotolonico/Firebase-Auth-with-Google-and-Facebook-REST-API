using System;

/// <summary>
/// Response object to exchanging the Facebook Auth Code with the Access Token
/// </summary>

[Serializable]
public class FacebookAccessTokenResponse
{
    public string access_token;
}
