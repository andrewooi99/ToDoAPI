using Dapper;
using System.Data;
using System.Text;
using ToDoAPI.DAL.Data;
using ToDoAPI.DAL.DataModels;
using ToDoAPI.DAL.Interfaces;

namespace ToDoAPI.DAL.Repositories
{
    public class ToDoItemTagRepository : IToDoItemTagRepository
    {
        private readonly DapperContext _context;

        public ToDoItemTagRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ToDoItemTag>> GetToDoItemTagByToDoItemId(long toDoItemId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT * FROM [ToDoItemTags] tdit WITH(NOLOCK) 
                        WHERE 
                             tdit.[ToDoItemId] = @ToDoItemId ");

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ToDoItemId", toDoItemId, DbType.Int64);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ToDoItemTag>(sb.ToString(), parameters);
                return result;
            }
        }

        public async Task<ToDoItemTag?> GetToDoItemTagById(long id, string? accessBy)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT * FROM [ToDoItemTags] tdit WITH(NOLOCK) 
                        WHERE 
                             tdit.[Id] = @Id ");

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int64);

            if (!string.IsNullOrWhiteSpace(accessBy))
            {
                sb.Append(" AND tdit.[CreatedBy] = @AccessBy ");
                parameters.Add("@AccessBy", accessBy, DbType.String);
            }

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<ToDoItemTag>(sb.ToString(), parameters);
                return result;
            }
        }

        public async Task<long> CreateToDoItemTag(ToDoItemTag create)
        {
            var sql = @"INSERT INTO [ToDoItemTags] 
                        ([ToDoItemId], [TagKey], [TagValue], [CreatedAt], [CreatedBy])
                        VALUES
                        (@ToDoItemId, @TagKey, @TagValue, @CreatedAt, @CreatedBy) 
                        SELECT SCOPE_IDENTITY() ";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ToDoItemId", create.ToDoItemId, DbType.Int64);
            parameters.Add("@TagKey", create.TagKey, DbType.String);
            parameters.Add("@TagValue", create.TagValue, DbType.String);
            parameters.Add("@CreatedAt", DateTime.Now, DbType.DateTime);
            parameters.Add("@CreatedBy", create.CreatedBy, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<long>(sql, parameters);
                return result;
            }
        }

        public async Task<bool> UpdateToDoItemTag(ToDoItemTag update)
        {
            var sql = @"UPDATE 
                            [ToDoItemTags] 
                        SET [TagKey] = @TagKey,
                            [TagValue] = @TagValue,
                            [UpdatedAt] = @UpdatedAt,
                            [UpdatedBy] = @UpdatedBy
                        WHERE [Id] = @Id ";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", update.Id, DbType.Int64);
            parameters.Add("@TagKey", update.TagKey, DbType.String);
            parameters.Add("@TagValue", update.TagValue, DbType.String);
            parameters.Add("@UpdatedAt", DateTime.Now, DbType.DateTime);
            parameters.Add("@UpdatedBy", update.UpdatedBy, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var rowAffected = await connection.ExecuteAsync(sql, parameters);
                return rowAffected > 0;
            }
        }

        public async Task<bool> DeleteToDoItemTag(long id)
        {
            var sql = @"DELETE FROM [ToDoItemTags] 
                        WHERE [Id] = @Id ";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int64);

            using (var connection = _context.CreateConnection())
            {
                var rowAffected = await connection.ExecuteAsync(sql, parameters);
                return rowAffected > 0;
            }
        }
    }
}
