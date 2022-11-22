using BlazorTipz.Models;

namespace BlazorTipz.ViewModels.Suggestion
{
    public class CommentViewmodel
    {
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string SugId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Comment { get; set; }
        public bool IsActive { get; set; }


        public CommentViewmodel()
        {
            EmpId = string.Empty;
            EmpName = string.Empty;
            SugId = string.Empty;
            Comment = string.Empty;
            IsActive = true;
        }
        public CommentViewmodel(CommentEntity comment)
        {
            this.EmpId = comment.employmentId;
            this.EmpName = comment.userName;
            this.SugId = comment.sugId;
            this.TimeStamp = comment.createdAt;
            this.Comment = comment.content;
            this.IsActive = comment.active;
        }


    }
}
