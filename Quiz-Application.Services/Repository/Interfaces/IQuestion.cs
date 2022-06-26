using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Quiz_Application.Services.Entities;

namespace Quiz_Application.Services.Repository.Interfaces
{
    public interface IQuestion<TEntity>
    {
        public int BulkQuestionEntry();
        Task<TEntity> GetQuestion(int QuestionID);
        Task<QnA> GetQuestionList(int ExamID);
        Task<IEnumerable<TEntity>> GetQuestionList();
        Task<IQueryable<TEntity>> SearchQuestion(Expression<Func<TEntity, bool>> search = null);
        Task<int> AddQuestion(TEntity entity);
        Task<int> UpdateQuestion(TEntity entity);
        Task<int> DeleteQuestion(TEntity entity);
    }
}
