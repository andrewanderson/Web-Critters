﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cas.Core;
using Cas.Core.Events;
using Cas.Core.Interfaces;
using Cas.TestImplementation;

namespace WebCritters
{
    public partial class WebCrittersForm : Form
    {
        public WebCrittersForm()
        {
            InitializeComponent();
        }

        private ISimulation CasSimulation;

        private SpeciesDetails SpeciesDetailsWindow = null;

        private CancellationTokenSource RunGenerationCTS = null;
        //private bool IsStopping = false;
        
        private void WebCrittersForm_Load(object sender, EventArgs e)
        {
            ToggleSimulationControls(false);
            ToggleLoadControls(true);
            UpdateSimulationDetails(false);
        }

        private void createSimulation_Click(object sender, EventArgs e)
        {
            ToggleLoadControls(false);
            CreateSimulationFromLoadControls();
            ToggleSimulationControls(true);

            this.locationList.Focus();
        }

        private void CreateSimulationFromLoadControls()
        {
            var config = new Configuration();

            // Agent settings
            config.AgentSettings.InteractionsPerGeneration = double.Parse(interactionsPerGeneration.Text);
            config.AgentSettings.MaximumAttemptsToFindSuitableTarget = int.Parse(scavengeAttempts.Text);
            config.AgentSettings.MaximumMigrationBonus = double.Parse(maxMigrationBonus.Text) / 100.0;
            config.AgentSettings.MigrationBaseChance = double.Parse(migrationBaseChance.Text) / 100.0;
            config.AgentSettings.MutationChance = double.Parse(mutationPercent.Text) / 100.0;
            config.AgentSettings.RandomDeathChance = double.Parse(randomDeathPercent.Text) / 100.0;
            config.AgentSettings.ReproductionInheritance = int.Parse(reproductionInheritancePercent.Text) / 100.0;
            config.AgentSettings.ReproductionThreshold = int.Parse(reproductionThresholdPercent.Text) / 100.0;
            config.AgentSettings.StartingTagComplexity = int.Parse(startingComplexity.Text);

            // Environment settings
            config.EnvironmentSettings.GlobalResourcePoolSize = int.Parse(uniqueResourceCount.Text);
            config.EnvironmentSettings.SetLocationCapacity(int.Parse(minimumCapacity.Text), int.Parse(maximumCapacity.Text));
            config.EnvironmentSettings.SetLocationResourceNodeRange(int.Parse(minResourceNodes.Text), int.Parse(maxResourceNodes.Text));
            config.EnvironmentSettings.SetLocationResourcesPerNode(int.Parse(minResourcesPerNode.Text), int.Parse(maxResourcesPerNode.Text));
            config.EnvironmentSettings.MaximumUpkeepCostPerLocation = int.Parse(maximumUpkeep.Text);
            config.EnvironmentSettings.UpkeepChance = int.Parse(upkeepPercent.Text) / 100.0;

            // Resource settings
            config.ResourceSettings.AllowWildcards = allowWildcards.Checked;
            config.ResourceSettings.Count = int.Parse(numberOfResources.Text);
            config.ResourceSettings.NormalToWildcardRatio = int.Parse(normalToWildcardRatio.Text);

            // Tag settings
            config.TagSettings.MaxSize = int.Parse(maxComplexity.Text);

            CasSimulation = new GridSimulation(int.Parse(gridHeight.Text), int.Parse(gridWidth.Text), config);
            CasSimulation.GenerationFinished += new EventHandler(CasSimulation_GenerationFinished);
            CasSimulation.LogHistory = this.trackAgentHistory.Checked;
            CasSimulation.Initialize();

            // TODO: Load up some default agents
            LoadStartingAgents(
                int.Parse(minStartingPopulation.Text),
                int.Parse(maxStartingPopulation.Text),
                int.Parse(startingComplexity.Text));

            UpdateSimulationDetails(false);
        }

