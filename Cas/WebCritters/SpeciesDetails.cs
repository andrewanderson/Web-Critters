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
        private ISpecies CurrentSpecies { get; set; }
        private ISimulation Simulation { get; set; }

        public SpeciesDetails(ISimulation simulation, ISpecies species)
        {
            if (simulation == null) throw new ArgumentNullException("simulation");
            if (species == null) throw new ArgumentNullException("species");

            CurrentSpecies = species;
            Simulation = simulation;

            InitializeComponent();
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
        }

    }
}
