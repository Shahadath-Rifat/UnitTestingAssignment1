using Microsoft.VisualBasic;
using System;
using System.Xml.Linq;
using UnitTestingA1Base.Models;

namespace UnitTestingA1Base.Data
{
    public class BusinessLogicLayer
    {
        private AppStorage _appStorage;

        public BusinessLogicLayer(AppStorage appStorage) {
            _appStorage = appStorage; // contructor maybe
        }

        public HashSet<Recipe> GetAllRecipes() // Optional testing
        {
            // Retrieve all recipes without any filtering.
            HashSet<Recipe> recipes = _appStorage.Recipes.ToHashSet();
            return recipes;
        }

        // all --endpoint 3
        public HashSet<Recipe> GetRecipes(int? id, string? name)
        {
            HashSet<Recipe> recipes = new HashSet<Recipe>();

            if (id != null || !string.IsNullOrEmpty(name))
            {
                if (id != null)
                {
                    recipes = _appStorage.Recipes.Where(r => r.Id == id).ToHashSet();

                    if (recipes.Count == 0) // Check if the set is empty.
                    {
                        throw new ArgumentOutOfRangeException($"No recipes found with ID {id}.");
                    }
                }
                else
                {
                    // Find recipes by name.
                    recipes = _appStorage.Recipes
                        .Where(r => r.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                        .ToHashSet();

                    if (recipes.Count == 0) // Check if the set is empty.
                    {
                        throw new ArgumentOutOfRangeException($"No recipes found with name '{name}'.");
                    }
                }
            }

            return recipes;
        }

        // endpoint 1

        public HashSet<Recipe> GetRecipesByIngredient(int? id, string? name)
        {
            Ingredient ingredient;
            HashSet<Recipe> recipes = new HashSet<Recipe>();

            if (id != null || !string.IsNullOrEmpty(name))
            {
                if (id != null)
                {
                    // Find the ingredient by ID.
                    ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Id == id);

                    if (ingredient == null)
                    {
                        throw new ArgumentOutOfRangeException($"Ingredient with ID {id} not found.");
                    }
                }
                else
                {
                    // Find the ingredient by name (first queried element that contains the entire string).
                    ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));

                    if (ingredient == null)
                    {
                        throw new ArgumentOutOfRangeException($"Ingredient with name '{name}' not found.");
                    }
                }

                // Find recipe ingredients related to the specified ingredient.
                HashSet<RecipeIngredient> recipeIngredients = _appStorage.RecipeIngredients
                     .Where(rI => rI.IngredientId == ingredient.Id)
                     .ToHashSet();

                if (recipeIngredients.Count == 0)
                {
                    throw new InvalidOperationException($"No recipe ingredients found for ingredient with ID {id}.");
                }

                // Find recipes that use the specified ingredient.
                recipes = _appStorage.Recipes
                    .Where(r => recipeIngredients.Any(ri => ri.RecipeId == r.Id))
                    .ToHashSet();

                if (recipes.Count == 0)
                {
                    throw new InvalidOperationException($"No recipes found that use ingredient with ID {id}.");
                }
            }

