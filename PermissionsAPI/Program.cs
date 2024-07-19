using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Nest;
using PermissionsAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PermissionsContext>(options => options.UseSqlServer(connectionString));

var elasticSettings = new ConnectionSettings(new Uri(builder.Configuration["Elasticsearch:Url"]!)).DefaultIndex("permissions");
var elasticClient = new ElasticClient(elasticSettings);
builder.Services.AddSingleton<IElasticClient>(elasticClient);

var kafkaConfig = new ProducerConfig { BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] };
var kafkaProducer = new ProducerBuilder<Null, string>(kafkaConfig).Build();
builder.Services.AddSingleton(kafkaProducer);
builder.Services.AddTransient<SeedDb>();

var app = builder.Build();
SeedData(app);

static void SeedData(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using var scope = scopedFactory!.CreateScope();
    var service = scope.ServiceProvider.GetService<SeedDb>();
    service!.SeedAsync().Wait();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();