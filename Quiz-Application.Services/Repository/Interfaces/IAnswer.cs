using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Quiz_Application.Services.Entities;

namespace Quiz_Application.Services.Repository.Interfaces
{
    public interface IAnswer<TEntity>
    {
        Task<TEntity> GetAnswer(int answerID);
        Task<IEnumerable<TEntity>> GetAnswerList();
        Task<int> AddAnswer(TEntity entity);
        Task<int> UpdateAnswer(TEntity entity);
        Task<int> DeleteAnswer(TEntity entity);
    }
}
