﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cas.Core.Interfaces;

namespace WebCritters
{
    public partial class SpeciesDetails : Form
    {
        private WebCrittersForm ParentForm { get; set; }
        private ISpecies CurrentSpecies { get; set; }
        private ISimulation Simulation { get; set; }

        public SpeciesDetails(WebCrittersForm parent, ISimulation simulation, ISpecies species)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (simulation == null) throw new ArgumentNullException("simulation");
            if (species == null) throw new ArgumentNullException("species");

            ParentForm = parent;
            CurrentSpecies = species;
            Simulation = simulation;

            InitializeComponent();
            UpdateSpeciesDetails();
        }

        public void SetSpecies(ISpecies species)
        {
            if (species == null) throw new ArgumentNullException("species");

            CurrentSpecies = species;
            UpdateSpeciesDetails();
        }
        
        private void UpdateSpeciesDetails()
        {
            this.Text = string.Format("Species {0}", CurrentSpecies);

            this.firstSeen.Text = CurrentSpecies.FirstSeen.ToString();
            this.exemplar.Text = CurrentSpecies.Exemplar.ToShortString();
            this.diet.Text = string.Format("({0}) - {1} agents, {2} resource nodes", CurrentSpecies.DietType, CurrentSpecies.ResourcesFromAgents, CurrentSpecies.ResourcesFromResourceNodes);
            
            int totalPopulation = Simulation.Environment.Locations.Sum(loc => loc.Agents.Count);
            int percentPopulation = (totalPopulation > 0) ? (int)((CurrentSpecies.Population * 100) / totalPopulation) : 0;
            this.population.Text = string.Format("{0} ({1}%)", CurrentSpecies.Population, percentPopulation);

            this.habitat.DataSource = CurrentSpecies.Habitat.ToList();
            this.prey.DataSource = CurrentSpecies.Prey.ToList();
            this.predators.DataSource = CurrentSpecies.Predators.ToList();

            // Hook up the parent species
            var wireUpSpecies = new Action<int, LinkLabel>((index, label) =>
            {
                if (CurrentSpecies.DerivedFromSpeciesIds.Count > index)
                {
                    long parentSpeciesId = CurrentSpecies.DerivedFromSpeciesIds[index];
                    var species = Simulation.GetSpeciesOrFossil(parentSpeciesId);

                    label.Text = species.ToString();
                    label.Enabled = (species is ISpecies);
                }
                else
                {
                    label.Text = string.Empty;
                    label.Enabled = false;
                }
            });

            wireUpSpecies(0, parent1);
            wireUpSpecies(1, parent2);

        }

        private void prey_Format(object sender, ListControlConvertEventArgs e)
        {
            // Format ResourceNodes more tersely
            e.Value = (e.ListItem is IResourceNode) ? (e.ListItem as IResourceNode).ToShortString() : e.ListItem.ToString();
        }

        private void parent1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SpeciesLinkClicked(0);
        }

        private void parent2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SpeciesLinkClicked(1);
        }

        public void SpeciesLinkClicked(int index)
        {
            var newSpecies = (CurrentSpecies.DerivedFromSpeciesIds.Count > index) 
                ? Simulation.GetSpeciesOrFossil(CurrentSpecies.DerivedFromSpeciesIds[index]) as ISpecies 
                : default(ISpecies);

            if (newSpecies != null)
            {
                this.CurrentSpecies = newSpecies;
                this.UpdateSpeciesDetails();
                this.ParentForm.SelectSpecies(newSpecies);
            }
        }

    }
}
