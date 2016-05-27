﻿using BeautifulWeight.Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautifulWeight.Persistence
{
    public interface KitchenLoader : Persistor
    {
        ISet<Ingredient> LoadIngredients();
        ISet<Dish> LoadDishes();
    }
}
