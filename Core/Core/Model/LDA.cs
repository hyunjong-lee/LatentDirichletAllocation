using Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class LDA
    {
        public Parameter Parameter { get { return _parameter; } }
        private Parameter _parameter;

        public LDA(Parameter parameters)
        {
            _parameter = parameters;
        }

        public void Inference()
        {
        }
    }
}
