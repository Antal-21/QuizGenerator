using edu.stanford.nlp.ling;
using edu.stanford.nlp.tagger.maxent;
using QuizGenweb.Model;
using System.Collections;
using System.Text;


namespace QuizGenweb.Nlp
{
    public class SentenceTokenizer
    {
        private static readonly string model = Environment.GetEnvironmentVariable("NLP_MODEL_PATH") ?? string.Empty;

        public static string NlpPOS(string text)
        {
            
            if(string.IsNullOrEmpty(model))
            {
                throw new ArgumentNullException("NLP model path is not set");
            }

            var tagger = new MaxentTagger(model);
            var sentences = MaxentTagger.tokenizeText(new java.io.StringReader(text)).toArray();
            var questions = new List<Question>();
            
            foreach (java.util.ArrayList sentence in sentences)
            {
                var taggedSentence = tagger.tagSentence(sentence);
                var taggedListString = SentenceUtils.listToString(taggedSentence, false);
                var subSentenceArr = taggedListString.Split(",/,");

                foreach(var subs in subSentenceArr)
                {
                    var q =ProcessSubSentence(subs);
                    if(q != null)
                    {
                        questions.Add(q);
                    }
                }
                
            }

            if(questions.Any())
            {
                StringBuilder result = new StringBuilder();
                foreach(var q in questions)
                {
                    result.Append(q.ToString());
                    result.AppendLine();
                }
                return result.ToString();
            }

            return "No questions found";
        }

        private static Question ProcessSubSentence(string subSentence)
        {
            var questionBuilder = new StringBuilder();
            var taggedWords = subSentence.Split(" ");
            bool hasNumber = false;
            int number = 0;

            foreach (var tw in taggedWords)
            {
                if (tw.Contains("/CD"))
                {
                    Console.WriteLine($"Found number {tw}");
                    questionBuilder.Append("... ");
                    hasNumber = int.TryParse(tw.Substring(0, tw.IndexOf('/')), out number);
                    continue;
                }

                if (tw.Length > 0 && tw.Contains('/'))
                {
                    questionBuilder.Append(tw.Substring(0, tw.IndexOf('/')) + " ");
                }
            }

            if(hasNumber)
            {
                // Generate question
                return GenerateQuestion(questionBuilder.ToString(), number);
            }

            return null;
        }

        private static Question GenerateQuestion(string questionString, int correctNumber)
        {
            var result = new Question { QuestionString = questionString };

            var rnd = new Random();
            int index = rnd.Next(0, 2);

            result.Choices[index] = correctNumber;
            result.CorrectIndex = index;

            for(int i=0; i<3; i++)
            {
                if(i == index)
                {
                    continue;
                }

                var diff = DateTime.Now.Year - (correctNumber + 30);

                int upperlimit = (diff > 0) ? (correctNumber + 30) : DateTime.Now.Year;


                result.Choices[i] = rnd.Next((int)(correctNumber - 50), upperlimit);
            }

            return result;
        }
    }
}
