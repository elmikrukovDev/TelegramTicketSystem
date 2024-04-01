using Dapper;
using Npgsql;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace TTS.DBService;

public class UserDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Department { get; set; }
    public string? Post { get; set; }
}

public class UserService
{
    public async Task<UserDTO?> Get(long id)
    {
        using NpgsqlConnection db = new(@"Server=localhost;Port=5432;User Id=postgres;Password=Mc962prm;Database=telagram_bot_db;");
        db.Open();
        var user = await db.QueryFirstOrDefaultAsync<UserDTO>(@$"select * from users where id = {id}", commandTimeout: 10);
        db.Close();
        return user;
    }

    public async Task<IEnumerable<UserDTO>> GetAll()
    {
        using NpgsqlConnection db = new(@"postgresql://localhost/telagram_bot_db");
        db.Open();
        var users = await db.QueryAsync<UserDTO>(@$"select * from users", commandTimeout: 10);
        db.Close();
        return users;
    }

    public async void Add([NotNull]UserDTO user)
    {
        using NpgsqlConnection db = new(@"Server=localhost;Port=5432;User Id=postgres;Password=Mc962prm;Database=telagram_bot_db;");
        db.Open();
        using NpgsqlTransaction tr = await db.BeginTransactionAsync();
        await db.ExecuteAsync($@"
insert into users (name, id, password, firstname, lastname, middlename, post, department) 
values (
'{user.Name}', 
{user.Id}, 
'{user.Password}', 
'{user.FirstName}', 
'{user.LastName}', 
'{user.MiddleName}', 
'{user.Post}', 
'{user.Department}')");
        tr.Commit();
        db.Close();
    }
}