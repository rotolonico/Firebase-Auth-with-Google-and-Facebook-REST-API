using System;
using System.Net;
using Proyecto26;
using UnityEngine;

/// <summary>
/// Handles calls to the Facebook provider for authentication
/// </summary>
public static class FacebookAuthenticator
{
    private const string ClientId = "[CLIENT_ID]"; //TODO: Change [CLIENT_ID] to your CLIENT_ID
    private const string ClientSecret = "[CLIENT_SECRET]"; //TODO: Change [CLIENT_SECRET] to your CLIENT_SECRET

    /// <summary>
    /// Opens a webpage that prompts the user to sign in 
    /// </summary>
    public static void GetAuthCode(Action<string> callback)
    {
        Application.OpenURL(
            $"https://www.facebook.com/v5.0/dialog/oauth?client_id={ClientId}&redirect_uri=http://localhost:1234/&response_type=code&state=state123");
        HandleWebPage(callback);
    }

    /// <summary>
    /// Exchanges the Auth Code with the user's Access Token
    /// </summary>
    /// <param name="code"> Auth Code </param>
    /// <param name="callback"> What to do after this is successfully executed </param>
    public static void ExchangeAuthCodeWithIdToken(string code, Action<string> callback)
    {
        RestClient.Post(
            $"https://graph.facebook.com/v5.0/oauth/access_token?code={code}&client_id={ClientId}&client_secret={ClientSecret}&redirect_uri=http://localhost:1234/",
            null).Then(
            response =>
            {
                var data =
                    StringSerializationAPI.Deserialize(typeof(FacebookAccessTokenResponse), response.Text) as
                        FacebookAccessTokenResponse;
                callback(data.access_token);
            }).Catch(Debug.Log);
    }

    /// <summary>
    /// In order to retrieve the auth code, we need to set up a web page that fetches the code for us 
    /// </summary>
    /// <param name="callback"> What to do after this is successfully executed </param>
    private static async void HandleWebPage(Action<string> callback)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:1234/");
        listener.Start();
        HttpListenerContext context = await listener.GetContextAsync();
        HttpListenerRequest request = context.Request;
        callback(request.QueryString["code"]);

        HttpListenerResponse response = context.Response;
        string responseString =
            "<HTML><BODY> Authenticated successfully! You can now go back to the App. </BODY></HTML>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        System.IO.Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
        listener.Stop();
    }
}