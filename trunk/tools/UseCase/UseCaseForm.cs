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

            
            SubscribeToEvents();

            this.stepRadioButton.Tag = new StepElementOption();
            this.includeRadioButton.Tag = new IncludeElementOption();

            this.elementTypeChosen( this.stepRadioButton, EventArgs.Empty );

            this.isDirty = false;
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
            this.xslTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.actorList = new UseCase.ItemListUserControl();
            this.preConditionsList = new UseCase.ItemListUserControl();
            this.postConditionsList = new UseCase.ItemListUserControl();
            this.elementPanel = new System.Windows.Forms.Panel();
            this.includeRadioButton = new System.Windows.Forms.RadioButton();
            this.stepRadioButton = new System.Windows.Forms.RadioButton();
            this.elementLabel = new System.Windows.Forms.Label();
            this.mainMenu = new System.Windows.Forms.MainMenu();
            this.fileItem = new System.Windows.Forms.MenuItem();
            this.newItem = new System.Windows.Forms.MenuItem();
            this.openItem = new System.Windows.Forms.MenuItem();
            this.saveItem = new System.Windows.Forms.MenuItem();
            this.saveAsItem = new System.Windows.Forms.MenuItem();
            this.exitItem = new System.Windows.Forms.MenuItem();
            this.elementPanel.SuspendLayout();
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
            this.summaryTextBox.Size = new System.Drawing.Size(272, 48);
            this.summaryTextBox.TabIndex = 3;
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
            this.elementsView.Size = new System.Drawing.Size(312, 96);
            this.elementsView.TabIndex = 11;
            // 
            // addElementTextBox
            // 
            this.addElementTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addElementTextBox.Location = new System.Drawing.Point(120, 352);
            this.addElementTextBox.Name = "addElementTextBox";
            this.addElementTextBox.Size = new System.Drawing.Size(280, 20);
            this.addElementTextBox.TabIndex = 5;
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
            this.deleteElementButton.Location = new System.Drawing.Point(568, 384);
            this.deleteElementButton.Name = "deleteElementButton";
            this.deleteElementButton.TabIndex = 17;
            this.deleteElementButton.Text = "Delete";
            this.deleteElementButton.Click += new System.EventHandler(this.deleteElementClick);
            // 
            // moveElementUpButton
            // 
            this.moveElementUpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.moveElementUpButton.Location = new System.Drawing.Point(568, 416);
            this.moveElementUpButton.Name = "moveElementUpButton";
            this.moveElementUpButton.TabIndex = 18;
            this.moveElementUpButton.Text = "Up";
            this.moveElementUpButton.Click += new System.EventHandler(this.moveElementUpClicked);
            // 
            // moveElementDownButton
            // 
            this.moveElementDownButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.moveElementDownButton.Location = new System.Drawing.Point(568, 448);
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
            // xslTextBox
            // 
            this.xslTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.xslTextBox.Location = new System.Drawing.Point(120, 600);
            this.xslTextBox.Name = "xslTextBox";
            this.xslTextBox.Size = new System.Drawing.Size(392, 20);
            this.xslTextBox.TabIndex = 21;
            this.xslTextBox.Text = "";
            // 
            // browseButton
            // 
            this.browseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.browseButton.Location = new System.Drawing.Point(528, 600);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 20);
            this.browseButton.TabIndex = 23;
            this.browseButton.Text = "Browse...";
            // 
            // actorList
            // 
            this.actorList.Location = new System.Drawing.Point(24, 72);
            this.actorList.Name = "actorList";
            this.actorList.Size = new System.Drawing.Size(456, 104);
            this.actorList.TabIndex = 2;
            this.actorList.Title = "Actors";
            this.actorList.Delete += new UseCase.ItemRemovedEventHandler(this.DeleteItem);
            this.actorList.Add += new UseCase.ItemAddedEventHandler(this.AddItem);
            // 
            // preConditionsList
            // 
            this.preConditionsList.Location = new System.Drawing.Point(24, 232);
            this.preConditionsList.Name = "preConditionsList";
            this.preConditionsList.Size = new System.Drawing.Size(456, 112);
            this.preConditionsList.TabIndex = 4;
            this.preConditionsList.Title = "Preconditions";
            this.preConditionsList.Delete += new UseCase.ItemRemovedEventHandler(this.DeleteItem);
            this.preConditionsList.Add += new UseCase.ItemAddedEventHandler(this.AddItem);
            // 
            // postConditionsList
            // 
            this.postConditionsList.Location = new System.Drawing.Point(24, 480);
            this.postConditionsList.Name = "postConditionsList";
            this.postConditionsList.Size = new System.Drawing.Size(456, 112);
            this.postConditionsList.TabIndex = 6;
            this.postConditionsList.Title = "Postconditions";
            this.postConditionsList.Delete += new UseCase.ItemRemovedEventHandler(this.DeleteItem);
            this.postConditionsList.Add += new UseCase.ItemAddedEventHandler(this.AddItem);
            // 
            // elementPanel
            // 
            this.elementPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                       this.includeRadioButton,
                                                                                       this.stepRadioButton});
            this.elementPanel.Location = new System.Drawing.Point(440, 376);
            this.elementPanel.Name = "elementPanel";
            this.elementPanel.Size = new System.Drawing.Size(120, 96);
            this.elementPanel.TabIndex = 27;
            // 
            // includeRadioButton
            // 
            this.includeRadioButton.Location = new System.Drawing.Point(8, 32);
            this.includeRadioButton.Name = "includeRadioButton";
            this.includeRadioButton.Size = new System.Drawing.Size(88, 24);
            this.includeRadioButton.TabIndex = 1;
            this.includeRadioButton.Text = "Include";
            this.includeRadioButton.Click += new System.EventHandler(this.elementTypeChosen);
            // 
            // stepRadioButton
            // 
            this.stepRadioButton.Checked = true;
            this.stepRadioButton.Location = new System.Drawing.Point(8, 8);
            this.stepRadioButton.Name = "stepRadioButton";
            this.stepRadioButton.Size = new System.Drawing.Size(48, 24);
            this.stepRadioButton.TabIndex = 0;
            this.stepRadioButton.TabStop = true;
            this.stepRadioButton.Text = "Step";
            this.stepRadioButton.Click += new System.EventHandler(this.elementTypeChosen);
            // 
            // elementLabel
            // 
            this.elementLabel.Location = new System.Drawing.Point(32, 352);
            this.elementLabel.Name = "elementLabel";
            this.elementLabel.Size = new System.Drawing.Size(80, 23);
            this.elementLabel.TabIndex = 28;
            this.elementLabel.Text = "label4";
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                     this.fileItem});
            // 
            // fileItem
            // 
            this.fileItem.Index = 0;
            this.fileItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                     this.newItem,
                                                                                     this.openItem,
                                                                                     this.saveItem,
                                                                                     this.saveAsItem,
                                                                                     this.exitItem});
            this.fileItem.Text = "&File";
            this.fileItem.Popup += new System.EventHandler(this.FilePopup);
            // 
            // newItem
            // 
            this.newItem.Index = 0;
            this.newItem.Text = "New";
            this.newItem.Click += new System.EventHandler(this.NewItemClick);
            // 
            // openItem
            // 
            this.openItem.Index = 1;
            this.openItem.Text = "&Open...";
            this.openItem.Click += new System.EventHandler(this.OpenItemClick);
            // 
            // saveItem
            // 
            this.saveItem.Index = 2;
            this.saveItem.Text = "&Save...";
            this.saveItem.Click += new System.EventHandler(this.SaveItemClick);
            // 
            // saveAsItem
            // 
            this.saveAsItem.Index = 3;
            this.saveAsItem.Text = "Save as...";
            this.saveAsItem.Click += new System.EventHandler(this.SaveAsItemClick);
            // 
            // exitItem
            // 
            this.exitItem.Index = 4;
            this.exitItem.Text = "&Exit";
            // 
            // UseCaseForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(656, 635);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.elementLabel,
                                                                          this.elementPanel,
                                                                          this.postConditionsList,
                                                                          this.preConditionsList,
                                                                          this.actorList,
                                                                          this.browseButton,
                                                                          this.xslTextBox,
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
            this.Menu = this.mainMenu;
            this.Name = "UseCaseForm";
            this.Text = "Use case";
            this.elementPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        public static void Main()
        {
            Application.Run( new UseCaseForm() );
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

            this.UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            this.useCaseModel.PostConditionsChanged +=
                new EventHandler( this.ModelChanged );
            this.useCaseModel.PreConditionsChanged +=
                new EventHandler( this.ModelChanged );                   
            this.useCaseModel.ActorsChanged += 
                new EventHandler( this.ModelChanged );

            this.useCaseModel.Changed += 
                new EventHandler( this.Changed );

            this.useCaseModel.ElementsChanged +=
                new EventHandler( this.ElementsChanged );

            this.useCaseModel.AtomsChanged +=
                new EventHandler( this.AtomsChanged );
        }

        private void UnsubscribeFromEvents()
        {
            this.useCaseModel.PostConditionsChanged -=
                new EventHandler( this.ModelChanged );
            this.useCaseModel.PreConditionsChanged -=
                new EventHandler( this.ModelChanged );                   
            this.useCaseModel.ActorsChanged -= 
                new EventHandler( this.ModelChanged );

            this.useCaseModel.ElementsChanged -=
                new EventHandler( this.ElementsChanged );

            this.useCaseModel.AtomsChanged -=
                new EventHandler( this.AtomsChanged );
        }

        private void RefreshFromModel()
        {
            this.ModelChanged( this.useCaseModel, EventArgs.Empty );
            this.ElementsChanged( this.useCaseModel, EventArgs.Empty );
            this.AtomsChanged( this.useCaseModel, EventArgs.Empty );

        }

        private void Changed( object sender, EventArgs e )
        {
            this.isDirty = true;
        }
        
        private void AtomsChanged( object sender, EventArgs e )
        {
            this.nameTextBox.Text = this.useCaseModel.Name;
            this.idTextBox.Text = this.useCaseModel.Id;
            this.summaryTextBox.Text = this.useCaseModel.Summary;

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
            ElementVisitor visitor = new ElementVisitor( this.elementsView.Nodes );

            IElement[] elts = this.useCaseModel.Elements;
            
            foreach( IElement element in elts )
                element.AcceptVisitor( visitor );

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
                IElement element = this.selectedOption.CreateElement(
                    this.addElementTextBox.Text );

                this.useCaseModel.AppendElement( element );
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

        

        private void elementTypeChosen(object sender, System.EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            this.selectedOption = (IFlowElementOption)btn.Tag;

            elementLabel.Text = this.selectedOption.LabelText;
        }

        private void OpenItemClick(object sender, System.EventArgs e)
        {
            using( OpenFileDialog ofd = new OpenFileDialog() )
            {
                ofd.Filter = "XML Files (*.xml)|*.xml";
                if ( ofd.ShowDialog() == DialogResult.OK )
                {
                    this.UnsubscribeFromEvents();
                    this.useCaseModel = UseCaseModel.FromFile( ofd.FileName ); 
                    this.filename = ofd.FileName;
                    this.Text = ofd.FileName;
                    this.SubscribeToEvents();
                    this.RefreshFromModel();

                    this.isDirty = false;
                }
            }
        }

        private void SaveItemClick(object sender, System.EventArgs e)
        {
            Save();
        
        }

        private bool Save()
        {
            if( this.filename == null )
                return SaveAs();
            else
            {
                this.useCaseModel.Save( this.filename, this.xslTextBox.Text );
                this.isDirty = false;
                return true;
            }
        }

        private void SaveAsItemClick(object sender, System.EventArgs e)
        {
            this.SaveAs();
        }

        private void NewItemClick(object sender, System.EventArgs e)
        {
            if ( this.isDirty )
            {
                DialogResult dr = MessageBox.Show( "Save now?", "Use case not saved", 
                    MessageBoxButtons.YesNoCancel );
                if ( dr == DialogResult.Cancel )
                    return;
                else if ( dr == DialogResult.Yes && !this.Save() )
                    return;
            }

            this.UnsubscribeFromEvents();
            this.useCaseModel = new UseCaseModel();
            this.SubscribeToEvents();
            this.RefreshFromModel();
            this.Text = "";

            this.filename = null;
        }

        private bool SaveAs()
        {
            using( SaveFileDialog sfd = new SaveFileDialog() )
            {
                sfd.DefaultExt = ".xml";
                sfd.Filter = "XML files (*.xml)|*.xml";

                if ( this.idTextBox.Text.Trim() != string.Empty )
                    sfd.FileName = this.idTextBox.Text + ".xml";

                if( sfd.ShowDialog() == DialogResult.OK )
                {
                    this.useCaseModel.Save( sfd.FileName, this.xslTextBox.Text );
                    this.filename = sfd.FileName;

                    this.Text = this.filename;

                    this.isDirty = false;

                    return true;
                }
                else
                    return false;
            }

        }

        private void FilePopup(object sender, System.EventArgs e)
        {
            if ( this.isDirty )
                this.saveItem.Enabled = true;
            else
                this.saveItem.Enabled = false;

        }

        #region private class ElementVisitor : IElementVisitor
        private class ElementVisitor : IElementVisitor
        {
            public ElementVisitor( TreeNodeCollection coll )
            {
                this.coll = coll;
                this.count = 1;
            }

            public void VisitStepElement(UseCase.StepElement element)
            {
                TreeNode node = new TreeNode( string.Format( "{0:0#}: {1}", 
                    this.count, element.Text ) );
                node.Tag = element;
                this.coll.Add( node );      
 
                this.count++;
            }

            public void VisitIncludeElement(UseCase.IncludeElement element)
            {
                TreeNode node = new TreeNode( String.Format( "{0:0#}: <Include '{1}'>", 
                    this.count, element.Text ) );
                node.Tag = element;

                this.coll.Add( node );   
         
                this.count++;
            }

            public TreeNodeCollection Nodes
            {
                get{ return this.coll; }
            }


            private TreeNodeCollection coll;
            private int count;
        }
        #endregion

        #region private data
        private UseCaseModel useCaseModel;
        private bool isDirty;

        private IFlowElementOption selectedOption;
        private string filename = null;

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
        private System.Windows.Forms.Button browseButton;
        private UseCase.ItemListUserControl actorList;
        private UseCase.ItemListUserControl preConditionsList;
        private UseCase.ItemListUserControl postConditionsList;
        private System.Windows.Forms.Panel elementPanel;
        private System.Windows.Forms.RadioButton stepRadioButton;
        private System.Windows.Forms.RadioButton includeRadioButton;
        private System.Windows.Forms.Label elementLabel;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem fileItem;
        private System.Windows.Forms.MenuItem openItem;
        private System.Windows.Forms.MenuItem saveItem;
        private System.Windows.Forms.MenuItem exitItem;
        private System.Windows.Forms.MenuItem saveAsItem;
        private System.Windows.Forms.MenuItem newItem;
        private System.Windows.Forms.TextBox xslTextBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        #endregion
	}
}
