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
        public string CorpusPath;
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
        public string ModelPath;
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

        private LDAModel()
        {
        }

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
                Z.Add(Enumerable.Repeat(0, document.WordCount).ToList());

            // NW
            NW = ModelHelper.InitializeMatrix<int>(vocaCount, topicCount);

            // ND
            ND = ModelHelper.InitializeMatrix<int>(docCount, topicCount);

            // NWCount
            NWCount = ModelHelper.InitializeList<int>(topicCount);

            // Theta
            Theta = ModelHelper.InitializeMatrix<double>(docCount, topicCount);

            // Phi
            Phi = ModelHelper.InitializeMatrix<double>(topicCount, vocaCount);

            #endregion
        }
       
    }

    public class LDA
    {
        public Parameter Parameter { get { return _parameter; } }
        private Parameter _parameter;

        public LDAModel LDAModel { get { return _ldaModel; } }
        private LDAModel _ldaModel;

        public LDA(Parameter parameters)
        {
            _parameter = parameters;
            _ldaModel = new LDAModel(_parameter);
        }

        public void Inference()
        {
            LogHelper.Log("initialize model");
            Initialize();

            LogHelper.Log("Start inference {0}", _parameter.TotalIterationStep);
            var inferTime = DateTime.Now;
            foreach (var step in Enumerable.Range(0, _parameter.TotalIterationStep))
            {
                LogHelper.Log("Iteration step {0}", step);
                Inference(step);

                var curInferTime = DateTime.Now;
                var elapsed = (curInferTime - inferTime).TotalMilliseconds / (step + 1);
                var estimatedInferTime = inferTime.AddMilliseconds(elapsed * _parameter.TotalIterationStep);
                LogHelper.Log("Estimated Finish Time: {0}", estimatedInferTime.ToString("HH:mm:ss"));
            }
            LogHelper.Log("Finish inference {0}", _parameter.TotalIterationStep);

            LogHelper.Log("compute theta");
            _ldaModel.ComputeTheta(this);
            
            LogHelper.Log("compute phi");
            _ldaModel.ComputePhi(this);

            LogHelper.Log("export parameters to {0}", _parameter.ModelPath);
            _parameter.Export(_parameter.ModelPath);
            
            LogHelper.Log("export LDA model to {0}", _parameter.ModelPath);
            _ldaModel.Export(_parameter.ModelPath);
            
            LogHelper.Log("export vocabulary to {0}", _parameter.ModelPath);
            ModelHelper.ExportVoca(_parameter.ModelPath);
        }

        private void Initialize()
        {
            // random topic assignment for each word in documents
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

            var topicDist = ModelHelper.InitializeList<double>(topicCount);
            foreach (var topicId in Enumerable.Range(0, topicCount))
            {
                topicDist[topicId] =
                    (_ldaModel.NW[wordId][topicId] + beta) / (_ldaModel.NWCount[topicId] + VBeta) *
                    (_ldaModel.ND[documentIdx][topicId] + alpha) / (NDCount + KAlpha);

                // cumul topic dist
                if (topicId > 0)
                    topicDist[topicId] += topicDist[topicId - 1];
            }

            topic = topicCount - 1;
            var sample = (new Random(DateTime.Now.Millisecond)).NextDouble() * topicDist.Last();
            foreach (var topicId in Enumerable.Range(0, topicCount - 1))
            {
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