            return recipes; // Return the set of recipes that follows the requirement
        }

        // endpoint 2

        public HashSet<Recipe> GetRecipesByDiet(int? id, string? name)
        {
            DietaryRestriction dietaryRestriction;
            HashSet<IngredientRestriction> ingredientRestrictions = new HashSet<IngredientRestriction>();
            HashSet<RecipeIngredient> recipeIngredients = new HashSet<RecipeIngredient>();
            HashSet<Recipe> recipes = new HashSet<Recipe>();

            if (id != null || !string.IsNullOrEmpty(name))
            {
                if (id != null)
                {
                    // Find thedietaryRestriction by ID.
                    dietaryRestriction = _appStorage.DietaryRestrictions.FirstOrDefault(dr => dr.Id == id);

                    if (dietaryRestriction == null)
                    {
                        throw new ArgumentOutOfRangeException($"dietaryRestriction with ID {id} not found.");
                    }
                }
                else
                {
                    // Find the dietaryRestriction by name (first queried element that contains the entire string).
                    dietaryRestriction = _appStorage.DietaryRestrictions.FirstOrDefault(i => i.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));

                    if (dietaryRestriction == null)
                    {
                        throw new ArgumentOutOfRangeException($"dietaryRestriction with name '{name}' not found.");
                    }
                }


                // Find ingredient restrictions related to the specified dietary restriction.
                ingredientRestrictions = _appStorage.IngredientRestrictions
                    .Where(IR => IR.DietaryRestrictionId == dietaryRestriction.Id)
                    .ToHashSet();

                if (ingredientRestrictions == null || ingredientRestrictions.Count == 0)
                {
                    throw new ArgumentOutOfRangeException("No ingredient restrictions found for the specified dietary restriction.");
                }

                // Find recipe ingredients related to the ingredient restrictions.
                foreach (var ir in ingredientRestrictions)
                {
                    var ingredientsForRestriction = _appStorage.RecipeIngredients
                        .Where(rI => rI.IngredientId == ir.IngredientId)
                        .ToHashSet();

                    recipeIngredients.UnionWith(ingredientsForRestriction);
                }
                if (recipeIngredients == null || recipeIngredients.Count == 0)
                {
                    throw new ArgumentOutOfRangeException("No recipe ingredients found for the specified ingredient restrictions.");
                }

                // Find recipes that use the specified recipe ingredients.
                recipes = _appStorage.Recipes
                    .Where(r => recipeIngredients.Any(rI => rI.RecipeId == r.Id))
                    .ToHashSet();
                if (recipes == null || recipes.Count == 0)
                {
                    throw new ArgumentOutOfRangeException("No recipes found for the specified recipe ingredients.");
                }

            }  

            return recipes;
        }

        


        // endpoint 4


        public void CreateRecipe(NewRecipe? newRecipe)
        {
            if (newRecipe == null)
            {
                throw new ArgumentNullException("newRecipe cannot be null.");
            }
            if (newRecipe.Ingredients == null)
            {
                throw new ArgumentNullException("Ingredients cannot be null.");
            }
            // if recipe with same name exits
            if (_appStorage.Recipes.Any(nr => nr.Name == newRecipe.Name)) { throw new ArgumentException("Same name recipe exists"); }


            // Create a new recipe with unique ID
            Recipe recipe = new Recipe()
            {
                Id = _appStorage.GeneratePrimaryKey(),
                Name = newRecipe.Name,
                Description = newRecipe.Description,
                Servings = newRecipe.Servings
            };
            // Initialize a collection for recipe ingredients
            HashSet<RecipeIngredient> recipeIngredients = new HashSet<RecipeIngredient>();

            foreach (Ingredient ingredient in newRecipe.Ingredients)
            {   // Check if the ingredient already exists
                Ingredient existingIngredient = _appStorage.Ingredients.FirstOrDefault(eI => eI.Name == ingredient.Name);

                if (existingIngredient == null)
                {
                    Ingredient newIngredient = new Ingredient()
                    {
                        Id = _appStorage.GeneratePrimaryKey(),
                        Name = ingredient.Name
                    };

                    _appStorage.Ingredients.Add(newIngredient);
                    recipeIngredients.Add(new RecipeIngredient { RecipeId = recipe.Id, IngredientId = newIngredient.Id });
                }
                else
                {
                    recipeIngredients.Add(new RecipeIngredient { RecipeId = recipe.Id, IngredientId = existingIngredient.Id });
                }
            }

            foreach (RecipeIngredient recipeIngredient in recipeIngredients)
            {
                _appStorage.RecipeIngredients.Add(recipeIngredient);
            }
            // Add the new recipe to the storage
            _appStorage.Recipes.Add(recipe);
        }
        


        // endpoint 5

        public Ingredient DeleteIngredient(int id, string name)
        {
            // Find the ingredient to delete by ID or name.
            Ingredient ingredientToDelete = _appStorage.Ingredients.FirstOrDefault(i => i.Id == id || i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (ingredientToDelete == null)
            {
                throw new ArgumentNullException("Ingredient not found.");
            }

            // Check if the ingredient is used by multiple recipes.
            int recipeCount = _appStorage.RecipeIngredients.Count(ri => ri.IngredientId == ingredientToDelete.Id);
            if (recipeCount > 1)
            {
                throw new InvalidOperationException("Ingredient is used by multiple recipes and cannot be deleted.");
            }

            // Delete the ingredient.
            _appStorage.Ingredients.Remove(ingredientToDelete);

            // If the ingredient was the only one used in a recipe, delete the recipe and associated data as well.
            if (recipeCount == 1)
            {
                Recipe recipeToDelete = _appStorage.Recipes.FirstOrDefault(r => r.Id == ingredientToDelete.Id);
                if (recipeToDelete != null)
                {
                    _appStorage.Recipes.Remove(recipeToDelete);
                }
            }

            return ingredientToDelete;

        }

        // endpoint 6

        public Recipe DeleteRecipe(int id, string name)
        {
            // Find the recipe to delete by ID or name.
            Recipe recipeToDelete = _appStorage.Recipes.FirstOrDefault(r => r.Id == id || r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (recipeToDelete == null)
            {
                throw new ArgumentNullException("Recipe not found.");
            }

            // Check if the recipe is used by multiple ingredients.
            int ingredientCount = _appStorage.RecipeIngredients.Count(ri => ri.RecipeId == recipeToDelete.Id);

            if (ingredientCount > 1)
            {
                throw new InvalidOperationException("Recipe is used by multiple ingredients and cannot be deleted.");
            }

            // Delete the recipe and associated RecipeIngredients.
            _appStorage.Recipes.Remove(recipeToDelete);

            // Delete associated RecipeIngredients.
            var recipeIngredientsToDelete = _appStorage.RecipeIngredients.Where(ri => ri.RecipeId == recipeToDelete.Id).ToList();
            foreach (var recipeIngredient in recipeIngredientsToDelete)
            {
                _appStorage.RecipeIngredients.Remove(recipeIngredient);
            }

            return recipeToDelete;
        }


        


    }
}
