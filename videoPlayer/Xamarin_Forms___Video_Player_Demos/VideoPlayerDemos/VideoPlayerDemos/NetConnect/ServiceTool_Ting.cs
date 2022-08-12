using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;

namespace NetCommom_Ting
{
    public class ServiceTool_Ting { }

    public class SrvClient
    {
        public Dictionary<MethodName, MethodType> MethodDic { get; set; }
        public WebClient Client { set; get; }
        public string Token { set; get; }
        public string Host { get; set; }

        public SrvClient(string _Host)
        {
            Host = _Host.TrimEnd('/');
            Client = new WebClient();
            MethodDic = new Dictionary<MethodName, MethodType>
            {
                { MethodName.AiRecognize, MethodType.Post },
                { MethodName.AiStarDetect, MethodType.Post },
                { MethodName.GetToken, MethodType.Post },
            };
        }
        private async Task<ReqReturn> Request(MethodName Method, params UrlParam[] UrlParams)
        {
            var ParamT = ToFullUrl(Method, UrlParams, out string SendParams, out string FileName);

            var ReqMethod = MethodDic[Method];
            SetHeader();

            ReqReturn RetReq;
            if (ParamT == ParamType.String)
                RetReq = await SendAction(SendParams, ReqMethod);
            else
                RetReq = await SendAction(SendParams, ReqMethod, FileName);

            if (!RetReq.IsSuccess)
                return RetReq;

            var Ret = JsonConvert.DeserializeObject<ReqReturn>(RetReq.Result.ToString());

            return Ret;
        }
        private async Task<ReqReturn> SendAction(string ReqUrl, MethodType ReqMethod, string FileName = "")
        {
            try
            {
                byte[] RetBs;

                if (ReqMethod == MethodType.Get)
                    RetBs = FileName == "" ?
                        await Client.DownloadDataTaskAsync(ReqUrl) :
                        await Client.UploadFileTaskAsync(ReqUrl, "GET", FileName);
                else
                    RetBs = FileName == "" ?
                        await Client.UploadDataTaskAsync(ReqUrl, new byte[1]) :
                        await Client.UploadFileTaskAsync(ReqUrl, FileName);

                if (RetBs == null)
                    return new ReqReturn("網路請求失敗");
                var RetStr = Encoding.UTF8.GetString(RetBs);
                return new ReqReturn(true, RetStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ReqReturn(ex.Message);
            }
        }
    
        public async Task<ReqReturn> GetToken(string UserId, string UserPw)
        {
            var Ret = await Request(MethodName.GetToken, new UrlParam("UserId", UserId), new UrlParam("UserPw", UserPw));
            if (Ret.IsSuccess)
                Token = Ret.Result.ToString();
            return Ret;
        }
        public async Task<ReqReturn> UpToRecognize(string FileName)
        {
            var Ret = await Request(MethodName.AiRecognize, new UrlParam("AsUseType", "Recognize"), new UrlParam(FileName));
            return Ret;
        }
        public async Task<ReqReturn> UpToStarDetect(string FileName)
        {
            var Ret = await Request(MethodName.AiStarDetect, new UrlParam("AsUseType", "DetectStars"), new UrlParam(FileName));
            return Ret;
        }
        public void LogOut()
        {
            Token = "";
        }
        private void SetHeader()
        {
            if (Token != "" && Token != null)
            {
                var HeaderKey = "Authorization";
                var SendKKey = "bearer " + Token;

                if (Client.Headers.AllKeys.Contains(HeaderKey))
                    Client.Headers[HeaderKey] = SendKKey;
                else
                    Client.Headers.Add(HeaderKey, SendKKey);
            }
        }
        public ParamType ToFullUrl(MethodName Method, UrlParam[] UrlParams, out string FullUrl, out string FileName)
        {
            FullUrl = Host + "/" + Method.ToString();
            FileName = null;

            var Ret = ParamType.String;
            if (UrlParams.Length > 0)
            {
                var StringParams = UrlParams.Where(Item => Item.Type == ParamType.String);
                var FileParams = UrlParams.Where(Item => Item.Type == ParamType.File);

                if (StringParams.Count() > 0)
                {
                    var SendParams = string.Join("&", StringParams.Select(Item => Item.Key + "=" + Item.Vaule));
                    FullUrl += "?" + SendParams;
                }
                if (FileParams.Count() > 0)
                {
                    Ret = ParamType.File;
                    FileName = FileParams.Select(Item => Item.FileName).ToArray()[0];
                }
            }
            return Ret;
        }
    }
}
