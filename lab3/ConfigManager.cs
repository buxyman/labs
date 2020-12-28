using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    class ConfigManager : IParser
    {
        readonly IParser parser;
        public ConfigManager(string path, string config)
        {
            if (Path.GetExtension(path) == ".xml")
            {
                parser = new XmlParse(path, config);
            }
            else parser = new JsonPars(path);
        }
        public Opt ParseOptions() => parser.ParseOptions();
        public Opt ParsersOptions()
        {
            throw new NotImplementedException();
        }
    }
}