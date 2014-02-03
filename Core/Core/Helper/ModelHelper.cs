using Core.Data;
using Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helper
{
    public static class ModelHelper
    {
        public static void Export(this LDA lda)
        {
            var executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var modelPath = Path.Combine(executionPath, lda.Parameter.ModelPath);
            if (!Directory.Exists(modelPath)) Directory.CreateDirectory(modelPath);

            // export Parameters
            using (var writer = new StreamWriter(Path.Combine(modelPath, "Parameters.dat")))
            {
                writer.WriteLine(lda.Parameter.Alpha);
                writer.WriteLine(lda.Parameter.Beta);
                writer.WriteLine(lda.Parameter.TopicCount);
                writer.WriteLine(lda.Parameter.CurrentIterationStep);
                writer.WriteLine(lda.Parameter.TotalIterationStep);
                writer.WriteLine(lda.Parameter.VocabularyCount);
                writer.WriteLine(lda.Parameter.DocumentCount);
            }

            // export Vocabulary
            using (var writer = new StreamWriter(Path.Combine(modelPath, "Voca.dat")))
            {
                foreach(var wordIdPair in WordManager.WordIterator())
                {
                    writer.WriteLine("{0}:{1}", wordIdPair.Key, wordIdPair.Value);
                }
            }

            // export Theta
            using (var writer = new StreamWriter(Path.Combine(modelPath, "Theta.dat")))
            {
                foreach (var values in lda.LDAModel.Theta)
                {
                    writer.WriteLine(string.Join(",", values));
                }
            }

            // export Phi
            using (var writer = new StreamWriter(Path.Combine(modelPath, "Phi.dat")))
            {
                foreach (var values in lda.LDAModel.Phi)
                {
                    writer.WriteLine(string.Join(",", values));
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
