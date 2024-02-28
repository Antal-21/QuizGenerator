using Microsoft.AspNetCore.Mvc;
using QuizGenweb.Nlp;
using QuizGenweb.OpenAI;

namespace QuizGenweb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuizQuestionController : Controller
    {


        [HttpGet(Name = "QuizQuestion")]
        public string Get(string param)
        {
            string aiResponse = OpenAiClient.GetopenAiDescription(param);
            return SentenceTokenizer.NlpPOS(aiResponse);
        }
    }
}
