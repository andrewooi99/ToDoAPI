using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using System.Text;
using ToDoAPI.DAL.Data;
using ToDoAPI.DAL.DataModels;
using ToDoAPI.DAL.Interfaces;

namespace ToDoAPI.DAL.Repositories
{
    public class ToDoItemRepository : IToDoItemRepository
    {
        private readonly DapperContext _context;

        public ToDoItemRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ToDoItem>> GetToDoItems(string? name, string? description, int[] statuses,
            DateTime dueDateFrom, DateTime dueDateTo, List<KeyValuePair<string, string>>? sortingList, string? accessBy)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT * FROM [ToDoItems] tdi WITH(NOLOCK) 
                        WHERE 
                             tdi.[DueDate] BETWEEN @DueFrom AND @DueTo AND 
                             tdi.[Status] IN @Statuses ");

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@DueFrom", dueDateFrom);
            parameters.Add("@DueTo", dueDateTo);
            parameters.Add("@Statuses", statuses);

            if (!string.IsNullOrWhiteSpace(accessBy))
            {
                sb.Append(" AND ([CreatedBy] = @AccessBy OR CONCAT(',', [SharedBy],',') LIKE CONCAT('%,', @AccessBy ,',%')) ");
                parameters.Add("@AccessBy", accessBy, DbType.String);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                sb.Append(" AND tdi.[Name] LIKE '%' + @Name + '%' ");
                parameters.Add("@Name", name);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                sb.Append(" AND tdi.[Description] LIKE '%' + @Description + '%' ");
                parameters.Add("@Description", description);
            }

