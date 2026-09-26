using System.Data;
using quanlycitieu.DAL;
using quanlycitieu.DTO;
using System;

namespace quanlycitieu.BLL
{
    public class CategoryBLL
    {
        private CategoryDAL categoryDAL = new CategoryDAL();

        public DataTable GetAllCategories(int userId)
        {
            return categoryDAL.GetAllCategories(userId);
        }

        public DataTable GetCategoriesByType(int userId, string type)
        {
            return categoryDAL.GetCategoriesByType(userId, type);
        }

        public void AddCategory(int userId, string name, string type)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type))
                throw new Exception("Vui lòng nhập tên và chọn loại.");
            
            var cat = new CategoryDTO { Name = name, Type = type };
            categoryDAL.InsertCategory(userId, cat);
        }

        public void UpdateCategory(int userId, int id, string name, string type)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type))
                throw new Exception("Dữ liệu không hợp lệ.");

            var cat = new CategoryDTO { Id = id, Name = name, Type = type };
            if (!categoryDAL.UpdateCategory(userId, cat))
                throw new Exception("Không thể cập nhật danh mục của người khác!");
        }

        public void DeleteCategory(int userId, int id)
        {
            if (id <= 0) return;
            try
            {
                if (!categoryDAL.DeleteCategory(userId, id))
                    throw new Exception("Không thể xóa danh mục của người khác!");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("người khác")) throw;
                throw new Exception("Không thể xóa danh mục này vì nó đã được sử dụng trong giao dịch!");
            }
        }
    }
}
