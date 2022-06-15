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
    public class AnswerService<TEntity> : IAnswer<TEntity> where TEntity : BaseEntity
    {
       private readonly QuizDBContext _dbContext;
       private DbSet<TEntity> _dbSet;
       public AnswerService(QuizDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAnswerList()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<int> DeleteAnswer(TEntity entity)
        {
            int output = 0;
            _dbSet.Remove(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }

       public async Task<int> AddAnswer(TEntity entity)
        {
            int output = 0;
            entity.CreatedBy = "SYSTEM";
            entity.CreatedOn = DateTime.Now;
            _dbSet.Add(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }


        public async Task<int> UpdateAnswer(TEntity entity)
        {
            int output = 0;
            entity.ModifiedBy = "SYSTEM";
            entity.ModifiedOn = DateTime.Now;
            _dbSet.Update(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }

        public async Task<TEntity> GetAnswer(int answerID)
        {
            return await _dbSet.FindAsync(answerID);
        }

    }
}
