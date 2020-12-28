using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace lab3
{
    class JsonPars : IParser
    {
        string path { get; }
        string configPath { get; }
        public JsonPars(string _path, string _configPath)
        {
            path = _path;
            configPath = _configPath;
        }

        public Opt ParseOpt()
        {
            try
            {
                Opt optJson;

                using (FileStream json = new FileStream(path, FileMode.OpenOrCreate))
                {
                    optJson = JsonSerializer.Deserialize<Opt>(json);
                }
                return new Opt(optJson.SourceDirectory, optJson.TargetDirectory, optJson.LogDirectory);

            }
            catch
            {
                throw new Exception("error");
            }
        }
    }
}