﻿using BeautifulWeight.DietCalculators;
using BeautifulWeight.Kitchen;
using BeautifulWeight.Model;
using BeautifulWeight.Users;
using BeautifulWeight.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace BeautifulWeight.Presenter
{
    class PersonalDetailsPresenter : Presenter
    {
        private readonly Panel _panel;
        private readonly RadPanel _menuPanel;

        public Panel ProfilePanel
        {
            get
            {
                return _panel;
            }
        }

        public RadPanel MenuPanel
        {
            get
            {
                return _menuPanel;
            }
        }

        public PersonalDetailsPresenter(Panel profilePanel, RadPanel profileMenuPanel, SingleProfileModel model) : base(model)
        {
            if (profilePanel == null || profileMenuPanel == null)
                throw new ArgumentNullException("control");
            _panel = profilePanel;
            _menuPanel = profileMenuPanel;
            Model.CurrentUserChanged += CurrentUserChangedHandler;
        }


        public void CurrentUserChangedHandler(Object sender, EventArgs e)
        {
            UserProfile current = Model.CurrentUser;
            if (current == null)
            {
                ClearUser();
            }
            else
            {
                PaintUser(current);
            }
        }

        private void PaintUser(UserProfile up)
        {
            PersonalDetails personalDetails = up.Details;
            ProfilePanel.Controls.Clear();
            TableLayoutPanel detailsPanel = new TableLayoutPanel();
            detailsPanel.RowCount = personalDetails.GetType().GetProperties().Length + 2;
            detailsPanel.ColumnCount = 2;
            detailsPanel.Dock = DockStyle.Top;
            detailsPanel.AutoSize = true;



            int i = 0;
            foreach (PropertyInfo pi in personalDetails.GetType().GetProperties())
            {
                Label name = new Label();
                name.Text = pi.Name;
                name.Dock = DockStyle.Fill;
                name.TextAlign = ContentAlignment.MiddleCenter;
                name.BorderStyle = BorderStyle.Fixed3D;
                detailsPanel.Controls.Add(name, 0, i);

                if (pi.PropertyType == typeof(Load))
                {
                    RadRating value = new RadRating();
                    value.Items.AddRange(new Telerik.WinControls.RadItem[] { new RatingHeartVisualElement(),
                                                                                new RatingHeartVisualElement(),
                                                                                new RatingHeartVisualElement(),
                                                                                new RatingHeartVisualElement(),
                                                                                new RatingHeartVisualElement()});

                    value.SelectionMode = RatingSelectionMode.FullItem;
                    value.Minimum = 0;
                    value.Maximum = 5;
                    value.Value = (int)pi.GetValue(personalDetails);
                    value.AutoSize = true;
                    value.ReadOnly = true;
                    value.Dock = DockStyle.Fill;
                    value.Anchor = AnchorStyles.None;
                    value.Tag = pi;
                    value.LostFocus += FieldChangedHandler;
                    detailsPanel.Controls.Add(value, 1, i++);
                }

                else if (pi.PropertyType == typeof(DateTime))
                {
                    DateTimePicker value = new DateTimePicker();
                    value.Value = (DateTime)pi.GetValue(personalDetails);
                    value.Enabled = false;
                    value.Format = DateTimePickerFormat.Short;
                    value.Dock = DockStyle.Fill;
                    value.DropDownAlign = LeftRightAlignment.Right;
                    value.Tag = pi;
                    value.Leave += FieldChangedHandler;
                    detailsPanel.Controls.Add(value, 1, i++);
                }

                else if (pi.PropertyType == typeof(int))
                {
                    NumericUpDown value = new NumericUpDown();
                    value.Minimum = 0;
                    value.Maximum = 1000;
                    value.Value = (int)pi.GetValue(personalDetails);
                    value.Enabled = false;
                    value.Dock = DockStyle.Fill;
                    value.TextAlign = HorizontalAlignment.Center;
                    value.Tag = pi;
                    value.Leave += FieldChangedHandler;
                    detailsPanel.Controls.Add(value, 1, i++);
                }

                else if (pi.PropertyType.IsEnum)
                {
                    GroupBox value = new GroupBox();
                    int y = 0;
                    foreach (object o in Enum.GetValues(pi.PropertyType))
                    {
                        RadioButton r = new RadioButton();
                        r.Text = o.ToString();
                        if (o.Equals(pi.GetValue(personalDetails)))
                            r.Checked = true;
                        r.Enabled = false;
                        r.Location = new Point(0, y);
                        y += r.Size.Height;
                        value.Tag = pi;
                        value.Leave += FieldChangedHandler;
                        value.Controls.Add(r);
                    }
                    value.Dock = DockStyle.Fill;
                    value.AutoSize = true;
                    detailsPanel.Controls.Add(value, 1, i++);
                }

                else {
                    TextBox value = new TextBox();
                    value.Text = "" + pi.GetValue(personalDetails);
                    value.Dock = DockStyle.Fill;
                    value.TextAlign = HorizontalAlignment.Center;
                    value.Enabled = false;
                    value.BorderStyle = BorderStyle.Fixed3D;
                    value.Tag = pi;
                    value.Leave += FieldChangedHandler;
                    detailsPanel.Controls.Add(value, 1, i++);
                }
            }


            Label goalLabel = new Label();
            goalLabel.Text = "Goal";
            goalLabel.Dock = DockStyle.Fill;
            goalLabel.TextAlign = ContentAlignment.MiddleCenter;
            goalLabel.BorderStyle = BorderStyle.Fixed3D;
            detailsPanel.Controls.Add(goalLabel, 0, i);

            GroupBox goal = new GroupBox();
            int k = 0;
            foreach (object o in Enum.GetValues(typeof(Goal)))
            {
                RadioButton r = new RadioButton();
                r.Text = o.ToString();
                if (o.Equals(up.Goal))
                    r.Checked = true;
                r.Enabled = false;
                r.Location = new Point(0, k);
                k += r.Size.Height;
                goal.Tag = typeof(Goal);
                goal.Leave += FieldChangedHandler;
                goal.Controls.Add(r);
            }
            goal.Dock = DockStyle.Fill;
            goal.Tag = up.Goal.GetType();
            detailsPanel.Controls.Add(goal, 1, i++);

            Label prefLabel = new Label();
            prefLabel.Text = "Preferences";
            prefLabel.Dock = DockStyle.Fill;
            prefLabel.TextAlign = ContentAlignment.MiddleCenter;
            prefLabel.BorderStyle = BorderStyle.Fixed3D;
            detailsPanel.Controls.Add(prefLabel, 0, i);

            Label preferences = new Label();
            string pref = "";
            foreach (object o in up.Preferences)
            {
                if (up.Preferences.IndexOf((Kitchen.Ingredient)o) == up.Preferences.Count - 1)
                    pref += o.ToString() + ".";
                else pref += o.ToString() + ", ";
            }
            preferences.Text = pref;
            preferences.Dock = DockStyle.Fill;
            preferences.TextAlign = ContentAlignment.MiddleCenter;
            preferences.BorderStyle = BorderStyle.Fixed3D;
            preferences.Tag = up.Preferences.GetType();
            detailsPanel.Controls.Add(preferences, 1, i++);


            //float percent = 100F / detailsPanel.RowCount;
            //for (int j = 0; j < detailsPanel.RowCount; j++)
            //{
            //    detailsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, percent));
            //}

            for (int j = 0; j < detailsPanel.ColumnCount; j++)
            {
                detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            }



            ProfilePanel.Controls.Add(detailsPanel);

            TableLayoutPanel buttonPanel = new TableLayoutPanel();
            buttonPanel.RowCount = 1;
            buttonPanel.ColumnCount = 2;
            RadButton modifica = new RadButton();
            modifica.Text = "Modifica";
            modifica.Click += ModifyClickHandler;
            RadButton elimina = new RadButton();
            elimina.Text = "Elimina";
            elimina.Click += DeleteClickHandler;

            modifica.Dock = DockStyle.Fill;
            elimina.Dock = DockStyle.Fill;
            buttonPanel.Controls.Add(modifica, 0, 0);
            buttonPanel.Controls.Add(elimina, 1, 0);
            buttonPanel.Dock = DockStyle.Fill;

            for (int j = 0; j < buttonPanel.ColumnCount; j++)
            {
                buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            }
            MenuPanel.Controls.Clear();
            MenuPanel.Controls.Add(buttonPanel);
        }


        private void FieldChangedHandler(object sender, EventArgs e)
        {
            RadButton salva = MenuPanel.Controls[0].Controls.OfType<RadButton>().First(b => b.Text == "Salva");
            Control field = (Control)sender;
            salva.Enabled = true;
            PropertyInfo pi = (PropertyInfo)field.Tag;
            object o;
            if (field.GetType() == typeof(GroupBox))
                o = System.Enum.Parse(pi.PropertyType, field.Controls.OfType<RadioButton>()
                                      .FirstOrDefault(r => r.Checked).Text);

            else if (field.GetType() == typeof(RadRating))
                o = (int)((RadRating)field).Value;

            else
                o = Convert.ChangeType(field.Text, pi.PropertyType);
            try
            {
                pi.SetValue(Model.CurrentUser.Details, o);
            }
            catch (Exception)
            {
                salva.Enabled = false;
                field.BackColor = Color.Orange;
            }
        }

        private void DeleteClickHandler(object sender, EventArgs e)
        {
            Model.DeleteUser();
        }


        private void ClearUser()
        {
            ProfilePanel.Controls.Clear();
            ProfilePanel.Controls.Clear();
        }

        private void ModifyClickHandler(Object sender, EventArgs e)
        {
            TableLayoutPanel detailsPanel = (TableLayoutPanel)ProfilePanel.Controls[0];
            for (int i = 0; i < detailsPanel.RowCount; i++)
            {
                Control control = detailsPanel.GetControlFromPosition(1, i);
                if (control.GetType() == typeof(RadRating))
                {
                    ((RadRating)control).ReadOnly = false;
                }
                else if (control.GetType() == typeof(GroupBox))
                {
                    (control.Controls.OfType<RadioButton>()).ToList<RadioButton>().ForEach(o => o.Enabled = true);
                }
                else if (control.Tag is Type && ((Type)control.Tag).GetInterfaces().Contains(typeof(IList)) && ((Type)control.Tag).IsGenericType)
                {
                    Button modificaPref = new Button();
                    modificaPref.Text = "Modifica";
                    modificaPref.Click += ModificaPreferenzeClickHandler;
                    modificaPref.Dock = DockStyle.Fill;
                    detailsPanel.Controls.Remove(control);
                    detailsPanel.Controls.Add(modificaPref, 1, i);
                }
                else
                {
                    control.Enabled = true;
                }

            }

            TableLayoutPanel buttonPanel = new TableLayoutPanel();
            buttonPanel.RowCount = 1;
            buttonPanel.ColumnCount = 2;
            RadButton salva = new RadButton();
            salva.Text = "Salva";
            salva.Click += SaveClickHandler;
            RadButton annulla = new RadButton();
            annulla.Text = "Annulla";
            annulla.Click += CancelClickHandler;

            salva.Dock = DockStyle.Fill;
            annulla.Dock = DockStyle.Fill;
            buttonPanel.Controls.Add(salva, 0, 0);
            buttonPanel.Controls.Add(annulla, 1, 0);
            buttonPanel.Dock = DockStyle.Fill;

            for (int j = 0; j < buttonPanel.ColumnCount; j++)
            {
                buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            }
            MenuPanel.Controls.Clear();
            MenuPanel.Controls.Add(buttonPanel);

            Model.StartModify();
        }

        private void ModificaPreferenzeClickHandler(object sender, EventArgs e)
        {
            PreferencesDialog dialog = new PreferencesDialog();
            dialog.PreferencesList.DataSource = ManagerProvider.getModelManager<KitchenManager>().Ingredients;
            dialog.PreferencesList.AllowColumnReorder = true;
            dialog.PreferencesList.FullRowSelect = false;

            foreach (Ingredient i in Model.CurrentUser.Preferences)
            {
                dialog.PreferencesList.Items.Where(el => el.DataBoundItem == i).Single().CheckState = Telerik.WinControls.Enumerations.ToggleState.On;
            }
            dialog.PreferencesList.SelectedItem = null;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                List<Ingredient> newPrefs = new List<Ingredient>(); 
                foreach (ListViewDataItem element in dialog.PreferencesList.CheckedItems)
                {
                    Ingredient i = (Ingredient) element.DataBoundItem;
                    newPrefs.Add(i);
                }
                Model.CurrentUser.Preferences = newPrefs;
            }
        }

        private void SaveClickHandler(object sender, EventArgs e)
        {
            Model.StopModify(true);
        }

        private void CancelClickHandler(object sender, EventArgs e)
        {
            Model.StopModify(false);
        }
    }
}