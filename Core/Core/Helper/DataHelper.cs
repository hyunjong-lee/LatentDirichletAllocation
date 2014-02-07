using Core.Data;
using Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helper
{
    public static class DataHelper
    {
        public static void LoadCorpus(this Parameter parameter)
        {
            var corpus = File.ReadAllLines(parameter.CorpusPath).Skip(1);
            foreach (var rawData in corpus)
            {
                var document = new Document("TEST", rawData);
                if (document.Count == 0) continue;

                parameter.DocumentList.Add(document);
            }
        }
    }
}
