using Core.Data;
using Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helper
{
    public static class ModelHelper
    {
        public static T Import<T>(this string path) where T : class
        {
            using (var reader = new StreamReader(Path.Combine(path, typeof(T).Name + ".bin")))
            {
                var compData = reader.ReadToEnd();
                var xmlData = compData.Decompress();
                return xmlData.Deserialize<T>();
            }
        }

        public static void Export<T>(this T obj, string path) where T : class
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            using (var writer = new StreamWriter(Path.Combine(path, typeof(T).Name + ".bin")))
            {
                var xmlData = obj.Serialize();
                var compData = xmlData.Compress();
                writer.WriteLine(compData);
            }
        }

        public static void ExportVoca(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            using (var writer = new StreamWriter(Path.Combine(path, typeof(WordManager).Name + ".bin")))
            {
                foreach (var wordItem in WordManager.WordIterator())
                    writer.WriteLine("{0}:{1}", wordItem.Value, wordItem.Key);
            }
        }

        public static void ImportVoca(string path)
        {
            WordManager.Clear();
            using (var reader = new StreamReader(Path.Combine(path, typeof(WordManager).Name + ".bin")))
            {
                var line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    var idx = line.IndexOf(':');
                    var word = line.Substring(idx + 1);
                    WordManager.ToWordId(word);
                }
            }
        }

        public static List<T> InitializeList<T>(int count)
        {
            var data = new List<T>(count);
            data.AddRange(Enumerable.Repeat<T>(default(T), count));

            return data;
        }

        public static List<List<T>> InitializeMatrix<T>(int row, int col)
        {
            var data = new List<List<T>>(row);
            foreach (var idx in Enumerable.Range(0, row))
                data.Add(Enumerable.Repeat<T>(default(T), col).ToList());

            return data;
        }

        public static void ComputeTheta(this LDAModel model, LDA lda)
        {
            var alpha = lda.Parameter.Alpha;
            var beta = lda.Parameter.Beta;
            var topicCount = lda.Parameter.TopicCount;
            foreach (var docIdx in Enumerable.Range(0, lda.Parameter.DocumentCount))
            {
                foreach (var topicId in Enumerable.Range(0, topicCount))
                {
                    model.Theta[docIdx][topicId] =
                        (model.ND[docIdx][topicId] + alpha) /
                        (model.NDCount(docIdx) + topicCount * alpha);
                }
            }
        }

        public static void ComputePhi(this LDAModel model, LDA lda)
        {
            var beta = lda.Parameter.Beta;
            var vocaCount = lda.Parameter.VocabularyCount;
            foreach (var topicId in Enumerable.Range(0, lda.Parameter.TopicCount))
            {
                foreach (var wordId in Enumerable.Range(0, vocaCount))
                {
                    model.Phi[topicId][wordId] =
                        (model.NW[wordId][topicId] + model.NW[wordId][topicId] + beta) /
                        (model.NWCount[topicId] + model.NWCount[topicId] + vocaCount * beta);
                }
            }
        }
    }
}