        void CasSimulation_GenerationFinished(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                this.runProgressBar.PerformStep();

                if (updateGuiWhileProcessing.Checked)
                {
                    int generationUpdateFrequency = int.Parse(this.generationsBetweenUpdate.Text);
                    if (CasSimulation.CurrentGeneration % generationUpdateFrequency == 0)
                    {
                        UpdateSimulationDetails(true);
                    }
                    else
                    {
                        this.currentGeneration.Text = this.CasSimulation.CurrentGeneration.ToString();
                    }
                }
            }));
        }

        private void LoadStartingAgents(int min, int max, int tagComplexity)
        {
            if (max < min) throw new ArgumentOutOfRangeException("max", "max must exceed min");

            int startingResourcePercent = int.Parse(this.agentResourcePercent.Text);

            foreach (var location in CasSimulation.Environment.Locations)
            {
                int agentsToAdd = min + RandomProvider.Next(max - min);
                for (int i = 0; i < agentsToAdd; i++)
                {
                    var agent = new GridAgent();
                    var cell = GridCell.New(tagComplexity);

                    int startingResourceCount = (int)(cell.Size / 100.0 * startingResourcePercent);
                    cell.AddRandomResources(startingResourceCount);

                    agent.Cells.Add(cell);
                    this.CasSimulation.RegisterBirth(agent, new CreationEvent(location, 0));

                    location.Agents.Add(agent);
                }
            }
        }

        private void UpdateSimulationDetails(bool generationCompleted)
        {
            if (this.CasSimulation == null)
            {
                this.shortSimulationDescription.Text = "Not Loaded";
                this.currentGeneration.Text = "0";
                this.totalPopulation.Text = "0";
                this.totalSpecies.Text = "0";
                this.percentCarnivore.Text = "0%";
                this.percentHerbivore.Text = "0%";
                this.percentOmnivore.Text = "0%";

                this.locationList.DataSource = null;
                this.speciesList.DataSource = null;
            }
            else
            {
                this.shortSimulationDescription.Text = string.Format("{0} : size {1}", this.CasSimulation.Environment, this.CasSimulation.Environment.Locations.Count);
                this.currentGeneration.Text = this.CasSimulation.CurrentGeneration.ToString();

                long population = this.CasSimulation.Environment.Locations.Sum(loc => loc.Agents.Count);
                this.totalPopulation.Text = population.ToString();

                this.totalSpecies.Text = CasSimulation.Species.Count.ToString();

                if (CasSimulation.Species.Count > 0)
                {
                    int carnivoreCount = 0;
                    int herbivoreCount = 0;
                    int omnivoreCount = 0;

                    CasSimulation.Species.ForEach(species =>
                        {
                            switch (species.DietType)
                            {
                                case DietType.Carnivore:
                                    carnivoreCount++;
                                    break;
                                case DietType.Herbivore:
                                    herbivoreCount++;
                                    break;
                                case DietType.Omnivore:
                                    omnivoreCount++;
                                    break;
                            }
                        });

                    this.percentCarnivore.Text = string.Format("{0:0.0}%", ((double)carnivoreCount / CasSimulation.Species.Count * 100.0));
                    this.percentHerbivore.Text = string.Format("{0:0.0}%", ((double)herbivoreCount / CasSimulation.Species.Count * 100.0));
                    this.percentOmnivore.Text = string.Format("{0:0.0}%", ((double)omnivoreCount / CasSimulation.Species.Count * 100.0));
                }

                int selectedLocationIndex = this.locationList.SelectedIndex;
                if (generationCompleted)
                {
                    this.locationList.DataSource = null;
                    this.locationList.SelectedItem = null;
                    this.speciesList.DataSource = null;
                    this.speciesList.SelectedItem = null;
                }
                this.locationList.DataSource = this.CasSimulation.Environment.Locations;

                if (selectedLocationIndex != -1)
                {
                    this.locationList.SelectedIndex = selectedLocationIndex;
                }

                this.speciesList.DataSource = CasSimulation.Species;
            }
        }

        private void ToggleSimulationControls(bool isEnabled)
        {
            splitContainer.Panel2.Enabled = isEnabled;
        }


        private void ToggleLoadControls(bool isEnabled)
        {
            this.splitContainer.Panel1.Enabled = isEnabled;
        }

        private void runGenerations_Click(object sender, EventArgs e)
        {
            int generationCount = int.Parse(this.generationsToRun.Text);
            if (generationCount < 1) return;

            // Set up the progress bar
            this.runProgressBar.Value = 0;
            this.runProgressBar.Minimum = 0;
            this.runProgressBar.Maximum = generationCount;

            // Spawn this work on a different thread.
            if (!(this.RunGenerationCTS == null || this.RunGenerationCTS.IsCancellationRequested)) this.RunGenerationCTS.Cancel();
            this.RunGenerationCTS = new CancellationTokenSource();
            var token = this.RunGenerationCTS.Token;
            Task.Factory.StartNew(() =>
                                      {
                                          for (int i = 0; i < generationCount; i++)
                                          {
                                              if (token.IsCancellationRequested) return;
                                              this.CasSimulation.RunGeneration();
                                          }
                                      })
                .ContinueWith(t =>
                                  {
                                      if (t.IsFaulted && t.Exception != null) DisplayException(t.Exception);
                                      this.Invoke(new MethodInvoker(() =>
                                                                        {
                                                                            this.UpdateSimulationDetails(true);
                                                                            this.runGenerations.Enabled = true;
                                                                            this.stopButton.Enabled = false;
                                                                        }));
                                  });

            this.stopButton.Enabled = true;
            this.runGenerations.Enabled = false;
        }

        private static void DisplayException(Exception e)
        {
            if (e == null) return;

            MessageBox.Show(
                string.Format(
                    @"{0} : {1}

Trace:
{2}",
    e.GetType().Name,
    e.Message,
    e.StackTrace), "Generation processing exception");
        }

        #region Validation

        void intTextBox_Validating(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            int i;

            e.Cancel = !int.TryParse(tb.Text, out i);
            errorProvider1.SetError(tb, "Must be a valid integer.");
        }

        void doubleTextBox_Validating(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            double d;

            e.Cancel = !double.TryParse(tb.Text, out d);
            errorProvider1.SetError(tb, "Must be a valid double.");
        }

        void control_Validated(object sender, System.EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            errorProvider1.SetError(control, "");
        }

        #endregion

        private void locationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var location = locationList.SelectedItem as ILocation;
            if (location == null)
            {
                locationName.Text = "N/A";
                upkeepCost.Text = "0";
                locationCapacity.Text = "0";
                locationRenewableCount.Text = "0";

                BindListBox<IResourceNode>(locationResourceList, locationResourceCount, null);
                BindListBox<IAgent>(agentList, locationAgentCount, null);
                BindListBox<ILocation>(connectionList, locationConnectionCount, null);
            }
            else
            {
                locationName.Text = location.ToString();
                upkeepCost.Text = location.UpkeepCost.ToString();
                locationCapacity.Text = location.ResourceCapacity.ToString();
                locationRenewableCount.Text = location.ResourceAllocation.Count.ToString();

                BindListBox<IResourceNode>(locationResourceList, locationResourceCount, location.CurrentResources);
                BindListBox<IAgent>(agentList, locationAgentCount, location.Agents);
                BindListBox<ILocation>(connectionList, locationConnectionCount, location.Connections);
            }
        }

        /// <summary>
        /// On double click we want to navigate to the selected item
        /// </summary>
        private void connectionList_DoubleClick(object sender, EventArgs e)
        {
            if (connectionList.SelectedItem != null)
            {
                this.locationList.SelectedItem = connectionList.SelectedItem;
            }
        }

        private void agentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeSelectedAgent(agentList.SelectedItem as IAgent);

        }

        // Drill into agent
        private void agentAgentList_DoubleClick(object sender, EventArgs e)
        {
            ChangeSelectedAgent(agentAgentList.SelectedItem as IAgent);
        }

        private void ChangeSelectedAgent(IAgent agent)
        {
            if (agent == null)
            {
                agentName.Text = "N/A";

                BindListBox<IResourceNode>(agentResourceList, agentResourceCount, null);
                BindListBox<IAgent>(agentAgentList, agentAgentsCount, null);
                BindListBox<ICell>(agentCellList, agentCellCount, null);
                BindListBox<IEvent>(agentHistoryList, null, null);
            }
            else
            {
                agentName.Text = agent.ToString();

                BindListBox<IResourceNode>(agentResourceList, agentResourceCount, agent.CurrentResources);
                BindListBox<IAgent>(agentAgentList, agentAgentsCount, agent.Agents);
                BindListBox<ICell>(agentCellList, agentCellCount, agent.Cells);
                BindListBox<IEvent>(agentHistoryList, null, agent.History);
            }
        }

        private void agentCellList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cell = agentCellList.SelectedItem as ICell;
            if (cell == null)
            {
                cellName.Text = "N/A";
                offenseTag.Text = "-";
                defenseTag.Text = "-";
                exchangeTag.Text = "-";
                matingTag.Text = "-";
                cellReservoir.Text = "() -";
            }
            else
            {
                cellName.Text = cell.ToString();
                offenseTag.Text = cell.Offense.ToString();
                defenseTag.Text = cell.Defense.ToString();
                exchangeTag.Text = cell.Exchange.ToString();
                matingTag.Text = cell.Mating.ToString();

                cellReservoir.Text = string.Format("({0}) - {1}", cell.CurrentResourceCount, cell.ShowResourcePool(","));
            }
        }

        private void agentHistoryList_MouseMove(object sender, MouseEventArgs e)
        {
            string tip = "";

            int index = agentHistoryList.IndexFromPoint(e.Location);
            if ((index >= 0) && (index < agentHistoryList.Items.Count))
            {
                tip = agentHistoryList.Items[index].ToString();
            }
            if (tip != toolTip1.GetToolTip(agentHistoryList))
            {
                toolTip1.SetToolTip(agentHistoryList, tip);
            }
        }

        private void BindListBox<T>(ListBox listBox, Label label, List<T> list)
        {
            if (listBox == null) throw new ArgumentNullException("listBox");

            string count = "0";

            if (list == null || list.Count == 0)
            {
                listBox.DataSource = null;
                listBox.SelectedItem = null;
            }
            else
            {
                listBox.DataSource = list;
                listBox.SelectedItem = list.FirstOrDefault();
                count = list.Count.ToString();
            }

            if (label != null)
            {
                label.Text = count;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (!(this.RunGenerationCTS == null || this.RunGenerationCTS.IsCancellationRequested))
            {
                this.RunGenerationCTS.Cancel();
            }

            this.stopButton.Enabled = false;
            this.runGenerations.Enabled = true;
        }

        private bool WasEnterPressed(object sender, KeyPressEventArgs e)
        {
            return (e.KeyChar == (char)13);
        }

        private void generationsToRun_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (WasEnterPressed(sender, e))
            {
                runGenerations_Click(null, null);
            }
        }

        private static Brush CarnivoreBrush = Brushes.Red;
        private static Brush HerbivoreBrush = Brushes.Green;
        private static Brush OmnivoreBrush = Brushes.DodgerBlue;
        private static Brush NeutralBush = Brushes.Black;

        private void agentList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1) return;
            var agent = agentList.Items[e.Index] as IAgent;

            DrawListItemColoredByDietType((ListBox)sender, e, agent.Species, false);
        }

        private void speciesList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1) return;
            var species = speciesList.Items[e.Index] as ISpecies;

            DrawListItemColoredByDietType((ListBox)sender, e, species, true);
        }

        private void DrawListItemColoredByDietType(ListBox sender, DrawItemEventArgs e, ISpecies species, bool includeSpeciesPercentPopulation)
        {
            if (e.Index == -1) return;

            e.DrawBackground();

            Brush myBrush = NeutralBush;

            switch (species.DietType)
            {
                case DietType.Carnivore:
                    myBrush = CarnivoreBrush;
                    break;
                case DietType.Herbivore:
                    myBrush = HerbivoreBrush;
                    break;
                case DietType.Omnivore:
                    myBrush = OmnivoreBrush;
                    break;
            }

            string itemText = sender.Items[e.Index].ToString();
            if (includeSpeciesPercentPopulation)
            {
                int totalPopulation = this.CasSimulation.Environment.Locations.Sum(loc => loc.Agents.Count);
                double percentPopulation = (totalPopulation > 0) ? ((species.Population * 100.0) / (double)totalPopulation) : 0.0;

                itemText = string.Format("{1:0.0}% - {0}", itemText, percentPopulation);
            }
            
            e.Graphics.DrawString(itemText, e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);

            e.DrawFocusRectangle();
        }

        private void trackAgentHistory_CheckedChanged(object sender, EventArgs e)
        {
            CasSimulation.LogHistory = this.trackAgentHistory.Checked;
        }

        public void SelectSpecies(ISpecies species) 
        {
            if (species == null) throw new ArgumentNullException("species");

            this.speciesList.SelectedItem = species;
        }

        private void speciesList_DoubleClick(object sender, EventArgs e)
        {
            if ((sender as ListBox).SelectedIndex > -1)
            {
                var species = (sender as ListBox).SelectedItem as ISpecies;

                if (SpeciesDetailsWindow == null || SpeciesDetailsWindow.IsDisposed)
                {
                    SpeciesDetailsWindow = new SpeciesDetails(this, this.CasSimulation, species);
                    SpeciesDetailsWindow.Show();
                }
                else
                {
                    SpeciesDetailsWindow.SetSpecies(species);
                    SpeciesDetailsWindow.Focus();
                }
            }
        }
    }

}
