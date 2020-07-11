#region API 참조
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
#endregion

namespace Server
{
    public class JsonController
    {
        #region 변수
        private string path;
        #endregion

        #region 생성자
        public JsonController(string fileName, string dirPath = "")
        {
            var dir = Path.Combine(Environment.CurrentDirectory) + "\\";
            if (dir != "")
                dir = Path.Combine(Environment.CurrentDirectory) + "\\" + dirPath + "\\";
            path = dir + fileName + ".json";
            var di = new DirectoryInfo(dir);
            if (!di.Exists)
                di.Create();
        }
        #endregion

        #region 쓰기
        public async Task Write(JObject obj)
        {
            File.WriteAllText(path, obj.ToString());
            
            using (StreamWriter file = File.CreateText(path))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                await obj.WriteToAsync(writer);
            }
        }
        #endregion

        #region 읽기
        public string Read()
        {
            try
            {
                var di = new FileInfo(path);
                if (!di.Exists)
                    return null;

                File.ReadAllText(path);

                using (StreamReader reader = File.OpenText(path))
                {
                    string read = reader.ReadToEnd();
                    return read;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}