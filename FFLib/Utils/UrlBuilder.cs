using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FFLib
{
    public class UrlBuilder
    {
        System.Collections.Specialized.NameValueCollection _qparams = new System.Collections.Specialized.NameValueCollection();
        string _query;

        public UrlBuilder() { }
        public UrlBuilder(string urlString)
        {
            this.Parse(urlString);
        }

        public void Parse(string url)
        {
            string regexPattern = @"^(?<s1>(?<s0>[^:/\?#]+):)?(?<a1>"
                + @"//(?<a0>[^/\?#]*))?(?<p0>[^\?#]*)"
                + @"(?<q1>\?(?<q0>[^#]*))?"
                + @"(?<f1>#(?<f0>.*))?";

            Regex re = new Regex(regexPattern, RegexOptions.ExplicitCapture);
            Match m = re.Match(url);

            Scheme = m.Groups["s0"].Value;
            Host = m.Groups["a0"].Value;
            Path = m.Groups["p0"].Value;
            Query = m.Groups["q0"].Value;
            Fragment = m.Groups["f0"].Value;
        }

        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        /// <summary>
        /// Get/Set URL encoded Query string
        /// </summary>
        public string Query { 
            get{
                List<string>kvl = new List<string>(100);
                foreach (string k in _qparams.AllKeys)
                    kvl.Add(k + "=" + _qparams[k]);
                return string.Join("&", kvl);
            }
            set {
                _query = value;
                if (string.IsNullOrWhiteSpace(value)) { _qparams.Clear(); return; }
                var kvList = value.Split('&');
                foreach(var kvp in kvList){
                    var kv = kvp.Split('=');
                    if (kv == null || kv.Length < 2) continue;
                    if (_qparams.Get(kv[0]) != null) _qparams.Remove(kv[0]);
                    _qparams.Add(kv[0],kv[1]);
                }

            } 
        }
        public string Fragment { get; set; }

        /// <summary>
        /// Set Query Parameter by Key. Value will be encoded automaticly.
        /// </summary>
        public void SetQueryParam(string key, string value)
        {
            _qparams.Add(key, System.Web.HttpUtility.UrlEncode(value));
        }

        /// <summary>
        /// Get Url Decoded Query Parmeter Value by Key.
        /// </summary>
        public string GetQueryParam(string key)
        {
            return System.Web.HttpUtility.UrlDecode(_qparams[key]);
        }

        public override string ToString()
        {
            var result = new StringBuilder(1000);
            if (!string.IsNullOrWhiteSpace(Scheme)) result.Append(Scheme + "://");
            if (Host != null) result.Append(Host);
            if (Path != null) result.Append(Path);
            if (!string.IsNullOrWhiteSpace(Query)) result.Append("?" + Query);
            if (!string.IsNullOrWhiteSpace(Fragment)) result.Append("#" + Fragment);
            return result.ToString();
        }
    }
}
