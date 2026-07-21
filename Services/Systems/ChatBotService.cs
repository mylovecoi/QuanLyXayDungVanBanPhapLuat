using DataAccess;
using DataAccess.Entities.Manages;
using DataAccess.Entities.Systems;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Systems
{
    public interface IChatBotService
    {
        Task<CommonResponse> GetQuestionsAsync(string description);
        Task<CommonResponse> GetAnswerAsync(Guid guid);
        Task<CommonResponse> StoreAsync(string question, string answer, string description);
        Task<CommonResponse> UpdateAsync(Guid? id, string question, string answer, string description);
        Task<CommonResponse> DeleteAsync(Guid guid);
        Task<CommonResponse> EditAsync(Guid guid);

        Task<CommonResponse> GetChatBotAsync(string Search, int PageSize, int PageCurrent);

    }

    public class ChatBotService : IChatBotService
    {
        private readonly ApplicationDbContext _dbContext;
        public ChatBotService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CommonResponse> GetQuestionsAsync(string description)
        {
            try
            {
                var data = await _dbContext.QuestionAnswers
                       .Where(x => EF.Functions.Like(x.Question, $"%{description}%"))
                       .Take(5)
                       .ToListAsync();
                return new CommonResponse { Status = "success", Data = data, Message = "Lấy danh sách câu hỏi thành công!" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }
        public async Task<CommonResponse> GetAnswerAsync(Guid guid)
        {
            try
            {
                var data = await _dbContext.QuestionAnswers.FindAsync(guid);
                string answer = data?.Answer ?? "Xin lỗi, tôi chưa tìm thấy thông tin câu hỏi của bạn.";
                return new CommonResponse { Status = "success", Data = answer, Message = "Lấy câu trả lời thành công!" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi. Vui lòng thử lại sau!" };
            }
        }
     
        public async Task<CommonResponse> StoreAsync(string question, string answer, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(question))
                    return new CommonResponse { Status = "error", Message = "Câu hỏi không được để trống!" };

                if (string.IsNullOrEmpty(answer))
                    return new CommonResponse { Status = "error", Message = "Câu trả lời không được để trống!" };

                var entity = new QuestionAnswer
                {
                    Id = Guid.NewGuid(),
                    Question = question,
                    Answer = answer,
                    Description = description
                };

                await _dbContext.QuestionAnswers.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse { Status = "success", Message = "Thêm mới câu hỏi thành công!" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi thêm dữ liệu!" };
            }
        }

        public async Task<CommonResponse> UpdateAsync(Guid? id, string question, string answer, string description)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(question))
                    return new CommonResponse { Status = "error", Message = "Câu hỏi không được để trống!" };

                if (string.IsNullOrWhiteSpace(answer))
                    return new CommonResponse { Status = "error", Message = "Câu trả lời không được để trống!" };

                if (string.IsNullOrWhiteSpace(description))
                    return new CommonResponse { Status = "error", Message = "Phần mô tả không được để trống!" };


                var entity = await _dbContext.QuestionAnswers.FindAsync(id);
                if (entity == null)
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy câu hỏi cần cập nhật!" };

                entity.Question = question;
                entity.Answer = answer;
                entity.Description = description;

                _dbContext.QuestionAnswers.Update(entity);
                await _dbContext.SaveChangesAsync();

                return new CommonResponse { Status = "success", Message = "Cập nhật câu hỏi thành công!" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi cập nhật dữ liệu!" };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid guid)
        {
            try
            {
                var data = await _dbContext.QuestionAnswers.FindAsync(guid);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu cần cập nhật" };
                }
                _dbContext.QuestionAnswers.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi xóa dữ liệu. Vui lòng thử lại sau!" };
            }
        }

        public async Task<CommonResponse> GetChatBotAsync(string Search, int PageSize, int PageCurrent)
        {
            try
            {
                var data = _dbContext.QuestionAnswers.AsQueryable();

                if (!string.IsNullOrEmpty(Search))
                {
                    data = data.Where(x => x.Question.Contains(Search) || x.Answer.Contains(Search));
                }

                int totalRecords = await data.CountAsync();
                var dataView = await data.Skip((PageCurrent - 1) * PageSize).Take(PageSize).ToListAsync();
                return new CommonResponse { Status = "success", Data = dataView, TotalRecord = totalRecords };
            }
            catch (Exception)
            {
                return new("error", "Có lỗi trong quá trình lấy dữ liệu. Hãy kiểm tra lại!");
            }
        }

        public async Task<CommonResponse> EditAsync(Guid guid)
        {
            try
            {
                var data = await _dbContext.QuestionAnswers.FindAsync(guid);
                if (data == null)
                {
                    return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin dữ liệu!" };
                }
                return new CommonResponse { Status = "success", Data = data ?? new QuestionAnswer() };
            }
            catch
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau!" };
            }
        }
    }
}
