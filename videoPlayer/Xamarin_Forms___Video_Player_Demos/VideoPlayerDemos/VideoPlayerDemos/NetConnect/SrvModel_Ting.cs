using System;
using System.Collections.Generic;
using System.Text;

namespace NetCommom_Ting
{
    public class SrvModel_Ting { }
    public class ReqReturn
    {
        public bool IsSuccess { get; set; }

        public object Result { get; set; }

        public string ErrorMsg { get; set; }

        public ReqReturn() { }
        public ReqReturn(bool _IsSuccess, object _Result, string _ErrorMsg = "")
        {
            IsSuccess = _IsSuccess;
            Result = _Result;
            ErrorMsg = _ErrorMsg;
        }
        public ReqReturn(string _ErrorMsg) : this(false, null, _ErrorMsg) { }
    }
    public class UrlParam
    {
        public string Key { get; set; }
        public string Vaule { get; set; }
        public string FileName { get; set; }
        public ParamType Type { get; set; }

        public UrlParam(string _Key, string _Vaule)
        {
            Type = ParamType.String;
            Key = _Key;
            Vaule = _Vaule;
        }
        public UrlParam(string _FileName)
        {
            Type = ParamType.File;
            FileName = _FileName;
        }
    }
    public class RecUser
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public string StarId { get; set; }
        public string StarName { get; set; }
        public string StarScore { get; set; }
        public RecUser() { }
    }

    public class RecStar
    {
        public RecStar() { }
        public string UserName { get; set; }
        public string UserId { get; set; }
    }

    public enum MethodName
    {
        AiRecognize,
        AiStarDetect,
        GetToken,
    }
    public enum MethodType
    {
        Get,
        Post,
    }
    public enum ParamType
    {
        String,
        File,
    }
}
