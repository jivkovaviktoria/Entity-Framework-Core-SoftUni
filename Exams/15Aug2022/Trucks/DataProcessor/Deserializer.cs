using System.Linq;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Trucks.Data.Models;
using Trucks.DataProcessor.ImportDto;

namespace Trucks.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Data;

    public class Deserializer
    {
        private static void InitializeMapper()
        {
            Mapper.Initialize(cfg => { cfg.AddProfile<TrucksProfile>(); });
        }

        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            throw new NotImplementedException();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var clientsDto = JsonConvert.DeserializeObject<List<ClientDto>>(jsonString);

            foreach (var cl in clientsDto)
            {
                if (IsValid(cl) == false) sb.AppendLine(ErrorMessage);

                foreach (var tr in cl.Trucks)
                {
                    if (IsValid(tr) == false) sb.AppendLine(ErrorMessage);
                }
            }
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
