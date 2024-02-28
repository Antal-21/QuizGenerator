using System.Text;

namespace QuizGenweb.Model
{
    public class Question
    {
        public string? QuestionString;

        public int[] Choices = new int[3];

        public int CorrectIndex;

        public override string ToString()
        {
            if(string.IsNullOrEmpty(QuestionString))
            {
                return string.Empty;
            }

            StringBuilder b = new StringBuilder();
            b.AppendLine(QuestionString);

            for (int i = 0; i<3; i++)
            {
                if(i == CorrectIndex)
                {
                    b.AppendLine($"\t[{GetAlphabet(i)}] {Choices[i]} [x]");
                    continue;
                }
                b.AppendLine($"\t[{GetAlphabet(i)}] {Choices[i]}");
            }

            return b.ToString();
        }

        private static char GetAlphabet(int i)
        {
            switch (i)
            {
                case 0:
                    return 'a';
                case 1:
                    return 'b';
                case 2:
                    return 'c';
                default:
                    break;
            }

            throw new ArgumentException($"Invalid index: {i}");
        }
    }
}
