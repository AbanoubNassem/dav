using System.Security.Cryptography;
using System.Text;
using AbanoubNassem.Trinity.Configurations;
using AbanoubNassem.Trinity.Extensions;
using AbanoubNassem.Trinity.Models;
using Microsoft.Data.Sqlite;
using SqlKata.Execution;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
new DavinciPlugin.DavinciPlugin();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

static string GetMd5Hash(string input)
{
    using var md5 = MD5.Create();
    var data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
    var sb = new StringBuilder();

    foreach (var t in data)
    {
        sb.Append(t.ToString("x2"));
    }

    return sb.ToString();
}

builder.Services.AddTrinity(configurations =>
{
    configurations.Prefix = "";
    configurations.ConnectionFactory =
        () => new SqliteConnection(builder.Configuration.GetConnectionString("SQLite"));

    configurations.DatabaseNotifications =
        new TrinityDatabaseNotificationsConfigurations(notificationsTable: "notifications");

    configurations.AuthenticateUser = async (_, email, address) =>
    {
        if (address.Length is < 26 or > 46)
            throw new Exception("Please use a correct address format!");

        await using var conn = new SqliteConnection(builder.Configuration.GetConnectionString("SQLite"));

        var existingUser = await conn.Query("users")
            .Where("id", address)
            .FirstOrDefaultAsync();

        if (existingUser != null && !existingUser?.email!.Equals(email))
        {
            throw new Exception("This Email doesn't belong to the given wallet!");
        }

        if (existingUser == null)
        {
            var res = await conn.Query("users").InsertAsync(new List<KeyValuePair<string, object>>()
            {
                new("id", address),
                new("name", ""),
                new("email", email),
                new("created_at", DateTime.Now),
                new("updated_at", DateTime.Now),
            });
            if (res <= 0)
            {
                throw new Exception("Whoops! there was an error , try again.");
            }
        }

        var user = new TrinityUser(
            address, existingUser?.name.ToString() ?? email.Split('@').First(),
            email, "user",
            $"https://www.gravatar.com/avatar/{GetMd5Hash(email.ToLower().Trim())}"
        );


        return user;
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseTrinity();
app.MapControllers();

app.Run();