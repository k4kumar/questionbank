using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Quiz_Application.Services.Entities;

namespace Quiz_Application.Services.Repository.Interfaces
{
    public interface IChoice<TEntity>
    {
        Task<TEntity> GetChoice(int choiceID);
        Task<int> AddChoice(TEntity entity);
        Task<int> UpdateChoice(TEntity entity);
        Task<int> DeleteChoice(TEntity entity);
        Task<IEnumerable<Choice>> GetChoiceList(int questionId);
        Task<IEnumerable<TEntity>> GetChoiceList();
    }
}
