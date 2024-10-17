namespace QuizApplicationMVC.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int Duration { get; set; }
        public int UserId { get; set; }
        public Users? User { get; set; }
        public List<Questions>? Questions { get; set; }
        public List<QuizUserHistory>? quizUserHistory { get; set; }

    }
}


//namespace QuizApplicationMVC.Models
//{
//    public class Quiz
//    {
//        public int Id { get; set; }
//        public string Title { get; set; }
//        public string Description { get; set; }

//        // Foreign key for Teacher (UserId renamed to TeacherId)
//        public int TeacherId { get; set; }
//        public Teacher? Teacher { get; set; }

//        // List of questions in the quiz
//        public List<Questions>? Questions { get; set; }

//        // List of quiz histories
//        public List<QuizUserHistory>? QuizUserHistories { get; set; }
//    }
//}
