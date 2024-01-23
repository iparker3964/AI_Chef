using AiChef.Server.Data;
using AiChef.Server.Services;
using AiChef.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace AiChef.Server.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IOpenAIAPI _openAIService;
        public RecipeController(IOpenAIAPI openAi)
        {
            _openAIService = openAi;
        }

        [HttpPost,Route("GetRecipeIdeas")]
        public async Task<ActionResult<List<Idea>>> GetRecipeIdeas(RecipeParms recipeParms)
        {
            string mealTime = recipeParms.MealTime;

            List<string> ingredients = recipeParms.Ingredients
                                                  .Where(x => !string.IsNullOrEmpty(x.Description))
                                                  .Select(x => x.Description!)
                                                  .ToList();

            if (string.IsNullOrEmpty(mealTime))
            {
                mealTime = "Breakfast";
            }

            var ideas = await _openAIService.CreateRecipeIdeas(mealTime,ingredients);

           return ideas;
        }
        [HttpPost,Route("GetRecipe")]
        public async Task<ActionResult<Recipe>> GetRecipe(RecipeParms recipeParms)
        {
            //return SampleData.Recipe;

            List<string> ingredients = recipeParms.Ingredients
                                                  .Where(x => !string.IsNullOrEmpty(x.Description))
                                                  .Select(x => x.Description!)
                                                  .ToList();

            string? title = recipeParms.SelectedIdea;

            if (string.IsNullOrEmpty(title))
            {
                return BadRequest();
            }

            var recipe = await _openAIService.CreateRecipe(title,ingredients);

            return recipe;

        }
        [HttpGet,Route("GetRecipeImage")]
        public async Task<RecipeImage> GetRecipeImage(string title)
        {
            //return SampleData.RecipeImage;
            var recipeImage = await _openAIService.CreateRecipeImage(title);

            return recipeImage ?? SampleData.RecipeImage;
        }
    }
}
