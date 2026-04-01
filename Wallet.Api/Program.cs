using Microsoft.EntityFrameworkCore;
using Wallet.Infrastructure.DbContext;
using Wallet.Infrastructure.Repositories;
using Wallet.Application.Interfaces;
using Wallet.Application.Services;
using Wallet.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAÇÃO DO BANCO ---
string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "wallet.db");
string connectionString = $"Data Source={dbPath}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// --- 2. CONFIGURAÇÃO DO CORS (PERMISSÃO) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// --- 3. REGISTRO DE DEPENDÊNCIAS ---
builder.Services.AddScoped<IDbConnection>(sp => new SqliteConnection(connectionString));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ISenhaService, SenhaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 4. PIPELINE DE EXECUÇÃO (A ORDEM AQUI É TUDO!) ---

// OBRIGATÓRIO: UseRouting e UseCors no topo
app.UseRouting();
app.UseCors("AllowReact");

// Depois vem o resto
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

// --- 5. FORÇAR CRIAÇÃO DAS TABELAS ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated(); 
    Console.WriteLine($"✅ Banco de dados pronto em: {dbPath}");
}

app.MapControllers();

app.Run();