            if (sortingList != null && sortingList.Count > 0)
            {
                sb.Append(" ORDER BY ");
                foreach (var sort in sortingList)
                {
                    sb.Append($"{sort.Key} {sort.Value},");
                }

                sb.Length--;
            }

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ToDoItem>(sb.ToString(), parameters);
                return result;
            }
        }

        public async Task<ToDoItem?> GetToDoItemById(long id, string? accessBy)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT * FROM [ToDoItems] WITH(NOLOCK) 
                        WHERE [Id] = @Id ");

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int64);

            if (!string.IsNullOrWhiteSpace(accessBy))
            {
                sb.Append(" AND ([CreatedBy] = @AccessBy OR CONCAT(',', [SharedBy],',') LIKE CONCAT('%,', @AccessBy ,',%')) ");
                parameters.Add("@AccessBy", accessBy, DbType.String);
            }

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<ToDoItem>(sb.ToString(), parameters);
                return result;
            }
        }

        public async Task<long> CreateToDoItem(ToDoItem create)
        {
            var sql = @"INSERT INTO [ToDoItems] 
                        ([Name], [Description], [DueDate], [Status], [Priority],
                         [SharedBy], [CreatedAt], [CreatedBy])
                        VALUES
                        (@Name, @Description, @DueDate, @Status, @Priority,
                         @SharedBy, @CreatedAt, @CreatedBy) 
                        SELECT SCOPE_IDENTITY() ";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Name", create.Name, DbType.String);
            parameters.Add("@Description", create.Description, DbType.String);
            parameters.Add("@DueDate", create.DueDate, DbType.DateTime);
            parameters.Add("@Status", create.Status, DbType.Int32);
            parameters.Add("@Priority", create.Priority, DbType.Int32);
            parameters.Add("@SharedBy", create.SharedBy, DbType.String);
            parameters.Add("@CreatedAt", DateTime.Now, DbType.DateTime);
            parameters.Add("@CreatedBy", create.CreatedBy, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<long>(sql, parameters);
                return result;
            }
        }

        public async Task<bool> CreateToDoItemAndItemTag(ToDoItem create, List<ToDoItemTag> toDoItemTagList)
        {
            var itemSql = @"INSERT INTO [ToDoItems] 
                        ([Name], [Description], [DueDate], [Status], [Priority],
                         [SharedBy], [CreatedAt], [CreatedBy])
                        VALUES
                        (@Name, @Description, @DueDate, @Status, @Priority,
                         @SharedBy, @CreatedAt, @CreatedBy) 
                        SELECT SCOPE_IDENTITY() ";

            DynamicParameters itemParameters = new DynamicParameters();
            itemParameters.Add("@Name", create.Name, DbType.String);
            itemParameters.Add("@Description", create.Description, DbType.String);
            itemParameters.Add("@DueDate", create.DueDate, DbType.DateTime);
            itemParameters.Add("@Status", create.Status, DbType.Int32);
            itemParameters.Add("@Priority", create.Priority, DbType.Int32);
            itemParameters.Add("@SharedBy", create.SharedBy, DbType.String);
            itemParameters.Add("@CreatedAt", DateTime.Now, DbType.DateTime);
            itemParameters.Add("@CreatedBy", create.CreatedBy, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var trans = connection.BeginTransaction())
                {
                    var itemId = await connection.ExecuteScalarAsync<long>(itemSql, itemParameters, trans);
                    if (itemId > 0)
                    {
                        var tagSql = @"INSERT INTO [ToDoItemTags] 
                                     ([ToDoItemId], [TagKey], [TagValue], [CreatedAt], [CreatedBy])
                                     VALUES
                                     (@ToDoItemId, @TagKey, @TagValue, @CreatedAt, @CreatedBy) 
                                     SELECT SCOPE_IDENTITY() ";

                        foreach (var itemTag in toDoItemTagList)
                        {
                            DynamicParameters tagParameters = new DynamicParameters();
                            tagParameters.Add("@ToDoItemId", itemId, DbType.Int64);
                            tagParameters.Add("@TagKey", itemTag.TagKey, DbType.String);
                            tagParameters.Add("@TagValue", itemTag.TagValue, DbType.String);
                            tagParameters.Add("@CreatedAt", DateTime.Now, DbType.DateTime);
                            tagParameters.Add("@CreatedBy", itemTag.CreatedBy, DbType.String);

                            var tagId = await connection.ExecuteScalarAsync<long>(tagSql, tagParameters, trans);
                            if (tagId <= 0)
                            {
                                trans.Dispose();
                                connection.Close();
                                return false;
                            }
                        }
                    }
                    else
                    {
                        trans.Dispose();
                        connection.Close();
                        return false;
                    }

                    trans.Commit();
                }
            }

            return true;
        }

        public async Task<bool> UpdateToDoItem(ToDoItem update)
        {
            var sql = @"UPDATE 
                            [ToDoItems] 
                        SET [Name] = @Name,
                            [Description] = @Description,
                            [DueDate] = @DueDate,
                            [Status] = @Status,
                            [Priority] = @Priority,
                            [SharedBy] = @SharedBy,
                            [UpdatedAt] = @UpdatedAt,
                            [UpdatedBy] = @UpdatedBy
                        WHERE [Id] = @Id ";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", update.Id, DbType.Int64);
            parameters.Add("@Name", update.Name, DbType.String);
            parameters.Add("@Description", update.Description, DbType.String);
            parameters.Add("@DueDate", update.DueDate, DbType.DateTime);
            parameters.Add("@Status", update.Status, DbType.Int32);
            parameters.Add("@Priority", update.Priority, DbType.Int32);
            parameters.Add("@SharedBy", update.SharedBy, DbType.String);
            parameters.Add("@UpdatedAt", DateTime.Now, DbType.DateTime);
            parameters.Add("@UpdatedBy", update.UpdatedBy, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var rowAffected = await connection.ExecuteAsync(sql, parameters);
                return rowAffected > 0;
            }
        }

        public async Task<bool> DeleteToDoItem(long id)
        {
            var sql = @"DELETE FROM [ToDoItems] 
                        WHERE [Id] = @Id ";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int64);

            using (var connection = _context.CreateConnection())
            {
                var rowAffected = await connection.ExecuteAsync(sql, parameters);
                return rowAffected >= 0;
            }
        }

        public async Task<bool> DeleteToDoItemAndTags(long id)
        {
            var toDoSql = @"DELETE FROM [ToDoItems] WHERE [Id] = @Id ";

            var toDoTagSql = @"DELETE FROM [ToDoItemTags] WHERE [ToDoItemId] = @ToDoItemId";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int64);
            parameters.Add("@ToDoItemId", id, DbType.Int64);

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var trans = connection.BeginTransaction())
                {
                    var deletedTagsRows = await connection.ExecuteAsync(toDoTagSql, parameters, trans);
                    if (deletedTagsRows >= 0)
                    {
                        var deletedToDoRows = await connection.ExecuteAsync(toDoSql, parameters, trans);
                        if (deletedToDoRows < 0)
                        {
                            trans.Dispose();
                            connection.Close();
                            return false;
                        }
                    }
                    else
                    {
                        trans.Dispose();
                        connection.Close();
                        return false;
                    }

                    trans.Commit();
                }
            }

            return true;
        }
    }
}
