using System;
using System.Collections.Generic;

namespace CyberSecurityChatbot
{
    public class Chatbot
    {
        private Random rand = new Random();

        
        private string lastTopic = "";
        private string userName = "";
        private string favouriteTopic = "";
        private int quizScore = 0;
        private int quizIndex = 0;
        private bool inQuiz = false;
        private List<string> activityLog = new List<string>();
        private DatabaseHelper db = new DatabaseHelper();

        public List<string> GetActivityLog()
        {
            return activityLog;
        }
        private string DetectIntent(string input)
        
        {
            input = input.ToLower();

            if (input.Contains("add task") ||
                input.Contains("create task") ||
                input.Contains("remind me") ||
                
                input.Contains("task"))
                return "task";

            if (input.Contains("quiz") ||
                input.Contains("test me") ||
                input.Contains("game"))
                return "quiz";

            if (input.Contains("activity log") ||
                input.Contains("what have you done") ||
                input.Contains("show log"))
                return "log";

            if (input.Contains("phishing")) return "phishing";
            if (input.Contains("password")) return "password";
            if (input.Contains("privacy")) return "privacy";
            if (input.Contains("scam")) return "scam";

            return "unknown";
        }
        private string GetPrivacyResponse()
        {
            lastTopic = "privacy";

            string[] responses =
            {
        "Review your privacy settings regularly.",
        "Limit personal information shared online.",
        "Enable two-factor authentication."
    };

            return responses[rand.Next(responses.Length)];
        }
        private string GetPasswordResponse()
        {
            lastTopic = "password";

            string[] responses =
            {
        "Use strong passwords with 12+ characters.",
        "A password manager helps create secure passwords.",
        "Avoid using personal info like birthdays."
    };

            return responses[rand.Next(responses.Length)];
        }
        private string GetScamResponse()
        {
            lastTopic = "scam";

            string[] responses =
            {
        "Never share banking details with strangers.",
        "Scammers use urgency to trick people.",
        "Always verify before sending money."
    };

            return responses[rand.Next(responses.Length)];
        }
        private string GetPhishingResponse()
        {
            lastTopic = "phishing";

            return phishingTips[rand.Next(phishingTips.Length)];
        }
        private string HandleTaskIntent(string input)
        {
            string task = input;

            task = task.Replace("add task", "")
                       .Replace("create task", "")
                       .Replace("remind me", "")
                       .Replace("task", "")
                       .Trim();

            if (task == "")
                return "What task would you like to add?";

            db.AddTask(task, "No description provided");

            activityLog.Add("Task added: " + task);

            return "Task saved successfully: " + task;
        }



        private string HandleQuizAnswer(string input)
        {
            bool correct = false;

            
            if (quizIndex >= answers.Length)
            {
                inQuiz = false;
                return "Quiz finished! Your score: " + quizScore + "/" + questions.Length;
            }

            
            foreach (string ans in answers[quizIndex])
            {
                if (input.Contains(ans))
                {
                    correct = true;
                    activityLog.Add("Quiz answer processed: " + input);
                    break;
                }
            }

            if (correct)
                quizScore++;

            quizIndex++;

            if (quizIndex < questions.Length)
            {
                return (correct ? "Correct! " : "Wrong! ") +
                       "\n\n" + questions[quizIndex];
            }
            else
            {
                inQuiz = false;
                return "Quiz finished! Your score: " + quizScore + "/" + questions.Length;
            }
        }
        private string HandleQuiz(string input)
        {
            input = input.ToLower();

            if (input.Contains("start"))
            {
                quizScore = 0;
                quizIndex = 0;
                inQuiz = true;

                activityLog.Add("Quiz started");

                return "Quiz started!\n" + questions[quizIndex];
            }

            return "Type 'start quiz' to begin.";
        }

        private string[] questions =
{
    "What is phishing?",
    "What should you use for strong passwords?",
    "True or False: You should share your password with friends",
    "What is two-factor authentication (2FA)?",
    "What should you do if you receive a suspicious email?"
};

        private string[][] answers =
        {
    new string[] { "a scam email", "fake email", "phishing email" },
    new string[] { "long password", "password manager", "12+ characters" },
    new string[] { "false" },
    new string[] { "extra security", "second step login", "verification code" },
    new string[] { "report it", "delete it", "ignore it" }
};

