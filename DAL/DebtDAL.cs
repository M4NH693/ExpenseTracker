using System;
using System.Data;
using System.Collections.Generic;
using Npgsql;
using quanlycitieu.DTO;

namespace quanlycitieu.DAL
{
    public class DebtDAL
    {
        public bool InsertDebt(DebtDTO debt)
        {
            string query = @"
                INSERT INTO Debts (UserId, DebtType, PersonName, Amount, StartDate, DueDate, Status, Note)
                VALUES (@uid, @type, @person, @amt, @sdate, @ddate, @status, @note)";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@uid", debt.UserId),
                new NpgsqlParameter("@type", debt.DebtType),
                new NpgsqlParameter("@person", debt.PersonName.Trim()),
                new NpgsqlParameter("@amt", debt.Amount),
                new NpgsqlParameter("@sdate", DateOnly.FromDateTime(debt.StartDate)),
                new NpgsqlParameter("@ddate", DateOnly.FromDateTime(debt.DueDate)),
                new NpgsqlParameter("@status", debt.Status),
                new NpgsqlParameter("@note", (object?)debt.Note ?? "")
            };

            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateDebt(DebtDTO debt)
        {
            string query = @"
                UPDATE Debts 
                SET DebtType = @type, PersonName = @person, Amount = @amt, 
                    StartDate = @sdate, DueDate = @ddate, Note = @note
                WHERE Id = @id AND UserId = @uid AND Status = 0";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@type", debt.DebtType),
                new NpgsqlParameter("@person", debt.PersonName.Trim()),
                new NpgsqlParameter("@amt", debt.Amount),
                new NpgsqlParameter("@sdate", DateOnly.FromDateTime(debt.StartDate)),
                new NpgsqlParameter("@ddate", DateOnly.FromDateTime(debt.DueDate)),
                new NpgsqlParameter("@note", (object?)debt.Note ?? ""),
                new NpgsqlParameter("@id", debt.Id),
                new NpgsqlParameter("@uid", debt.UserId)
            };

            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteDebt(int userId, int debtId)
        {
            string query = "DELETE FROM Debts WHERE Id = @id AND UserId = @uid";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@id", debtId),
                new NpgsqlParameter("@uid", userId)
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public DataTable GetDebts(int userId, int? debtTypeFilter, int? statusFilter, string searchText)
        {
            string where = "UserId = @uid";
            var paramList = new List<NpgsqlParameter> { new NpgsqlParameter("@uid", userId) };

            if (debtTypeFilter.HasValue && debtTypeFilter.Value > 0)
            {
                where += " AND DebtType = @dtype";
                paramList.Add(new NpgsqlParameter("@dtype", debtTypeFilter.Value));
            }

            if (statusFilter.HasValue && statusFilter.Value >= 0)
            {
                where += " AND Status = @status";
                paramList.Add(new NpgsqlParameter("@status", statusFilter.Value));
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                where += " AND (PersonName ILIKE @search OR Note ILIKE @search)";
                paramList.Add(new NpgsqlParameter("@search", $"%{searchText.Trim()}%"));
            }

            string query = $@"
                SELECT 
                    Id,
                    CASE DebtType WHEN 1 THEN 'Cho vay' ELSE 'Đi vay' END as ""Loai"",
                    DebtType,
                    PersonName as ""NguoiLienQuan"",
                    Amount as ""SoTien"",
                    StartDate::timestamp as ""NgayVay"",
                    DueDate::timestamp as ""HanTra"",
                    CASE Status WHEN 1 THEN 'Đã tất toán' ELSE 'Chưa trả' END as ""TrangThai"",
                    Status,
                    Note as ""GhiChu"",
                    TransactionId
                FROM Debts
                WHERE {where}
                ORDER BY Status ASC, DueDate ASC, Id DESC";

            return DbConnection.ExecuteQuery(query, paramList.ToArray());
        }

        public DebtDTO? GetDebtById(int userId, int debtId)
        {
            string query = @"
                SELECT 
                    Id, UserId, DebtType, PersonName, Amount, 
                    StartDate::timestamp as StartDate, 
                    DueDate::timestamp as DueDate, 
                    Status, Note, TransactionId 
                FROM Debts 
                WHERE Id = @id AND UserId = @uid";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@id", debtId),
                new NpgsqlParameter("@uid", userId)
            };

            DataTable dt = DbConnection.ExecuteQuery(query, parameters);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new DebtDTO
            {
                Id = Convert.ToInt32(row["Id"]),
                UserId = Convert.ToInt32(row["UserId"]),
                DebtType = Convert.ToInt32(row["DebtType"]),
                PersonName = row["PersonName"]?.ToString() ?? "",
                Amount = Convert.ToDecimal(row["Amount"]),
                StartDate = ToDateTime(row["StartDate"]),
                DueDate = ToDateTime(row["DueDate"]),
                Status = Convert.ToInt32(row["Status"]),
                Note = row["Note"]?.ToString() ?? "",
                TransactionId = row["TransactionId"] != DBNull.Value ? Convert.ToInt32(row["TransactionId"]) : null
            };
        }

        private static DateTime ToDateTime(object? val)
        {
            if (val == null || val == DBNull.Value) return DateTime.Today;
            if (val is DateTime dt) return dt;
            if (val is DateOnly d) return d.ToDateTime(TimeOnly.MinValue);
            if (DateTime.TryParse(val.ToString(), out DateTime parsed)) return parsed;
            return DateTime.Today;
        }

        /// <summary>
        /// Thực thi tất toán nợ và TỰ ĐỘNG sinh giao dịch đối ứng (Thu Nhập hoặc Chi Tiêu) trong 1 Transaction an toàn
        /// </summary>
        public bool SettleDebtWithTransaction(int userId, int debtId, out string resultMessage)
        {
            resultMessage = "";
            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Lấy thông tin khoản nợ
                        int debtType;
                        string personName;
                        decimal amount;
                        string note;
                        int status;

                        using (var cmd = new NpgsqlCommand(
                            "SELECT DebtType, PersonName, Amount, Note, Status FROM Debts WHERE Id = @id AND UserId = @uid FOR UPDATE",
                            conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@id", debtId);
                            cmd.Parameters.AddWithValue("@uid", userId);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    resultMessage = "Không tìm thấy khoản vay/nợ này!";
                                    trans.Rollback();
                                    return false;
                                }
                                debtType = reader.GetInt32(0);
                                personName = reader.GetString(1);
                                amount = reader.GetDecimal(2);
                                note = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                status = reader.GetInt32(4);
                            }
                        }

                        if (status == 1)
                        {
                            resultMessage = "Khoản nợ này đã được tất toán trước đó!";
                            trans.Rollback();
                            return false;
                        }

                        // 2. Tìm hoặc tạo danh mục tương ứng (Thu Nợ / Trả Nợ)
                        string targetCatName = debtType == 1 ? "Thu Nợ" : "Trả Nợ";
                        string targetCatType = debtType == 1 ? "Thu Nhập" : "Chi Tiêu";
                        int categoryId;

                        using (var cmd = new NpgsqlCommand(
                            "SELECT Id FROM Categories WHERE Name = @name AND Type = @type AND (UserId = @uid OR UserId IS NULL) LIMIT 1",
                            conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@name", targetCatName);
                            cmd.Parameters.AddWithValue("@type", targetCatType);
                            cmd.Parameters.AddWithValue("@uid", userId);
                            var catObj = cmd.ExecuteScalar();
                            if (catObj != null && catObj != DBNull.Value)
                            {
                                categoryId = Convert.ToInt32(catObj);
                            }
                            else
                            {
                                // Tạo mới nếu chưa có
                                using (var cmdInsertCat = new NpgsqlCommand(
                                    "INSERT INTO Categories (Name, Type, UserId) VALUES (@name, @type, @uid) RETURNING Id",
                                    conn, trans))
                                {
                                    cmdInsertCat.Parameters.AddWithValue("@name", targetCatName);
                                    cmdInsertCat.Parameters.AddWithValue("@type", targetCatType);
                                    cmdInsertCat.Parameters.AddWithValue("@uid", userId);
                                    categoryId = Convert.ToInt32(cmdInsertCat.ExecuteScalar());
                                }
                            }
                        }

                        // 3. Tự động sinh giao dịch đối ứng vào Transactions
                        string transType = debtType == 1 ? "Thu Nhập" : "Chi Tiêu";
                        decimal transAmount = debtType == 1 ? amount : -amount;
                        string transDesc = debtType == 1 
                            ? $"Thu hồi nợ từ {personName}" + (string.IsNullOrWhiteSpace(note) ? "" : $" ({note})")
                            : $"Trả nợ cho {personName}" + (string.IsNullOrWhiteSpace(note) ? "" : $" ({note})");

                        int newTransId;
                        using (var cmdTrans = new NpgsqlCommand(
                            @"INSERT INTO Transactions (UserId, Type, CategoryId, Amount, Date, Description)
                              VALUES (@uid, @type, @catId, @amt, CURRENT_TIMESTAMP, @desc)
                              RETURNING Id",
                            conn, trans))
                        {
                            cmdTrans.Parameters.AddWithValue("@uid", userId);
                            cmdTrans.Parameters.AddWithValue("@type", transType);
                            cmdTrans.Parameters.AddWithValue("@catId", categoryId);
                            cmdTrans.Parameters.AddWithValue("@amt", transAmount);
                            cmdTrans.Parameters.AddWithValue("@desc", transDesc);
                            newTransId = Convert.ToInt32(cmdTrans.ExecuteScalar());
                        }

                        // 4. Cập nhật Status = 1 và TransactionId vào Debts
                        using (var cmdUpdateDebt = new NpgsqlCommand(
                            "UPDATE Debts SET Status = 1, TransactionId = @tid WHERE Id = @id AND UserId = @uid",
                            conn, trans))
                        {
                            cmdUpdateDebt.Parameters.AddWithValue("@tid", newTransId);
                            cmdUpdateDebt.Parameters.AddWithValue("@id", debtId);
                            cmdUpdateDebt.Parameters.AddWithValue("@uid", userId);
                            cmdUpdateDebt.ExecuteNonQuery();
                        }

                        trans.Commit();
                        resultMessage = $"Tất toán thành công!\nĐã tự động tạo giao dịch '{transType}' số tiền {amount:N0} đ vào sổ chi tiêu.";
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        resultMessage = "Lỗi tất toán: " + ex.Message;
                        return false;
                    }
                }
            }
        }
    }
}
