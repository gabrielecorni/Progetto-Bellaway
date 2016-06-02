﻿using BeautifulWeight.Menu;
using BeautifulWeight.Users;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI;

namespace BeautifulWeight.View
{
    class DailyMenuVisualItem : SimpleListViewVisualItem
    {
        MealsListView _mealsListView;
        LightVisualElement _dayLabel;

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            _mealsListView = new MealsListView();
            _dayLabel = new LightVisualElement();
            this.Container.Add(_dayLabel);
            this.Container.Add(_mealsListView);
            this.Padding = new Padding(5);
            this.DrawFill = true;
            this.BackColor = Color.Aqua;
            this.GradientStyle = GradientStyles.Solid;
            this.BorderColor = Color.Aqua;
            this.BorderGradientStyle = GradientStyles.Solid;
            this.DrawBorder = true;
        }

        protected override void SynchronizeProperties()
        {
            base.SynchronizeProperties();
            this._mealsListView.DataSource = (IEnumerable<Meal>)Data["Meals"];
            this._dayLabel.Text = ((DayOfWeek) Data["Day"]).ToString();
        }

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(SimpleListViewVisualItem);
            }
        }
    }
}
