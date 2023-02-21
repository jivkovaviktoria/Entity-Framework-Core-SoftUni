using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using AutoMapper;
using ProductShop.Data;
using ProductShop.InputModels;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var db = new ProductShopContext();
            Mapper.Initialize(cfg => { cfg.AddProfile<ProductShopProfile>(); });
            
            //01. Import Users
            // string inputJson = File.ReadAllText("../../../Datasets/users.json");
            // console.WriteLine(ImportUsers(db, inputJson));
            
            //02. Import Products
            // string inputJson = File.ReadAllText("../../../Datasets/products.json");
            // Console.WriteLine(ImportProducts(db, inputJson));
            
            string inputJson = File.ReadAllText("../../../Datasets/categories.json");
            Console.WriteLine(ImportCategories(db, inputJson));
        }
        
        // 01. ImportUsers
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            var usersDTO = JsonConvert.DeserializeObject<List<UserInputModel>>(inputJson, serializerSettings);

            var users = usersDTO
                .Select(udto => Mapper.Map<User>(udto))
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
        
        // 02. Import Products

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var productsDtos = JsonConvert.DeserializeObject<IEnumerable<ProductInputModel>>(inputJson);
            var products = Mapper.Map<IEnumerable<Product>>(productsDtos);
            context.Products.AddRange(products);
            context.SaveChanges();
            
            return $"Successfully imported {products.Count()}";
        }
        
        // 03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var dtoCategories = JsonConvert.DeserializeObject<IEnumerable<CategoryInputModel>>(inputJson)
                .Where(c => c.Name != null);

            var categories = Mapper.Map<IEnumerable<Category>>(dtoCategories);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }
    }
}