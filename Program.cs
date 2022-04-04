using SendGrid;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var sendGridApiKey = app.Configuration.GetSection("SendGrid:ApiKey").Get<string>();
var sendGridClient = new SendGridClient(sendGridApiKey);

app.MapGet("/email/template", async (int pageSize, string? pageToken) =>
{
    var queryParams = @"{
            'generations': 'dynamic',
            'page_size': {0},
            'page_token': '{1}'
        }"
        .Replace("{0}", pageSize.ToString())
        .Replace("{1}", pageToken);
    var response = await sendGridClient.RequestAsync(
        BaseClient.Method.GET,
        urlPath: "/templates",
        queryParams: queryParams);
    var templates = await response.Body.ReadAsStringAsync();

    return Results.Content(templates, "application/json");
});

app.MapGet("/email/template/{id}", async (string id) =>
{
    var response = await sendGridClient.RequestAsync(
        BaseClient.Method.GET,
        urlPath: $"/templates/{id}");
    var templates = await response.Body.ReadAsStringAsync();

    return Results.Content(templates, "application/json");
});

app.Run();
