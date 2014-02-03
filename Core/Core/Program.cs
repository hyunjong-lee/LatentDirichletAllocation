using Core.Helper;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 6)
            {
                LogHelper.Log("Invalid arguments!");
                LogHelper.Log("Example) Core.exe [Corpus Path] [Alpha] [Beta] [Topic Count] [Inference Iteration Step] [Model Export Path]");

                return;
            }

            try
            {
                var parameter = new Parameter();
                parameter.CorpusPath = args[0];
                parameter.Alpha = double.Parse(args[1]);
                parameter.Beta = double.Parse(args[2]);
                parameter.TopicCount = int.Parse(args[3]);
                parameter.TotalIterationStep = int.Parse(args[4]);
                parameter.ModelPath = args[5];

                parameter.LoadCorpus();
                var lda = new LDA(parameter);
                lda.Inference();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }
        }
    }
}
