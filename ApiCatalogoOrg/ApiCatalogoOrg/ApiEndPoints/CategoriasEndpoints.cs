using ApiCatalogoOrg.Context;
using ApiCatalogoOrg.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoOrg.ApiEndPoints
{
    public static class CategoriasEndpoints
    {
        public static void MapCategoriasEndpoints(this WebApplication app)
        {
            // definir os endpoints
            //Get Swagger
           // app.MapGet("/", () => "Catálogo de Produtos - 2023").ExcludeFromDescription();


            //Post
            app.MapPost("/categorias", async (Categoria categoria, AppDbContext db)
                =>
            {
                db.Categorias.Add(categoria);
                await db.SaveChangesAsync();

                return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
            });

            //Get
            app.MapGet("/categorias", async (AppDbContext db) =>
                await db.Categorias.ToListAsync()).WithTags("Categorias").RequireAuthorization();

            //GetId
            app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db)
                =>
            {
                return await db.Categorias.FindAsync(id)
                             is Categoria categoria
                             ? Results.Ok(categoria)
                             : Results.NotFound();
            });

            //PutId
            app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, AppDbContext db)
                =>
            {
                if (categoria.CategoriaId != id)
                {
                    return Results.BadRequest();
                }

                var categoriaDB = await db.Categorias.FindAsync(id);

                if (categoriaDB is null) return Results.NotFound();

                categoriaDB.Nome = categoria.Nome;
                categoriaDB.Descricao = categoria.Descricao;

                await db.SaveChangesAsync();
                return Results.Ok(categoriaDB);
            });

            //DeleteId
            app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db)
                =>
            {
                var Categoria = await db.Categorias.FindAsync(id);

                if (Categoria is null)
                {
                    return Results.NotFound();
                }

                db.Categorias.Remove(Categoria);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            
        }

    }
}
