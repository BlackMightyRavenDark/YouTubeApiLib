using System.IO;
using System.Net;
using System.Windows.Forms;

namespace YouTube_API
{
    public class Utils
    {
        public static int HttpsPost(string url, string body, out string responseString)
        {
            responseString = "Client error";
            int res = 400;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentLength = body.Length;
            httpWebRequest.Host = "www.youtube.com";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3591.2 Safari/537.36";
            httpWebRequest.Method = "POST";
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            try
            {
                streamWriter.Write(body);
                streamWriter.Dispose();
            }
            catch
            {
                if (streamWriter != null)
                {
                    streamWriter.Dispose();
                }
                return res;
            }
            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
                try
                {
                    responseString = streamReader.ReadToEnd();
                    streamReader.Dispose();
                    res = (int)httpResponse.StatusCode;
                }
                catch
                {
                    if (streamReader != null)
                    {
                        streamReader.Dispose();
                    }
                    return 400;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
                    responseString = ex.Message;
                    res = (int)httpWebResponse.StatusCode;
                }
            }
            return res;
        }

        public static void SetClipboardText(string text)
        {
            bool res;
            do
            {
                try
                {
                    Clipboard.SetText(text);
                    res = true;
                    return;
                }
                catch
                {
                    res = false;
                }
            } while (!res);
        }

    }
}
