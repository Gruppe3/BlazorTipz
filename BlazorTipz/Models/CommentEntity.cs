using BlazorTipz.ViewModels.Suggestion;

namespace BlazorTipz.Models
{
    public class CommentEntity
    {
        public string employmentId { get; set; }
        public string userName { get; set; }
        public string sugId { get; set; }
        public DateTime createdAt { get; set; }
        public string content { get; set; }
        public bool active { get; set; }



        public CommentEntity()
        {
            employmentId = string.Empty;
            userName = string.Empty;
            sugId = string.Empty;
            content = string.Empty;
            active = true;
        }

        public CommentEntity(CommentViewmodel comment)
        {
            this.employmentId = comment.EmpId;
            this.userName = comment.EmpName;
            this.sugId = comment.SugId;
            this.createdAt = comment.TimeStamp;
            this.content = comment.Comment;
            this.active = comment.IsActive;
        }

    }
}
