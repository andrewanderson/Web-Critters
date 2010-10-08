using System;
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
        private new WebCrittersForm ParentForm { get; set; }
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
            SetSpecies(species, false);
        }

        private void SetSpecies(ISpecies species, bool updateParentForm)
        {
            if (species == null) throw new ArgumentNullException("species");

            CurrentSpecies = species;
            UpdateSpeciesDetails();

            if (updateParentForm)
            {
                this.ParentForm.SelectSpecies(species);
            }
        }
        
        private void UpdateSpeciesDetails()
        {
            this.Text = string.Format("Species {0}", CurrentSpecies);

            this.firstSeen.Text = CurrentSpecies.FirstSeen.ToString();
            this.exemplar.Text = CurrentSpecies.Exemplar.ToShortString();
            this.diet.Text = string.Format("({0}) - {1} agents, {2} resource nodes", CurrentSpecies.DietType, CurrentSpecies.ResourcesFromAgents, CurrentSpecies.ResourcesFromResourceNodes);
            
            int totalPopulation = Simulation.Environment.Locations.Sum(loc => loc.Agents.Count);
            double percentPopulation = (totalPopulation > 0) ? ((double)CurrentSpecies.Population / totalPopulation * 100.0) : 0.0;
            this.population.Text = string.Format("{0} ({1:0.0}%)", CurrentSpecies.Population, percentPopulation);

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
                SetSpecies(newSpecies, true);
            }
        }

        private void nextSpeciesButton_Click(object sender, EventArgs e)
        {
            var speciesList = this.Simulation.Species; // underlying method is Linq, so grab a local copy
            int currentIndex = speciesList.IndexOf(CurrentSpecies);
            
            if (currentIndex < 0) return;

            int nextIndex = 0;
            if (currentIndex < speciesList.Count - 1)
            {
                nextIndex = currentIndex + 1;
            }

            SetSpecies(speciesList[nextIndex], true);
        }

        private void previousSpeciesButton_Click(object sender, EventArgs e)
        {
            var speciesList = this.Simulation.Species; // underlying method is Linq, so grab a local copy
            int currentIndex = speciesList.IndexOf(CurrentSpecies);

            if (currentIndex < 0) return;

            int nextIndex = speciesList.Count - 1;
            if (currentIndex > 0)
            {
                nextIndex = currentIndex - 1;
            }

            SetSpecies(speciesList[nextIndex], true);
        }

    }
}
