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
    public class QuestionService<TEntity> : IQuestion<TEntity> where TEntity : BaseEntity
    {
        private readonly QuizDBContext _dbContext;
        private DbSet<TEntity> _dbSet;
        public QuestionService(QuizDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public async Task<int> DeleteQuestion(TEntity entity)
        {
            int output = 0;
            _dbSet.Remove(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }

        public async Task<QnA> GetQuestionList(int ExamID)
        {
            QnA objQnA = null;
            AnswerDetails _objA = null;
            List<QuestionDetails> _objQlst = new List<QuestionDetails>();
            string _examName = await  _dbContext.Exam.Where(e => e.ExamID == ExamID).Select(o => o.Name).SingleOrDefaultAsync();
            var questions = await _dbContext.Question.Where(q => q.ExamID == ExamID).ToListAsync();
            foreach (var Qitem in questions)
            {
                List<OptionDetails> _objOlst = new List<OptionDetails>();
                QuestionDetails _objQ = new QuestionDetails();
                _objQ.QuestionID = Qitem.QuestionID;
                _objQ.QuestionType = Qitem.QuestionType;
                _objQ.QuestionText = Qitem.DisplayText;
                                          
                var options = await _dbContext.Choice.Where(q => q.QuestionID == Qitem.QuestionID).Select(o => new { OptionID = o.ChoiceID, Option = o.DisplayText }).ToListAsync();
               
                foreach (var Oitem in options)
                {
                    OptionDetails _objO = new OptionDetails()
                    {
                        OptionID=Oitem.OptionID,
                        Option=Oitem.Option
                    };
                    _objOlst.Add(_objO);
                }
                _objQ.options = _objOlst;

                var ans =await _dbContext.Answer.Where(q => q.QuestionID == Qitem.QuestionID).Select(o => new { AnswerID = o.Sl_No, OptionID = o.ChoiceID, Answer = o.DisplayText, }).FirstOrDefaultAsync();
               
                _objA = new AnswerDetails()
                {
                    AnswarID = ans.AnswerID,
                    OptionID=ans.OptionID,
                    Answar = ans.Answer                   
                };
                _objQ.answer = _objA;

                _objQlst.Add(_objQ);
            }

            objQnA = new QnA()
            {
                ExamID = ExamID,
                Exam= _examName,
                questions = _objQlst
            };
            return objQnA;
        }

        public async Task<TEntity> GetQuestion(int QuestionID) {
            return await _dbSet.FindAsync(QuestionID);
        }


        public async Task<IEnumerable<TEntity>> GetQuestionList()
        {
            IEnumerable<TEntity> _questionlst = await _dbSet.ToListAsync();
            return _questionlst;
        }

        public async Task<int> AddQuestion(TEntity entity)
        {
            int output = 0;
            entity.CreatedBy = "SYSTEM";
            entity.CreatedOn = DateTime.Now;
            _dbSet.Add(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }

        public async Task<IQueryable<TEntity>> SearchQuestion(Expression<Func<TEntity, bool>> search = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (search != null)
            {
                query = query.Where(search);
            }
            return query;
        }

        public async Task<int> UpdateQuestion(TEntity entity)
        {
            int output = 0;
            entity.ModifiedBy = "SYSTEM";
            entity.ModifiedOn = DateTime.Now;
            _dbSet.Update(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }

        public int BulkQuestionEntry()
        {
            try
            {
                int result = 0;
                result = _dbContext.Database.ExecuteSqlRaw("Execute BulkQuestionEntry");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
            }
        }

    }
}
