using System.Data;
using Npgsql;
using quanlycitieu.DTO;

namespace quanlycitieu.DAL
{
    public class CategoryDAL
    {
        public DataTable GetAllCategories(int userId)
        {
            // Hiển thị STT liên tục thay vì ID thật
            string query = @"
                SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) as ""STT"", 
                       Name as ""Tên"", Type as ""Loại"" 
                FROM Categories 
                WHERE UserId = @uid OR UserId IS NULL 
                ORDER BY Id";
            NpgsqlParameter[] parameters = { new NpgsqlParameter("@uid", userId) };
            return DbConnection.ExecuteQuery(query, parameters);
        }

        public DataTable GetCategoriesByType(int userId, string type)
        {
            string query = "SELECT Id, Name FROM Categories WHERE Type = @type AND (UserId = @uid OR UserId IS NULL)";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@type", type),
                new NpgsqlParameter("@uid", userId)
            };
            return DbConnection.ExecuteQuery(query, parameters);
        }

        public bool InsertCategory(int userId, CategoryDTO category)
        {
            string query = "INSERT INTO Categories (Name, Type, UserId) VALUES (@name, @type, @uid)";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@name", category.Name),
                new NpgsqlParameter("@type", category.Type),
                new NpgsqlParameter("@uid", userId)
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateCategory(int userId, CategoryDTO category)
        {
            string query = "UPDATE Categories SET Name = @name, Type = @type WHERE Id = @id AND (UserId = @uid OR UserId IS NULL)";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@name", category.Name),
                new NpgsqlParameter("@type", category.Type),
                new NpgsqlParameter("@id", category.Id),
                new NpgsqlParameter("@uid", userId)
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteCategory(int userId, int id)
        {
            // Xóa danh mục (CASCADE sẽ tự xóa giao dịch liên quan)
            string query = "DELETE FROM Categories WHERE Id = @id AND (UserId = @uid OR UserId IS NULL)";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@id", id),
                new NpgsqlParameter("@uid", userId)
            };
            int rows = DbConnection.ExecuteNonQuery(query, parameters);
            
            if (rows > 0)
            {
                // Đánh lại số thứ tự ID liên tục sau khi xóa
                ReorderCategoryIds();
            }
            
            return rows > 0;
        }

        private void ReorderCategoryIds()
        {
            // Dùng transaction để đảm bảo toàn vẹn dữ liệu
            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // Tạm tắt FK constraint bằng cách dùng DEFERRABLE hoặc cập nhật cascade
                        // Bước 1: Tạo bảng tạm lưu mapping Id cũ -> Id mới
                        using (var cmd = new NpgsqlCommand(@"
                            CREATE TEMP TABLE id_map AS
                            SELECT Id as old_id, ROW_NUMBER() OVER (ORDER BY Id) as new_id
                            FROM Categories;
                        ", conn, trans)) { cmd.ExecuteNonQuery(); }

                        // Bước 2: Cập nhật CategoryId trong Transactions theo mapping
                        using (var cmd = new NpgsqlCommand(@"
                            UPDATE Transactions t
                            SET CategoryId = m.new_id::int
                            FROM id_map m
                            WHERE t.CategoryId = m.old_id;
                        ", conn, trans)) { cmd.ExecuteNonQuery(); }

                        // Bước 3: Cập nhật Id trong Categories theo mapping
                        using (var cmd = new NpgsqlCommand(@"
                            UPDATE Categories c
                            SET Id = m.new_id::int
                            FROM id_map m
                            WHERE c.Id = m.old_id;
                        ", conn, trans)) { cmd.ExecuteNonQuery(); }

                        // Bước 4: Reset sequence về giá trị max hiện tại
                        using (var cmd = new NpgsqlCommand(@"
                            SELECT setval('categories_id_seq', COALESCE((SELECT MAX(Id) FROM Categories), 0));
                        ", conn, trans)) { cmd.ExecuteNonQuery(); }

                        // Bước 5: Dọn bảng tạm
                        using (var cmd = new NpgsqlCommand("DROP TABLE IF EXISTS id_map;", conn, trans)) 
                        { cmd.ExecuteNonQuery(); }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }
                }
            }
        }
    }
}
