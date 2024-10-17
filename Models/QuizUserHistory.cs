using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApplicationMVC.Models
{
    public class QuizUserHistory
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int Score { get; set; }


        [Display(Name = "Date Taken")]
        public DateTime DateTaken { get; set; }

        // Navigation properties
        public Users User { get; set; }
        public Quiz Quiz { get; set; }
    }
}


//using System;
//using System.ComponentModel.DataAnnotations;

//namespace QuizApplicationMVC.Models
//{
//    public class QuizUserHistory
//    {
//        public int Id { get; set; }

//        [Required]
//        public int StudentId { get; set; }  // Foreign key for Student

//        [Required]
//        public int QuizId { get; set; }     // Foreign key for Quiz

//        [Required]
//        public int Score { get; set; }

//        [Display(Name = "Date Taken")]
//        public DateTime DateTaken { get; set; }

//        // Navigation properties
//        public Student? Student { get; set; }
//        public Quiz? Quiz { get; set; }
//    }
//}
