﻿using Jack.FoodTracker.Entities;
using Jack.FoodTracker.EntityDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations;

namespace Jack.FoodTracker
{
    public class FoodTrackerService
    {
        private readonly UnitOfWork UnitOfWork;

        public FoodTrackerService(UnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;

            //Check Uncategorised Category exists, else initiate it
            List<FoodCategory> cats = UnitOfWork.FoodCategoryRepository.GetAll();

            if(!cats.Where(x => x.Name.Equals("Uncategorised")).Any())
            {
                UnitOfWork.FoodCategoryRepository.Add(new FoodCategory() { Name = "Uncategorised", Order = int.MaxValue });
                UnitOfWork.Save();
            }
        }

        public Food parseFoodDTO(FoodDTO dto)
        {
            int calories = 0;
            double sugar = 0;
            double fat = 0;
            double saturates = 0;
            double salt = 0;
            if (dto.Name == null || dto.Name == "")
            {
                throw new ValidationException("Name cannot be blank");
            }
            if (dto.Category == null)
            {
                throw new ValidationException("Category cannot be blank");
            }
            dto.Name = dto.Name.Trim();
            if (dto.Description != null)
            {
                dto.Description = dto.Description.Trim();
            }
            

            if(dto.Calories != "" && !int.TryParse(dto.Calories, out calories))
            {
                throw new ValidationException("Calories is not a number.");
            }

            if (dto.Sugar != "" && !double.TryParse(dto.Sugar, out sugar))
            {
                throw new ValidationException("Sugar is not a number.");
            }

            if (dto.Fat != "" && !double.TryParse(dto.Fat, out fat))
            {
                throw new ValidationException("Fat is not a number.");
            }

            if (dto.Saturates != "" && !double.TryParse(dto.Saturates, out saturates))
            {
                throw new ValidationException("Saturates is not a number.");
            }

            if (dto.Salt != "" && !double.TryParse(dto.Salt, out salt))
            {
                throw new ValidationException("Salt is not a number.");
            }

            return new Food()
            {
                Name = dto.Name,
                Category = dto.Category,
                Description = dto.Description,
                Calories = calories,
                Sugars = sugar,
                Fat = fat,
                Saturates = saturates,
                Salt = salt
            };
        }
        public void AddBlankFood(Food food)
        {
            UnitOfWork.FoodRepository.Add(food);
        }
        public void AddFood(FoodDTO dto)
        {
            Food newFood;
            try
            {
                newFood = parseFoodDTO(dto);
                Validator.ValidateObject(newFood, new ValidationContext(newFood), true);
            }
            catch(ValidationException exp)
            {
                throw new ArgumentException(exp.Message);
            }
            

            //Check the food doesn't already exist in the database
            if (UnitOfWork.FoodRepository.GetAll().Where(x => x.Name.ToLower().Equals(newFood.Name.ToLower())).Any())
            {
                throw new ArgumentException("This food already exists.");
            }

            //Add the food to the database
            UnitOfWork.FoodRepository.Add(newFood);

        }

        public void EditFood( Food food)
        {
            if (food.Name == null || food.Name == "")
            {
                throw new ValidationException("Name cannot be blank");
            }
            if (food.Category == null)
            {
                throw new ValidationException("Category cannot be blank");
            }

            food.Name = food.Name.Trim();
            if (food.Description != null)
            {
                food.Description = food.Description.Trim();
            }

            try
            {
                Validator.ValidateObject(food, new ValidationContext(food), true);
            }
            catch (ValidationException exp)
            {
                throw new ArgumentException(exp.Message);
            }
            //if the name has changed check that there isn't an existing food with the new name
            if (UnitOfWork.FoodRepository.NameChanged(food) && UnitOfWork.FoodRepository.GetAll().Where(x => x.Name.ToLower().Equals(food.Name.ToLower())).Any())
            {
                throw new ValidationException("A food with this name already exists.");
            }

            food.Update(food);

            //Add the food to the database
            UnitOfWork.FoodRepository.Edit(food);
        }

        public void DeleteFood(Food food)
        {
            try
            {
                UnitOfWork.FoodRepository.Delete(food);
            }
            catch(InvalidOperationException)
            {
                throw new ArgumentException("This food does not exist, hence can't be deleted.");
            }
        }

        private FoodCategory parseCategoryDTO(CategoryDTO dto)
        {
            dto.Name = dto.Name.Trim();

            return new FoodCategory()
            {
                Name = dto.Name
            };
        }

