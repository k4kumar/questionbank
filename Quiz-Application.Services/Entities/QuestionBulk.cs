using System.ComponentModel.DataAnnotations;

namespace Quiz_Application.Services.Entities
{

    public  class QuestionBulk
    {
        [Key]
        public int Id { get; set; }
        public string Exam { get; set; }    
        public string DisplayText { get; set; }
        public string Choice1 { get; set; }
        public string Choice2 { get; set; }
        public string Choice3 { get; set; }
        public string Choice4 { get; set; }
        public string Answer { get; set; }
    }
}
