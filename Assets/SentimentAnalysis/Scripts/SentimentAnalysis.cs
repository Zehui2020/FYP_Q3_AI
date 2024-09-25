using UnityEngine;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace UnitySentiment
{
    [System.Serializable]
    public class SentimentAnalysis : MonoBehaviour
    {

        #region PRIVATE_FIELDS

        [SerializeField]
        [HideInInspector]
        private string PositiveWordsFileText;
        [SerializeField]
        [HideInInspector]
        private string NegativeWordsFileText;
        [SerializeField]
        [HideInInspector]
        private string ContractionWordsFileText;
        [SerializeField]
        [HideInInspector]
        private string StopWordsFileText;

        private List<string> PositiveWords = new List<string>();
        private List<string> PositiveWeights = new List<string>();

        private List<string> NegativeWords = new List<string>();
        private List<string> NegativeWeights = new List<string>();

        private string[] ContractionWords;
        private string[] StopWords;

        private List<float> Values = new List<float>();

        private readonly object analysisSyncObj = new object();

        public delegate void AnalysisFinished(Vector3 SentimentAnalysis);
        public static event AnalysisFinished OnAnlysisFinished;

        public delegate void Error(int ErrorCode, string ErrorMessage);
        public static event Error OnErrorOccurs;       

        private Stemmer wordStemmer = new Stemmer();

        private bool initialized = false;

        [SerializeField]
        [HideInInspector]
        private float SentimentValue = .5f;

        #endregion

        #region PUBLIC_METHOD

        public void Initialize()
        {
            if (!initialized)
            {
                InitializeReadingData();
            }
            else
            {
                ThrowError(0, "Already Initialized");
            }
        }

        #endregion

        public void PredictSentimentText(string Text)
        {
            if (!initialized)
            {
                ThrowError(4, "Database not Initilized");
            }
            else
            {
                Thread predictionThread = new Thread(() => PredictSentiment(Text));
                predictionThread.Start();
            }
        }

        public void SetSentimentValue(float SentimentValue)
        {
            this.SentimentValue = Mathf.Clamp01(SentimentValue);
        }

        public float GetSentimentValue() { return SentimentValue; }

        public void SetPositiveWordsFileText(string PositiveWordsFileText)
        {
            this.PositiveWordsFileText = PositiveWordsFileText;
        }

        public string GetPositiveWordsFileText() { return PositiveWordsFileText; }

        public void SetNegativeWordsFileText(string NegativeWordsFileText)
        {
            this.NegativeWordsFileText = NegativeWordsFileText;
        }

        public string GetNegativeWordsFileText() { return NegativeWordsFileText; }

        public void SetContractionWordsFileText(string ContractionWordsFileText)
        {
            this.ContractionWordsFileText = ContractionWordsFileText;
        }

        public string GetContractionWordsFileText() { return ContractionWordsFileText; }

        public void SetStopWordsFileText(string StopWordsFileText)
        {
            this.StopWordsFileText = StopWordsFileText;
        }

        public string GetStopWordsFileText() { return StopWordsFileText; }

        #region PRIVATE_METHOD

        private void InitializeReadingData()
        {
            Debug.Log("Sentiment Database Initialization Started...");

            Monitor.TryEnter(analysisSyncObj);
            {
                PopulateLists(PositiveWords, PositiveWeights, OpenFile(PositiveWordsFileText));
                PopulateLists(NegativeWords, NegativeWeights, OpenFile(NegativeWordsFileText));

                ContractionWords = OpenFile(ContractionWordsFileText);
                StopWords = OpenFile(StopWordsFileText);
            }
            Monitor.Exit(analysisSyncObj);

            initialized = true;
            Debug.Log("Sentiment Database Initialization Finished");
        }

        private void PopulateLists(List<string> word, List<string> values, string[] weightedData)
        {
            for (int i = 0; i < weightedData.Length; i++)
            {
                if (!string.IsNullOrEmpty(weightedData[i]))
                {
                    try
                    {
                        string w = weightedData[i].Split(':')[0];
                        string v = weightedData[i].Split(':')[1];

                        if (!string.IsNullOrEmpty(w)
                            && !string.IsNullOrEmpty(v))
                        {
                            word.Add(w);
                            values.Add(v);
                        }
                    }
                    catch
                    {
                        ThrowError(3, "Invalid File Data Format: " + weightedData[i]);
                    }
                }
            }
        }

        private void PredictSentiment(string Text)
        {
            if (Monitor.TryEnter(analysisSyncObj))
            {
                Debug.Log("Sentiment Process Started...");

                lock (analysisSyncObj)
                {
                    // have we inserted some text ?
                    if (!string.IsNullOrEmpty(Text))
                    {
                        //clear the values of the list
                        Values.Clear();
                        // the input Text is set to LowerCase and trimmed
                        Text = Text.ToLower().Trim();
                        // Add a . to the and of the text if there isn't one
                        if (Text[Text.Length - 1] != '.') Text += ".";
                        // Split tthe text in sentences
                        string[] Sentences = Text.Split('.');

                        for (int sentence = 0; sentence < Sentences.Length; sentence++)
                        {
                            Sentences[sentence].Replace(';', ',');
                            // split the sentences in parts
                            string[] sentencePart = Sentences[sentence].Split(',');
                            List<string> firstPartSentence = new List<string>();
                            List<string> secondPartSentence = new List<string>();

                            // Add Parts based on how they are splitted
                            for (int i = 0; i < sentencePart.Length; i++)
                            {
                                if (i % 2 == 0)
                                {
                                    firstPartSentence.Add(sentencePart[i].Trim());
                                }
                                else
                                {
                                    secondPartSentence.Add(sentencePart[i].Trim());
                                }
                            }

                            string fullSentence = System.String.Empty;
                            // add the parts in one sentence
                            for (int i = 0; i < firstPartSentence.Count; i++)
                            {
                                fullSentence += firstPartSentence[i];
                                fullSentence += " ";
                            }

                            for (int i = 0; i < secondPartSentence.Count; i++)
                            {
                                fullSentence += secondPartSentence[i];
                                fullSentence += " ";
                            }
                            // split the sentence in words
                            string[] Words = fullSentence.Trim().Split(' ');
                            // get the values for each ford
                            for (int i = 0; i < Words.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(Words[i].Trim()))
                                {
                                    float valueOfWord = Check(Words[i].Trim());
                                    // save the values in a List
                                    Values.Add(valueOfWord);
                                }
                            }
                        }

                        float positiveValue = 0f, negativeValue = 0f, neutralValue = 0f;

                        bool ContractionWord = false;
                        // Bot System Morale is the sensibility to Negative or Positive Expressions
                        float systemPositiveMoraleSens = 1f - GetSentimentValue();
                        float systemNegativeMoraleSens = GetSentimentValue();
                        // check the returned values
                        for (int i = 0; i < Values.Count; i++)
                        {
                            // unknwon word, marked as neutral
                            if (Values[i] == -4f)
                            {
                                neutralValue += .005f;
                                //positiveValue += 1f * (neutralValue * .05f) * systemPositiveMoraleSens; 
                            }
                            else if (Values[i] == -2f) // contraction Word
                            {
                                ContractionWord = true;
                            }
                            else if (Values[i] > 0f && Values[i] != -3f) // positive Word (not a stopWord)
                            {
                                if (!ContractionWord)
                                {
                                    positiveValue += Values[i] * systemPositiveMoraleSens;
                                }
                                else
                                {
                                    negativeValue += Values[i] * systemNegativeMoraleSens * .3f;
                                    positiveValue -= Values[i] * systemNegativeMoraleSens;
                                }

                                if (negativeValue < 0f) negativeValue = 0f;
                                if (positiveValue < 0f) positiveValue = 0f;

                                ContractionWord = false;
                            }
                            else if (Values[i] < 0f && Values[i] != -3f) // negative Word (not a stopWord)
                            {
                                if (!ContractionWord)
                                {
                                    negativeValue += (-Values[i]) * systemNegativeMoraleSens;
                                    positiveValue -= (-Values[i]) * systemNegativeMoraleSens * .7f;
                                }
                                else
                                {
                                    positiveValue += (-Values[i]) * systemPositiveMoraleSens;
                                }

                                if (positiveValue < 0f) positiveValue = 0f;
                                if (negativeValue < 0f) negativeValue = 0f;

                                ContractionWord = false;
                            }
                        }

                        float sum = positiveValue + negativeValue + neutralValue;
                        if (sum == 0f) sum = 1f;
                        // sums up all the values
                        positiveValue *= 100 / sum;
                        negativeValue *= 100 / sum;
                        neutralValue *= 100 / sum;

                        if (OnAnlysisFinished != null)
                        {
                            // Throws an event for reading the results
                            OnAnlysisFinished(new Vector3(positiveValue, negativeValue, neutralValue));
                        }
                    }
                    else
                    {
                        ThrowError(5, "Please insert some Text");
                    }
                    // release the thread
                    Monitor.Exit(analysisSyncObj);
                    Debug.Log("Sentiment Process Finished!");
                }
            }
            else
            {
                ThrowError(2, "This action is locked by another Thread, please wait for the end of the Analysis or Initialization. We ensure a Thread-Safe system by locking computations");
            }
        }

        private float Check(string Word)
        {
            // return the value of a negative / positive words in the Trained Data Weighted
            float PositiveValue = GetWeightedValueIfExist(PositiveWords, PositiveWeights, Word, true);
            float Negativevalue = GetWeightedValueIfExist(NegativeWords, NegativeWeights, Word, false);
            // is the words a Contraction or a stop Word ?
            bool IsContraction = CheckIfExist(ContractionWords, Word);
            bool IsStopWord = CheckIfExist(StopWords, Word);

            if (PositiveValue != 0f) return PositiveValue;

            if (Negativevalue != 0f) return (-Negativevalue);

            if (IsContraction) return (-2f);

            if (IsStopWord) return (-3f);

            return (-4); // unknown, marked as neutral
        }

        // For Weighted Positive / Negative Words
        private float GetWeightedValueIfExist(List<string> WordList, List<string> ValuesList, string element, bool pos)
        {
            wordStemmer.SetCharArray(element.ToCharArray(), element.Length);
            wordStemmer.stem();

            element = wordStemmer.Tostring();

            if (WordList.Contains(element))
            {
                /*if (pos)
                {
                    print("Positive Found: " + WordList[WordList.IndexOf(element)] + "\nWord: " + element + "\nWeight: " + ValuesList[ WordList.IndexOf(element) ]);
                }
                else
                {
                    print("Negative Found: " + WordList[WordList.IndexOf(element)] + "\nWord: " + element + "\nWeight: " + ValuesList[ WordList.IndexOf(element) ]);
                }*/

                return float.Parse(ValuesList[WordList.IndexOf(element)]);
            }

            return 0f;
        }

        // For StopWords or ContractionWords
        private bool CheckIfExist(string[] ArrayList, string element)
        {
            for (int i = 0; i < ArrayList.Length; i++)
            {
                if (element == ArrayList[i])
                {
                    return true;
                }
            }

            return false;
        }

        // Open a file
        private string[] OpenFile(string filePath)
        {
            TextAsset txt = (TextAsset)Resources.Load(filePath, typeof(TextAsset));

            if (txt != null)
            {
                string[] textOfFile = txt.text.Trim().Split(',');
                return textOfFile;
            }
            else
            {
                ThrowError(1, "Error Loading");
                return new string[0];
            }           
        }

        private void ThrowError(int ErrorCode, string ErrorMessage)
        {
            if (OnErrorOccurs != null)
            {
                OnErrorOccurs(ErrorCode, ErrorMessage);
            }
        }

        #endregion
    }

    #region LevenshteinDistance

    // minimum iteration needed to form a specific word
    static class LevenshteinDistance
    {
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    d[i, j] = Mathf.Min(
                        Mathf.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
    }

    #endregion

    #region StemmerClass

    public class Stemmer
    {
        private char[] b;
        private int i,     /* offset into b */
        i_end, /* offset to end of stemmed word */
        j, k;
        private static int INC = 50;
        /* unit of size whereby b is increased */

        public Stemmer()
        {
            reset();
        }

        private void reset()
        {
            b = new char[INC];
            i = 0;
            i_end = 0;
        }
        /**
    * Add a character to the word being stemmed.  When you are finished
    * adding characters, you can call stem(void) to stem the word.
    */

        public void SetChar(char ch)
        {
            reset();

            if (i == b.Length)
            {
                char[] new_b = new char[i + INC];
                for (int c = 0; c < i; c++)
                {
                    new_b[c] = b[c];
                }
                b = new_b;
            }
            b[i] = ch;
            i++;
        }


        /** Adds wLen characters to the word being stemmed contained in a portion
    * of a char[] array. This is like repeated calls of add(char ch), but
    * faster.
    */

        public void SetCharArray(char[] w, int wLen)
        {
            reset();

            if (i + wLen >= b.Length)
            {
                char[] new_b = new char[i + wLen + INC];

                for (int c = 0; c < i; c++)
                {
                    new_b[c] = b[c];
                }

                b = new_b;
            }
            for (int c = 0; c < wLen; c++)
            {
                b[i] = w[c];
                i++;
            }
        }

        /**
         * After a word has been stemmed, it can be retrieved by tostring(),
         * or a reference to the internal buffer can be retrieved by getResultBuffer
         * and getResultLength (which is generally more efficient.)
         */
        public string Tostring()
        {
            return new string(b, 0, i_end);
        }

        /**
         * Returns the length of the word resulting from the stemming process.
         */
        public int getResultLength()
        {
            return i_end;
        }

        /**
         * Returns a reference to a character buffer containing the results of
         * the stemming process.  You also need to consult getResultLength()
         * to determine the length of the result.
         */
        public char[] getResultBuffer()
        {
            return b;
        }

        /* cons(i) is true <=> b[i] is a consonant. */
        private bool cons(int i)
        {
            switch (b[i])
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u': return false;
                case 'y': return (i == 0) ? true : !cons(i - 1);
                default: return true;
            }
        }

        /* m() measures the number of consonant sequences between 0 and j. if c is
           a consonant sequence and v a vowel sequence, and <..> indicates arbitrary
           presence,

              <c><v>       gives 0
              <c>vc<v>     gives 1
              <c>vcvc<v>   gives 2
              <c>vcvcvc<v> gives 3
              ....
        */
        private int m()
        {
            int n = 0;
            int i = 0;
            while (true)
            {
                if (i > j) return n;
                if (!cons(i)) break; i++;
            }
            i++;
            while (true)
            {
                while (true)
                {
                    if (i > j) return n;
                    if (cons(i)) break;
                    i++;
                }
                i++;
                n++;
                while (true)
                {
                    if (i > j) return n;
                    if (!cons(i)) break;
                    i++;
                }
                i++;
            }
        }

        /* vowelinstem() is true <=> 0,...j contains a vowel */
        private bool vowelinstem()
        {
            int i;
            for (i = 0; i <= j; i++)
            {
                if (!cons(i))
                    return true;
            }
            return false;
        }

        /* doublec(j) is true <=> j,(j-1) contain a double consonant. */
        private bool doublec(int j)
        {
            if (j < 1)
                return false;
            if (b[j] != b[j - 1])
                return false;
            return cons(j);
        }

        /* cvc(i) is true <=> i-2,i-1,i has the form consonant - vowel - consonant
           and also if the second c is not w,x or y. this is used when trying to
           restore an e at the end of a short word. e.g.

              cav(e), lov(e), hop(e), crim(e), but
              snow, box, tray.

        */
        private bool cvc(int i)
        {
            if (i < 2 || !cons(i) || cons(i - 1) || !cons(i - 2))
                return false;
            int ch = b[i];
            if (ch == 'w' || ch == 'x' || ch == 'y')
                return false;
            return true;
        }

        private bool ends(string s)
        {
            int l = s.Length;
            int o = k - l + 1;
            if (o < 0)
                return false;
            char[] sc = s.ToCharArray();
            for (int i = 0; i < l; i++)
                if (b[o + i] != sc[i])
                    return false;
            j = k - l;
            return true;
        }

        /* setto(s) sets (j+1),...k to the characters in the string s, readjusting
           k. */
        private void setto(string s)
        {
            int l = s.Length;
            int o = j + 1;
            char[] sc = s.ToCharArray();
            for (int i = 0; i < l; i++)
                b[o + i] = sc[i];
            k = j + l;
        }

        /* r(s) is used further down. */
        private void r(string s)
        {
            if (m() > 0)
                setto(s);
        }

        /* step1() gets rid of plurals and -ed or -ing. e.g.
               caresses  ->  caress
               ponies    ->  poni
               ties      ->  ti
               caress    ->  caress
               cats      ->  cat

               feed      ->  feed
               agreed    ->  agree
               disabled  ->  disable

               matting   ->  mat
               mating    ->  mate
               meeting   ->  meet
               milling   ->  mill
               messing   ->  mess

               meetings  ->  meet

        */

        private void step1()
        {
            if (b[k] == 's')
            {
                if (ends("sses"))
                    k -= 2;
                else if (ends("ies"))
                    setto("i");
                else if (b[k - 1] != 's')
                    k--;
            }
            if (ends("eed"))
            {
                if (m() > 0)
                    k--;
            }
            else if ((ends("ed") || ends("ing")) && vowelinstem())
            {
                k = j;
                if (ends("at"))
                    setto("ate");
                else if (ends("bl"))
                    setto("ble");
                else if (ends("iz"))
                    setto("ize");
                else if (doublec(k))
                {
                    k--;
                    int ch = b[k];
                    if (ch == 'l' || ch == 's' || ch == 'z')
                        k++;
                }
                else if (m() == 1 && cvc(k)) setto("e");
            }
        }

        /* step2() turns terminal y to i when there is another vowel in the stem. */
        private void step2()
        {
            if (ends("y") && vowelinstem())
                b[k] = 'i';
        }

        /* step3() maps double suffices to single ones. so -ization ( = -ize plus
           -ation) maps to -ize etc. note that the string before the suffix must give
           m() > 0. */
        private void step3()
        {
            if (k == 0)
                return;

            /* For Bug 1 */
            switch (b[k - 1])
            {
                case 'a':
                    if (ends("ational")) { r("ate"); break; }
                    if (ends("tional")) { r("tion"); break; }
                    break;
                case 'c':
                    if (ends("enci")) { r("ence"); break; }
                    if (ends("anci")) { r("ance"); break; }
                    break;
                case 'e':
                    if (ends("izer")) { r("ize"); break; }
                    break;
                case 'l':
                    if (ends("bli")) { r("ble"); break; }
                    if (ends("alli")) { r("al"); break; }
                    if (ends("entli")) { r("ent"); break; }
                    if (ends("eli")) { r("e"); break; }
                    if (ends("ousli")) { r("ous"); break; }
                    break;
                case 'o':
                    if (ends("ization")) { r("ize"); break; }
                    if (ends("ation")) { r("ate"); break; }
                    if (ends("ator")) { r("ate"); break; }
                    break;
                case 's':
                    if (ends("alism")) { r("al"); break; }
                    if (ends("iveness")) { r("ive"); break; }
                    if (ends("fulness")) { r("ful"); break; }
                    if (ends("ousness")) { r("ous"); break; }
                    break;
                case 't':
                    if (ends("aliti")) { r("al"); break; }
                    if (ends("iviti")) { r("ive"); break; }
                    if (ends("biliti")) { r("ble"); break; }
                    break;
                case 'g':
                    if (ends("logi")) { r("log"); break; }
                    break;
                default:
                    break;
            }
        }

        /* step4() deals with -ic-, -full, -ness etc. similar strategy to step3. */
        private void step4()
        {
            switch (b[k])
            {
                case 'e':
                    if (ends("icate")) { r("ic"); break; }
                    if (ends("ative")) { r(""); break; }
                    if (ends("alize")) { r("al"); break; }
                    break;
                case 'i':
                    if (ends("iciti")) { r("ic"); break; }
                    break;
                case 'l':
                    if (ends("ical")) { r("ic"); break; }
                    if (ends("ful")) { r(""); break; }
                    break;
                case 's':
                    if (ends("ness")) { r(""); break; }
                    break;
            }
        }

        /* step5() takes off -ant, -ence etc., in context <c>vcvc<v>. */
        private void step5()
        {
            if (k == 0)
                return;

            /* for Bug 1 */
            switch (b[k - 1])
            {
                case 'a':
                    if (ends("al")) break; return;
                case 'c':
                    if (ends("ance")) break;
                    if (ends("ence")) break; return;
                case 'e':
                    if (ends("er")) break; return;
                case 'i':
                    if (ends("ic")) break; return;
                case 'l':
                    if (ends("able")) break;
                    if (ends("ible")) break; return;
                case 'n':
                    if (ends("ant")) break;
                    if (ends("ement")) break;
                    if (ends("ment")) break;
                    /* element etc. not stripped before the m */
                    if (ends("ent")) break; return;
                case 'o':
                    if (ends("ion") && j >= 0 && (b[j] == 's' || b[j] == 't')) break;
                    /* j >= 0 fixes Bug 2 */
                    if (ends("ou")) break; return;
                /* takes care of -ous */
                case 's':
                    if (ends("ism")) break; return;
                case 't':
                    if (ends("ate")) break;
                    if (ends("iti")) break; return;
                case 'u':
                    if (ends("ous")) break; return;
                case 'v':
                    if (ends("ive")) break; return;
                case 'z':
                    if (ends("ize")) break; return;
                default:
                    return;
            }
            if (m() > 1)
                k = j;
        }

        /* step6() removes a final -e if m() > 1. */
        private void step6()
        {
            j = k;

            if (b[k] == 'e')
            {
                int a = m();
                if (a > 1 || a == 1 && !cvc(k - 1))
                    k--;
            }
            if (b[k] == 'l' && doublec(k) && m() > 1)
                k--;
        }

        /** Stem the word placed into the Stemmer buffer through calls to add().
         * Returns true if the stemming process resulted in a word different
         * from the input.  You can retrieve the result with
         * getResultLength()/getResultBuffer() or tostring().
         */
        public void stem()
        {
            k = i - 1;
            if (k > 1)
            {
                step1();
                step2();
                step3();
                step4();
                step5();
                step6();
            }
            i_end = k + 1;
            i = 0;
        }
    }

    #endregion   
}