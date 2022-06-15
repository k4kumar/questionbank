using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quiz_Application.Services.Entities;
using Quiz_Application.Services.Repository.Interfaces;

namespace Quiz_Application.Services.Repository.Base
{
    public class ChoiceService<TEntity> : IChoice<TEntity> where TEntity : BaseEntity
    {
       private readonly QuizDBContext _dbContext;
       private DbSet<TEntity> _dbSet;
       public ChoiceService(QuizDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetChoiceList()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<int> DeleteChoice(TEntity entity)
        {
            int output = 0;
            _dbSet.Remove(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }

       public async Task<int> AddChoice(TEntity entity)
        {
            int output = 0;
            entity.CreatedBy = "SYSTEM";
            entity.CreatedOn = DateTime.Now;
            _dbSet.Add(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }


        public async Task<int> UpdateChoice(TEntity entity)
        {
            int output = 0;
            entity.ModifiedBy = "SYSTEM";
            entity.ModifiedOn = DateTime.Now;
            _dbSet.Update(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }

        public async Task<IEnumerable<Choice>> GetChoiceList(int questionId)
        {
            return await _dbContext.Choice.Where(e=> e.QuestionID == questionId).ToListAsync();
        }

        public async Task<TEntity> GetChoice(int choiceID)
        {
            return await _dbSet.FindAsync(choiceID);
        }

    }
}
