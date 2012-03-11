using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReQuizClient;
using System.Net;
using System.Threading;

namespace ReQuizStressTest
{
    class Program
    {
        static Random randGen = new Random();
        static IPAddress serverAddr;
        static int port;
        static string username;

        static string RandomName()
        {
            string result = "";
            for (int i = 0; i < 8; i++) result += (char)(randGen.Next(26) + 65);
            return result;
        }
        static void Main(string[] args)
        {
            serverAddr = IPAddress.Parse(args[0]);
            port = 1234;
            username = RandomName();

            Console.WriteLine("ReQuiz stress tester.");

            QuestionReceiver questionReceiver = new QuestionReceiver(serverAddr, port, username);
            questionReceiver.QuestionsFetched += ReceivedQuestionsEvent;
            questionReceiver.ConnectFailed += ConnectFailedEvent;
            questionReceiver.ClientLog += LogEvent;
            questionReceiver.ReceiveQuestions();
            while (true)
            {
                Thread.Sleep(1);
            }
        }

        static void LogEvent(object sender, ClientLogEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        static void ConnectFailedEvent(object sender, ConnectFailedEventArgs e)
        {
            Console.WriteLine("Connection failed: " + e.Message);
            Console.WriteLine("The tester will now exit.");
            Console.ReadKey();
            Environment.Exit(0);
        }

        static void ReceivedQuestionsEvent(object sender, QuestionsFetchedEventArgs e)
        {
            QuizParameters questions = e.Parameters;
            QuizAnswers answers = new QuizAnswers();
            answers.answers = new List<string>();
            foreach(QuizQuestionRaw question in questions.questions) {
                answers.answers.Add("test");
            }
            answers.hintsUsed = 0;

            MarkReceiver markReceiver = new MarkReceiver(serverAddr, port, username, answers);
            markReceiver.ClientLog += LogEvent;
            markReceiver.MarkReceived += MarkReceivedEvent;
            markReceiver.SendAnswersReceiveMark();
        }

        static void MarkReceivedEvent(object sender, MarkReceivedEventArgs e)
        {
            Console.WriteLine("Mark received.");
            QuizResult result = e.Result;
            if (result.correctAnswers == 0)
            {
                Console.WriteLine("Correct answers: 0, as expected");
            }
            else
            {
                Console.WriteLine("Correct answers: " + result.correctAnswers + " -- wrong");
            }

            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
