using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helper
{
    public static class ModelHelper
    {
        public static void Export(this LDA lda, string directory)
        {
        }

        public static void Import(this LDA lda, string directory)
        {
        }

        public static void Initialize<T>(this List<List<T>> data, int row, int col)
        {
            data = new List<List<T>>(row);
            foreach (var idx in Enumerable.Range(0, row))
                data[idx] = new List<T>(col);
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
