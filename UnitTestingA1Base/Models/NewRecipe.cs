namespace UnitTestingA1Base.Models
{
    public class NewRecipe
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Servings { get; set; }
        public List<Ingredient>? Ingredients { get; set; } = new List<Ingredient>();
    }
}
