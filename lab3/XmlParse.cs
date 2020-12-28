using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;

namespace lab3
{
    class XmlParse
    {
        private readonly string Path;
        private readonly string OptiPath;
        public XmlParse(string path, string optiPath)
        {
            Path = path;
            OptiPath = optiPath;
        }
        public Opt MyParseOptions()
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(Path);
                xml.Schemas.Add(null, OptiPath);
                var optionsXml = DeserializationXML(Path);
                return new Opt(optionsXml.SourceDirectory, optionsXml.TargetDirectory, optionsXml.LogDirectory);
            }
            catch
            {
                throw new Exception("error");
            }
        }
        public Opt DeserializationXML(string PathXML)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Opt));

                using (Stream sx = new FileStream(PathXML, FileMode.Open))
                {
                    Opt optix = (Opt)serializer.Deserialize(sx);
                    serializer.Close();
                    return optix;
                }
            }
            catch
            {
                throw new Exception("error");
            }
        }
    }
}