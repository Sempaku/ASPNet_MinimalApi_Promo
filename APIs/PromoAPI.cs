using Promo_2022_API.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Promo_2022_API.APIs
{
    public class PromoAPI : IApi
    {
        private readonly List<Promo> promoList;

        public PromoAPI(List<Promo> promoList)
        {
            this.promoList = promoList;
        }

        public void RegisterAPI(WebApplication app)
        {
            app.MapPost("/promo", AddPromoPost);

            app.MapGet("/promo", GetAllPromo);
            
            app.MapGet("/promo/{id}", GetPromoById);

            app.MapPut("/promo/{id}", PutPromoById);

            app.MapDelete("promo/{id}", DeletePromoById);

            app.MapPost("/promo/{id}/participant", AddParticipantByPromoId);

            app.MapDelete("/promo/{promoId}/participant/{participantId}", DeleteParticipantByPromoId);

            app.MapPost("/promo/{id}/prize", AddPrizeByPromoId);

            app.MapDelete("/promo/{promoId}/prize/{prizeId}", DeletePrizeByPromoId);

            app.MapPost("/promo/{id}/raffle", CheckRafflePost);
        }

        private IResult AddPromoPost(Promo fullPromo)
        {
            promoList.Add(fullPromo);
            return Results.Ok(fullPromo.Id);
        }

        private IResult GetAllPromo()
        {
            var listWithoutLists = (
                from item in promoList
                select new Promo()
                {
                    Id = null,
                    Name = item.Name,
                    Description = item.Description,
                    Participants = null,
                    Prizes = null
                                        
                }).ToList();
            
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };


            return Results.Json(listWithoutLists, jsonOptions, "application/json", StatusCodes.Status200OK);
            
        }

        private IResult GetPromoById(int id)
        {
            Promo? fullPromo = promoList.FirstOrDefault(p => p.Id == id);
            if (fullPromo == null) return Results.NotFound();

            return Results.Ok(fullPromo);
        }

        private IResult PutPromoById(int id, Promo promo) 
        {
            var promoIndex = promoList.FindIndex(p => p.Id == id);
            if (promoIndex <= 0) 
                return Results.NotFound(id);

            var oldPromo = promoList[promoIndex];

            if (promo.Name == string.Empty || promo.Name == null)
                promo.Name = promoList[promoIndex].Name;

            promo.Id = promoList[promoIndex].Id;

            var newPromo = new Promo
            {
                Id = oldPromo.Id,
                Name = promo.Name,
                Description = promo.Description,
                Participants = oldPromo.Participants,
                Prizes = oldPromo.Prizes
            };

            promoList[promoIndex] = newPromo;
                return Results.Ok();
        }

        private IResult DeletePromoById(int id)
        {
            var index = promoList.FindIndex(p => p.Id == id);
            if (index <= 0)
                Results.NotFound();

            promoList.RemoveAt(index);

            return Results.Ok();
        }

        private IResult AddParticipantByPromoId(int id, Participant participant) 
        {
            var promoIndex = promoList.FindIndex(p => p.Id == id);
            if (promoIndex <= 0) 
                return Results.NotFound();

            promoList[promoIndex].Participants?.Add(participant);

            return Results.Ok(participant.Id);
        }

        private IResult DeleteParticipantByPromoId(int promoId, int participantId)
        {
            Promo promo = promoList.FirstOrDefault(prom => prom.Id == promoId);
            if (promo == null) 
                return Results.NotFound();

            int? partIndex = promo.Participants?.FindIndex(part => part.Id == participantId);
            promo.Participants?.RemoveAt((int)partIndex!);

            return Results.Ok();

        }

        private IResult AddPrizeByPromoId(int id, Prize prize)
        {
            var promo = promoList.FirstOrDefault(p => p.Id == id);
            if (promo == null) 
                return Results.NotFound();

            promo.Prizes?.Add(prize);

            return Results.Ok(prize.Id);
        }

        private IResult DeletePrizeByPromoId(int promoId, int prizeId) 
        {
            var promo = promoList.FirstOrDefault(prom => prom.Id == promoId);
            if (promo == null) 
                return Results.NotFound();

            var prizeIndex = promo.Prizes?.FindIndex(prize => prize.Id == prizeId);
            if (prizeIndex <= 0) 
                return Results.NotFound();

            promo.Prizes?.RemoveAt((int) prizeIndex!);

            return Results.Ok();
        }

        private IResult CheckRafflePost(int id) 
        {
            var promo = promoList.FirstOrDefault(p => p.Id == id);
            if (promo == null) 
                return Results.NotFound();

            bool CanTakeResult = CanResult(id, promo);

            if (CanTakeResult == false)
                return Results.Conflict();

            return Results.Ok(GetResult(id, promo));
        }


        private static List<ResultPromo> GetResult(int id, Promo promo)
        {
            var prizeList = promo.Prizes;
            var participantsList = promo.Participants;

            List<ResultPromo> results = new List<ResultPromo>();

            for (int indexPrize = 0; indexPrize < prizeList.Count; indexPrize++)
            {
                for (int indexPart = indexPrize; indexPart <= prizeList.Count; indexPart++)
                {
                    results.Add(new ResultPromo { Prize = prizeList[indexPrize], Winner = participantsList[indexPart] });
                    break;
                }
            }
            return results;
        }

        private static bool CanResult(int id, Promo promo)
        {
            int? lenPart = promo.Participants?.Count;
            int? lenPrize = promo.Prizes?.Count;

            if (lenPart == lenPrize && lenPart > 0)
                return true;

            return false;
        }

    }
}
