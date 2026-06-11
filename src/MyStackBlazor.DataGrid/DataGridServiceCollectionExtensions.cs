using Microsoft.Extensions.DependencyInjection;

namespace MyStackBlazor.DataGrid;

public static class DataGridServiceCollectionExtensions
{
    /// <summary>
    /// Registers MyStackBlazor DataGrid services.
    /// Call in Program.cs: builder.Services.AddMyStackBlazorDataGrid()
    /// </summary>
    public static IServiceCollection AddMyStackBlazorDataGrid(this IServiceCollection services)
    {
        // DataGrid is stateless — no per-instance services needed at DI level.
        // Register your IGridDataProvider<T> implementations separately.
        return services;
    }
}
