using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Text;
using System.Net;
using System;

namespace LoLA.Utils
{
    public class WebRequestExt
    {
        private readonly string AppHost = "127.0.0.1";
        private readonly string AppPort;

        private readonly string Username = "riot";
        private readonly string RemotingAuthToken;

        private readonly string Authorization;

        private readonly RemoteCertificateValidationCallback RemoteCertificateValidationCallback = new RemoteCertificateValidationCallback(CertificateValidation);

        public WebRequestExt(string appPort, string remotingAuthToken)
        {
            AppPort = appPort;
            RemotingAuthToken = remotingAuthToken;
            Authorization = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{RemotingAuthToken}"))}";
        }

        public HttpWebRequest CreateRequest(string target)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create($"{Protocol.HTTP}{AppHost}:{AppPort}{target}");
            webRequest.Headers.Add(HttpRequestHeader.Authorization, Authorization);
            webRequest.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
            return webRequest;
        }

        private static bool CertificateValidation(object httpRequestMessage, X509Certificate cert, X509Chain certChain, SslPolicyErrors policyErrors)
        {
            return true;
        }
    }
}
