using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace UseCase
{
	/// <summary>
	/// Summary description for UseCaseForm.
	/// </summary>
	public class UseCaseForm : System.Windows.Forms.Form
	{
        

		public UseCaseForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            this.useCaseModel = new UseCaseModel();

            
            this.useCaseModel.PostConditionsChanged +=
                new ModelChangedEventHandler( this.ModelChanged );
            this.useCaseModel.PreConditionsChanged +=
                new ModelChangedEventHandler( this.ModelChanged );                   
            this.useCaseModel.ActorsChanged += 
               new ModelChangedEventHandler( this.ModelChanged );

            this.useCaseModel.ElementsChanged +=
                new ModelChangedEventHandler( this.ElementsChanged );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.elementsView = new System.Windows.Forms.TreeView();
            this.actorList = new UseCase.ItemListUserControl();
            this.preConditionsList = new UseCase.ItemListUserControl();
            this.postConditionsList = new UseCase.ItemListUserControl();
            this.addElementTextBox = new System.Windows.Forms.TextBox();
            this.addElementButton = new System.Windows.Forms.Button();
            this.deleteElementButton = new System.Windows.Forms.Button();
            this.moveElementUpButton = new System.Windows.Forms.Button();
            this.moveElementDownButton = new System.Windows.Forms.Button();
            this.viewXmlButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // titleTextBox
            // 
            this.titleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.titleTextBox.Location = new System.Drawing.Point(120, 8);
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.TabIndex = 0;
            this.titleTextBox.Text = "";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(32, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Title";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(32, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "ID";
            // 
            // idTextBox
            // 
            this.idTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.idTextBox.Location = new System.Drawing.Point(120, 40);
            this.idTextBox.Name = "idTextBox";
            this.idTextBox.TabIndex = 1;
            this.idTextBox.Text = "";
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Location = new System.Drawing.Point(120, 168);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(144, 48);
            this.textBox2.TabIndex = 4;
            this.textBox2.Text = "";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(32, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 23);
            this.label3.TabIndex = 5;
            this.label3.Text = "Summary";
            // 
            // elementsView
            // 
            this.elementsView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.elementsView.ImageIndex = -1;
            this.elementsView.Location = new System.Drawing.Point(120, 368);
            this.elementsView.Name = "elementsView";
            this.elementsView.SelectedImageIndex = -1;
            this.elementsView.ShowPlusMinus = false;
            this.elementsView.ShowRootLines = false;
            this.elementsView.Size = new System.Drawing.Size(408, 97);
            this.elementsView.TabIndex = 11;
            // 
            // actorList
            // 
            this.actorList.Location = new System.Drawing.Point(24, 72);
            this.actorList.Name = "actorList";
            this.actorList.Size = new System.Drawing.Size(336, 96);
            this.actorList.TabIndex = 12;
            this.actorList.Title = "Actors";
            this.actorList.Delete += new UseCase.ActionPerformedEventHandler(this.DeleteItem);
            this.actorList.Add += new UseCase.ActionPerformedEventHandler(this.AddItem);
            // 
            // preConditionsList
            // 
            this.preConditionsList.Location = new System.Drawing.Point(24, 240);
            this.preConditionsList.Name = "preConditionsList";
            this.preConditionsList.Size = new System.Drawing.Size(336, 88);
            this.preConditionsList.TabIndex = 13;
            this.preConditionsList.Title = "Preconditions";
            this.preConditionsList.Delete += new UseCase.ActionPerformedEventHandler(this.DeleteItem);
            this.preConditionsList.Add += new UseCase.ActionPerformedEventHandler(this.AddItem);
            // 
            // postConditionsList
            // 
            this.postConditionsList.Location = new System.Drawing.Point(24, 480);
            this.postConditionsList.Name = "postConditionsList";
            this.postConditionsList.Size = new System.Drawing.Size(336, 88);
            this.postConditionsList.TabIndex = 14;
            this.postConditionsList.Title = "Postconditions";
            this.postConditionsList.Delete += new UseCase.ActionPerformedEventHandler(this.DeleteItem);
            this.postConditionsList.Add += new UseCase.ActionPerformedEventHandler(this.AddItem);
            // 
            // addElementTextBox
            // 
            this.addElementTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addElementTextBox.Location = new System.Drawing.Point(120, 344);
            this.addElementTextBox.Name = "addElementTextBox";
            this.addElementTextBox.Size = new System.Drawing.Size(280, 20);
            this.addElementTextBox.TabIndex = 15;
            this.addElementTextBox.Text = "";
            // 
            // addElementButton
            // 
            this.addElementButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addElementButton.Location = new System.Drawing.Point(408, 344);
            this.addElementButton.Name = "addElementButton";
            this.addElementButton.Size = new System.Drawing.Size(75, 20);
            this.addElementButton.TabIndex = 16;
            this.addElementButton.Text = "Add";
            this.addElementButton.Click += new System.EventHandler(this.AddElementClick);
            // 
            // deleteElementButton
            // 
            this.deleteElementButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteElementButton.Location = new System.Drawing.Point(536, 376);
            this.deleteElementButton.Name = "deleteElementButton";
            this.deleteElementButton.TabIndex = 17;
            this.deleteElementButton.Text = "Delete";
            this.deleteElementButton.Click += new System.EventHandler(this.deleteElementClick);
            // 
            // moveElementUpButton
            // 
            this.moveElementUpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.moveElementUpButton.Location = new System.Drawing.Point(536, 408);
            this.moveElementUpButton.Name = "moveElementUpButton";
            this.moveElementUpButton.TabIndex = 18;
            this.moveElementUpButton.Text = "Up";
            // 
            // moveElementDownButton
            // 
            this.moveElementDownButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.moveElementDownButton.Location = new System.Drawing.Point(536, 440);
            this.moveElementDownButton.Name = "moveElementDownButton";
            this.moveElementDownButton.TabIndex = 19;
            this.moveElementDownButton.Text = "Down";
            // 
            // viewXmlButton
            // 
            this.viewXmlButton.Location = new System.Drawing.Point(560, 24);
            this.viewXmlButton.Name = "viewXmlButton";
            this.viewXmlButton.TabIndex = 20;
            this.viewXmlButton.Text = "View XML";
            this.viewXmlButton.Click += new System.EventHandler(this.viewXmlClick);
            // 
            // UseCaseForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(656, 595);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.viewXmlButton,
                                                                          this.moveElementDownButton,
                                                                          this.moveElementUpButton,
                                                                          this.deleteElementButton,
                                                                          this.addElementButton,
                                                                          this.addElementTextBox,
                                                                          this.postConditionsList,
                                                                          this.preConditionsList,
                                                                          this.actorList,
                                                                          this.elementsView,
                                                                          this.label3,
                                                                          this.textBox2,
                                                                          this.idTextBox,
                                                                          this.label2,
                                                                          this.label1,
                                                                          this.titleTextBox});
            this.Name = "UseCaseForm";
            this.Text = "UseCaseForm";
            this.ResumeLayout(false);

        }
		#endregion

        public static void Main()
        {
            Application.Run( new UseCaseForm() );
        }

             

        private void ModelChanged( object sender, EventArgs e )
        {
            this.actorList.Items = this.useCaseModel.Actors.Items;
            this.postConditionsList.Items = this.useCaseModel.PostConditions.Items;
            this.preConditionsList.Items = this.useCaseModel.PreConditions.Items;
        }

        private void ElementsChanged( object sender, EventArgs e )
        {
            this.elementsView.Nodes.Clear();
            foreach( IElement element in this.useCaseModel.Elements )
            {
                TreeNode node = new TreeNode( element.Text );
                node.Tag = element;
                this.elementsView.Nodes.Add( node );
            }
        }


        private void AddItem(object sender, UseCase.ActionPerformedEventArgs e)
        {
            if ( sender == this.actorList )
                this.useCaseModel.Actors.Add( e.Text );
            else if ( sender == this.preConditionsList )
                this.useCaseModel.PreConditions.Add( e.Text );
            else if ( sender == this.postConditionsList )
                this.useCaseModel.PostConditions.Add( e.Text );
        
        }

        private void DeleteItem(object sender, UseCase.ActionPerformedEventArgs e)
        {
            if ( sender == this.actorList )
                this.useCaseModel.Actors.Delete( e.Text );
            else if ( sender == this.preConditionsList )
                this.useCaseModel.PreConditions.Delete( e.Text );
            else if ( sender == this.postConditionsList )
                this.useCaseModel.PostConditions.Delete( e.Text );
        
        }

        private void AddElementClick(object sender, System.EventArgs e)
        {
            if ( this.addElementTextBox.Text.Trim() != string.Empty )
            {
                this.useCaseModel.AppendElement( 
                    new FlowElement( this.addElementTextBox.Text ) );
                this.addElementTextBox.Text = string.Empty;
            }
                
        }

        private void deleteElementClick(object sender, System.EventArgs e)
        {
            if ( this.elementsView.SelectedNode != null )
            {
                this.useCaseModel.RemoveElement(
                    (IElement)this.elementsView.SelectedNode.Tag );
            }        
        }

        private void viewXmlClick(object sender, System.EventArgs e)
        {
            if ( this.xmlViewForm == null )
            {
                this.xmlViewForm = new XmlViewForm( this.useCaseModel );
                this.xmlViewForm.Closed += new EventHandler( this.XmlViewFormClosed );
                this.xmlViewForm.Show();
            }            
        }

        private void XmlViewFormClosed( object sender, EventArgs e )
        {
            this.xmlViewForm.Dispose();
            this.xmlViewForm = null;
        }

        private UseCaseModel useCaseModel;

        private XmlViewForm xmlViewForm;

        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox idTextBox;
        private UseCase.ItemListUserControl actorList;
        private UseCase.ItemListUserControl preConditionsList;
        private UseCase.ItemListUserControl postConditionsList;
        private System.Windows.Forms.TextBox addElementTextBox;
        private System.Windows.Forms.Button deleteElementButton;
        private System.Windows.Forms.Button moveElementUpButton;
        private System.Windows.Forms.Button moveElementDownButton;
        private System.Windows.Forms.TreeView elementsView;
        private System.Windows.Forms.Button addElementButton;
        private System.Windows.Forms.Button viewXmlButton;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        

        
        
	}
}
