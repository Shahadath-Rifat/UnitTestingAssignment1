using System.Runtime.Intrinsics.X86;
using UnitTestingA1Base.Data;
using UnitTestingA1Base.Models;
using static System.Net.Mime.MediaTypeNames;

namespace RecipeUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private AppStorage _appStorage;
        private BusinessLogicLayer _initializeBusinessLogic()
        {
            return new BusinessLogicLayer(new AppStorage());
        }

        // 1st endpoint


        [TestMethod]
        public void GetRecipesByIngredient_ValidId_ReturnsRecipesWithIngredient()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ingredientId = 6;
            int recipeCount = 2;

            // act
            HashSet<Recipe> recipes = bll.GetRecipesByIngredient(ingredientId, null);
            // Assert
            Assert.AreEqual(recipeCount, recipes.Count);
        }

        [TestMethod]
        public void GetRecipesByIngredient_ValidName_ReturnsRecipesWithIngredient()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string ingredientName = "Spaghetti";
            int recipeCount = 1;

            // Act
            HashSet<Recipe> recipes = bll.GetRecipesByIngredient(null, ingredientName);

            // Assert
            Assert.AreEqual(recipeCount, recipes.Count);
        }

        [TestMethod]
        public void GetRecipesByIngredient_InvalidId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int invalidIngredientId = -1;

            // Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                HashSet<Recipe> recipes = bll.GetRecipesByIngredient(invalidIngredientId, null);
            });
        }


        [TestMethod]
        public void GetRecipesByIngredient_InvalidName_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string invalidIngredientName = "Cumin Seed"; // a non-existent ingredient name.

            // Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                HashSet<Recipe> recipes = bll.GetRecipesByIngredient(null, invalidIngredientName);
            });
        }

        [TestMethod]
        public void GetRecipesByIngredient_NoRecipeIngredients_ThrowsInvalidOperationException()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ingredientIdWithNoRecipe = 11; //  an ingredient ID that has no recipe ingredients.

            // Act and Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                HashSet<Recipe> recipes = bll.GetRecipesByIngredient(ingredientIdWithNoRecipe, null);
            });
        }

        [TestMethod]
        public void GetRecipesByIngredient_NoRecipes_ThrowsInvalidOperationException()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string ingredientNameWithNoRecipes = "berries"; // an ingredient name that has no recipes.

            // Act and Assert
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                HashSet<Recipe> recipes = bll.GetRecipesByIngredient(null, ingredientNameWithNoRecipes);
            });
        }


        // 2nd Endpoint 


        [TestMethod]
        public void GetRecipesByDiet_ValidId_ReturnsRecipesWithDiet()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int dietaryId = 2; //  valid dietary restriction ID.
            int recipeCount = 3; //  expected number of recipes that match this dietary restriction.

            // Act
            HashSet<Recipe> recipes = bll.GetRecipesByDiet(dietaryId, null);

            // Assert
            Assert.AreEqual(recipeCount, recipes.Count);
        }

        [TestMethod]
        public void GetRecipesByDiet_ValidName_ReturnsRecipesWithDiet()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string dietName = "Lactose-Free"; //  dietary restriction name.
            int recipeCount = 2; //  the expected number of recipes that match this dietary restriction.

            // Act
            HashSet<Recipe> recipes = bll.GetRecipesByDiet(null, dietName);

            // Assert
            Assert.AreEqual(recipeCount, recipes.Count);
        }

        [TestMethod]
        public void GetRecipesByDiet_InvalidId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int invalidDietId = -1; // an invalid dietary restriction ID.

            // Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                HashSet<Recipe> recipes = bll.GetRecipesByDiet(invalidDietId, null);
            });
        }

        [TestMethod]
        public void GetRecipesByDiet_InvalidName_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string invalidDietName = "NonExistentDiet"; // a non-existent dietary restriction name.

            // Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                HashSet<Recipe> recipes = bll.GetRecipesByDiet(null, invalidDietName);
            });
        }


        [TestMethod]
        public void GetRecipesByDiet_NoIngredientRestrictions_ThrowsArgumentException()
        {
            // Arrange
            BusinessLogicLayer businessLogicLayer = _initializeBusinessLogic();
            int validDietaryId = 6; // An ID with no associated restrictions.

            // Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                HashSet<Recipe> recipes = businessLogicLayer.GetRecipesByDiet(validDietaryId, null);
            });
        }

        [TestMethod]
        public void GetRecipesByDiet_NoRecipeIngredients_ThrowsArgumentException()
        {
            // Arrange
            BusinessLogicLayer businessLogicLayer = _initializeBusinessLogic();
            int validDietaryId = 11; // A valid dietary restriction with no associated ingredients.

            // Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                HashSet<Recipe> recipes = businessLogicLayer.GetRecipesByDiet(validDietaryId, null);
            });
        }

        [TestMethod]
        public void GetRecipesByDiet_NoRecipesFound_ThrowsArgumentException()
        {
            // Arrange
            BusinessLogicLayer businessLogicLayer = _initializeBusinessLogic();
            int validDietaryId = 12; // A valid dietary restriction with associated ingredients, but no matching recipes.

            // Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                HashSet<Recipe> recipes = businessLogicLayer.GetRecipesByDiet(validDietaryId, null);
            });
        }



        // 3rd enpoint 


        [TestMethod]
        public void GetRecipesById_ValidId_ReturnsRecipes()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int validRecipeId = 1; // A valid recipe ID.
            int expectedRecipeCount = 1;

            // Act
            HashSet<Recipe> recipes = bll.GetRecipes(validRecipeId, null);

            // Assert
            Assert.AreEqual(expectedRecipeCount, recipes.Count);
        }

        [TestMethod]
        public void GetRecipesByName_ValidName_ReturnsRecipes()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string validRecipeName = "Spaghetti"; // A valid recipe name.
            int expectedRecipeCount = 1;

            // Act
            HashSet<Recipe> recipes = bll.GetRecipes(null, validRecipeName);

            // Assert
            Assert.AreEqual(expectedRecipeCount, recipes.Count);
        }

        [TestMethod]
        public void GetRecipes_NoRecipesFoundById_ThrowsNoRecipesFoundException()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int invalidRecipeId = 999; // An invalid recipe ID that doesn't exist.

            // Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                HashSet<Recipe> recipes = bll.GetRecipes(invalidRecipeId, null);
            });
        }

        [TestMethod]
        public void GetRecipes_NoRecipesFoundByName_ThrowsNoRecipesFoundException()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string nonExistentRecipeName = "NonExistentRecipe"; // A name that doesn't match any recipes.

            // Act and Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                HashSet<Recipe> recipes = bll.GetRecipes(null, nonExistentRecipeName);
            });
        }

        // 4th endpoint


        [TestMethod]
        public void CreateRecipe_WithValidRecipe_CreatesRecipe()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();

            NewRecipe newRecipe = new NewRecipe()
            {
                Name = "Sample Dish",
                Description = "A new recipe to test",
                Servings = 2,
                Ingredients = { new Ingredient() { Name = "Tomato" } }
            };

            string ingredientName = "Tomato";

            // Act and Assert
            bll.CreateRecipe(newRecipe);
            HashSet<Recipe> recipes = bll.GetRecipesByIngredient(null, ingredientName);

            int expectedCount = 1;
            int actualCount = recipes.Count();
            Assert.AreEqual(expectedCount, actualCount, "Expected one recipe with the specified unique ingredient 'Distinct Flavor'.");
        }

        [TestMethod]
       
        public void CreateRecipe_WithEmptyIngredientsList_CreatesRecipe()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            NewRecipe newRecipe = new NewRecipe()
            {
                Name = "Sample Dish",
                Description = "A new recipe to test",
                Servings = 2,
                Ingredients = new List<Ingredient>() // Provide an empty ingredient list
            };

            string ingredientName = "Tomato";

            // Act and Assert
            bll.CreateRecipe(newRecipe);
            HashSet<Recipe> recipes = bll.GetRecipesByIngredient(null, ingredientName);

            int expectedCount = 1;
            int actualCount = recipes.Count();
            Assert.AreEqual(expectedCount, actualCount, "Expected one recipe with an empty ingredient list.");
        }

        [TestMethod]
        public void CreateRecipe_WithExistingRecipeName_ThrowsArgumentException()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            NewRecipe newRecipe = new NewRecipe()
            {
                Name = "Chocolate Cake"
            };

            // act and assert
            Assert.ThrowsException<ArgumentException>(() =>
            {
                bll.CreateRecipe(newRecipe);
            });
        }

        // endpoint 5



        public void DeleteIngredient_ValidId_RemovesIngredient()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ingredientId = 3;

            // act
            bll.DeleteIngredient(ingredientId, null);

            // assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                bll.GetRecipesByIngredient(ingredientId, null);
            });
        }

        [TestMethod]
        public void DeleteIngredient_MultipleRecipes_ThrowsInvalidOperationException()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ingredientId = 6;

            // act and assert
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                bll.DeleteIngredient(ingredientId, null);
            });
        }

        [TestMethod]
        public void DeleteIngredient_InvalidName_ThrowsArgumentNullException()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int invalidIngredientId = 969; // An invalid ingredient ID that doesn't exist
            string ingredientName = "Invalid Name";

            // Act and Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                bll.DeleteIngredient(invalidIngredientId, ingredientName);
            });
        }


        // endpoint 6



      
        [TestMethod]
        public void DeleteRecipe_ValidId_RemoveRecipeFromDatabase()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int recipeId = 10; // 1 doesnt works as it has muliple ingredients to it

            // act
            bll.DeleteRecipe(recipeId, null);

            // assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                bll.GetRecipes(recipeId, null);
            });
        }



        [TestMethod]
        public void DeleteRecipe_InvalidId_ThrowsArgumentNullException()
        {
            // Arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int recipeId = 16;
            // Act and Assert            
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                bll.DeleteRecipe(recipeId, null);
            });
        }

    }
}