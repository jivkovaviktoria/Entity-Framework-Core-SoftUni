using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
            
            //03. Import Categories
            // string inputJson = File.ReadAllText("../../../Datasets/categories.json");
            // Console.WriteLine(ImportCategories(db, inputJson));
            
            //04. Import Cateogires and Products
            // string inputJson = File.ReadAllText("../../../Datasets/categories-products.json");
            // Console.WriteLine(ImportCategoryProducts(db, inputJson));
            
            //05. Export Products in Range
            // var result = GetProductsInRange(db);
            // File.WriteAllText("../../../Datasets/products-in-range.json", result);
            
            //06. Export Successfully Solg Products
            //var result = GetSoldProducts(db);
            //File.WriteAllText("../../../Datasets/users-sold-products.json", result);
            
            //07. Export Users and Products
            var result = GetUsersWithProducts(db);
            File.WriteAllText("../../../Datasets/users-and-products.json", result);
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
        
        // 04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProductsDto =
                JsonConvert.DeserializeObject<IEnumerable<CategoryProductInputModel>>(inputJson);
            
            var categoriesProducts = Mapper.Map<IEnumerable<CategoryProduct>>(categoriesProductsDto);
            
            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count()}";
        }
        
        // 05. Export Products in Range

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price > 500 && x.Price <= 1000)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = x.Seller.FirstName + ' ' + x.Seller.LastName
                })
                .OrderBy(x => x.price)
                .ToList();

            var result = JsonConvert.SerializeObject(products, Formatting.Indented);
            return result;
        }
        
        // 06. Export Successfully Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(p => p.Buyer != null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold.Where(p => p.BuyerId != null).Select(a => new
                        {
                            name = a.Name,
                            price = a.Price,
                            buyerFirstName = a.Buyer.FirstName,
                            buyerLastName = a.Buyer.LastName
                        })
                        .ToList()
                })
                .OrderBy(x => x.lastName)
                .ThenBy(x => x.lastName)
                .ToList();

            var result = JsonConvert.SerializeObject(users, Formatting.Indented);
            return result;
        }
        
        // 08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Include(x=>x.ProductsSold)
                .ToList()
                .Where(x => x.ProductsSold.Any(b=>b.BuyerId !=null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold.Where(b => b.BuyerId != null).Count(),
                        products = x.ProductsSold.Where(b=>b.BuyerId != null).Select(p => new
                            {
                                name = p.Name,
                                price = p.Price
                            })
                            .ToList()
                    }
                })
                .ToList()
                .OrderByDescending(x => x.soldProducts.products.Count())
                .ToList();

            var resultObject = new
            {
                usersCount = users.Count(),
                users = users
            };

            var jsonSerializingOptions = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var resultJson = JsonConvert.SerializeObject(resultObject, Formatting.Indented, jsonSerializingOptions);

            return resultJson;
        }
    }
}