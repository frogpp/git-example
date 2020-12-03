using Newtonsoft.Json.Linq;
using SDO.Models;
using System;
using System.Dynamic;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Http;

namespace SDO
{
    public class testSig
    {
        public string signB64 { get; set; }
        public string certB64 { get; set; }
        public string tbs { get; set; }
    }

    public class HiPKIController : ApiController
    {
        // POST api/<controller>
        /// <summary>
        /// 驗證IC卡加簽資訊
        /// </summary>
        /// <param name="value"></param>
        public RtnResultModel Post([FromBody]string sigResult)
        {
            JObject ret = JObject.Parse(sigResult);
            SignedCms signedCms = new SignedCms();
            signedCms.Decode(Convert.FromBase64String(ret["signature"].ToString()));
            try
            {
                signedCms.CheckSignature(true);
                if (signedCms.SignerInfos.Count > 0)
                {
                    SignerInfo si = signedCms.SignerInfos[0];
                    dynamic advAttr = parseAttribute(si);
                    X509Certificate2 x509 = si.Certificate;
                    return new RtnResultModel(true, "", new
                    {
                        tbs = Encoding.UTF8.GetString(signedCms.ContentInfo.Content),
                        x509.Subject,
                        x509.Issuer,
                        x509.SerialNumber,
                        advAttr.signTime,
                        advAttr.cardNumber,
                        x509.NotBefore,
                        x509.NotAfter
                    });
                }
                return new RtnResultModel(false, HttpUtility.HtmlEncode(i18N.Message.R16));
            }
            catch (Exception ex)
            {
                return new RtnResultModel(false, HttpUtility.HtmlEncode(i18N.Message.R16), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        private dynamic parseAttribute(SignerInfo si)
        {
            dynamic rtnAttr = new ExpandoObject();
            foreach (CryptographicAttributeObject attr in si.SignedAttributes)
            {
                AsnEncodedData[] data = new AsnEncodedData[1];
                attr.Values.CopyTo(data, 0);
                switch (attr.Oid.Value)
                {
                    case "1.2.840.113549.1.9.5":
                        rtnAttr.signTime = DateTime.ParseExact(Encoding.UTF8.GetString(data[0].RawData, 2, data[0].RawData.Length - 2), "yyMMddHHmmssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                        break;
                    case "2.16.886.1.100.2.204":
                        rtnAttr.cardNumber = Encoding.UTF8.GetString(data[0].RawData, 2, data[0].RawData.Length - 2);
                        break;
                }

            }
            return rtnAttr;
        }
    }
}
