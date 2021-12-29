using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Aop.Api.Util
{

    /// <summary>
    /// 支付宝签名类,从阿里的sdk中抽出
    /// </summary>
    public class AlipaySignature
    {
        /** 默认编码字符集 */
        private static string DEFAULT_CHARSET = "utf-8";

        public static string GetSignContent(IDictionary<string, string> parameters)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append("=").Append(value).Append("&");
                }
            }
            string content = query.ToString().Substring(0, query.Length - 1);

            return content;
        }

        public static string RSASign(IDictionary<string, string> parameters, string privateKeyPem, string charset, string signType)
        {
            string signContent = GetSignContent(parameters);

            return RSASignCharSet(signContent, privateKeyPem, charset, signType);
        }

        public static string RSASign(string data, string privateKeyPem, string charset, string signType)
        {
            return RSASignCharSet(data, privateKeyPem, charset, signType);
        }
        ///*
        public static string RSASign(IDictionary<string, string> parameters, string privateKeyPem, string charset, bool keyFromFile, string signType)
        {
            string signContent = GetSignContent(parameters);

            return RSASignCharSet(signContent, privateKeyPem, charset, keyFromFile, signType);
        }

        public static string RSASign(string data, string privateKeyPem, string charset, string signType, bool keyFromFile)
        {
            return RSASignCharSet(data, privateKeyPem, charset, keyFromFile, signType);
        }
        //*/
        public static string RSASignCharSet(string data, string privateKeyPem, string charset, string signType)
        {
            RSACryptoServiceProvider rsaCsp = LoadCertificateFile(privateKeyPem, signType);
            byte[] dataBytes = null;
            if (string.IsNullOrEmpty(charset))
            {
                dataBytes = Encoding.UTF8.GetBytes(data);
            }
            else
            {
                dataBytes = Encoding.GetEncoding(charset).GetBytes(data);
            }


            if ("RSA2".Equals(signType))
            {

                byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA256");

                return Convert.ToBase64String(signatureBytes);

            }
            else
            {
                byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");

                return Convert.ToBase64String(signatureBytes);
            }
        }


        public static string RSASignCharSet(string data, string privateKeyPem, string charset, bool keyFromFile, string signType)
        {

            byte[] signatureBytes = null;
            try
            {
                RSACryptoServiceProvider rsaCsp = null;
                if (keyFromFile)
                {//文件读取
                    rsaCsp = LoadCertificateFile(privateKeyPem, signType);
                }
                else
                {
                    //字符串获取
                    rsaCsp = LoadCertificateString(privateKeyPem, signType);
                }

                byte[] dataBytes = null;
                if (string.IsNullOrEmpty(charset))
                {
                    dataBytes = Encoding.UTF8.GetBytes(data);
                }
                else
                {
                    dataBytes = Encoding.GetEncoding(charset).GetBytes(data);
                }
                if (null == rsaCsp)
                {
                    throw new Exception("您使用的私钥格式错误，请检查RSA私钥配置" + ",charset = " + charset);
                }
                if ("RSA2".Equals(signType))
                {

                    signatureBytes = rsaCsp.SignData(dataBytes, "SHA256");

                }
                else
                {
                    signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("您使用的私钥格式错误，请检查RSA私钥配置" + ",charset = " + charset);
            }
            return Convert.ToBase64String(signatureBytes);
        }


        //public static bool RSACheckV1(IDictionary<string, string> parameters, string publicKeyPem, string charset)
        //{
        //    string sign = parameters["sign"];

        //    parameters.Remove("sign");
        //    parameters.Remove("sign_type");
        //    string signContent = GetSignContent(parameters);
        //    return RSACheckContent(signContent, sign, publicKeyPem, charset, "RSA");
        //}

        //public static bool RSACheckV1(IDictionary<string, string> parameters, string publicKeyPem)
        //{
        //    string sign = parameters["sign"];

        //    parameters.Remove("sign");
        //    parameters.Remove("sign_type");
        //    string signContent = GetSignContent(parameters);

        //    return RSACheckContent(signContent, sign, publicKeyPem, DEFAULT_CHARSET, "RSA");
        //}

        //public static bool RSACheckV1(IDictionary<string, string> parameters, string publicKeyPem, string charset, string signType, bool keyFromFile)
        //{
        //    string sign = parameters["sign"];

        //    parameters.Remove("sign");
        //    parameters.Remove("sign_type");
        //    string signContent = GetSignContent(parameters);
        //    return RSACheckContent(signContent, sign, publicKeyPem, charset, signType, keyFromFile);
        //}

        //public static bool RSACheckV2(IDictionary<string, string> parameters, string publicKeyPem)
        //{
        //    string sign = parameters["sign"];

        //    parameters.Remove("sign");
        //    string signContent = GetSignContent(parameters);

        //    return RSACheckContent(signContent, sign, publicKeyPem, DEFAULT_CHARSET, "RSA");
        //}

        public static bool RSACheckV2(IDictionary<string, string> parameters, string publicKeyPem, string charset)
        {
            string sign = parameters["sign"];

            parameters.Remove("sign");
            string signContent = GetSignContent(parameters);

            return RSACheckContent(signContent, sign, publicKeyPem, charset, "RSA");
        }

        public static bool RSACheckV2(IDictionary<string, string> parameters, string publicKeyPem, string charset, string signType, bool keyFromFile)
        {
            string sign = parameters["sign"];

            parameters.Remove("sign");
            string signContent = GetSignContent(parameters);

            return RSACheckContent(signContent, sign, publicKeyPem, charset, signType, keyFromFile);
        }

        public static bool RSACheckContent(string signContent, string sign, string publicKeyPem, string charset, string signType)
        {

            try
            {
                if (string.IsNullOrEmpty(charset))
                {
                    charset = DEFAULT_CHARSET;
                }


                if ("RSA2".Equals(signType))
                {
                    string sPublicKeyPEM = File.ReadAllText(publicKeyPem);

                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.PersistKeyInCsp = false;
                    rsa.LoadPublicKeyPEM(sPublicKeyPEM);
                    //RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa,sPublicKeyPEM );

                    bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), "SHA256", Convert.FromBase64String(sign));
                    return bVerifyResultOriginal;

                }
                else
                {
                    string sPublicKeyPEM = File.ReadAllText(publicKeyPem);
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.PersistKeyInCsp = false;
                    RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);

                    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), sha1, Convert.FromBase64String(sign));
                    return bVerifyResultOriginal;
                }
            }
            catch
            {
                return false;
            }

        }
        public static bool RSACheckContent(string signContent, string sign, string publicKeyPem, string charset, string signType, bool keyFromFile)
        {

            try
            {
                if (string.IsNullOrEmpty(charset))
                {
                    charset = DEFAULT_CHARSET;
                }

                string sPublicKeyPEM;

                if (keyFromFile)
                {
                    sPublicKeyPEM = File.ReadAllText(publicKeyPem);
                }
                else
                {
                    sPublicKeyPEM = "-----BEGIN PUBLIC KEY-----\r\n";
                    sPublicKeyPEM += publicKeyPem;
                    sPublicKeyPEM += "-----END PUBLIC KEY-----\r\n\r\n";
                }


                if ("RSA2".Equals(signType))
                {
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.PersistKeyInCsp = false;
                    RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);

                    bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), "SHA256", Convert.FromBase64String(sign));
                    return bVerifyResultOriginal;
                }
                else
                {
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.PersistKeyInCsp = false;
                    RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);

                    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), sha1, Convert.FromBase64String(sign));
                    return bVerifyResultOriginal;
                }
            }
            catch
            {
                return false;
            }

        }
        public static bool RSACheckContent(string signContent, string sign, string publicKeyPem, string charset, bool keyFromFile)
        {
            try
            {
                string sPublicKeyPEM;
                if (keyFromFile)
                {
                    sPublicKeyPEM = File.ReadAllText(publicKeyPem);
                }
                else
                {
                    sPublicKeyPEM = "-----BEGIN PUBLIC KEY-----\r\n";
                    sPublicKeyPEM = sPublicKeyPEM + publicKeyPem;
                    sPublicKeyPEM = sPublicKeyPEM + "-----END PUBLIC KEY-----\r\n\r\n";
                }
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.PersistKeyInCsp = false;
                RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                if (string.IsNullOrEmpty(charset))
                {
                    charset = DEFAULT_CHARSET;
                }
                bool bVerifyResultOriginal = rsa.VerifyData(Encoding.GetEncoding(charset).GetBytes(signContent), sha1, Convert.FromBase64String(sign));
                return bVerifyResultOriginal;
            }
            catch (Exception ex)
            {
                string s = ex.Message.ToString();
                return false;
            }

        }

        public static string CheckSignAndDecrypt(IDictionary<string, string> parameters, string alipayPublicKey,
                                             string cusPrivateKey, bool isCheckSign,
                                             bool isDecrypt)
        {
            string charset = parameters["charset"];
            string bizContent = parameters["biz_content"];
            if (isCheckSign)
            {
                if (!RSACheckV2(parameters, alipayPublicKey, charset))
                {
                    throw new Exception("rsaCheck failure:rsaParams=" + parameters);
                }
            }

            if (isDecrypt)
            {
                return RSADecrypt(bizContent, cusPrivateKey, charset, "RSA");
            }

            return bizContent;
        }

        public static string CheckSignAndDecrypt(IDictionary<string, string> parameters, string alipayPublicKey,
                                             string cusPrivateKey, bool isCheckSign,
                                             bool isDecrypt, string signType, bool keyFromFile)
        {
            string charset = parameters["charset"];
            string bizContent = parameters["biz_content"];
            if (isCheckSign)
            {
                if (!RSACheckV2(parameters, alipayPublicKey, charset, signType, keyFromFile))
                {
                    throw new Exception("rsaCheck failure:rsaParams=" + parameters);
                }
            }

            if (isDecrypt)
            {
                return RSADecrypt(bizContent, cusPrivateKey, charset, signType, keyFromFile);
            }

            return bizContent;
        }

        public static string encryptAndSign(string bizContent, string alipayPublicKey,
                                        string cusPrivateKey, string charset, bool isEncrypt,
                                        bool isSign, string signType, bool keyFromFile)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(charset))
            {
                charset = DEFAULT_CHARSET;
            }
            sb.Append("<?xml version=\"1.0\" encoding=\"" + charset + "\"?>");
            if (isEncrypt)
            {// 加密
                sb.Append("<alipay>");
                String encrypted = RSAEncrypt(bizContent, alipayPublicKey, charset, keyFromFile);
                sb.Append("<response>" + encrypted + "</response>");
                sb.Append("<encryption_type>" + signType + "</encryption_type>");
                if (isSign)
                {
                    String sign = RSASign(encrypted, cusPrivateKey, charset, signType, keyFromFile);
                    sb.Append("<sign>" + sign + "</sign>");
                    sb.Append("<sign_type>" + signType + "</sign_type>");
                }
                sb.Append("</alipay>");
            }
            else if (isSign)
            {// 不加密，但需要签名
                sb.Append("<alipay>");
                sb.Append("<response>" + bizContent + "</response>");
                String sign = RSASign(bizContent, cusPrivateKey, charset, signType, keyFromFile);
                sb.Append("<sign>" + sign + "</sign>");
                sb.Append("<sign_type>" + signType + "</sign_type>");
                sb.Append("</alipay>");
            }
            else
            {// 不加密，不加签
                sb.Append(bizContent);
            }
            return sb.ToString();
        }

        public static string encryptAndSign(string bizContent, string alipayPublicKey,
                                        string cusPrivateKey, string charset, bool isEncrypt,
                                        bool isSign)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(charset))
            {
                charset = DEFAULT_CHARSET;
            }
            sb.Append("<?xml version=\"1.0\" encoding=\"" + charset + "\"?>");
            if (isEncrypt)
            {// 加密
                sb.Append("<alipay>");
                String encrypted = RSAEncrypt(bizContent, alipayPublicKey, charset);
                sb.Append("<response>" + encrypted + "</response>");
                sb.Append("<encryption_type>RSA</encryption_type>");
                if (isSign)
                {
                    String sign = RSASign(encrypted, cusPrivateKey, charset, "RSA");
                    sb.Append("<sign>" + sign + "</sign>");
                    sb.Append("<sign_type>RSA</sign_type>");
                }
                sb.Append("</alipay>");
            }
            else if (isSign)
            {// 不加密，但需要签名
                sb.Append("<alipay>");
                sb.Append("<response>" + bizContent + "</response>");
                String sign = RSASign(bizContent, cusPrivateKey, charset, "RSA");
                sb.Append("<sign>" + sign + "</sign>");
                sb.Append("<sign_type>RSA</sign_type>");
                sb.Append("</alipay>");
            }
            else
            {// 不加密，不加签
                sb.Append(bizContent);
            }
            return sb.ToString();
        }

        public static string RSAEncrypt(string content, string publicKeyPem, string charset)
        {
            try
            {
                string sPublicKeyPEM = File.ReadAllText(publicKeyPem);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.PersistKeyInCsp = false;
                RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);
                if (string.IsNullOrEmpty(charset))
                {
                    charset = DEFAULT_CHARSET;
                }
                byte[] data = Encoding.GetEncoding(charset).GetBytes(content);
                int maxBlockSize = rsa.KeySize / 8 - 11; //加密块最大长度限制
                if (data.Length <= maxBlockSize)
                {
                    byte[] cipherbytes = rsa.Encrypt(data, false);
                    return Convert.ToBase64String(cipherbytes);
                }
                MemoryStream plaiStream = new MemoryStream(data);
                MemoryStream crypStream = new MemoryStream();
                Byte[] buffer = new Byte[maxBlockSize];
                int blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
                while (blockSize > 0)
                {
                    Byte[] toEncrypt = new Byte[blockSize];
                    Array.Copy(buffer, 0, toEncrypt, 0, blockSize);
                    Byte[] cryptograph = rsa.Encrypt(toEncrypt, false);
                    crypStream.Write(cryptograph, 0, cryptograph.Length);
                    blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
                }

                return Convert.ToBase64String(crypStream.ToArray(), Base64FormattingOptions.None);
            }
            catch (Exception ex)
            {
                throw new Exception("EncryptContent = " + content + ",charset = " + charset, ex);
            }
        }
        public static string RSAEncrypt(string content, string publicKeyPem, string charset, bool keyFromFile)
        {
            try
            {
                string sPublicKeyPEM;
                if (keyFromFile)
                {
                    sPublicKeyPEM = File.ReadAllText(publicKeyPem);
                }
                else
                {
                    sPublicKeyPEM = "-----BEGIN PUBLIC KEY-----\r\n";
                    sPublicKeyPEM += publicKeyPem;
                    sPublicKeyPEM += "-----END PUBLIC KEY-----\r\n\r\n";
                }
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.PersistKeyInCsp = false;
                RSACryptoServiceProviderExtension.LoadPublicKeyPEM(rsa, sPublicKeyPEM);
                if (string.IsNullOrEmpty(charset))
                {
                    charset = DEFAULT_CHARSET;
                }
                byte[] data = Encoding.GetEncoding(charset).GetBytes(content);
                int maxBlockSize = rsa.KeySize / 8 - 11; //加密块最大长度限制
                if (data.Length <= maxBlockSize)
                {
                    byte[] cipherbytes = rsa.Encrypt(data, false);
                    return Convert.ToBase64String(cipherbytes);
                }
                MemoryStream plaiStream = new MemoryStream(data);
                MemoryStream crypStream = new MemoryStream();
                Byte[] buffer = new Byte[maxBlockSize];
                int blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
                while (blockSize > 0)
                {
                    Byte[] toEncrypt = new Byte[blockSize];
                    Array.Copy(buffer, 0, toEncrypt, 0, blockSize);
                    Byte[] cryptograph = rsa.Encrypt(toEncrypt, false);
                    crypStream.Write(cryptograph, 0, cryptograph.Length);
                    blockSize = plaiStream.Read(buffer, 0, maxBlockSize);
                }

                return Convert.ToBase64String(crypStream.ToArray(), Base64FormattingOptions.None);
            }
            catch (Exception ex)
            {
                throw new  Exception("EncryptContent = " + content + ",charset = " + charset, ex);
            }
        }

        public static string RSADecrypt(string content, string privateKeyPem, string charset, string signType)
        {
            try
            {
                RSACryptoServiceProvider rsaCsp = LoadCertificateFile(privateKeyPem, signType);
                if (string.IsNullOrEmpty(charset))
                {
                    charset = DEFAULT_CHARSET;
                }
                byte[] data = Convert.FromBase64String(content);
                int maxBlockSize = rsaCsp.KeySize / 8; //解密块最大长度限制
                if (data.Length <= maxBlockSize)
                {
                    byte[] cipherbytes = rsaCsp.Decrypt(data, false);
                    return Encoding.GetEncoding(charset).GetString(cipherbytes);
                }
                MemoryStream crypStream = new MemoryStream(data);
                MemoryStream plaiStream = new MemoryStream();
                Byte[] buffer = new Byte[maxBlockSize];
                int blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                while (blockSize > 0)
                {
                    Byte[] toDecrypt = new Byte[blockSize];
                    Array.Copy(buffer, 0, toDecrypt, 0, blockSize);
                    Byte[] cryptograph = rsaCsp.Decrypt(toDecrypt, false);
                    plaiStream.Write(cryptograph, 0, cryptograph.Length);
                    blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                }

                return Encoding.GetEncoding(charset).GetString(plaiStream.ToArray());
            }
            catch (Exception ex)
            {
                throw new  Exception("DecryptContent = " + content + ",charset = " + charset, ex);
            }
        }

        public static string RSADecrypt(string content, string privateKeyPem, string charset, string signType, bool keyFromFile)
        {
            try
            {
                RSACryptoServiceProvider rsaCsp = null;
                if (keyFromFile)
                {
                    //文件读取
                    rsaCsp = LoadCertificateFile(privateKeyPem, signType);
                }
                else
                {
                    //字符串获取
                    rsaCsp = LoadCertificateString(privateKeyPem, signType);
                }
                if (string.IsNullOrEmpty(charset))
                {
                    charset = DEFAULT_CHARSET;
                }
                byte[] data = Convert.FromBase64String(content);
                int maxBlockSize = rsaCsp.KeySize / 8; //解密块最大长度限制
                if (data.Length <= maxBlockSize)
                {
                    byte[] cipherbytes = rsaCsp.Decrypt(data, false);
                    return Encoding.GetEncoding(charset).GetString(cipherbytes);
                }
                MemoryStream crypStream = new MemoryStream(data);
                MemoryStream plaiStream = new MemoryStream();
                Byte[] buffer = new Byte[maxBlockSize];
                int blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                while (blockSize > 0)
                {
                    Byte[] toDecrypt = new Byte[blockSize];
                    Array.Copy(buffer, 0, toDecrypt, 0, blockSize);
                    Byte[] cryptograph = rsaCsp.Decrypt(toDecrypt, false);
                    plaiStream.Write(cryptograph, 0, cryptograph.Length);
                    blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                }

                return Encoding.GetEncoding(charset).GetString(plaiStream.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception("DecryptContent = " + content + ",charset = " + charset, ex);
            }
        }

        private static byte[] GetPem(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = String.Format("-----BEGIN {0}-----\\n", type);
            string footer = String.Format("-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));

            return Convert.FromBase64String(base64);
        }

        private static RSACryptoServiceProvider LoadCertificateFile(string filename, string signType)
        {
            using (System.IO.FileStream fs = System.IO.File.OpenRead(filename))
            {
                byte[] data = new byte[fs.Length];
                byte[] res = null;
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    res = GetPem("RSA PRIVATE KEY", data);
                }
                try
                {
                    RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(res, signType);
                    return rsa;
                }
                catch (Exception ex)
                {
                }
                return null;
            }
        }
        private static RSACryptoServiceProvider LoadCertificateString(string strKey, string signType)
        {
            byte[] data = null;
            //读取带
            //ata = Encoding.Default.GetBytes(strKey);
            data = Convert.FromBase64String(strKey);
            //data = GetPem("RSA PRIVATE KEY", data);
            try
            {
                RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(data, signType);
                return rsa;
            }
            catch (Exception ex)
            {
                //    throw new AopException("EncryptContent = woshihaoren,zheshiyigeceshi,wanerde", ex);
            }
            return null;
        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey, string signType)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);


                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                CspParameters CspParameters = new CspParameters();
                CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;

                int bitLen = 1024;
                if ("RSA2".Equals(signType))
                {
                    bitLen = 2048;
                }

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(bitLen, CspParameters);
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {	//remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }
    }

    //public static class RSACryptoServiceProviderExtension
    //{
    //    #region Methods

    //    /// <summary>Extension method which initializes an RSACryptoServiceProvider from a DER public key blob.</summary>
    //    public static void LoadPublicKeyDER(this RSACryptoServiceProvider provider, byte[] DERData)
    //    {
    //        byte[] RSAData = RSACryptoServiceProviderExtension.GetRSAFromDER(DERData);
    //        byte[] publicKeyBlob = RSACryptoServiceProviderExtension.GetPublicKeyBlobFromRSA(RSAData);
    //        provider.ImportCspBlob(publicKeyBlob);
    //    }

    //    /// <summary>Extension method which initializes an RSACryptoServiceProvider from a DER private key blob.</summary>
    //    public static void LoadPrivateKeyDER(this RSACryptoServiceProvider provider, byte[] DERData)
    //    {
    //        byte[] privateKeyBlob = RSACryptoServiceProviderExtension.GetPrivateKeyDER(DERData);
    //        provider.ImportCspBlob(privateKeyBlob);
    //    }

    //    /// <summary>Extension method which initializes an RSACryptoServiceProvider from a PEM public key string.</summary>
    //    public static void LoadPublicKeyPEM(this RSACryptoServiceProvider provider, string sPEM)
    //    {
    //        byte[] DERData = RSACryptoServiceProviderExtension.GetDERFromPEM(sPEM);
    //        RSACryptoServiceProviderExtension.LoadPublicKeyDER(provider, DERData);
    //    }

    //    /// <summary>Extension method which initializes an RSACryptoServiceProvider from a PEM private key string.</summary>
    //    public static void LoadPrivateKeyPEM(this RSACryptoServiceProvider provider, string sPEM)
    //    {
    //        byte[] DERData = RSACryptoServiceProviderExtension.GetDERFromPEM(sPEM);
    //        RSACryptoServiceProviderExtension.LoadPrivateKeyDER(provider, DERData);
    //    }

    //    /// <summary>Returns a public key blob from an RSA public key.</summary>
    //    internal static byte[] GetPublicKeyBlobFromRSA(byte[] RSAData)
    //    {
    //        byte[] data = null;
    //        UInt32 dwCertPublicKeyBlobSize = 0;
    //        if (RSACryptoServiceProviderExtension.CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING,
    //            new IntPtr((int)CRYPT_OUTPUT_TYPES.RSA_CSP_PUBLICKEYBLOB), RSAData, (UInt32)RSAData.Length, CRYPT_DECODE_FLAGS.NONE,
    //            data, ref dwCertPublicKeyBlobSize))
    //        {
    //            data = new byte[dwCertPublicKeyBlobSize];
    //            if (!RSACryptoServiceProviderExtension.CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING,
    //                new IntPtr((int)CRYPT_OUTPUT_TYPES.RSA_CSP_PUBLICKEYBLOB), RSAData, (UInt32)RSAData.Length, CRYPT_DECODE_FLAGS.NONE,
    //                data, ref dwCertPublicKeyBlobSize))
    //                throw new Win32Exception(Marshal.GetLastWin32Error());
    //        }
    //        else
    //            throw new Win32Exception(Marshal.GetLastWin32Error());
    //        return data;
    //    }

    //    /// <summary>Converts DER binary format to a CAPI CRYPT_PRIVATE_KEY_INFO structure.</summary>
    //    internal static byte[] GetPrivateKeyDER(byte[] DERData)
    //    {
    //        byte[] data = null;
    //        UInt32 dwRSAPrivateKeyBlobSize = 0;
    //        IntPtr pRSAPrivateKeyBlob = IntPtr.Zero;
    //        if (RSACryptoServiceProviderExtension.CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING, new IntPtr((int)CRYPT_OUTPUT_TYPES.PKCS_RSA_PRIVATE_KEY),
    //            DERData, (UInt32)DERData.Length, CRYPT_DECODE_FLAGS.NONE, data, ref dwRSAPrivateKeyBlobSize))
    //        {
    //            data = new byte[dwRSAPrivateKeyBlobSize];
    //            if (!RSACryptoServiceProviderExtension.CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING, new IntPtr((int)CRYPT_OUTPUT_TYPES.PKCS_RSA_PRIVATE_KEY),
    //                DERData, (UInt32)DERData.Length, CRYPT_DECODE_FLAGS.NONE, data, ref dwRSAPrivateKeyBlobSize))
    //                throw new Win32Exception(Marshal.GetLastWin32Error());
    //        }
    //        else
    //            throw new Win32Exception(Marshal.GetLastWin32Error());
    //        return data;
    //    }

    //    /// <summary>Converts DER binary format to a CAPI CERT_PUBLIC_KEY_INFO structure containing an RSA key.</summary>
    //    internal static byte[] GetRSAFromDER(byte[] DERData)
    //    {
    //        byte[] data = null;
    //        byte[] publicKey = null;
    //        CERT_PUBLIC_KEY_INFO info;
    //        UInt32 dwCertPublicKeyInfoSize = 0;
    //        IntPtr pCertPublicKeyInfo = IntPtr.Zero;
    //        if (RSACryptoServiceProviderExtension.CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING, new IntPtr((int)CRYPT_OUTPUT_TYPES.X509_PUBLIC_KEY_INFO),
    //            DERData, (UInt32)DERData.Length, CRYPT_DECODE_FLAGS.NONE, data, ref dwCertPublicKeyInfoSize))
    //        {
    //            data = new byte[dwCertPublicKeyInfoSize];
    //            if (RSACryptoServiceProviderExtension.CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING, new IntPtr((int)CRYPT_OUTPUT_TYPES.X509_PUBLIC_KEY_INFO),
    //                DERData, (UInt32)DERData.Length, CRYPT_DECODE_FLAGS.NONE, data, ref dwCertPublicKeyInfoSize))
    //            {
    //                GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
    //                try
    //                {
    //                    info = (CERT_PUBLIC_KEY_INFO)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(CERT_PUBLIC_KEY_INFO));
    //                    publicKey = new byte[info.PublicKey.cbData];
    //                    Marshal.Copy(info.PublicKey.pbData, publicKey, 0, publicKey.Length);
    //                }
    //                finally
    //                {
    //                    handle.Free();
    //                }
    //            }
    //            else
    //                throw new Win32Exception(Marshal.GetLastWin32Error());
    //        }
    //        else
    //            throw new Win32Exception(Marshal.GetLastWin32Error());
    //        return publicKey;
    //    }

    //    /// <summary>Extracts the binary data from a PEM file.</summary>
    //    internal static byte[] GetDERFromPEM(string sPEM)
    //    {
    //        UInt32 dwSkip, dwFlags;
    //        UInt32 dwBinarySize = 0;

    //        if (!RSACryptoServiceProviderExtension.CryptStringToBinary(sPEM, (UInt32)sPEM.Length, CRYPT_STRING_FLAGS.CRYPT_STRING_BASE64HEADER, null, ref dwBinarySize, out dwSkip, out dwFlags))
    //            throw new Win32Exception(Marshal.GetLastWin32Error());

    //        byte[] decodedData = new byte[dwBinarySize];
    //        if (!RSACryptoServiceProviderExtension.CryptStringToBinary(sPEM, (UInt32)sPEM.Length, CRYPT_STRING_FLAGS.CRYPT_STRING_BASE64HEADER, decodedData, ref dwBinarySize, out dwSkip, out dwFlags))
    //            throw new Win32Exception(Marshal.GetLastWin32Error());
    //        return decodedData;
    //    }

    //    #endregion Methods

    //    #region P/Invoke Constants

    //    /// <summary>Enumeration derived from Crypto API.</summary>
    //    internal enum CRYPT_ACQUIRE_CONTEXT_FLAGS : uint
    //    {
    //        CRYPT_NEWKEYSET = 0x8,
    //        CRYPT_DELETEKEYSET = 0x10,
    //        CRYPT_MACHINE_KEYSET = 0x20,
    //        CRYPT_SILENT = 0x40,
    //        CRYPT_DEFAULT_CONTAINER_OPTIONAL = 0x80,
    //        CRYPT_VERIFYCONTEXT = 0xF0000000
    //    }

    //    /// <summary>Enumeration derived from Crypto API.</summary>
    //    internal enum CRYPT_PROVIDER_TYPE : uint
    //    {
    //        PROV_RSA_FULL = 1
    //    }

    //    /// <summary>Enumeration derived from Crypto API.</summary>
    //    internal enum CRYPT_DECODE_FLAGS : uint
    //    {
    //        NONE = 0,
    //        CRYPT_DECODE_ALLOC_FLAG = 0x8000
    //    }

    //    /// <summary>Enumeration derived from Crypto API.</summary>
    //    internal enum CRYPT_ENCODING_FLAGS : uint
    //    {
    //        PKCS_7_ASN_ENCODING = 0x00010000,
    //        X509_ASN_ENCODING = 0x00000001,
    //    }

    //    /// <summary>Enumeration derived from Crypto API.</summary>
    //    internal enum CRYPT_OUTPUT_TYPES : int
    //    {
    //        X509_PUBLIC_KEY_INFO = 8,
    //        RSA_CSP_PUBLICKEYBLOB = 19,
    //        PKCS_RSA_PRIVATE_KEY = 43,
    //        PKCS_PRIVATE_KEY_INFO = 44
    //    }

    //    /// <summary>Enumeration derived from Crypto API.</summary>
    //    internal enum CRYPT_STRING_FLAGS : uint
    //    {
    //        CRYPT_STRING_BASE64HEADER = 0,
    //        CRYPT_STRING_BASE64 = 1,
    //        CRYPT_STRING_BINARY = 2,
    //        CRYPT_STRING_BASE64REQUESTHEADER = 3,
    //        CRYPT_STRING_HEX = 4,
    //        CRYPT_STRING_HEXASCII = 5,
    //        CRYPT_STRING_BASE64_ANY = 6,
    //        CRYPT_STRING_ANY = 7,
    //        CRYPT_STRING_HEX_ANY = 8,
    //        CRYPT_STRING_BASE64X509CRLHEADER = 9,
    //        CRYPT_STRING_HEXADDR = 10,
    //        CRYPT_STRING_HEXASCIIADDR = 11,
    //        CRYPT_STRING_HEXRAW = 12,
    //        CRYPT_STRING_NOCRLF = 0x40000000,
    //        CRYPT_STRING_NOCR = 0x80000000
    //    }

    //    #endregion P/Invoke Constants

    //    #region P/Invoke Structures

    //    /// <summary>Structure from Crypto API.</summary>
    //    [StructLayout(LayoutKind.Sequential)]
    //    internal struct CRYPT_OBJID_BLOB
    //    {
    //        internal UInt32 cbData;
    //        internal IntPtr pbData;
    //    }

    //    /// <summary>Structure from Crypto API.</summary>
    //    [StructLayout(LayoutKind.Sequential)]
    //    internal struct CRYPT_ALGORITHM_IDENTIFIER
    //    {
    //        internal IntPtr pszObjId;
    //        internal CRYPT_OBJID_BLOB Parameters;
    //    }

    //    /// <summary>Structure from Crypto API.</summary>
    //    [StructLayout(LayoutKind.Sequential)]
    //    struct CRYPT_BIT_BLOB
    //    {
    //        internal UInt32 cbData;
    //        internal IntPtr pbData;
    //        internal UInt32 cUnusedBits;
    //    }

    //    /// <summary>Structure from Crypto API.</summary>
    //    [StructLayout(LayoutKind.Sequential)]
    //    struct CERT_PUBLIC_KEY_INFO
    //    {
    //        internal CRYPT_ALGORITHM_IDENTIFIER Algorithm;
    //        internal CRYPT_BIT_BLOB PublicKey;
    //    }

    //    #endregion P/Invoke Structures

    //    #region P/Invoke Functions

    //    /// <summary>Function for Crypto API.</summary>
    //    [DllImport("advapi32.dll", SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    internal static extern bool CryptDestroyKey(IntPtr hKey);

    //    /// <summary>Function for Crypto API.</summary>
    //    [DllImport("advapi32.dll", SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    internal static extern bool CryptImportKey(IntPtr hProv, byte[] pbKeyData, UInt32 dwDataLen, IntPtr hPubKey, UInt32 dwFlags, ref IntPtr hKey);

    //    /// <summary>Function for Crypto API.</summary>
    //    [DllImport("advapi32.dll", SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    internal static extern bool CryptReleaseContext(IntPtr hProv, Int32 dwFlags);

    //    /// <summary>Function for Crypto API.</summary>
    //    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    internal static extern bool CryptAcquireContext(ref IntPtr hProv, string pszContainer, string pszProvider, CRYPT_PROVIDER_TYPE dwProvType, CRYPT_ACQUIRE_CONTEXT_FLAGS dwFlags);

    //    /// <summary>Function from Crypto API.</summary>
    //    [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    internal static extern bool CryptStringToBinary(string sPEM, UInt32 sPEMLength, CRYPT_STRING_FLAGS dwFlags, [Out] byte[] pbBinary, ref UInt32 pcbBinary, out UInt32 pdwSkip, out UInt32 pdwFlags);

    //    /// <summary>Function from Crypto API.</summary>
    //    [DllImport("crypt32.dll", SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    internal static extern bool CryptDecodeObjectEx(CRYPT_ENCODING_FLAGS dwCertEncodingType, IntPtr lpszStructType, byte[] pbEncoded, UInt32 cbEncoded, CRYPT_DECODE_FLAGS dwFlags, IntPtr pDecodePara, ref byte[] pvStructInfo, ref UInt32 pcbStructInfo);

    //    /// <summary>Function from Crypto API.</summary>
    //    [DllImport("crypt32.dll", SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    internal static extern bool CryptDecodeObject(CRYPT_ENCODING_FLAGS dwCertEncodingType, IntPtr lpszStructType, byte[] pbEncoded, UInt32 cbEncoded, CRYPT_DECODE_FLAGS flags, [In, Out] byte[] pvStructInfo, ref UInt32 cbStructInfo);

    //    #endregion P/Invoke Functions
    //}
}
