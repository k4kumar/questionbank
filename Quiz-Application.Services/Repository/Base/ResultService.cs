using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Quiz_Application.Services.Entities;
using Quiz_Application.Services.Repository.Interfaces;
using System.Text;

namespace Quiz_Application.Services.Repository.Base
{
    public class ResultService<TEntity> : IResult<TEntity> where TEntity : BaseEntity
    {
        private readonly QuizDBContext _dbContext;
        private DbSet<TEntity> _dbSet;
        public ResultService(QuizDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }
        public async Task<int> AddResult(List<TEntity> entity)
        {
            int output = 0;
            _dbSet.AddRange(entity);
            output = await _dbContext.SaveChangesAsync();
            return output;
        }
        public async Task<IEnumerable<QuizAttempt>> GetAttemptHistory(string argCandidateID)
        {
            try
            {
                List<QuizAttempt> obj = await _dbContext.Set<QuizAttempt>().FromSqlRaw(@"SELECT
                CAST(ROW_NUMBER() OVER (ORDER BY R.CreatedOn DESC) AS int) Sl_No,
                R.SessionID,
                R.ExamID,
                E.Name AS Exam,
                CONVERT(varchar, R.CreatedOn, 106) AS Date,
                (CAST(COUNT(R.Sl_No) as varchar(20)) + '/' + CAST(CAST(E.FullMarks AS INT) AS VARCHAR(20))) AS Score,
				CASE 
					WHEN ((CAST(COUNT(R.Sl_No) AS decimal)/E.FullMarks *100) >50) THEN '1' 
					ELSE '0'
				END AS 'Status'
                FROM Result R
                LEFT JOIN Exam E ON R.ExamID = E.ExamID
                WHERE R.CandidateID ='" + argCandidateID + "' AND R.IsCorrent = 1"
                +"GROUP BY R.SessionID, R.ExamID, E.Name, E.FullMarks, R.CreatedOn", argCandidateID).ToListAsync();
                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
            }
        }
        public async Task<IEnumerable<QuizReport>> ScoreReport(ReqReport argRpt)
        {
            try
            {
                List<QuizReport> obj = await _dbContext.Set<QuizReport>().FromSqlRaw(@"EXEC GetReport {0},{1},{2}", argRpt.ExamID, argRpt.CandidateID, argRpt.SessionID).ToListAsync();  
                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
            }
        }
        public async Task<string> GetCertificateString(ReqCertificate argRpt)
        {
            Candidate _candidate =await _dbContext.Candidate.Where(e => e.Candidate_ID == argRpt.CandidateID.ToString()).FirstOrDefaultAsync();          

            try
            {
                string cert =null;
                cert= @"<html>
    <head>
        <style type='text/css'>
            
            body {
                color: black;
                display: table;
                font-family: Georgia, serif;
                font-size: 24px;
                text-align: center;
            }
            .center{
                text-align: center;
            }
            .container {
                border: 20px solid tan;
                width:1400px;
                height: 800px;
                display: table-cell;
                vertical-align: middle;
            }
            .logo {
                color: red;
				font-size: 48px;
            }

            .marquee {
                color: tan;
                font-size: 48px;
                margin: 20px;
            }
            .assignment {
                margin: 20px;
            }
            .person {
                border-bottom: 2px solid black;
                font-size: 32px;
                font-style: italic;
                margin: 20px auto;
                width: 400px;
            }
            .reason {
                margin: 20px;
            }
        </style>
    </head>
    <body>
        <div class='container center'>
            <div class='logo'>
                Rupali Bank Ltd.
            </div>

            <div class='marquee'>
                Certificate of Completion
            </div>

            <div class='assignment'>
                This certificate is presented to
            </div>

            <div class='person'>
                " + _candidate.Name + @"
            </div>
            <div class='reason'>
                For " + argRpt.Exam + @" <br/>
                with the score of <b>" + argRpt.Score + @"</b>
            </div>
        </div>
    </body>
</html>";
                return cert.ToString();
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