        public FoodCategory AddCategory(CategoryDTO categoryDto)
        {
            FoodCategory newCategory = parseCategoryDTO(categoryDto);

            Validator.ValidateObject(newCategory, new ValidationContext(newCategory), true);

            if (UnitOfWork.FoodCategoryRepository.GetAll().Where(x => x.Name.ToLower().Equals(newCategory.Name.ToLower())).Any())
            {
                throw new ValidationException("A category with this name already exists.");
            }

            UnitOfWork.FoodCategoryRepository.Add(newCategory);
            UnitOfWork.Save();

            return newCategory;
        }

        public void EditFoodCategory(CategoryDTO categoryDto, FoodCategory foodCategory)
        {
            FoodCategory editedCategory = parseCategoryDTO(categoryDto);

            Validator.ValidateObject(editedCategory, new ValidationContext(editedCategory), true);

            if (editedCategory.Name.ToLower() != foodCategory.Name.ToLower() &&
                UnitOfWork.FoodCategoryRepository.GetAll().Where(o => o.Name.ToLower().Equals(editedCategory.Name.ToLower())).Any())
            {
                throw new ArgumentException("A category with this name already exists."); 
            }

            foodCategory.Update(editedCategory);

            UnitOfWork.FoodCategoryRepository.Edit(foodCategory);
        }

        public IList<FoodCategory> GetAllFoodCategories(bool showUncategorised)
        {
            IList<FoodCategory> foodCategories = UnitOfWork.FoodCategoryRepository.GetAll();

            foodCategories = foodCategories.OrderBy(o => o.Order).ToList();

            if(!showUncategorised)
            {
                FoodCategory uncategorised = foodCategories.Where(o => o.Name == "Uncategorised").First();

                foodCategories.Remove(uncategorised);
            }

            return foodCategories;
            
        }

        public IList<Food> GetFoodByCategory(FoodCategory category)
        {

            return UnitOfWork.FoodRepository.GetByCategory(category);
        }

        public void DeleteCategory(FoodCategory foodCategory)
        {
            if (foodCategory.Name == "Uncategorised")
            {
                throw new ArgumentException("Cannot delete the Uncategorised Category.");
            }

            IList<Food> foodsInCat = GetFoodByCategory(foodCategory);

            FoodCategory uncategorised = UnitOfWork.FoodCategoryRepository.GetAll().Where(o => o.Name == "Uncategorised").First();

            foreach (Food food in foodsInCat)
            {
                food.Category = uncategorised;
                UnitOfWork.FoodRepository.Edit(food);
            }

            try
            {
                UnitOfWork.FoodCategoryRepository.Delete(foodCategory);
                UnitOfWork.Save();
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException("This food does not exist, hence can't be deleted.");
            }
        }

        public void SwapCategoryOrder(FoodCategory foodCategory1, FoodCategory foodCategory2)
        {
            int tempOrder = foodCategory1.Order;
            foodCategory1.Order = foodCategory2.Order;
            foodCategory2.Order = tempOrder;

            UnitOfWork.FoodCategoryRepository.Edit(foodCategory1);
            UnitOfWork.FoodCategoryRepository.Edit(foodCategory2);
            UnitOfWork.Save();
        }

        public IList<Food> SearchFoodByName(String searchText)
        {
            return UnitOfWork.FoodRepository.SearchByName(searchText);
        }

        public IList<Food> GetAllFood()
        {
            return UnitOfWork.FoodRepository.GetAll();
        }
        
        public IList<FoodCategory> GetNonEmptyFoodCategories(bool showUncategorised)
        {
            return GetAllFoodCategories(showUncategorised).Where(cat => cat.Foods.Count > 0).ToList();
        }

        public FoodCategory GetCategoryByName(string Name)
        {
            return UnitOfWork.FoodCategoryRepository.GetByName(Name);
        }

        public Food GetFoodByName(string name)
        {
            return UnitOfWork.FoodRepository.GetByName(name);
        }

        public IList<Food> GetChanges(string mode)
        {
            return UnitOfWork.FoodRepository.GetChanges(mode);
        }
        public bool CheckChanges()
        {
            return UnitOfWork.FoodRepository.CheckChanges();
        }
        public void DiscardChanges()
        {
            UnitOfWork.FoodRepository.DiscardChanges();
            UnitOfWork.Save();
        }
        public void Store()
        {
            UnitOfWork.Save();
        }

    }
}
