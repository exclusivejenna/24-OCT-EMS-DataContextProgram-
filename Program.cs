
using Microsoft.AspNetCore.Components.Authorization;
using BlazorCRUDSsr.Data;
using BlazorUserRole.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorUserRole;
using BlazorUserRole.Authentication;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
.AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

/// 1.
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthStateProvider>();

//2.
builder.Services.AddDbContext<DataContext>
    (o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



///3.       Creating Policiy for Roles (2 person with same role admin one can add record):
    builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = IdentityConstants.ApplicationScheme;
    o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();
    builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<DataContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();


//4.        Adding Policy:
    builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminPolicy", policy => {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("admin","moderator");
    });
  
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);    
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


/// 5:
app.MapSignOutEndpoint();
app.Run();
