// -----------------------------------------------------------
// This program is private software, based on C# source code.
// To sell or change credits of this software is forbidden,
// except if someone approves it from the LeafyCoding INC. team.
// -----------------------------------------------------------
// Copyrights (c) 2016 Leafy-IRCBot.Quotes INC. All rights reserved.
// -----------------------------------------------------------

#region

using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using IRCBot.Quotes.Classes;

#endregion

namespace IRCBot.Quotes.Handlers
{
    internal static class TwitterHandler
    {
        public static bool TweetSuccess;
        public static string TweetErr = string.Empty;

        public static void SendTweet(string message)
        {
            TweetSuccess = false;
            TweetErr = string.Empty;

            const string URL = "https://api.twitter.com/1.1/statuses/update.json";

            var oauth_consumer_key = Config.Twitter_Consumer;
            var oauth_consumer_secret = Config.Twitter_ConsumerSecret;
            var oauth_token = Config.Twitter_Token;
            var oauth_token_secret = Config.Twitter_TokenSecret;
            const string oauth_version = "1.0";
            const string oauth_signature_method = "HMAC-SHA1";

            var oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            const string baseFormat =
                "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&status={6}";

            var baseString = string.Format(baseFormat, oauth_consumer_key, oauth_nonce, oauth_signature_method,
                oauth_timestamp, oauth_token, oauth_version,
                Uri.EscapeDataString(message));

            string oauth_signature;
            using (
                var hasher =
                    new HMACSHA1(
                        Encoding.ASCII.GetBytes(Uri.EscapeDataString(oauth_consumer_secret) + "&" +
                                                Uri.EscapeDataString(oauth_token_secret)))
                )
            {
                oauth_signature =
                    Convert.ToBase64String(
                        hasher.ComputeHash(
                            Encoding.ASCII.GetBytes("POST&" + Uri.EscapeDataString(URL) + "&" +
                                                    Uri.EscapeDataString(baseString))));
            }

            var authorizationFormat = "OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", "
                                      + "oauth_timestamp=\"{4}\", oauth_token=\"{5}\", oauth_version=\"{6}\"";

            var authorizationHeader = string.Format(authorizationFormat, Uri.EscapeDataString(oauth_consumer_key),
                Uri.EscapeDataString(oauth_nonce),
                Uri.EscapeDataString(oauth_signature), Uri.EscapeDataString(oauth_signature_method),
                Uri.EscapeDataString(oauth_timestamp),
                Uri.EscapeDataString(oauth_token), Uri.EscapeDataString(oauth_version));

            var objHttpWebRequest = (HttpWebRequest) WebRequest.Create(URL);
            objHttpWebRequest.Headers.Add("Authorization", authorizationHeader);
            objHttpWebRequest.Method = "POST";
            objHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            using (var objStream = objHttpWebRequest.GetRequestStream())
            {
                var content = Encoding.ASCII.GetBytes("status=" + Uri.EscapeDataString(message));
                objStream.Write(content, 0, content.Length);
            }

            // ReSharper disable once NotAccessedVariable
            var responseResult = string.Empty;
            try
            {
                var objWebResponse = objHttpWebRequest.GetResponse();
                var objWebResponseStream = objWebResponse.GetResponseStream();
                if (objWebResponseStream != null)
                {
                    var objStreamReader = new StreamReader(objWebResponseStream);
                    // ReSharper disable once RedundantAssignment
                    responseResult = objStreamReader.ReadToEnd();
                    TweetSuccess = true;
                }
            }
            catch (Exception ex)
            {
                TweetSuccess = false;
                TweetErr = ex.Message;
            }
        }
    }
}