        private string[] phishingTips =
        {
            "Be careful of emails asking for personal information.",
            "Never click suspicious links from unknown senders.",
            "Check the sender's email carefully before responding."
        };

       
        public void SetUserName(string name)
        {
            userName = name;
        }

        
        public string GetResponse(string input)
        {
            input = input.ToLower().Trim();
            if (inQuiz)
            {
                activityLog.Add("Quiz answer received: " + input);
                return HandleQuizAnswer(input);
            }

            string intent = DetectIntent(input);

            
            switch (intent)
            {
                case "task":
                    return HandleTaskIntent(input);

                case "quiz":
                    return HandleQuiz(input);

                case "log":
                    return "Use 'Show activity log' command in Form.";

                case "phishing":
                    return GetPhishingResponse();

                case "password":
                    return GetPasswordResponse();

                case "privacy":
                    return GetPrivacyResponse();

                case "scam":
                    return GetScamResponse();

                default:
                    break;
            }


            if (input.Contains("thank you") || input.Contains("thanks"))
            {
                return "You're welcome! Stay safe online.";
            }

           
            bool worried = input.Contains("worried");
            bool frustrated = input.Contains("frustrated");
            bool curious = input.Contains("curious");

            string emotionIntro = "";

            if (worried)
                emotionIntro = "It's understandable to feel worried. ";
            else if (frustrated)
                emotionIntro = "I understand this can feel frustrating. ";
            else if (curious)
                emotionIntro = "Great curiosity! ";

            
            if (input.Contains("my name is"))
            {
                int start = input.IndexOf("my name is") + 11;
                string remaining = input.Substring(start).Trim();

                string[] stopWords =
                {
                    "tell me",
                    "password",
                    "privacy",
                    "phishing",
                    "scam",
                    "im interested in",
                    "i am interested in"
                };

                foreach (string stop in stopWords)
                {
                    int index = remaining.IndexOf(stop);
                    if (index != -1)
                        remaining = remaining.Substring(0, index).Trim();
                }

                string[] words = remaining.Split(' ');
                userName = words[0];

                return "Nice to meet you, " + userName + "!";
            }

            
            if (input.Contains("i am interested in") ||
                input.Contains("im interested in") ||
                input.Contains("my favourite topic is"))
            {
                if (input.Contains("password"))
                    favouriteTopic = "password";
                else if (input.Contains("scam"))
                    favouriteTopic = "scam";
                else if (input.Contains("privacy"))
                    favouriteTopic = "privacy";
                else if (input.Contains("phishing"))
                    favouriteTopic = "phishing";

                return "Great! I'll remember that you're interested in " + favouriteTopic + ".";
            }

            
            if (input.Contains("hello") || input.Contains("hi"))
            {
                string greeting = "Hello";

                if (!string.IsNullOrEmpty(userName))
                    greeting += " " + userName;

                greeting += "! ";

                if (!string.IsNullOrEmpty(favouriteTopic))
                    greeting += "I remember you're interested in " + favouriteTopic + ". ";

                greeting += "How can I help you stay safe online?";

                return greeting;
            }

            
            if (input.Contains("password"))
            {
                lastTopic = "password";

                string[] responses =
                {
                    "Use strong, unique passwords with symbols and numbers.",
                    "A password manager helps create secure passwords.",
                    "Avoid using personal info like birthdays."
                };

                return emotionIntro + responses[rand.Next(responses.Length)];
            }

           
            if (input.Contains("scam"))
            {
                lastTopic = "scam";

                string[] responses =
                {
                    "Never share banking details with strangers.",
                    "Scammers use urgency to trick people.",
                    "Always verify before sending money."
                };

                return emotionIntro + responses[rand.Next(responses.Length)];
            }

            
            if (input.Contains("privacy"))
            {
                lastTopic = "privacy";

                string[] responses =
                {
                    "Review your privacy settings regularly.",
                    "Limit personal information shared online.",
                    "Enable two-factor authentication."
                };

                return emotionIntro + responses[rand.Next(responses.Length)];
            }

            
            if (input.Contains("phishing"))
            {
                lastTopic = "phishing";

                return emotionIntro + phishingTips[rand.Next(phishingTips.Length)];
            }

            
            if (input.Contains("tell me more") ||
                input.Contains("another tip") ||
                input.Contains("explain more"))
            {
                return GetFollowUpResponse();
            }

            
            if (input.Contains("how are you"))
            {
                return "I'm functioning well and ready to help you stay safe online!";
            }

            
            return "I'm not sure I understand. Try asking about passwords, scams, privacy, or phishing.";
        }

        
        private string GetFollowUpResponse()
        {
            switch (lastTopic)
            {
                case "password":
                    return "Try using passphrases and never reuse passwords.";

                case "scam":
                    return "Be careful of fake urgency and requests for money.";

                case "privacy":
                    return "Check app permissions and privacy settings.";

                case "phishing":
                    return "Always check links before clicking.";

                default:
                    return "Ask me about a cybersecurity topic first.";
            }
        }
    }
}