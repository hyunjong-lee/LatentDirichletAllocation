using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public static class WordManager
    {
        private static Dictionary<string, int> WordIdMap = new Dictionary<string,int>();
        private static Dictionary<int, string> IdWordMap = new Dictionary<int, string>();

        public static int ToWordId(this string word)
        {
            if (WordIdMap.ContainsKey(word))
                return WordIdMap[word];

            lock (WordIdMap)
            {
                if (!WordIdMap.ContainsKey(word))
                {
                    lock (WordIdMap)
                    {
                        var wordId = WordIdMap.Count;
                        WordIdMap.Add(word, wordId);
                        IdWordMap.Add(wordId, word);
                    }
                }
            }

            return WordIdMap[word];
        }

        public static string ToWord(this int wordId)
        {
            if (IdWordMap.ContainsKey(wordId))
                return IdWordMap[wordId];

            return null;
        }
    }

    public class Document
    {
        public readonly string Id;
        public readonly Dictionary<int, int> BagOfWords;

        public Document(string id, string content)
        {
            Id = id;
            BagOfWords = new Dictionary<int,int>();
            foreach (var word in content.ToLower().Split())
            {
                var wordId = word.ToWordId();
                if (!BagOfWords.ContainsKey(wordId))
                    BagOfWords.Add(wordId, 0);
                BagOfWords[wordId]++;
            }
        }
    }
}
