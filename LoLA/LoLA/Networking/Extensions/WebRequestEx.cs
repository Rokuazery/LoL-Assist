using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Text;
using System.Net;
using System;

namespace LoLA.Networking.Extensions
{
    public class WebRequestEx
    {
        private readonly string _appHost = "127.0.0.1";
        private readonly string _appPort;
        private readonly string _username = "riot";
        private readonly string _remotingAuthToken;
        private readonly string _authorization;

        private readonly RemoteCertificateValidationCallback _remoteCertificateValidationCallback = new RemoteCertificateValidationCallback(CertificateValidation);

        public WebRequestEx(string appPort, string remotingAuthToken)
        {
            _appPort = appPort;
            _remotingAuthToken = remotingAuthToken;
            _authorization = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_remotingAuthToken}"))}";
        }

        public HttpWebRequest CreateRequest(string target)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create($"{Protocol.HTTPS}{_appHost}:{_appPort}{target}");
            webRequest.Headers.Add(HttpRequestHeader.Authorization, _authorization);
            webRequest.ServerCertificateValidationCallback = _remoteCertificateValidationCallback;
            return webRequest;
        }

        private static bool CertificateValidation(object httpRequestMessage, X509Certificate cert, X509Chain certChain, SslPolicyErrors policyErrors) => true;
    }
}
