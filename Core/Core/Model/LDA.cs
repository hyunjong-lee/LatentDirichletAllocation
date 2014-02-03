using Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Helper;

namespace Core.Model
{
    public class Parameter
    {
        // Bag of words parameters
        public List<Document> DocumentList = new List<Document>();
        public int VocabularyCount { get { return WordManager.VocabularyCount; } }
        public int DocumentCount { get { return DocumentList.Count; } }

        // hyper parameters for topic distribution
        public int TopicCount;
        public double Alpha;
        public double Beta;

        // iteration step - to export or resume learning states
        public int CurrentIterationStep;
        public int TotalIterationStep;

        // model export path
        public string ModelExportPath;
    }

    public class LDAModel
    {
        public readonly Parameter Parameter;
        
        public List<List<int>> Z; // topic assignment for each word in documents: | Documents | * | Word Count for each document |
        public List<List<int>> NW; // topic assignment for each word in vocabulary: | Vocabulary | * | Topic Count |
        public List<List<int>> ND; // topic assignment for each document: | Documents | * | Topic Count |
        public List<int> NWCount; // the number of word counts that are assigned to each topic: | Topic Count |
        public int NDCount(int docIdx) { return Parameter.DocumentList[docIdx].Count; } // word count for document indexed by docIdx: | Documents |

        public List<List<double>> Theta; // topic-document distribution: | Documents | * | Topic Count |
        public List<List<double>> Phi; // topic-vocabulary distribution: | Topic Count | * | Vocabulary |
       
        public LDAModel(Parameter parameter)
        {
            Parameter = parameter;

            #region initialize LDA model hyper paramters

            var docCount = Parameter.DocumentList.Count;
            var vocaCount = WordManager.VocabularyCount;
            var topicCount = Parameter.TopicCount;

            // Z
            Z = new List<List<int>>(docCount);
            foreach (var document in Parameter.DocumentList.Select((document, idx) => new { Index = idx, WordCount = document.Count }))
                Z[document.Index] = new List<int>(document.WordCount);

            // NW
            NW.Initialize(vocaCount, topicCount);

            // ND
            ND.Initialize(docCount, topicCount);

            // NWCount
            NWCount = new List<int>(topicCount);

            // Theta
            Theta.Initialize(docCount, topicCount);

            // Phi
            Phi.Initialize(topicCount, vocaCount);

            #endregion
        }
       
    }

    public class LDA
    {
        public Parameter Parameter { get { return _parameter; } }
        private Parameter _parameter;
        private LDAModel _ldaModel;

        public LDA(Parameter parameters)
        {
            _parameter = parameters;
            _ldaModel = new LDAModel(_parameter);
        }

        public void Inference()
        {
            LogHelper.Log("Start to initialize model");
            Initialize();
            LogHelper.Log("Initialize model finished");

            LogHelper.Log("Start inference {0}", _parameter.TotalIterationStep);
            foreach (var step in Enumerable.Range(0, _parameter.TotalIterationStep))
            {
                LogHelper.Log("Iteration step {0} stared", step);
                Inference(step);
                LogHelper.Log("Iteration step {0} finished", step);
            }
            LogHelper.Log("Finish inference {0}", _parameter.TotalIterationStep);

            LogHelper.Log("Start to compute theta paramter");
            _ldaModel.ComputeTheta(this);
            LogHelper.Log("Compute theta paramter finished");
            
            LogHelper.Log("Start to compute phi paramter");
            _ldaModel.ComputePhi(this);
            LogHelper.Log("Compute phi paramter finished");

            LogHelper.Log("Start to export LDA result to {0}", _parameter.ModelExportPath);
            this.Export(_parameter.ModelExportPath);
            LogHelper.Log("Export LDA result to {0} finished", _parameter.ModelExportPath);
        }

        private void Initialize()
        {
            var random = new Random(DateTime.Now.Millisecond);
            foreach (var docIdx in Enumerable.Range(0, _ldaModel.Z.Count))
            {
                foreach (var wordIdx in Enumerable.Range(0, _ldaModel.Z[docIdx].Count))
                {
                    var topic = random.Next(0, _parameter.TopicCount);
                    _ldaModel.Z[docIdx][wordIdx] = topic;
                    _ldaModel.NW[_parameter.DocumentList[docIdx].WordSequence[wordIdx]][topic]++;
                    _ldaModel.ND[docIdx][topic]++;
                    _ldaModel.NWCount[topic]++;
                }
            }
        }

        private void Inference(int step)
        {
            // inference for each document
            foreach (var docIdx in Enumerable.Range(0, _parameter.DocumentCount))
            {
                // inference for each word in a document
                foreach (var wordIdx in Enumerable.Range(0, _parameter.DocumentList[docIdx].Count))
                {
                    var topic = Sampling(docIdx, wordIdx);
                    _ldaModel.Z[docIdx][wordIdx] = topic;
                }
            }
        }

        private int Sampling(int documentIdx, int wordIdx)
        {
            var topic = _ldaModel.Z[documentIdx][wordIdx];
            var wordId = _parameter.DocumentList[documentIdx].WordSequence[wordIdx];

            _ldaModel.NW[wordId][topic]--;
            _ldaModel.ND[documentIdx][topic]--;
            _ldaModel.NWCount[topic]--;
            var NDCount = _ldaModel.NDCount(documentIdx) - 1;

            var vocaCount = _parameter.VocabularyCount;
            var topicCount = _parameter.TopicCount;
            var alpha = _parameter.Alpha;
            var beta = _parameter.Beta;

            var VBeta = vocaCount * beta;
            var KAlpha = topicCount * alpha;

            var topicDist = new List<double>(topicCount);
            foreach (var topicId in Enumerable.Range(0, topicCount))
            {
                topicDist[topicId] =
                    (_ldaModel.NW[wordId][topicId] + beta) / (_ldaModel.NWCount[topicId] + VBeta) *
                    (_ldaModel.ND[documentIdx][topicId] + alpha) / (NDCount + KAlpha);
            }

            topic = topicCount - 1;
            var sample = (new Random(DateTime.Now.Millisecond)).NextDouble() * topicDist.Last();
            foreach (var topicId in Enumerable.Range(1, topicCount - 1))
            {
                topicDist[topicId] += topicDist[topicId - 1];
                if (topicDist[topicId] > sample)
                {
                    topic = topicId;
                    break;
                }
            }

            _ldaModel.NW[wordId][topic]++;
            _ldaModel.ND[documentIdx][topic]++;
            _ldaModel.NWCount[topic]++;

            return topic;
        }
    }
}
