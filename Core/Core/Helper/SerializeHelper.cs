using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.Helper
{
    public static class SerializeHelper
    {
        public static string Serialize<T>(this T obj) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        public static T Deserialize<T>(this string objXml) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringReader(objXml))
            {
                var obj = serializer.Deserialize(writer);
                return (T)obj;
            }
        }
    }
}
