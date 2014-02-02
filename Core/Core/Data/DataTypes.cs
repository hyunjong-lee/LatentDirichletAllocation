using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public static class WordManager
    {
        public static int VocabularyCount { get { return WordIdMap.Count; } }
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
        public readonly Dictionary<int, int> BagOfWords = new Dictionary<int,int>();
        public readonly List<int> WordSequence = new List<int>();

        public Document(string id, string content, params char[] delimiter)
        {
            Id = id;
            foreach (var word in content.ToLower().Split(delimiter))
            {
                var wordId = word.ToWordId();
                if (!BagOfWords.ContainsKey(wordId))
                    BagOfWords.Add(wordId, 0);
                BagOfWords[wordId]++;

                WordSequence.Add(wordId);
            }
        }
    }

}
