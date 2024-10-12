using Base.Models;
using Base.Services;
using BaseApi.Services;
using EarthLib.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Text;

namespace EarthApi.Services
{
#pragma warning disable CA2211 // 非常數欄位不應可見
    public static class _Xp
    {
        //AES & JWT key
        //private const string AesKey = "YourAesKey";
        //private const string JwtKey = "YourJwtKey";

        //Captcha Fid in Cache
        public const string CaptchaCacheFid = "Captcha";

        //cms type msg
        public const string CmsMsg = "M";

        //from config file
        //public static XpConfigDto Config = null!;
        public static bool HasEther = false;

        //dir path
        //public static string NoImagePath = _Fun.DirRoot + "_image/noImage.jpg";
        public static string DirTemplate = _Fun.Dir("_template");
        public static string DirUpload = _Fun.Dir("_upload");
        public static string DirVol = DirUpload2("Vol");

        private static string DirUpload2(string subDir, bool sep = true)
        {
            return DirUpload + subDir + (sep ? _Fun.DirSep : "");
        }

        public static SymmetricSecurityKey GetJwtKey()
        {
            //return _jwtKey16;
            return new(Encoding.UTF8.GetBytes(_Str.PreZero(16, _Http.GetIp(), true)));
        }

        public static async Task<string> GetTodayKeyA(string table, Db? db = null)
        {
            var ym = _Date.NowYm();     //yyyyMM
            var sql = $"select top 1 Id from {table} where Id like '{ym}%' order by Id desc";
            var key = await _Db.GetStrA(sql, null, db);
            key = string.IsNullOrEmpty(key)
                ? "0001"
                : _Str.PreZero(4, Convert.ToInt32(key[6..]) + 1);
            return ym + key;
        }

        public static async Task<FileResult?> ViewActImageA(string key, string ext)
        {
            return await ViewFileA(_XpLib.Config.DirActImage, "ImageFile", key, ext);
        }

        public static string GetVolPhotoPath(string key, string ext)
        {
            return $"{DirVol}PhotoFile_{key}.{ext}";
        }

        private static async Task<FileResult?> ViewFileA(string dir, string fid, string key, string ext)
        {
            var path = $"{dir}{fid}_{key}.{ext}";
            return await _HttpFile.ViewFileA(path, $"{fid}.{ext}");
        }

        /// <summary>
        /// get tx source and related row
        /// </summary>
        /// <param name="isInput">Input or Output</param>
        /// <param name="id">row key</param>
        /// <returns></returns>
        public static async Task<JObject?> GetTxSourceAndRow(bool isInput, string id)
        {
            //get row
            var db = new Db();
            var table = isInput ? "Input" : "Output";
            var row = await db.GetRowA($"select * from dbo.{table} where Id=@Id", new() { "Id", id });

            //get tx source
            if (row != null)
            {
                row["TxLogSource"] = isInput
                    ? await _CntrAccessLog.GetInputLogSourceA(id)
                    : await _CntrAccessLog.GetOutputLogSourceA(id);
            }
            await db.DisposeAsync();
            return row;
        }

        /// <summary>
        /// 寫入一筆 Msg table row
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="isDonate">true(donate), false(volunteer)</param>
        /// <param name="json">前端輸入資料</param>
        /// <returns></returns>
        public static async Task<ResultDto> CreateMsgA(string ctrl, bool isDonate, string json)
        {
            //get userId
            var userId = _Http.GetCookie("UserId");

            //get input row & set IsDonate field
            var json2 = _Str.ToJson(json)!;
            var row = _Json.GetRows0(json2)!;
            row["IsDonate"] = isDonate ? 1 : 0;

            //compare captcha for input and cache
            return (row["_Captcha"]!.ToString() == _Cache.GetStr(userId, CaptchaCacheFid))
                ? await new MsgE(ctrl).CreateA(json2)
                : _Model.GetError("驗證碼輸入錯誤");
        }

    } //class
#pragma warning restore CA2211 // 非常數欄位不應可見
}
