using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        public ISimulation CasSimulation;

        private bool IsStopping = false;
        private bool IsRunning = false;

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
            CasSimulation = new GridSimulation(
                int.Parse(gridHeight.Text),
                int.Parse(gridWidth.Text),
                int.Parse(minResourceNodes.Text),
                int.Parse(maxResourceNodes.Text),
                int.Parse(minResourcesPerNode.Text),
                int.Parse(maxResourcesPerNode.Text),
                int.Parse(startingComplexity.Text),
                int.Parse(uniqueResourceCount.Text),
                int.Parse(maximumUpkeep.Text),
                int.Parse(upkeepPercent.Text) / 100.0,
                double.Parse(interactionsPerGeneration.Text),
                int.Parse(reproductionThresholdPercent.Text) / 100.0,
                int.Parse(reproductionInheritancePercent.Text) / 100.0,
                double.Parse(migrationBaseChance.Text) / 100.0,
                double.Parse(maxMigrationBonus.Text) / 100.0,
                double.Parse(randomDeathPercent.Text) / 100.0);

            CasSimulation.GenerationFinished += new EventHandler(CasSimulation_GenerationFinished);

            CasSimulation.Initialize(
                int.Parse(numberOfResources.Text), 
                int.Parse(normalToWildcardRatio.Text), 
                int.Parse(maxComplexity.Text));

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
                    agent.History.Add(new CreationEvent(location.Id, 0));
                    var cell = GridCell.New(tagComplexity);

                    int startingResourceCount = (int)(cell.Size / 100.0 * startingResourcePercent);
                    cell.AddRandomResources(startingResourceCount);

                    agent.Cells.Add(cell);
                    this.CasSimulation.Register(agent);

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

                this.locationList.DataSource = null;
            }
            else
            {
                this.shortSimulationDescription.Text = string.Format("{0} : size {1}", this.CasSimulation.Environment, this.CasSimulation.Environment.Locations.Count);
                this.currentGeneration.Text = this.CasSimulation.CurrentGeneration.ToString();

                long population = this.CasSimulation.Environment.Locations.Sum(loc => loc.Agents.Count);
                this.totalPopulation.Text = population.ToString();

                if (generationCompleted)
                {
                    this.locationList.DataSource = null;
                    this.locationList.SelectedItem = null;
                }
                this.locationList.DataSource = this.CasSimulation.Environment.Locations;
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
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                IsRunning = true;
                IsStopping = false;

                for (int i = 0; (i < generationCount && !IsStopping); i++)
                {
                    this.CasSimulation.RunGeneration();
                }

                IsStopping = false;
                IsRunning = false;
            });
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

                BindListBox<IResourceNode>(locationResourceList, locationResourceCount, null);
                BindListBox<IAgent>(agentList, locationAgentCount, null);
                BindListBox<ILocation>(connectionList, locationConnectionCount, null);
            }
            else
            {
                locationName.Text = location.ToString();
                upkeepCost.Text = location.UpkeepCost.ToString();

                BindListBox<IResourceNode>(locationResourceList, locationResourceCount, location.ResourceAllocation);
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
                cellReservoir.Text = "() -";
            }
            else
            {
                cellName.Text = cell.ToString();
                offenseTag.Text = cell.Offense.ToString();
                defenseTag.Text = cell.Defense.ToString();
                exchangeTag.Text = cell.Exchange.ToString();

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
            IsStopping = true;
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

    }

}
