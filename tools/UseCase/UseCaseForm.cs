using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

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
                new EventHandler( this.ModelChanged );
            this.useCaseModel.PreConditionsChanged +=
                new EventHandler( this.ModelChanged );                   
            this.useCaseModel.ActorsChanged += 
               new EventHandler( this.ModelChanged );

            this.useCaseModel.ElementsChanged +=
                new EventHandler( this.ElementsChanged );

            this.idTextBox.Leave += new EventHandler( 
                    this.IdChanged );
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
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.summaryTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.elementsView = new System.Windows.Forms.TreeView();
            this.addElementTextBox = new System.Windows.Forms.TextBox();
            this.addElementButton = new System.Windows.Forms.Button();
            this.deleteElementButton = new System.Windows.Forms.Button();
            this.moveElementUpButton = new System.Windows.Forms.Button();
            this.moveElementDownButton = new System.Windows.Forms.Button();
            this.viewXmlButton = new System.Windows.Forms.Button();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.browseButton = new System.Windows.Forms.Button();
            this.actorList = new UseCase.ItemListUserControl();
            this.preConditionsList = new UseCase.ItemListUserControl();
            this.postConditionsList = new UseCase.ItemListUserControl();
            this.SuspendLayout();
            // 
            // nameTextBox
            // 
            this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTextBox.Location = new System.Drawing.Point(120, 8);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.TabIndex = 0;
            this.nameTextBox.Text = "";
            this.nameTextBox.Leave += new System.EventHandler(this.TitleEntered);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(32, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
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
            this.idTextBox.Leave += new System.EventHandler(this.IdEntered);
            // 
            // summaryTextBox
            // 
            this.summaryTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.summaryTextBox.Location = new System.Drawing.Point(120, 176);
            this.summaryTextBox.Multiline = true;
            this.summaryTextBox.Name = "summaryTextBox";
            this.summaryTextBox.Size = new System.Drawing.Size(144, 48);
            this.summaryTextBox.TabIndex = 4;
            this.summaryTextBox.Text = "";
            this.summaryTextBox.Leave += new System.EventHandler(this.SummaryEntered);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(32, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 23);
            this.label3.TabIndex = 5;
            this.label3.Text = "Summary";
            // 
            // elementsView
            // 
            this.elementsView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.elementsView.ImageIndex = -1;
            this.elementsView.Location = new System.Drawing.Point(120, 376);
            this.elementsView.Name = "elementsView";
            this.elementsView.SelectedImageIndex = -1;
            this.elementsView.ShowPlusMinus = false;
            this.elementsView.ShowRootLines = false;
            this.elementsView.Size = new System.Drawing.Size(408, 96);
            this.elementsView.TabIndex = 11;
            // 
            // addElementTextBox
            // 
            this.addElementTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addElementTextBox.Location = new System.Drawing.Point(120, 352);
            this.addElementTextBox.Name = "addElementTextBox";
            this.addElementTextBox.Size = new System.Drawing.Size(280, 20);
            this.addElementTextBox.TabIndex = 6;
            this.addElementTextBox.Text = "";
            // 
            // addElementButton
            // 
            this.addElementButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addElementButton.Location = new System.Drawing.Point(408, 352);
            this.addElementButton.Name = "addElementButton";
            this.addElementButton.Size = new System.Drawing.Size(75, 20);
            this.addElementButton.TabIndex = 16;
            this.addElementButton.Text = "Add";
            this.addElementButton.Click += new System.EventHandler(this.AddElementClick);
            // 
            // deleteElementButton
            // 
            this.deleteElementButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteElementButton.Location = new System.Drawing.Point(536, 384);
            this.deleteElementButton.Name = "deleteElementButton";
            this.deleteElementButton.TabIndex = 17;
            this.deleteElementButton.Text = "Delete";
            this.deleteElementButton.Click += new System.EventHandler(this.deleteElementClick);
            // 
            // moveElementUpButton
            // 
            this.moveElementUpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.moveElementUpButton.Location = new System.Drawing.Point(536, 416);
            this.moveElementUpButton.Name = "moveElementUpButton";
            this.moveElementUpButton.TabIndex = 18;
            this.moveElementUpButton.Text = "Up";
            this.moveElementUpButton.Click += new System.EventHandler(this.moveElementUpClicked);
            // 
            // moveElementDownButton
            // 
            this.moveElementDownButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.moveElementDownButton.Location = new System.Drawing.Point(536, 448);
            this.moveElementDownButton.Name = "moveElementDownButton";
            this.moveElementDownButton.TabIndex = 19;
            this.moveElementDownButton.Text = "Down";
            this.moveElementDownButton.Click += new System.EventHandler(this.moveElementDownClicked);
            // 
            // viewXmlButton
            // 
            this.viewXmlButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.viewXmlButton.Location = new System.Drawing.Point(560, 24);
            this.viewXmlButton.Name = "viewXmlButton";
            this.viewXmlButton.Size = new System.Drawing.Size(75, 20);
            this.viewXmlButton.TabIndex = 20;
            this.viewXmlButton.Text = "View XML";
            this.viewXmlButton.Click += new System.EventHandler(this.viewXmlClick);
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileNameTextBox.Location = new System.Drawing.Point(120, 600);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.Size = new System.Drawing.Size(392, 20);
            this.fileNameTextBox.TabIndex = 21;
            this.fileNameTextBox.Text = "";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(32, 600);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 20);
            this.button1.TabIndex = 22;
            this.button1.Text = "Save as...";
            this.button1.Click += new System.EventHandler(this.saveButtonClicked);
            // 
            // browseButton
            // 
            this.browseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.browseButton.Location = new System.Drawing.Point(528, 600);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 20);
            this.browseButton.TabIndex = 23;
            this.browseButton.Text = "Browse...";
            this.browseButton.Click += new System.EventHandler(this.browseButtonClick);
            // 
            // actorList
            // 
            this.actorList.Location = new System.Drawing.Point(24, 72);
            this.actorList.Name = "actorList";
            this.actorList.Size = new System.Drawing.Size(456, 104);
            this.actorList.TabIndex = 24;
            this.actorList.Title = "Actors";
            this.actorList.Delete += new UseCase.ItemRemovedEventHandler(this.DeleteItem);
            this.actorList.Add += new UseCase.ItemAddedEventHandler(this.AddItem);
            // 
            // preConditionsList
            // 
            this.preConditionsList.Location = new System.Drawing.Point(24, 232);
            this.preConditionsList.Name = "preConditionsList";
            this.preConditionsList.Size = new System.Drawing.Size(456, 112);
            this.preConditionsList.TabIndex = 25;
            this.preConditionsList.Title = "Preconditions";
            this.preConditionsList.Delete += new UseCase.ItemRemovedEventHandler(this.DeleteItem);
            this.preConditionsList.Add += new UseCase.ItemAddedEventHandler(this.AddItem);
            // 
            // postConditionsList
            // 
            this.postConditionsList.Location = new System.Drawing.Point(24, 480);
            this.postConditionsList.Name = "postConditionsList";
            this.postConditionsList.Size = new System.Drawing.Size(456, 112);
            this.postConditionsList.TabIndex = 26;
            this.postConditionsList.Title = "Postconditions";
            this.postConditionsList.Delete += new UseCase.ItemRemovedEventHandler(this.DeleteItem);
            this.postConditionsList.Add += new UseCase.ItemAddedEventHandler(this.AddItem);
            // 
            // UseCaseForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(656, 635);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.postConditionsList,
                                                                          this.preConditionsList,
                                                                          this.actorList,
                                                                          this.browseButton,
                                                                          this.button1,
                                                                          this.fileNameTextBox,
                                                                          this.viewXmlButton,
                                                                          this.moveElementDownButton,
                                                                          this.moveElementUpButton,
                                                                          this.deleteElementButton,
                                                                          this.addElementButton,
                                                                          this.addElementTextBox,
                                                                          this.elementsView,
                                                                          this.label3,
                                                                          this.summaryTextBox,
                                                                          this.idTextBox,
                                                                          this.label2,
                                                                          this.label1,
                                                                          this.nameTextBox});
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
            this.actorList.Items = this.useCaseModel.Actors;
            this.postConditionsList.Items = this.useCaseModel.PostConditions;
            this.preConditionsList.Items = this.useCaseModel.PreConditions;
        }

        private void ElementsChanged( object sender, EventArgs e )
        {
            this.elementsView.Nodes.Clear();
            int i = 1;
            foreach( IItem element in this.useCaseModel.Elements )
            {
                TreeNode node = new TreeNode( 
                    String.Format( "{0:0#}: {1}", i, element ) );
                node.Tag = element;
                this.elementsView.Nodes.Add( node );

                ++i;
            }
        }


        private void AddItem(object sender, string value )
        {
            if ( sender == this.actorList )
                this.useCaseModel.AddActor( value);
            else if ( sender == this.preConditionsList )
                this.useCaseModel.AddPrecondition( value );
            else if ( sender == this.postConditionsList )
                this.useCaseModel.AddPostcondition( value );
        
        }

        private void IdChanged( object sender, EventArgs e )
        {
            if ( idTextBox.Text.Trim() != string.Empty &&
                fileNameTextBox.Text.Trim() != string.Empty )
            {
                string directory = Path.GetDirectoryName( fileNameTextBox.Text.Trim() );
                fileNameTextBox.Text = Path.Combine( 
                    directory, idTextBox.Text + ".xml" );
                
            }
        }

        private void DeleteItem(object sender, object item )
        {
            if ( sender == this.actorList )
                this.useCaseModel.RemoveActor( ((IItem)item) );
            else if ( sender == this.preConditionsList )
                this.useCaseModel.RemovePrecondition( ((IItem)item) );
            else if ( sender == this.postConditionsList )
                this.useCaseModel.RemovePostcondition( ((IItem)item) );
        
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

        
        private void IdEntered(object sender, System.EventArgs e)
        {
            this.useCaseModel.Id = this.idTextBox.Text;            
        }

        private void TitleEntered(object sender, System.EventArgs e)
        {
            this.useCaseModel.Name = this.nameTextBox.Text;        
        }

        private void SummaryEntered(object sender, System.EventArgs e)
        {
            this.useCaseModel.Summary = this.summaryTextBox.Text;
        }

        private void moveElementUpClicked(object sender, System.EventArgs e)
        {
            if( this.elementsView.SelectedNode != null &&
                this.elementsView.SelectedNode.PrevNode != null )
            {
                this.useCaseModel.MoveElementBefore( 
                    (IItem)this.elementsView.SelectedNode.Tag,
                    (IItem)this.elementsView.SelectedNode.PrevNode.Tag );
            }
        }

        private void moveElementDownClicked(object sender, System.EventArgs e)
        {
            if ( this.elementsView.SelectedNode != null &&
                this.elementsView.SelectedNode.NextNode != null )
            {
                this.useCaseModel.MoveElementAfter(
                    (IItem)this.elementsView.SelectedNode.Tag,
                    (IItem)this.elementsView.SelectedNode.NextNode.Tag );
            }
        
        }

        private void browseButtonClick(object sender, System.EventArgs e)
        {
            using( SaveFileDialog sfd = new SaveFileDialog() )
            {
                sfd.DefaultExt = ".xml";
                if ( sfd.ShowDialog() == DialogResult.OK )
                    fileNameTextBox.Text = sfd.FileName;
            }      
        }

        private void saveButtonClicked(object sender, System.EventArgs e)
        {
//            XmlModel model = new XmlModel( this.useCaseModel );
//            model.Xsl = "UseCaseXSL1.xsl";
//            model.Save( fileNameTextBox.Text );        
        }

        private UseCaseModel useCaseModel;

        private XmlViewForm xmlViewForm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox idTextBox;
        private System.Windows.Forms.TextBox addElementTextBox;
        private System.Windows.Forms.Button deleteElementButton;
        private System.Windows.Forms.Button moveElementUpButton;
        private System.Windows.Forms.Button moveElementDownButton;
        private System.Windows.Forms.TreeView elementsView;
        private System.Windows.Forms.Button addElementButton;
        private System.Windows.Forms.Button viewXmlButton;
        private System.Windows.Forms.TextBox summaryTextBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button browseButton;
        private UseCase.ItemListUserControl actorList;
        private UseCase.ItemListUserControl preConditionsList;
        private UseCase.ItemListUserControl postConditionsList;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;



        
	}
}
