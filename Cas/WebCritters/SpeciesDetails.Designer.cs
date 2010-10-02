namespace WebCritters
{
    partial class SpeciesDetails
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.firstSeen = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.exemplar = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.diet = new System.Windows.Forms.Label();
            this.habitat = new System.Windows.Forms.ListBox();
            this.prey = new System.Windows.Forms.ListBox();
            this.predators = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.population = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.parent1 = new System.Windows.Forms.LinkLabel();
            this.parent2 = new System.Windows.Forms.LinkLabel();
            this.nextSpeciesButton = new System.Windows.Forms.Button();
            this.previousSpeciesButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "First seen in generation: ";
            // 
            // firstSeen
            // 
            this.firstSeen.AutoSize = true;
            this.firstSeen.Location = new System.Drawing.Point(140, 9);
            this.firstSeen.Name = "firstSeen";
            this.firstSeen.Size = new System.Drawing.Size(13, 13);
            this.firstSeen.TabIndex = 1;
            this.firstSeen.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Exemplar:";
            // 
            // exemplar
            // 
            this.exemplar.AutoSize = true;
            this.exemplar.Location = new System.Drawing.Point(71, 45);
            this.exemplar.Name = "exemplar";
            this.exemplar.Size = new System.Drawing.Size(22, 13);
            this.exemplar.TabIndex = 3;
            this.exemplar.Text = "- - -";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Diet: ";
            // 
            // diet
            // 
            this.diet.AutoSize = true;
            this.diet.Location = new System.Drawing.Point(50, 67);
            this.diet.Name = "diet";
            this.diet.Size = new System.Drawing.Size(149, 13);
            this.diet.TabIndex = 5;
            this.diet.Text = "() - x agents, y resource nodes";
            // 
            // habitat
            // 
            this.habitat.FormattingEnabled = true;
            this.habitat.Location = new System.Drawing.Point(10, 106);
            this.habitat.Name = "habitat";
            this.habitat.Size = new System.Drawing.Size(238, 225);
            this.habitat.TabIndex = 6;
            // 
            // prey
            // 
            this.prey.FormattingEnabled = true;
            this.prey.Location = new System.Drawing.Point(264, 106);
            this.prey.Name = "prey";
            this.prey.Size = new System.Drawing.Size(254, 225);
            this.prey.TabIndex = 7;
            this.prey.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.prey_Format);
            // 
            // predators
            // 
            this.predators.FormattingEnabled = true;
            this.predators.Location = new System.Drawing.Point(534, 106);
            this.predators.Name = "predators";
            this.predators.Size = new System.Drawing.Size(240, 225);
            this.predators.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(10, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Habitat";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(261, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Prey";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(531, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Predators";
            // 
            // population
            // 
            this.population.AutoSize = true;
            this.population.Location = new System.Drawing.Point(140, 28);
            this.population.Name = "population";
            this.population.Size = new System.Drawing.Size(34, 13);
            this.population.TabIndex = 13;
            this.population.Text = "x (y%)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Population:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(242, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Descended from:";
            // 
            // parent1
            // 
            this.parent1.AutoSize = true;
            this.parent1.Location = new System.Drawing.Point(337, 27);
            this.parent1.Name = "parent1";
            this.parent1.Size = new System.Drawing.Size(27, 13);
            this.parent1.TabIndex = 15;
            this.parent1.TabStop = true;
            this.parent1.Text = "N/A";
            this.parent1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.parent1_LinkClicked);
            // 
            // parent2
            // 
            this.parent2.AutoSize = true;
            this.parent2.Location = new System.Drawing.Point(337, 45);
            this.parent2.Name = "parent2";
            this.parent2.Size = new System.Drawing.Size(27, 13);
            this.parent2.TabIndex = 16;
            this.parent2.TabStop = true;
            this.parent2.Text = "N/A";
            this.parent2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.parent2_LinkClicked);
            // 
            // nextSpeciesButton
            // 
            this.nextSpeciesButton.Location = new System.Drawing.Point(739, -2);
            this.nextSpeciesButton.Name = "nextSpeciesButton";
            this.nextSpeciesButton.Size = new System.Drawing.Size(34, 23);
            this.nextSpeciesButton.TabIndex = 17;
            this.nextSpeciesButton.Text = "=>";
            this.nextSpeciesButton.UseVisualStyleBackColor = true;
            this.nextSpeciesButton.Click += new System.EventHandler(this.nextSpeciesButton_Click);
            // 
            // previousSpeciesButton
            // 
            this.previousSpeciesButton.Location = new System.Drawing.Point(699, -1);
            this.previousSpeciesButton.Name = "previousSpeciesButton";
            this.previousSpeciesButton.Size = new System.Drawing.Size(34, 23);
            this.previousSpeciesButton.TabIndex = 18;
            this.previousSpeciesButton.Text = "<=";
            this.previousSpeciesButton.UseVisualStyleBackColor = true;
            this.previousSpeciesButton.Click += new System.EventHandler(this.previousSpeciesButton_Click);
            // 
            // SpeciesDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 349);
            this.Controls.Add(this.previousSpeciesButton);
            this.Controls.Add(this.nextSpeciesButton);
            this.Controls.Add(this.parent2);
            this.Controls.Add(this.parent1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.population);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.predators);
            this.Controls.Add(this.prey);
            this.Controls.Add(this.habitat);
            this.Controls.Add(this.diet);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.exemplar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.firstSeen);
            this.Controls.Add(this.label1);
            this.Name = "SpeciesDetails";
            this.Text = "SpeciesDetails";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label firstSeen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label exemplar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label diet;
        private System.Windows.Forms.ListBox habitat;
        private System.Windows.Forms.ListBox prey;
        private System.Windows.Forms.ListBox predators;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label population;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel parent1;
        private System.Windows.Forms.LinkLabel parent2;
        private System.Windows.Forms.Button nextSpeciesButton;
        private System.Windows.Forms.Button previousSpeciesButton;
    }
}